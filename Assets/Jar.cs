using UnityEngine;
using UnityEngine.UI;

// TODO?: ��� ������� �������������� ����� �������� ������ ������ � ������������ � ���������
public class Jar : MonoBehaviour
{

    [SerializeField, Range(0f,360f)]
    private float requiredLidRotation = 170f;
    [SerializeField]
    private Text hintText;

    private HingeJoint _joint;

    private void Start()
    {
        _joint = GetComponent<HingeJoint>();
    }

    private void Update()
    {
        if(_joint == null)
        {
            return;
        }
        if(_joint.angle >= requiredLidRotation)
        {
            Destroy(_joint);
            hintText.text = "���������� ���������";
        }
    }

}
