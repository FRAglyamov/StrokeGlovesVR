using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabPhysics : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField]
    private Transform _palmTransform;

    public GameObject grabbedObject = null;

    private FixedJoint _joint;
    private List<GameObject> items = new List<GameObject>();

    private bool _isGrab = false;
    public bool[,] detectorsFirstGroup = new bool[4, 3]; // phalanges of index, middle, ring, pinky
    public bool[][] detectorsSecondGroup = new bool[2][]; // palm and phalanges of thumb

    [SerializeField]
    private bool isFirstGroupDetect = false, isSecondGroupDetect = false;

    private void Start()
    {
        detectorsSecondGroup[0] = new bool[1]; // palm
        detectorsSecondGroup[1] = new bool[2]; // phalanges of thumb
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

            for (int i = 0; i < detectorsFirstGroup.GetLength(0); i++)
            {
                for (int j = 0; j < detectorsFirstGroup.GetLength(1); j++)
                {
                    if (detectorsFirstGroup[i, j])
                    {
                        Debug.Log($"Finger collision is true: i={i}, j={j}");
                    }
                }
            }

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
            //Debug.LogError("grabbedObject reference is null!");
            return;
        }
        if (grabbedObject.GetComponent<Joint>() == null)
        {
            _joint = grabbedObject.AddComponent<FixedJoint>();
            _joint.connectedArticulationBody = this.gameObject.GetComponent<ArticulationBody>();
        }
        _isGrab = true;
    }

    private void UnGrab()
    {
        Destroy(grabbedObject.GetComponent<Joint>());
        grabbedObject = null;
        _isGrab = false;
        // TODO: Add force for grabbed object based on hand velocity
    }

    //private void Grab(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    //{
    //    grabbedObject = GetClosestObject();
    //    if (grabbedObject.GetComponent<Joint>() == null)
    //    {
    //        _joint = grabbedObject.AddComponent<FixedJoint>();
    //        _joint.connectedArticulationBody = this.gameObject.GetComponent<ArticulationBody>();
    //    }
    //}

    //private void UnGrab(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    //{
    //    Destroy(grabbedObject.GetComponent<Joint>());
    //    grabbedObject = null;
    //    //_controller.selectAction.action.performed -= Grab;
    //    //_controller.selectAction.action.canceled -= UnGrab;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!items.Contains(other.gameObject))
        {
            items.Add(other.gameObject);
        }

        //_controller.selectAction.action.performed += Grab;
        //_controller.selectAction.action.canceled += UnGrab;
    }

    private void OnTriggerExit(Collider other)
    {
        items.Remove(other.gameObject);
    }

    private GameObject GetClosestObject()
    {
        if (items.Count == 0)
        {
            //Debug.LogError("0 items in triggers' collider array");
            return null;
        }

        float minDistance = 10f;
        GameObject closestObject = null;

        foreach (var item in items)
        {
            var distance = Vector3.Distance(item.transform.position, _palmTransform.position);
            if (distance < minDistance)
            {
                closestObject = item;
                minDistance = distance;
            }
        }

        return closestObject;
    }
}
