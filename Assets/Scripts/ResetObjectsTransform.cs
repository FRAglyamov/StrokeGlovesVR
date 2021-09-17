using System.Collections.Generic;
using UnityEngine;

public class ResetObjectsTransform : MonoBehaviour
{
    [SerializeField] private List<Transform> objectTransforms;

    private List<Vector3> _positions = new List<Vector3>();
    private List<Quaternion> _rotations = new List<Quaternion>();
    private List<Vector3> _localScales = new List<Vector3>();
    void Start()
    {
        foreach (Transform t in objectTransforms)
        {
            _positions.Add(t.position);
            _rotations.Add(t.rotation);
            _localScales.Add(t.localScale);
        }
    }

    public void ResetObjects()
    {
        for (int i = 0; i < objectTransforms.Count; i++)
        {
            objectTransforms[i].position = _positions[i];
            objectTransforms[i].rotation = _rotations[i];
            objectTransforms[i].localScale = _localScales[i];

            var tmpBody = objectTransforms[i].GetComponent<Rigidbody>();
            tmpBody.velocity = Vector3.zero;
            tmpBody.angularVelocity = Vector3.zero;
        }
    }
}
