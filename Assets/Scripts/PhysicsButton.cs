using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] 
    private float threshold = 0.1f;

    [SerializeField] 
    private float deadZone = 0.025f;

    private bool _isPressed;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;

    public UnityEvent onPressed, onReleased;

    private void Start()
    {
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        if(!_isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        if (_isPressed && GetValue() - threshold <= 0)
        {
            Released();
        }
    }

    private float GetValue()
    {
        float value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;
        if(Mathf.Abs(value) < deadZone)
        {
            value = 0f;
        }
        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Pressed()
    {
        _isPressed = true;
        Debug.Log("Button pressed");
        onPressed.Invoke();
    }

    private void Released()
    {
        _isPressed = false;
        Debug.Log("Button released");
        onReleased.Invoke();
    }
}
