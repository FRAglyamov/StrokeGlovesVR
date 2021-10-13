using System.Collections.Generic;
using UnityEngine;

public class GrabManagerAveragePoint : MonoBehaviour
{
    public GameObject grabbedObject = null;

    private bool _isGrab = false;
    [SerializeField, Tooltip("Index, middle, ring, pinky")]
    private GrabDetectorAveragePoint[] lastPhalanges = new GrabDetectorAveragePoint[4];
    [SerializeField, Tooltip("Index, middle, ring, pinky")]
    private GrabDetectorAveragePoint[] midPhalanges = new GrabDetectorAveragePoint[4];
    [SerializeField]
    private GrabDetectorAveragePoint[] thumbPhalanges = new GrabDetectorAveragePoint[2];

    private Collider[] _lastPhalangesColliders = new Collider[4];
    private Collider[] _midPhalangesColliders = new Collider[4];
    private Collider[] _thumbPhalangesColliders = new Collider[2];

    [SerializeField]
    private bool isFirstGroupDetect = false, isSecondGroupDetect = false;
    [SerializeField]
    private LayerMask excludingHandsMask;

    [SerializeField]
    private GloveListenerArticulation gloveListener;

    private void Start()
    {
        for (int i = 0; i < lastPhalanges.Length; i++)
        {
            _lastPhalangesColliders[i] = lastPhalanges[i].GetComponent<Collider>();
            _midPhalangesColliders[i] = midPhalanges[i].GetComponent<Collider>();
        }
        for (int i = 0; i < thumbPhalanges.Length; i++)
        {
            _thumbPhalangesColliders[i] = thumbPhalanges[i].GetComponent<Collider>();
        }
    }

    private void Update()
    {
        UpdateGroupsDetection();

        if (!_isGrab && isFirstGroupDetect && isSecondGroupDetect && gloveListener.IsHaveRequiredFlex(5f))
        {
            Grab();
        }
        else if (_isGrab && (!isFirstGroupDetect || !isSecondGroupDetect || !gloveListener.IsHaveRequiredFlex(5f)))
        {
            UnGrab();
        }

    }

    private void UpdateGroupsDetection()
    {
        isFirstGroupDetect = false;
        isSecondGroupDetect = false;

        foreach (GrabDetectorAveragePoint lastPhalange in lastPhalanges)
        {
            if (lastPhalange.isTouching == true)
            {
                isFirstGroupDetect = true;
                break;
            }
        }
        if (!isFirstGroupDetect)
        {
            foreach (GrabDetectorAveragePoint midPhalange in midPhalanges)
            {
                if (midPhalange.isTouching == true)
                {
                    isFirstGroupDetect = true;
                    break;
                }
            }
        }
        foreach (GrabDetectorAveragePoint thumbPhalange in thumbPhalanges)
        {
            if (thumbPhalange.isTouching == true)
            {
                isSecondGroupDetect = true;
                break;
            }
        }
    }

    private GameObject GetObjectOnAveragePoint()
    {
        List<Vector3> touchPoints = new List<Vector3>();

        for (int i = 0; i < lastPhalanges.Length; i++)
        {
            if (lastPhalanges[i].isTouching == true)
            {
                touchPoints.Add(_lastPhalangesColliders[i].bounds.center);
            }
            if (midPhalanges[i].isTouching == true)
            {
                touchPoints.Add(_midPhalangesColliders[i].bounds.center);
            }
        }
        for (int i = 0; i < thumbPhalanges.Length; i++)
        {
            touchPoints.Add(_thumbPhalangesColliders[i].bounds.center);
        }

        Vector3 averagePoint = touchPoints[0];
        for (int i = 1; i < touchPoints.Count; i++)
        {
            averagePoint += touchPoints[i];
        }
        averagePoint /= (float)touchPoints.Count;

        Collider[] intersecting = Physics.OverlapSphere(averagePoint, 0f, excludingHandsMask);
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
        }
        else
        {
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

        // Find a parent of object with rigidbody (if it is just a part of complex object).
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
    }
}
