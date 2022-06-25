using UnityEngine;
using UnityEngine.UI;

public class BoxAndBlocksTest : MonoBehaviour
{
    [SerializeField]
    private Text hintText;
    private int requiredCubesAmount = 9;
    private int _movedCubesAmount = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            _movedCubesAmount++;
            if (_movedCubesAmount >= requiredCubesAmount)
            {
                hintText.text = "���������� ���������!";
                AssistantSystem.Instance.progressSystem.SaveResultIntoJSON();
                this.enabled = false;
                return;
            }
            hintText.text = "���������� ������ �� ����� ����� ������� � ������.\n";
            hintText.text += $"������� ���-��: {_movedCubesAmount} �� {requiredCubesAmount}";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            _movedCubesAmount--;
            hintText.text = "���������� ������ �� ����� ����� ������� � ������.\n";
            hintText.text += $"������� ���-��: {_movedCubesAmount} �� {requiredCubesAmount}";
        }
    }
}
