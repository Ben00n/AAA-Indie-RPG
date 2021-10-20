using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations.Controller.AI
{

    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Play Mode")]
    public class PlayModeTask : MTask
    {
        public enum PlayWhen { Once, Forever }

        [Space, Tooltip("Mode you want to activate when the brain is using this task")]
        public ModeID modeID;
        [Tooltip("Ability ID for the Mode... if is set to -1 it will play a random Ability")]
        public IntReference AbilityID = new IntReference(-1);
        [Tooltip("Play the mode only when the animal has arrived to the target")]
        public bool near = false;

        [Space, Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        [Tooltip("Play Once: it will play only at the start of the Task. Play Forever: will play forever using the Cooldown property")]
        public PlayWhen Play = PlayWhen.Forever;
        [Tooltip("Time elapsed to Play the Mode again and Again")]
        public FloatReference CoolDown = new FloatReference(2f);
        [Tooltip("Play the Mode if the Animal is Looking at the Target. Avoid playing modes while the target is behind the animal when this value is set to 180")]
        [Range(0f,360f)]
        public float ModeAngle = 360f;

        [Space, Tooltip("Align with a Look At towards the Target when Playing a mode")]
        public bool lookAtAlign = false;
        [Hide("lookAtAlign", true), Tooltip("Align time to rotate towards the Target")]
        public float alignTime = 0.3f;



        public override void StartTask(MAnimalBrain brain, int index)
        {
            if (Play == PlayWhen.Once)
            {
                if (near && !brain.AIMovement.HasArrived) return; //Dont play if Play on target is true but we are not near the target.
                PlayMode(brain);
               // brain.TaskDone(index); //Let know that the play once task is done
            }
        }


        public override void UpdateTask(MAnimalBrain brain, int index)
        {
            if (Play == PlayWhen.Forever) //If the animal is in range of the Target
            {
                if (near && !brain.ArrivedToTarget) return; //Dont play if Play on target is true but we are not near the target.

                if (MTools.ElapsedTime(brain.TasksTime[index], CoolDown))
                {
                    PlayMode(brain);
                    brain.SetElapsedTaskTime(index);
                }
            }
            else
            {
                switch (affect)
                {
                    case Affected.Self:
                        if (!brain.Animal.IsPlayingMode) brain.TaskDone(index);
                        break;
                    case Affected.Target:
                        if (brain.TargetAnimal && !brain.TargetAnimal.IsPlayingMode) brain.TaskDone(index);
                        break;
                    default:
                        break;
                }
            }
        }

        private void PlayMode(MAnimalBrain brain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = brain.Target!= null ? (brain.Target.position - brain.Eyes.position) : brain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.Animal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                            {
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, brain.AIMovement.GetTargetPosition(), alignTime));
                            }
                        }
                    }
                    break;
                case Affected.Target:
                    Direction_to_Target = brain.Eyes.position - brain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(brain.Target.forward, brain.Animal.UpVector);
                    if (Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.TargetAnimal && brain.TargetAnimal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                            {
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.TargetAnimal.transform, brain.transform, alignTime));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void PlayMode(MAnimal animal)
        {
            animal.Mode_TryActivate(modeID, AbilityID);
        }



#if UNITY_EDITOR
        public override void DrawGizmos(MAnimalBrain brain)
        {
            var Eyes = brain.Eyes;

            if (Eyes && brain.AIMovement  && ModeAngle < 360)
            {
                Color c = (Color.green + Color.yellow) /2;
                c.a = 1f;

                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);
                Vector3 rotatedForward = Quaternion.Euler(0, -ModeAngle * 0.5f, 0) * EyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(Eyes.position, Vector3.up, rotatedForward, ModeAngle, 2);
            }
        }
#endif

        void Reset() { Description = "Plays a mode on the Animal(Self or the Target)"; }
    }
}