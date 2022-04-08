using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for mapping/display progress data from exercises' results in the form of graph/plot.
/// </summary>
[RequireComponent(typeof(ProgressSystem))]
public class WindowGraph : MonoBehaviour
{

    [SerializeField]
    private RectTransform graphContainer = null;
    [Header("UI Element Templates")]
    public RectTransform dotTemplate = null;
    public RectTransform dotConnectionTemplate = null;
    public RectTransform barTemplate = null;
    [SerializeField]
    private RectTransform labelTemplateX = null;
    [SerializeField]
    private RectTransform labelTemplateY = null;
    [SerializeField]
    private RectTransform dashTemplateX = null;
    [SerializeField]
    private RectTransform dashTemplateY = null;
    private List<GameObject> _gameObjectList = new List<GameObject>();

    private ProgressSystem _system;
    private int _visibleElementsAmount = -1;

    // Cached values
    private List<float> _valueList;
    private IGraphVisual _graphVisual;
    private Func<int, string> _getAxisLabelX = null;
    private Func<float, string> _getAxisLabelY = null;

    private void Start()
    {
        _system = GetComponent<ProgressSystem>();
        _system.FilesInfoUpdate();
        _visibleElementsAmount = _system.Files.Length;
        List<float> timeList;
        List<DateTime> dateList;
        LoadExerciseData(out timeList, out dateList);

        // Remark: Remove?
        //graphContainer ??= transform.Find("Graph Container").GetComponent<RectTransform>();
        //dotTemplate ??= graphContainer.Find("Dot Template").GetComponent<RectTransform>();
        //barTemplate ??= graphContainer.Find("Bar Template").GetComponent<RectTransform>();
        //labelTemplateX ??= graphContainer.Find("Label Template X").GetComponent<RectTransform>();
        //labelTemplateY ??= graphContainer.Find("Label Template Y").GetComponent<RectTransform>();
        //dashTemplateX ??= graphContainer.Find("Dash Template Y").GetComponent<RectTransform>();
        //dashTemplateY ??= graphContainer.Find("Dash Template Y").GetComponent<RectTransform>();

        LineGraphVisual lineGraphVisual = new LineGraphVisual(this, Color.green, new Color(1, 1, 1, 0.5f));
        ShowGraph(timeList, lineGraphVisual, (int _i) => dateList[_i].ToString("dd\nMM\nyy"), (float _f) => $"{_f.ToString("f2")}");
    }

