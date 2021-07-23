using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    //public bool isTouching = false;

    [SerializeField]
    private GrabManager grabManager;

    [SerializeField, Range(1, 2)]
    private short detectorsGroup = 1;
    [SerializeField]
    private short arrayIndex1;
    [SerializeField]
    private short arrayIndex2;


    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.GetContact(0).otherCollider == null)
        //{
        //    return;
        //}
        foreach (var contact in collision.contacts)
        {
            if (contact.thisCollider.name == gameObject.name)
            {
                if (grabManager.grabbedObject != null && collision.gameObject == grabManager.grabbedObject
                        || grabManager.grabbedObject == null)
                {
                    //isTouching = true;
                    //Debug.Log($"{gameObject.name} is touching = true");
                    if (detectorsGroup == 1)
                    {
                        grabManager.detectorsFirstGroup[arrayIndex1, arrayIndex2] = true;
                    }
                    else
                    {
                        grabManager.detectorsSecondGroup[arrayIndex1][arrayIndex2] = true;
                    }
                }
            }
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.GetContact(0).otherCollider == null)
        //{
        //    return;
        //}
        if (collision.gameObject == grabManager.grabbedObject || grabManager.grabbedObject == null)
        {
            //isTouching = false;
            //Debug.Log($"{gameObject.name} is touching = false");
            if (detectorsGroup == 1)
            {
                grabManager.detectorsFirstGroup[arrayIndex1, arrayIndex2] = false;
            }
            else
            {
                grabManager.detectorsSecondGroup[arrayIndex1][arrayIndex2] = false;
            }
        }
    }
}
