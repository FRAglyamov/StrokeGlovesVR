using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && BallStepManager.Instance.isBallInGlass)
        {
            BallStepManager.Instance.isBallDrinked = true;
        }
    }
}
