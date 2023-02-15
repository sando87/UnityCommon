using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum InputType
{
    None, Click, KeyA, KeyB
}
public enum InputDeviceType
{
    None = 0,
    DualShock = 1,
    XInput = 2,
    Switch = 3,
    Keyboard = 4,
    FastKeyboard = 4,
}

public class InputWrapper : SingletonMono<InputWrapper>
{
    private InputActions mIA = null;

    public event Action<InputType> EventDownTriggered;
    public event Action<InputType> EventUpTriggered;

    public event System.Action<InputDeviceType> EventChangedActiveDevice = null;
    public InputDeviceType CurrentActiveDevice { get; private set; } = InputDeviceType.None;

    protected override void Awake()
    {
        base.Awake();

        mIA = new InputActions();

        mIA.InGame.OnClick.started += context => OnPressButton(context, InputType.Click);
        mIA.InGame.KeyA.started += context => OnPressButton(context, InputType.KeyA);
        mIA.InGame.KeyB.started += context => OnPressButton(context, InputType.KeyB);

        mIA.InGame.OnClick.canceled += context => OnReleaseButton(context, InputType.Click);
        mIA.InGame.KeyA.canceled += context => OnReleaseButton(context, InputType.KeyA);
        mIA.InGame.KeyB.canceled += context => OnReleaseButton(context, InputType.KeyB);
    }

    void Start()
    {
        // Device가 연결이 해제되거나 새로 연결된 경우 호출됨
        // InputSystem.onDeviceChange += (device, change) =>
        // {
        //     LOG.trace(device.name);
        //     LOG.trace(change);
        // };

        // 특정 액션과 연결된 모든 버튼이 눌리거나 움직이는 등 입력이 일어날때마나 호출됨
        InputSystem.onActionChange += (device, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                InputAction inputAction = (InputAction)device;
                InputControl lastControl = inputAction.activeControl;
                InputDevice currentDevice = lastControl.device;

                InputDeviceType deviceType = DeviceNameToType(currentDevice.name);
                if (CurrentActiveDevice != deviceType)
                {
                    CurrentActiveDevice = deviceType;
                    EventChangedActiveDevice?.Invoke(deviceType);
                }
            }
        };
    }

    void OnEnable()
    {
        mIA.InGame.Enable();
    }

    void OnDisable()
    {
        mIA.InGame.Disable();
    }

    void OnPressButton(CallbackContext context, InputType type)
    {
        EventDownTriggered?.Invoke(type);
    }
    void OnReleaseButton(CallbackContext context, InputType type)
    {
        EventUpTriggered?.Invoke(type);
    }

    public float GetInput(InputType type)
    {
        if(mIA == null) return 0;

        switch (type)
        {
            case InputType.Click: return mIA.InGame.OnClick.ReadValue<float>();
            case InputType.KeyA: return mIA.InGame.KeyA.ReadValue<float>();
            case InputType.KeyB: return mIA.InGame.KeyB.ReadValue<float>();
            default: break;
        }
        return 0;
    }

    public Vector2 MousePosition()
    {
        if (mIA == null) return Vector2.zero;

        Vector2 pos = mIA.InGame.PointerMoving.ReadValue<Vector2>();
        return pos;
    }

    public bool IsKeyDownTrigger_F1()
    {
        return Keyboard.current.f1Key.wasPressedThisFrame;
    }

    private InputDeviceType DeviceNameToType(string deviceName)
    {
        if (deviceName.Contains("Keyboard"))
            return InputDeviceType.Keyboard;
        else if (deviceName.Contains("DualShock"))
            return InputDeviceType.DualShock;
        else if (deviceName.Contains("XInput"))
            return InputDeviceType.XInput;
        else if (deviceName.Contains("Mouse"))
            return InputDeviceType.None;

        LOG.warn("No Matching DeviceName : " + deviceName);
        return InputDeviceType.None;
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = MousePosition();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}

