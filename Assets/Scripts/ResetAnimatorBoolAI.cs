using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorBoolAI : ResetAnimatorBool
{
    public string isPhaseShifting = "isPhaseShifting";
    public bool isPhaseShiftingStatus = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(isPhaseShifting, isPhaseShiftingStatus);
    }
}
