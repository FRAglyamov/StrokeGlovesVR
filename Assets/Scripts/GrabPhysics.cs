using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabPhysics : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField]
    private Transform _palmTransform;

    public GameObject grabbedObject;

    private FixedJoint _joint;
    private List<GameObject> items = new List<GameObject>();

    private void Grab(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        grabbedObject = GetClosestObject();
        if (grabbedObject.GetComponent<Joint>() == null)
        {
            _joint = grabbedObject.AddComponent<FixedJoint>();
            _joint.connectedArticulationBody = this.gameObject.GetComponent<ArticulationBody>();
        }
    }

    private void UnGrab(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Destroy(grabbedObject.GetComponent<Joint>());
        grabbedObject = null;
        _controller.selectAction.action.performed -= Grab;
        _controller.selectAction.action.canceled -= UnGrab;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!items.Contains(other.gameObject))
        {
            items.Add(other.gameObject);
        }

        _controller.selectAction.action.performed += Grab;
        _controller.selectAction.action.canceled += UnGrab;
    }

    private void OnTriggerExit(Collider other)
    {
        items.Remove(other.gameObject);
    }

    private GameObject GetClosestObject()
    {
        if (items.Count == 0)
        {
            Debug.LogError("0 items in triggers' collider array");
            return null;
        }

        float minDistance = 100f;
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
