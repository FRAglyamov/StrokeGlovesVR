using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [SerializeField]
    private Text hintText;

    private float _startAngle = 0f;
    private float _endAngle = -80f;
    [SerializeField]
    private float startDamp = 0f;
    [SerializeField]
    private float endDamp = 50f;
    [SerializeField]
    private float startMass = 10f;
    [SerializeField]
    private float endMass = 50f;
    private Rigidbody _rb;
    private HingeJoint _joint;
    // TODO: Make 3 different levels?

    private void Start()
    {
        _joint = GetComponent<HingeJoint>();
        _startAngle = _joint.angle;
        _rb = GetComponent<Rigidbody>();
        hintText.text = "Потяните рычаг";
        AssistantSystem.Instance.progressSystem.StartTimer();
    }

    private void FixedUpdate()
    {
        // Closer to end - tighter.
        var spring = _joint.spring;
        spring.damper = Remap(_joint.angle, _startAngle, _endAngle, startDamp, endDamp);
        _joint.spring = spring;
        _rb.mass = Remap(_joint.angle, _startAngle, _endAngle, startMass, endMass);

        float leverAngle = Mathf.Abs(_joint.angle);
        float requiredAngle = Mathf.Abs(_endAngle);
        hintText.text = $"Угол рычага: {leverAngle:f1} из {requiredAngle}";

        if (leverAngle >= requiredAngle)
        {
            hintText.text = "Упражнение завершено!";
            AssistantSystem.Instance.progressSystem.SaveResultIntoJSON();
            this.enabled = false;
            return;
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
