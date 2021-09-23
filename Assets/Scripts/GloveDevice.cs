using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public struct GloveDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('G', 'L', 'O', 'V');

    [InputControl(layout = "Axis")]
    public float index;
    [InputControl(layout = "Axis")]
    public float middle;
    [InputControl(layout = "Axis")]
    public float pinky;
    [InputControl(layout = "Axis")]
    public float ring;
    [InputControl(layout = "Axis")]
    public float thumb;
    [InputControl]
    public Quaternion deviceRotation;
    //[InputControl(layout = "Button")]
    //public bool isTracked;
    //[InputControl]
    //public int trackingState;
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(GloveDeviceState))]
public class GloveDevice : InputDevice, IInputUpdateCallbackReceiver
{
#if UNITY_EDITOR
    static GloveDevice()
    {
        Initialize();
    }

#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterLayout<GloveDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("Glove"));
    }
    public AxisControl index { get; private set; }
    public AxisControl middle { get; private set; }
    public AxisControl pinky { get; private set; }
    public AxisControl ring { get; private set; }
    public AxisControl thumb { get; private set; }
    //public QuaternionControl rotation { get; private set; }
    public QuaternionControl deviceRotation { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        index = GetChildControl<AxisControl>("index");
        middle = GetChildControl<AxisControl>("middle");
        pinky = GetChildControl<AxisControl>("pinky");
        ring = GetChildControl<AxisControl>("ring");
        thumb = GetChildControl<AxisControl>("thumb");
        deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
    }

    public static GloveDevice current { get; private set; }
    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Glove Device/Create Device")]
    private static void CreateDevice()
    {
        InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "Glove",
            product = "Glove Product"
        });
    }

    [MenuItem("Tools/Glove Device/Remove Device")]
    private static void RemoveDevice()
    {
        var gloveDevice = InputSystem.devices.FirstOrDefault(x => x is GloveDevice);
        if (gloveDevice != null)
            InputSystem.RemoveDevice(gloveDevice);
    }

#endif

    public void OnUpdate()
    {
        //var state = new GloveDeviceState();
        //Debug.Log(ring.ReadValue());
        //InputSystem.QueueStateEvent(this, state);
    }
}
