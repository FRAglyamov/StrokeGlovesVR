using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ViveTrackerPos : MonoBehaviour
{
    // Start is called before the first frame update
    InputDevice tracker;
    void Start()
    {
        // on start
        var allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        tracker = allDevices.FirstOrDefault(d => d.role == InputDeviceRole.HardwareTracker);
        // NOTE!!! pos and rot are in world position so you have to translate it to floor
    }

    // Update is called once per frame
    void Update()
    {
        // on update
        tracker.TryGetFeatureValue(CommonUsages.devicePosition, out var pos);

        tracker.TryGetFeatureValue(CommonUsages.deviceRotation, out var rot);
        Debug.Log(pos + " " + rot);
    }
}
