using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;
// using static UnityEngine.InputSystem.InputAction;

public class InputWrapper
{
    // protected override float GetHorizontal() { return Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0); }
    // protected override bool KeyTrigger_Jump() { return Input.GetKeyDown(KeyCode.UpArrow); }
    // protected override bool KeyDown_Attack1() { return Input.GetKey(KeyCode.Z); }
    // protected override bool KeyDown_Attack2() { return Input.GetKey(KeyCode.X); }
    // protected override bool KeyDown_Rolling() { return Input.GetKey(KeyCode.C); }
    // protected override bool KeyDown_Throw() { return Input.GetKey(KeyCode.V); }
    // protected override float GetVertical() { return Input.GetKey(KeyCode.DownArrow) ? -1 : (Input.GetKey(KeyCode.UpArrow) ? 1 : 0); }

    // private MetalSuitsInputAction mNewInputSystem = null;

    // void Awake()
    // {
    //     mNewInputSystem = new MetalSuitsInputAction();

    //     mNewInputSystem.Player.MoveKey.started += context => OnPressVector(context);
    //     mNewInputSystem.Player.NormalAttackKey.started += context => OnPressButton(context, CharacterInputType.AttackNormal);
    //     mNewInputSystem.Player.MeleeAttackKey.started += context => OnPressButton(context, CharacterInputType.AttackMelee);
    //     mNewInputSystem.Player.SpecialAttackKey.started += context => OnPressButton(context, CharacterInputType.Throw);
    //     mNewInputSystem.Player.RollKey.started += context => OnPressButton(context, CharacterInputType.Rolling);
    //     mNewInputSystem.Player.JumpKey.started += context => OnPressButton(context, CharacterInputType.Jump);
    //     mNewInputSystem.Player.UseKey.started += context => OnPressButton(context, CharacterInputType.Interact);
    //     mNewInputSystem.Player.PauseButtonKey.started += context => OnPressButton(context, CharacterInputType.Pause);
    // }

    // void OnEnable()
    // {
    //     mNewInputSystem.Player.Enable();
    // }

    // void OnDisable()
    // {
    //     mNewInputSystem.Player.Disable();
    // }

    // void OnPressVector(CallbackContext context)
    // {
    //     Vector2 data = context.ReadValue<Vector2>();
    //     if (data.x != 0)
    //     {
    //         InvokeInputEvnt(CharacterInputType.MoveHori);
    //     }

    //     if (data.y != 0)
    //     {
    //         InvokeInputEvnt(CharacterInputType.MoveVert);
    //     }
    // }
    // void OnPressButton(CallbackContext context, CharacterInputType type)
    // {
    //     InvokeInputEvnt(type);
    // }

    // public override float GetInput(CharacterInputType type)
    // {
    //     if (Lock) return 0;

    //     switch (type)
    //     {
    //         case CharacterInputType.MoveHori: return ClipX(mNewInputSystem.Player.MoveKey.ReadValue<Vector2>().x);
    //         case CharacterInputType.MoveVert: return ClipY(mNewInputSystem.Player.MoveKey.ReadValue<Vector2>().y);
    //         case CharacterInputType.AttackNormal: return mNewInputSystem.Player.NormalAttackKey.ReadValue<float>();
    //         case CharacterInputType.AttackMelee: return mNewInputSystem.Player.MeleeAttackKey.ReadValue<float>();
    //         case CharacterInputType.Throw: return mNewInputSystem.Player.SpecialAttackKey.ReadValue<float>();
    //         case CharacterInputType.Rolling: return mNewInputSystem.Player.RollKey.ReadValue<float>();
    //         case CharacterInputType.Jump: return mNewInputSystem.Player.JumpKey.ReadValue<float>();
    //         case CharacterInputType.Interact: return mNewInputSystem.Player.UseKey.ReadValue<float>();
    //         case CharacterInputType.Pause: return mNewInputSystem.Player.PauseButtonKey.ReadValue<float>();
    //         default: break;
    //     }
    //     return base.GetInput(type);
    // }

    // // 조이스틱의 아날로그방식 절상 : 좌우방향은 살짝이라도 움직이면 입력 감지
    // private float ClipX(float analogInput)
    // {
    //     return analogInput > 0 ? 1 : (analogInput < 0 ? -1 : 0);
    // }
    // // 조이스틱의 아날로그방식 절상 : 상하방향은 절반이상 움직이면 입력 감지
    // private float ClipY(float analogInput)
    // {
    //     return analogInput > 0.5f ? 1 : (analogInput < -0.5f ? -1 : 0);
    // }

}

