using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private Text textField;
    //private RectTransform _rectTransform;
    //private void Start()
    //{
    //    _rectTransform = GetComponent<RectTransform>();
    //}
    public void SetText(string text)
    {
        textField.text = text;
        Vector2 position = Mouse.current.position.ReadValue();
        transform.position = position;
    }

    private void Update()
    {
        Vector2 position = Mouse.current.position.ReadValue();
        //float pivotX = position.x / Screen.width;
        //float pivotY = position.y / Screen.height;
        //_rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
