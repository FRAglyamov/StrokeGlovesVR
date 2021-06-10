using UnityEngine;
using UnityEngine.UI;

public class BounceMeter : MonoBehaviour
{
    [SerializeField]
    private Text hintText;
    [SerializeField]
    private int requiredBounceAmount = 3;

    private int _currentBounceAmount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
        {
            return;
        }
        _currentBounceAmount++;
        if (_currentBounceAmount >= requiredBounceAmount)
        {
            hintText.text = "���������� ���������";
        }
        else
        {
            hintText.text = "���������� �������� �������������: " + _currentBounceAmount + " �� " + requiredBounceAmount;
        }
    }
}