    private void LoadExerciseData(out List<float> timeList, out List<DateTime> dateList)
    {
        timeList = new List<float>();
        dateList = new List<DateTime>();

        if (_visibleElementsAmount == 0)
        {
            Debug.LogWarning("0 files, nothing to load");
        }
        // Remark: Make several dots in one day? Or leave only the best?
        for (int i = 0; i < _visibleElementsAmount; i++)
        {
            ExerciseResult tmpResult = _system.LoadResultFromJSON(_system.Files[i].FullName);
            if (tmpResult == null)
            {
                Debug.LogWarning("No ExersiceResult files to load");
                break;
            }
            DateTime tmpDateTime = DateTime.ParseExact(tmpResult.date, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
            float tmpTime = float.Parse(tmpResult.time);
            if (dateList.Count > 0 && tmpDateTime.Date == dateList[dateList.Count - 1].Date)
            {
                if (timeList.Count > 0 && tmpTime < timeList[timeList.Count - 1])
                {
                    timeList[timeList.Count - 1] = tmpTime;
                    dateList[dateList.Count - 1] = tmpDateTime;
                }
                continue;
            }
            timeList.Add(tmpTime);
            dateList.Add(tmpDateTime);
        }
        _visibleElementsAmount = timeList.Count;
        timeList.Reverse();
        dateList.Reverse();
    }

    /// <summary>
    /// Change visual mode between LineGraph and BarChart.
    /// </summary>
    public void ChangeVisual()
    {
        if (_graphVisual is LineGraphVisual)
        {
            BarChartVisual barChartVisual = new BarChartVisual(this, Color.cyan, 0.9f);
            ShowGraph(_valueList, barChartVisual, _getAxisLabelX, _getAxisLabelY);
        }
        else
        {
            LineGraphVisual lineGraphVisual = new LineGraphVisual(this, Color.green, new Color(1, 1, 1, 0.5f));
            ShowGraph(_valueList, lineGraphVisual, _getAxisLabelX, _getAxisLabelY);
        }
    }

    /// <summary>
    /// Change amount of visible/showed elements in graph.
    /// </summary>
    /// <param name="change"></param>
    public void ChangeVisibleAmount(int change)
    {
        _visibleElementsAmount += change;
        ShowGraph(_valueList, _graphVisual, _getAxisLabelX, _getAxisLabelY);
    }

    /// <summary>
    /// Main method to render whole graph and elements.
    /// </summary>
    /// <param name="valueList"></param>
    /// <param name="graphVisual"></param>
    /// <param name="getAxisLabelX"></param>
    /// <param name="getAxisLabelY"></param>
    private void ShowGraph(List<float> valueList, IGraphVisual graphVisual, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        _valueList = valueList;
        _graphVisual = graphVisual;
        _getAxisLabelX = getAxisLabelX;
        _getAxisLabelY = getAxisLabelY;

        getAxisLabelX ??= delegate (int _i) { return _i.ToString(); };
        getAxisLabelY ??= delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };

        graphVisual.Reset();

        foreach (var go in _gameObjectList)
        {
            Destroy(go);
        }
        _gameObjectList.Clear();

        if ((_visibleElementsAmount > valueList.Count) || (_visibleElementsAmount <= 0))
        {
            _visibleElementsAmount = valueList.Count;
        }

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0], yMinimum = valueList[0];
        for (int i = 0; i < _visibleElementsAmount; i++)
        {
            float value = valueList[i];
            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }
        // Get a little space between graph and panel borders.
        float yDifference = (yMaximum - yMinimum);
        if (yDifference <= 0)
        {
            yDifference = 1f;
        }
        yMaximum += yDifference * 0.2f;
        yMinimum -= yDifference * 0.2f;

        float xSize = graphWidth / (_visibleElementsAmount + 1);
        for (int i = 0; i < _visibleElementsAmount; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            string tooltipText = getAxisLabelY(valueList[i]);
            graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize, tooltipText);

            InsantiateGraphElement(labelTemplateX, out RectTransform labelX);
            labelX.anchoredPosition = new Vector2(xPosition, -20f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);

            InsantiateGraphElement(dashTemplateX, out RectTransform dashX);
            dashX.anchoredPosition = new Vector2(xPosition, -5f);
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            InsantiateGraphElement(labelTemplateY, out RectTransform labelY);
            float normalizedValue = (i * 1f / separatorCount);
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + normalizedValue * (yMaximum - yMinimum));

            InsantiateGraphElement(dashTemplateY, out RectTransform dashY);
            dashY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
        }

    }

    /// <summary>
    /// Create  RectTransform element, put it under graphContainer and add reference to list (if we need to delete it, when rerendering).
    /// </summary>
    /// <param name="template"></param>
    /// <param name="uiElement"></param>
    public void InsantiateGraphElement(RectTransform template, out RectTransform uiElement)
    {
        uiElement = Instantiate(template);
        uiElement.SetParent(graphContainer);
        //uiElement.SetParent(graphContainer, isWorldPositionStays);
        uiElement.gameObject.SetActive(true);
        _gameObjectList.Add(uiElement.gameObject);
    }

    /// <summary>
    /// Load save data about new exercise and call ShowGraph with new data to rerender.
    /// </summary>
    /// <param name="savePath"></param>
    public void ChangeExercise(string savePath)
    {
        _system.FilesInfoUpdate(savePath);
        _visibleElementsAmount = _system.Files.Length;
        List<float> timeList;
        List<DateTime> dateList;
        LoadExerciseData(out timeList, out dateList);
        ShowGraph(timeList, _graphVisual, (int _i) => dateList[_i].ToString("dd\nMM\nyy"), _getAxisLabelY);
    }
}
