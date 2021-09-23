using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    //[SerializeField]
    //private Sprite dotSprite = null;
    [SerializeField]
    private RectTransform dotTemplate = null;
    [SerializeField]
    private RectTransform graphContainer = null;
    [SerializeField]
    private RectTransform labelTemplateX = null;
    [SerializeField]
    private RectTransform labelTemplateY = null;
    [SerializeField]
    private RectTransform dashTemplateX = null;
    [SerializeField]
    private RectTransform dashTemplateY = null;

    private void Start()
    {
        graphContainer ??= transform.Find("Graph Container").GetComponent<RectTransform>();
        labelTemplateX ??= transform.Find("Label Template X").GetComponent<RectTransform>();
        labelTemplateY ??= transform.Find("Label Template Y").GetComponent<RectTransform>();
        dashTemplateX ??= transform.Find("Dash Template Y").GetComponent<RectTransform>();
        dashTemplateY ??= transform.Find("Dash Template Y").GetComponent<RectTransform>();

        List<int> valueList = new List<int>() { 5, 12, 13, 29, 43, 45, 48, 76, 77, 88, 90, 98, 78, 58, 38, 18 };
        ShowGraph(valueList);
    }

    private RectTransform CreateDot(Vector2 anchoredPosition)
    {
        //GameObject dotObject = new GameObject("Dot", typeof(Image));
        //dotObject.transform.SetParent(graphContainer, false);
        //dotObject.GetComponent<Image>().sprite = dotSprite;
        InsantiateGraphElement(dotTemplate, out RectTransform dot);
        //RectTransform rectTransform = dotObject.GetComponent<RectTransform>();
        dot.anchoredPosition = anchoredPosition;
        dot.sizeDelta = new Vector2(15, 15);
        dot.anchorMin = new Vector2(0, 0);
        dot.anchorMax = new Vector2(0, 0);

        return dot;
    }

    private void ShowGraph(List<int> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 100f;
        float xSize = 50f;
        RectTransform lastDot = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            RectTransform dot = CreateDot(new Vector2(xPosition, yPosition));
            if (lastDot != null)
            {
                CreateDotConnection(lastDot.anchoredPosition, dot.anchoredPosition);
            }
            lastDot = dot;

            InsantiateGraphElement(labelTemplateX, out RectTransform labelX);
            labelX.anchoredPosition = new Vector2(xPosition, -20f);
            labelX.GetComponent<Text>().text = i.ToString();

            InsantiateGraphElement(dashTemplateX, out RectTransform dashX);
            dashX.anchoredPosition = new Vector2(xPosition, -5f);
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            InsantiateGraphElement(labelTemplateY, out RectTransform labelY);
            float normalizedValue = (i * 1f / separatorCount);
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
        }
    }

    private void InsantiateGraphElement(RectTransform template, out RectTransform uiElement)
    {
        uiElement = Instantiate(template);
        uiElement.SetParent(graphContainer);
        uiElement.gameObject.SetActive(true);
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject dotConnection = new GameObject("Dot Connection", typeof(Image));
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
