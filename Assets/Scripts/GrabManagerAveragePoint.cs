using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabManagerAveragePoint : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    public GameObject grabbedObject = null;

    private bool _isGrab = false;
    [SerializeField]
    private GrabDetectorAveragePoint[] lastPhalanges = new GrabDetectorAveragePoint[4];
    [SerializeField]
    private GrabDetectorAveragePoint[] midPhalanges = new GrabDetectorAveragePoint[4];
    [SerializeField]
    private GrabDetectorAveragePoint[] thumbPhalanges = new GrabDetectorAveragePoint[2];

    [SerializeField]
    private bool isFirstGroupDetect = false, isSecondGroupDetect = false;
    [SerializeField]
    private LayerMask excludingHandsMask;

    private void Update()
    {
        isFirstGroupDetect = false;
        isSecondGroupDetect = false;

        foreach (var d1 in lastPhalanges)
        {
            if (d1.isTouching == true)
            {
                isFirstGroupDetect = true;
                Debug.Log($"Last phalanges isTouching: {d1.gameObject.name}");
                break;
            }
        }
        if (!isFirstGroupDetect)
        {
            foreach (var d1 in midPhalanges)
            {
                if (d1.isTouching == true)
                {
                    isFirstGroupDetect = true;
                    Debug.Log($"Mid phalanges isTouching: {d1.gameObject.name}");
                    break;
                }
            }
        }
        foreach (var d2 in thumbPhalanges)
        {
            if (d2.isTouching == true)
            {
                isSecondGroupDetect = true;
                break;
            }
        }

        if (!_isGrab && isFirstGroupDetect && isSecondGroupDetect)
        {
            Grab();
        }
        else if (_isGrab && (!isFirstGroupDetect || !isSecondGroupDetect))
        {
            UnGrab();
        }

    }

    private GameObject GetObjectOnAveragePoint()
    {
        List<Vector3> touchPoints = new List<Vector3>();
        foreach (var d1 in lastPhalanges)
        {
            if (d1.isTouching == true)
            {
                touchPoints.Add(d1.transform.position);
            }
        }
        foreach (var d1 in midPhalanges)
        {
            if (d1.isTouching == true)
            {
                touchPoints.Add(d1.transform.position);
            }
        }
        foreach (var d2 in thumbPhalanges)
        {
            if (d2.isTouching == true)
            {
                touchPoints.Add(d2.transform.position);
            }
        }

        Vector3 averagePoint = touchPoints[0];
        for (int i = 1; i < touchPoints.Count; i++)
        {
            averagePoint += touchPoints[i];
        }
        averagePoint /= (float)touchPoints.Count;
        //Debug.Log($"Average point: x={averagePoint.x}, y={averagePoint.y}, z={averagePoint.z}");

        Collider[] intersecting = Physics.OverlapSphere(averagePoint, 0.03f, excludingHandsMask);
        if (intersecting.Length > 0)
        {
            
            GameObject closestObject = intersecting[0].gameObject;
            float minDistance = Vector3.Distance(intersecting[0].transform.position, averagePoint);
            for (int i = 1; i < intersecting.Length; i++)
            {
                var distance = Vector3.Distance(intersecting[i].transform.position, averagePoint);
                if (distance < minDistance)
                {
                    closestObject = intersecting[i].gameObject;
                    minDistance = distance;
                }
            }
            return closestObject;

            //Debug.Log($"Intersect with {intersecting[0].gameObject.name}");
            //return intersecting[0].gameObject;
        }
        else
        {
            //Debug.Log($"Zero intersections");
            if (grabbedObject != null)
            {
                UnGrab();
            }
            return null;
        }

    }

    private void Grab()
    {
        grabbedObject = GetObjectOnAveragePoint();

        if (grabbedObject == null)
        {
            _isGrab = false;
            return;
        }

        // Find a parent of object with rigidbody (if it is just a part of complex object)
        if (grabbedObject.GetComponent<Rigidbody>() == null && grabbedObject.GetComponentInParent<Rigidbody>() != null)
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
}
