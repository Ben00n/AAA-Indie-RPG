using UnityEngine;

namespace MalbersAnimations
{
    public class IsKinematicB : StateMachineBehaviour
    {
        public enum OnEnterOnExit { OnEnter, OnExit, OnEnterOnExit}
        public OnEnterOnExit SetKinematic = OnEnterOnExit.OnEnterOnExit;

        [Tooltip("Changes the Kinematic property of the RigidBody On Enter/OnExit")]
        [Hide("onenterexit",true,true)]
        public bool isKinematic = true;
        CollisionDetectionMode current;

        Rigidbody rb;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponent<Rigidbody>();

            if (SetKinematic == OnEnterOnExit.OnEnter)
            {
                if (isKinematic == true)
                {
                    current = rb.collisionDetectionMode;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }

                rb.isKinematic = isKinematic;
            }
            else if (SetKinematic == OnEnterOnExit.OnEnterOnExit)
            {
                current = rb.collisionDetectionMode;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (SetKinematic == OnEnterOnExit.OnExit)
            {
                if (isKinematic == true)
                {
                    current = rb.collisionDetectionMode;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
                else
                {
                    rb.collisionDetectionMode = current;
                }

                rb.isKinematic = isKinematic;
            }
            else if (SetKinematic == OnEnterOnExit.OnEnterOnExit)
            {
                rb.isKinematic = false;
                rb.collisionDetectionMode = current;
            }
        }

        [HideInInspector] public bool onenterexit;
        private void OnValidate()
        {
            onenterexit = SetKinematic == OnEnterOnExit.OnEnterOnExit;
        }
    }
}