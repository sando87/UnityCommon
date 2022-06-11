using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum InputType
{
    None, Click, KeyA, KeyB
}

public class InputWrapper : SingletonMono<InputWrapper>
{
    private InputActions mIA = null;

    public event Action<InputType> EventDownTriggered;
    public event Action<InputType> EventUpTriggered;

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
}

