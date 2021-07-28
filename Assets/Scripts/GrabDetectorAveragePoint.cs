using UnityEngine;

[System.Serializable]
public class GrabDetectorAveragePoint : MonoBehaviour
{
    public bool isTouching = false;

    [SerializeField]
    private GrabManagerAveragePoint grabManager;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.thisCollider.name == gameObject.name)
            {
                if (grabManager.grabbedObject != null && collision.gameObject == grabManager.grabbedObject
                        || grabManager.grabbedObject == null)
                {
                    isTouching = true;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.thisCollider.name == gameObject.name)
            {
                if(grabManager.grabbedObject != null && collision.gameObject == grabManager.grabbedObject)
                {
                    isTouching = true;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouching = false;
    }
}
