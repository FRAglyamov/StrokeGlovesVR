using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDriveTarget : MonoBehaviour
{
    [SerializeField] bool _modifyFromEditor;
    [SerializeField] ArticulationBody[] _bodies;
    [SerializeField] List<float> _targets = new List<float>(); 

    private int _layerMask;

    private void Awake()
    {
        IgnoreCollision();
        for (int i = 0; i < _bodies.Length; i++)
        {
            if (i == 0)
            { // first art body has 2 DOFs: y and z
                _targets.Add(_bodies[i].yDrive.target);
                _targets.Add(_bodies[i].zDrive.target);
            }
            else // second two drives have only 1 DOF: x
                _targets.Add(_bodies[i].xDrive.target);
        }
    }

    private void FixedUpdate()
    {
        if (!_modifyFromEditor)
            return;

        for (int i = 0; i < _bodies.Length; i++)
        {
            if (i == 0)
            { // first art body has 2 DOFs: y and z
                var yDrive = _bodies[i].yDrive;
                var zDrive = _bodies[i].yDrive;
                yDrive.target = _targets[i];
                zDrive.target = _targets[i + 1];

                _bodies[i].yDrive = yDrive;
                _bodies[i].zDrive = zDrive;
            }
            else // second two drives have only 1 DOF: x
            {
                var xDrive = _bodies[i].xDrive;
                xDrive.target = _targets[i + 1];
                _bodies[i].xDrive = xDrive;
            }
        }
    }

    private void IgnoreCollision()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
    }

}
