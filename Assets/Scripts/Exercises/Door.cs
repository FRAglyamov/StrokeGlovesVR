using UnityEngine;

public class Door : MonoBehaviour
{
    private Rigidbody _rb;
    private HingeJoint _doorHingeJoint;
    private HingeJoint _handleHingeJoint;
    [SerializeField, Range(0f, 90f)]
    private float requiredHandleAngle = 75f;
    [SerializeField, Range(0f, 90f)]
    private float requiredDoorAngle = 15f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _doorHingeJoint = GetComponent<HingeJoint>();
        _handleHingeJoint = GetComponentInChildren<HingeJoint>();
    }

    private void Update()
    {
        if(_handleHingeJoint.angle >= requiredHandleAngle)
        {
            _rb.isKinematic = false;
            // TODO: Add sound or visual effect to make it more clear?
        }
        if(_doorHingeJoint.angle >= requiredDoorAngle)
        {
            // Exercise complete.
        }
    }
}
