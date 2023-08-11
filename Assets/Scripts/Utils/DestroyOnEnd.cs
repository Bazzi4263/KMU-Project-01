using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEnd : StateMachineBehaviour
{
    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.transform.parent != null) Destroy(animator.gameObject.transform.parent.gameObject);
        else Destroy(animator.gameObject);
    }
}
