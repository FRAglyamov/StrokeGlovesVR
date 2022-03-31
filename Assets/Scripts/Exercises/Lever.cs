using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [SerializeField]
    private Text hintText;

    private float _startAngle = 0f;
    private float _endAngle = -90f;
    [SerializeField]
    private float _startDamp = 0f;
    [SerializeField]
    private float _endDamp = 50f;
    [SerializeField]
    private float _startMass = 10f;
    [SerializeField]
    private float _endMass = 50f;
    private Rigidbody _rb;
    private HingeJoint _joint;
    // TODO: Make 3 different levels?

    private void Start()
    {
        _joint = GetComponent<HingeJoint>();
        _startAngle = _joint.angle;
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var spring = _joint.spring;
        spring.damper = Remap(_joint.angle, _startAngle, _endAngle, _startDamp, _endDamp);
        _joint.spring = spring;
        _rb.mass = Remap(_joint.angle, _startAngle, _endAngle, _startMass, _endMass);

        if (_joint.angle >= _endAngle)
        {
            hintText.text = "Упражнение завершено";
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
