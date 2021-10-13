using UnityEngine;
using UnityEngine.UI;

public class LineGraphVisual : IGraphVisual
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
    }
}
