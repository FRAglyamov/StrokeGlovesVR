using UnityEngine;

public class GlassTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallStepManager.Instance.isBallInGlass = true;
        }
    }
}
