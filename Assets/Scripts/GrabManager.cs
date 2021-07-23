using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField]
    private Collider grabTrigger;

    public GameObject grabbedObject = null;

    private List<GameObject> _items = new List<GameObject>();

    private bool _isGrab = false;
    public bool[,] detectorsFirstGroup = new bool[4, 3]; // phalanges of index, middle, ring, pinky
    public bool[][] detectorsSecondGroup = new bool[2][]; // palm and phalanges of thumb

    [SerializeField]
    private bool isFirstGroupDetect = false, isSecondGroupDetect = false;

    private void Start()
    {
        detectorsSecondGroup[0] = new bool[1]; // palm
        detectorsSecondGroup[1] = new bool[2]; // phalanges of thumb

        if (grabTrigger == null)
        {
            grabTrigger = GetComponent<Collider>();
        }
    }

    private void Update()
    {
        isFirstGroupDetect = false;
        isSecondGroupDetect = false;

        foreach (var d1 in detectorsFirstGroup)
        {
            if (d1 == true)
            {
                isFirstGroupDetect = true;
                break;
            }
        }
        foreach (var d2array in detectorsSecondGroup)
        {
            foreach (var d2 in d2array)
            {
                if (d2 == true)
                {
                    isSecondGroupDetect = true;
                    break;
                }
            }
        }

        if (!_isGrab && isFirstGroupDetect && isSecondGroupDetect)
        {
            //for (int i = 0; i < detectorsFirstGroup.GetLength(0); i++)
            //{
            //    for (int j = 0; j < detectorsFirstGroup.GetLength(1); j++)
            //    {
            //        if (detectorsFirstGroup[i, j])
            //        {
            //            Debug.Log($"Finger collision is true: i={i}, j={j}");
            //        }
            //    }
            //}

            Grab();
        }
        else if(_isGrab && (!isFirstGroupDetect || !isSecondGroupDetect))
        {
            UnGrab();
        }

    }

    private void Grab()
    {
        grabbedObject = GetClosestObject();
        if (grabbedObject == null)
        {
            return;
        }

        // Find a parent of object with rigidbody (if it is only a part of complex object)
        if (grabbedObject.GetComponent<Rigidbody>() == null && grabbedObject.GetComponentInParent<Rigidbody>()!=null)
        {
            grabbedObject = grabbedObject.GetComponentInParent<Rigidbody>().gameObject;
        }

        if (grabbedObject.GetComponent<Joint>() == null)
        {
            var joint = grabbedObject.AddComponent<FixedJoint>();
            joint.connectedArticulationBody = this.gameObject.GetComponent<ArticulationBody>();
        }
        _isGrab = true;
    }

    private void UnGrab()
    {
        Destroy(grabbedObject.GetComponent<Joint>());
        grabbedObject = null;
        _isGrab = false;
        // TODO: Add force for grabbed object based on hand velocity?
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_items.Contains(other.gameObject))
        {
            _items.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _items.Remove(other.gameObject);
    }

    private GameObject GetClosestObject()
    {
        if (_items.Count == 0)
        {
            return null;
        }

        float minDistance = 10f;
        GameObject closestObject = null;

        foreach (var item in _items)
        {
            var distance = Vector3.Distance(item.transform.position, grabTrigger.bounds.center);
            if (distance < minDistance)
            {
                closestObject = item;
                minDistance = distance;
            }
        }

        return closestObject;
    }
}
