using UnityEngine;

namespace MalbersAnimations.Controller
{
    public class ModeBehaviour : StateMachineBehaviour
    {
        public ModeID ModeID;
        private MAnimal animal;
        private Mode modeOwner;
        private Ability AnimationAbility;

        [Tooltip("Calls 'Animation Tag Enter' on the Modes")]  
        public bool EnterMode = true;
        [Tooltip("Calls 'Animation Tag Exit' on the Modes")]
        public bool ExitMode = true;

        [Tooltip("Used for Playing an Ability and Finish on another Ability Mode")]
        [Hide("ExitMode", true, false)]
        public bool ExitOnAbility = false;
        [Tooltip("Next Ability to do on the Mode. -1 is the Default and the Exit On Ability Logic will be ignored")] 
        [Hide("ExitOnAbility" ,true,false)]public int ExitAbility = -1;
       

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponentInParent<MAnimal>();
            if (animal.ModeInt == Int_ID.Loop) return;            //Means is Looping

            if (ModeID == null) Debug.LogError("Mode behaviour needs an ID");

            modeOwner = animal.Mode_Get(ModeID);
            AnimationAbility = modeOwner.ActiveAbility;

            if (EnterMode)
            {
                modeOwner?.AnimationTagEnter();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animal) return;

            if (animal.ModeInt == Int_ID.Loop && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash) return;         //Means is Looping to itself So Skip the Exit Mode

            if (ExitMode)
            {
                modeOwner?.AnimationTagExit(AnimationAbility,ExitAbility);
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            modeOwner?.OnModeStateMove(stateInfo, animator, layerIndex);
        }
    }
}