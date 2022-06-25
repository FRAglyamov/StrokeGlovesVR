using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    private Rigidbody _rb;
    private HingeJoint _doorHingeJoint;
    [SerializeField]
    private HingeJoint handleHingeJoint;
    [SerializeField, Range(0f, 90f)]
    private float requiredHandleAngle = 75f;
    [SerializeField, Range(0f, 90f)]
    private float requiredDoorAngle = 25f;
    [SerializeField]
    private Text hintText;
    private bool isHandleStage = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _doorHingeJoint = GetComponent<HingeJoint>();
        //_handleHingeJoint = GetComponentInChildren<HingeJoint>();
        hintText.text = "��������� ������� �����";
    }

    private void Update()
    {
        if (isHandleStage)
        {
            float handleAngle = Mathf.Abs(handleHingeJoint.angle);
            hintText.text = "��������� ������� �����\n";
            hintText.text += $"�������: {handleAngle:f1} �� {requiredHandleAngle}";
            
            if (handleAngle >= requiredHandleAngle)
            {
                _rb.isKinematic = false;
                isHandleStage = false;
                //hintText.text = "�������� ����� �� ����";
                // TODO: Add sound or visual effect to make it more clear?
            }
        }
        else
        {
            float doorAngle = Mathf.Abs(_doorHingeJoint.angle);
            hintText.text = "�������� ����� �� ����\n";
            hintText.text += $"�������: {doorAngle:f1} �� {requiredDoorAngle}";
            if (doorAngle >= requiredDoorAngle)
            {
                hintText.text = "���������� ���������";
                AssistantSystem.Instance.progressSystem.SaveResultIntoJSON();
                this.enabled = false;
            }
        }

    }
}
