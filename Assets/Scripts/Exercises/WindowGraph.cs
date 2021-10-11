using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ProgressSystem))]
public class WindowGraph : MonoBehaviour
{

    [SerializeField]
    private RectTransform graphContainer = null;
    [Header("UI Element Templates")]
    [SerializeField]
    private RectTransform dotTemplate = null;
    [SerializeField]
    private RectTransform dotConnectionTemplate = null;
    [SerializeField]
    private RectTransform barTemplate = null;
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
        List<float> timeList = new List<float>();
        List<DateTime> dateList = new List<DateTime>();
        for (int i = 0; i < _visibleElementsAmount; i++)
        {
            ExerciseResult tmpResult = _system.LoadResultFromJSON(_system.Files[i].FullName);
            if (tmpResult == null)
            {
                Debug.LogWarning("No ExersiceResult files to load");
                break;
            }
            timeList.Add(float.Parse(tmpResult.time));
            dateList.Add(DateTime.ParseExact(tmpResult.date, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture));
        }
        // Remark: Remove later? How it work from perfomance view?
        timeList.Reverse();
        dateList.Reverse();

        //graphContainer ??= transform.Find("Graph Container").GetComponent<RectTransform>();
        //dotTemplate ??= graphContainer.Find("Dot Template").GetComponent<RectTransform>();
        //barTemplate ??= graphContainer.Find("Bar Template").GetComponent<RectTransform>();
        //labelTemplateX ??= graphContainer.Find("Label Template X").GetComponent<RectTransform>();
        //labelTemplateY ??= graphContainer.Find("Label Template Y").GetComponent<RectTransform>();
        //dashTemplateX ??= graphContainer.Find("Dash Template Y").GetComponent<RectTransform>();
        //dashTemplateY ??= graphContainer.Find("Dash Template Y").GetComponent<RectTransform>();

        List<float> valueList = new List<float>() { 5, 12, 13, 29, 43, 45, 48, 76, 77, 88, 90, 98, 78, 58, 38, 18 };
        //ShowGraph(valueList, (int _i) => $"Day {_i + 1}", (float _f) => $"{_f.ToString("f2")}");

