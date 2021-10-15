using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent onEnter, onStay, onExit;

    private void OnTriggerEnter(Collider other)
    {
        onEnter.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        onStay.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        onExit.Invoke();
    }
}
