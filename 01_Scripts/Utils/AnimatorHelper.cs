using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    private Animator mAnim = null;

    void Awake()
    {
        mAnim = GetComponent<Animator>();
    }

    public void SetTrigger(AnimActionID actionID)
    {
        mAnim.SetInteger(AnimParam.ActionType, (int)actionID);
        mAnim.SetTrigger(AnimParam.DoActionTrigger);
    }

    public float GetCurrentNormalizedTime(int aniLayerIndex)
    {
        return mAnim.GetCurrentAnimatorStateInfo(aniLayerIndex).normalizedTime;
    }

    // public void SetAnimParamVelocityY(float velocityY)
    // {
    //     mAnim.SetFloat(AnimParam.VelocityY, velocityY);
    // }
    // public void SetAnimParamHangingRight(bool isHangingRight)
    // {
    //     mAnim.SetBool(AnimParam.IsHangingRight, isHangingRight);
    // }
    // public void SetAnimParamIsSitDown(bool isSitDown)
    // {
    //     mAnim.SetBool(AnimParam.IsSitDown, isSitDown);
    // }
    // public void SetAnimParamIsRun(bool isRun)
    // {
    //     mAnim.SetBool(AnimParam.IsRun, isRun);
    // }
    // public void SetAnimParamDerivedType(int derivedType)
    // {
    //     mAnim.SetInteger(AnimParam.DerivedType, derivedType);
    // }
    // public bool GetAnimParamIsRun()
    // {
    //     return mAnim.GetBool(AnimParam.IsRun);
    // }
}
