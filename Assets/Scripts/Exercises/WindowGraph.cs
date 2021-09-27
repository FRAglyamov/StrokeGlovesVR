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
    private RectTransform labelTemplateX = null;
    [SerializeField]
    private RectTransform labelTemplateY = null;
    [SerializeField]
    private RectTransform dashTemplateX = null;
    [SerializeField]
    private RectTransform dashTemplateY = null;
    private List<GameObject> _gameObjectList = new List<GameObject>();

    private ProgressSystem _system;
    private int _elementsAmount = 7;

    private void Start()
    {
        _system = GetComponent<ProgressSystem>();
        _system.FilesInfoUpdate();
        List<float> timeList = new List<float>();
        List<DateTime> dateList = new List<DateTime>();
        for (int i = 0; i < _elementsAmount; i++)
        {
            ExerciseResult tmpResult = _system.LoadResultFromJSON(_system.Files[i].FullName);
            timeList.Add(float.Parse(tmpResult.time));
            dateList.Add(DateTime.ParseExact(tmpResult.date, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture));
        }
        // Remark: Remove later? How it work from perfomance view?
        timeList.Reverse();
        dateList.Reverse();

        graphContainer ??= transform.Find("Graph Container").GetComponent<RectTransform>();
        labelTemplateX ??= transform.Find("Label Template X").GetComponent<RectTransform>();
        labelTemplateY ??= transform.Find("Label Template Y").GetComponent<RectTransform>();
        dashTemplateX ??= transform.Find("Dash Template Y").GetComponent<RectTransform>();
        dashTemplateY ??= transform.Find("Dash Template Y").GetComponent<RectTransform>();

        List<float> valueList = new List<float>() { 5, 12, 13, 29, 43, 45, 48, 76, 77, 88, 90, 98, 78, 58, 38, 18 };
        //ShowGraph(valueList, (int _i) => $"Day {_i + 1}", (float _f) => $"{_f.ToString("f2")}");

        // TODO: Change "Day" to Date: dd/MM/yy, get date and time (highest result at the day) from progress saves.
        ShowGraph(timeList, (int _i) => dateList[_i].ToString("dd\nMM\nyy"), (float _f) => $"{_f.ToString("f2")}");
    }

    private RectTransform CreateDot(Vector2 anchoredPosition)
    {
        InsantiateGraphElement(dotTemplate, out RectTransform dot);
        dot.anchoredPosition = anchoredPosition;
        dot.sizeDelta = new Vector2(15, 15);
        dot.anchorMin = new Vector2(0, 0);
        dot.anchorMax = new Vector2(0, 0);

        return dot;
    }

    private void ShowGraph(List<float> valueList, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        getAxisLabelX ??= delegate (int _i) { return _i.ToString(); };
        getAxisLabelY ??= delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };

        foreach (var go in _gameObjectList)
        {
            Destroy(go);
        }
        _gameObjectList.Clear();

        if((_elementsAmount > valueList.Count) || (_elementsAmount <= 0))
        {
            _elementsAmount = valueList.Count;
        }

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0], yMinimum = valueList[0];
        for (int i = 0; i < _elementsAmount; i++)
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

        float xSize = graphWidth / (_elementsAmount + 1);
        RectTransform lastDot = null;
        for (int i = 0; i < _elementsAmount; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            RectTransform dot = CreateDot(new Vector2(xPosition, yPosition));
            if (lastDot != null)
            {
                CreateDotConnection(lastDot.anchoredPosition, dot.anchoredPosition);
            }
            lastDot = dot;

            InsantiateGraphElement(labelTemplateX, out RectTransform labelX);
            labelX.anchoredPosition = new Vector2(xPosition, -20f);
            labelX.GetComponent<Text>().text = getAxisLabelX( i);

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

    private void InsantiateGraphElement(RectTransform template, out RectTransform uiElement)
    {
        uiElement = Instantiate(template);
        uiElement.SetParent(graphContainer);
        uiElement.gameObject.SetActive(true);
        _gameObjectList.Add(uiElement.gameObject);
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject dotConnection = new GameObject("Dot Connection", typeof(Image));
        _gameObjectList.Add(dotConnection);
        dotConnection.transform.SetParent(graphContainer, false);
        dotConnection.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        RectTransform rectTransform = dotConnection.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);

        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;

        float angle = (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg;
        rectTransform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
