using UnityEngine;
using UnityEngine.UI;

public class BarChartVisual : IGraphVisual
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
