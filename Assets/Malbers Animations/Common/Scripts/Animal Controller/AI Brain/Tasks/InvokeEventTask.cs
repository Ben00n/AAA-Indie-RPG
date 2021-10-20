using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Invoke Event")]
    public class InvokeEventTask : MTask
    {
        [Space]
        public float Delay = 0;
        public UnityEvent Raise = new UnityEvent();

        public override void StartTask(MAnimalBrain brain, int index)
        {
            if (Delay == 0)
            {
                Raise.Invoke();
                brain.TaskDone(index);
            }
        }

        public override void UpdateTask(MAnimalBrain brain, int index)
        {
            if (MTools.ElapsedTime(brain.TasksTime[index], Delay))
            {
                Raise.Invoke();
                brain.TaskDone(index);
            }
        }

        void Reset() { Description = "Raise the Event when the Task start. Use this only for Scriptable Assets"; }
    }
}
