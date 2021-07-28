using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [SerializeField]
    private Text hintText;

    private float _startAngle = 0f;
    private float _endAngle = -90f;
    private float _startDamp = 0f;
    private float _endDamp = 15f; // TODO: Make 3 different levels?
    private HingeJoint _joint;

    private void Start()
    {
        _joint = GetComponent<HingeJoint>();
        _startAngle = _joint.angle;
    }

    private void FixedUpdate()
    {
        var spring = _joint.spring;
        spring.damper = Remap(_joint.angle, _startAngle, _endAngle, _startDamp, _endDamp);
        _joint.spring = spring;

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