        // TODO: Change "Day" to Date: dd/MM/yy, get date and time (highest result at the day) from progress saves.
        LineGraphVisual lineGraphVisual = new LineGraphVisual(this, Color.green, new Color(1, 1, 1, 0.5f));
        //BarChartVisual barChartVisual = new BarChartVisual(this, Color.cyan, 0.9f);
        ShowGraph(timeList, lineGraphVisual, (int _i) => dateList[_i].ToString("dd\nMM\nyy"), (float _f) => $"{_f.ToString("f2")}");
    }

    public void ChangeVisual()
    {
        if (_graphVisual is LineGraphVisual)
        {
            BarChartVisual barChartVisual = new BarChartVisual(this, Color.cyan, 0.9f);
            SetLineGraphVisual(barChartVisual);
        }
        else
        {
            LineGraphVisual lineGraphVisual = new LineGraphVisual(this, Color.green, new Color(1, 1, 1, 0.5f));
            SetLineGraphVisual(lineGraphVisual);
        }
    }

    private void SetLineGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(_valueList, graphVisual, _getAxisLabelX, _getAxisLabelY);
    }

    public void ChangeVisibleAmount(int change)
    {
        _visibleElementsAmount += change;
        _graphVisual.Reset();
        ShowGraph(_valueList, _graphVisual, _getAxisLabelX, _getAxisLabelY);
    }



    private void ShowGraph(List<float> valueList, IGraphVisual graphVisual, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        _valueList = valueList;
        _graphVisual = graphVisual;
        _getAxisLabelX = getAxisLabelX;
        _getAxisLabelY = getAxisLabelY;

        getAxisLabelX ??= delegate (int _i) { return _i.ToString(); };
        getAxisLabelY ??= delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };

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

            //lineGraphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize);
            //barChartVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize);
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

    public void InsantiateGraphElement(RectTransform template, out RectTransform uiElement, bool isWorldPositionStays = true)
    {
        uiElement = Instantiate(template);
        uiElement.SetParent(graphContainer, isWorldPositionStays);
        uiElement.gameObject.SetActive(true);
        _gameObjectList.Add(uiElement.gameObject);
    }

    public interface IGraphVisual
    {
        void AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void Reset();
    }

    private class LineGraphVisual : IGraphVisual
    {
        private WindowGraph _windowGraph;
        private RectTransform _lastDot = null;
        private Color _dotColor;
        private Color _dotConnectionColor;
        public LineGraphVisual(WindowGraph windowGraph, Color dotColor, Color dotConnectionColor)
        {
            _windowGraph = windowGraph;
            _dotColor = dotColor;
            _dotConnectionColor = dotConnectionColor;
        }

        public void AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            //RectTransform dot = CreateDot(new Vector2(xPosition, yPosition));
            RectTransform dot = CreateDot(graphPosition);
            if (_lastDot != null)
            {
                CreateDotConnection(_lastDot.anchoredPosition, dot.anchoredPosition);
            }
            _lastDot = dot;
            dot.GetComponent<TooltipTrigger>().text = tooltipText;
        }

        public void Reset()
        {
            _lastDot = null;
        }

        private RectTransform CreateDot(Vector2 anchoredPosition)
        {
            _windowGraph.InsantiateGraphElement(_windowGraph.dotTemplate, out RectTransform dot);
            dot.GetComponent<Image>().color = _dotColor;
            dot.anchoredPosition = anchoredPosition;
            dot.sizeDelta = new Vector2(15, 15);
            dot.anchorMin = new Vector2(0, 0);
            dot.anchorMax = new Vector2(0, 0);

            return dot;
        }

        private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            //GameObject dotConnection = new GameObject("Dot Connection", typeof(Image));
            //_gameObjectList.Add(dotConnection);
            //dotConnection.transform.SetParent(_windowGraph.graphContainer, false);
            _windowGraph.InsantiateGraphElement(_windowGraph.dotConnectionTemplate, out RectTransform dotConnection, false);
            dotConnection.GetComponent<Image>().color = _dotConnectionColor;

            dotConnection.anchorMin = new Vector2(0, 0);
            dotConnection.anchorMax = new Vector2(0, 0);

            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);

            dotConnection.sizeDelta = new Vector2(distance, 3f);
            dotConnection.anchoredPosition = dotPositionA + dir * distance * 0.5f;

            float angle = (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg;
            dotConnection.eulerAngles = new Vector3(0f, 0f, angle);

            //RectTransform rectTransform = dotConnection.GetComponent<RectTransform>();
            //rectTransform.anchorMin = new Vector2(0, 0);
            //rectTransform.anchorMax = new Vector2(0, 0);

            //Vector2 dir = (dotPositionB - dotPositionA).normalized;
            //float distance = Vector2.Distance(dotPositionA, dotPositionB);

            //rectTransform.sizeDelta = new Vector2(distance, 3f);
            //rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;

            //float angle = (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg;
            //rectTransform.eulerAngles = new Vector3(0f, 0f, angle);
        }
    }

    private class BarChartVisual : IGraphVisual
    {
        private WindowGraph _windowGraph;
        private Color _barColor;
        private float _barWidthMultiplier;
        public BarChartVisual(WindowGraph windowGraph, Color barColor, float barWidthMultiplier)
        {
            _windowGraph = windowGraph;
            _barColor = barColor;
            _barWidthMultiplier = barWidthMultiplier;
        }

        public void AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            //GameObject bar = CreateBar(new Vector2(xPosition, yPosition), xSize * 0.9f);
            RectTransform bar = CreateBar(graphPosition, graphPositionWidth);
            bar.GetComponent<TooltipTrigger>().text = tooltipText;
        }

        public void Reset()
        {
        }

        private RectTransform CreateBar(Vector2 graphPosition, float barWidth)
        {
            _windowGraph.InsantiateGraphElement(_windowGraph.barTemplate, out RectTransform bar);
            bar.anchoredPosition = new Vector2(graphPosition.x, 0f);
            bar.GetComponent<Image>().color = _barColor;
            bar.sizeDelta = new Vector2(barWidth * _barWidthMultiplier, graphPosition.y);
            bar.anchorMin = new Vector2(0, 0);
            bar.anchorMax = new Vector2(0, 0);
            bar.pivot = new Vector2(0.5f, 0f);

            return bar;
        }
    }
}
