//using System.Linq;
//using System.Runtime.InteropServices;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Controls;
//using UnityEngine.InputSystem.Layouts;
//using UnityEngine.InputSystem.LowLevel;
//using UnityEngine.InputSystem.Utilities;
//using UnityEngine.InputSystem.XR;
//using UnityEngine.InputSystem.HID;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//public struct ViveTrackerDeviceState : IInputStateTypeInfo
//{
//    public FourCC format => new FourCC('T', 'R', 'A', 'C');

//    //[InputControl]
//    //public Quaternion deviceRotation;
//    //[InputControl(layout = "Button")]
//    //public bool isTracked;
//    //[InputControl]
//    //public int trackingState;
//}

//#if UNITY_EDITOR
//[InitializeOnLoad]
//#endif
//[InputControlLayout(stateType = typeof(ViveTrackerDeviceState))]
//public class ViveTrackerDevice : TrackedDevice, IInputUpdateCallbackReceiver
//{
//#if UNITY_EDITOR
//    static ViveTrackerDevice()
//    {
//        Initialize();
//    }

//#endif

//    [RuntimeInitializeOnLoadMethod]
//    private static void Initialize()
//    {
//        InputSystem.RegisterLayout<ViveTrackerDevice>(
//            matches: new InputDeviceMatcher()
//                //.WithInterface("HID")
//                //.WithManufacturer("Valve Software")
//                .WithProduct("Watchman Dongle"));
//    }
//    //public QuaternionControl rotation { get; private set; }
//    //public QuaternionControl deviceRotation { get; private set; }

//    protected override void FinishSetup()
//    {
//        base.FinishSetup();
//        //deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
//    }

//    public static ViveTrackerDevice current { get; private set; }
//    public override void MakeCurrent()
//    {
//        base.MakeCurrent();
//        current = this;
//    }

//    protected override void OnRemoved()
//    {
//        base.OnRemoved();
//        if (current == this)
//            current = null;
//    }

//#if UNITY_EDITOR
//    //[MenuItem("Tools/ViveTracker Device/Create Device")]
//    //private static void CreateDevice()
//    //{
//    //    InputSystem.AddDevice(new InputDeviceDescription
//    //    {
//    //        interfaceName = "ViveTracker",
//    //        product = "ViveTracker Product"
//    //    });
//    //}

//    //[MenuItem("Tools/ViveTracker Device/Remove Device")]
//    //private static void RemoveDevice()
//    //{
//    //    var viveTrackerDevice = InputSystem.devices.FirstOrDefault(x => x is ViveTrackerDevice);
//    //    if (viveTrackerDevice != null)
//    //        InputSystem.RemoveDevice(viveTrackerDevice);
//    //}

//#endif

//    public void OnUpdate()
//    {
//        //Debug.Log(HIDSupport.supportedHIDUsages);
//        //Debug.Log(this.native);
//        //var state = new ViveTrackerDeviceState();
//        //Debug.Log(ring.ReadValue());
//        //InputSystem.QueueStateEvent(this, state);
//    }
//}
