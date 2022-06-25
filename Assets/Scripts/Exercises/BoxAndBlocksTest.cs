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
                hintText.text = "Упражнение завершено!";
                AssistantSystem.Instance.progressSystem.SaveResultIntoJSON();
                this.enabled = false;
                return;
            }
            hintText.text = "Переложите кубики из левой части коробки в правую.\n";
            hintText.text += $"Текущее кол-во: {_movedCubesAmount} из {requiredCubesAmount}";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            _movedCubesAmount--;
            hintText.text = "Переложите кубики из левой части коробки в правую.\n";
            hintText.text += $"Текущее кол-во: {_movedCubesAmount} из {requiredCubesAmount}";
        }
    }
}
