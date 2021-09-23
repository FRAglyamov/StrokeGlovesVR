using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform object1;
    [SerializeField]
    private Transform object2;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, object1.position);
        _lineRenderer.SetPosition(1, object2.position);
    }
}
