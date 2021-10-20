using UnityEngine;

namespace MalbersAnimations
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/ai/ai-target")]
    /// <summary>
    /// This Script allows to set a gameObject as a Target for the Ai Logic. 
    /// So when an AI Animal sets a gameObject holding this script  as a target, 
    /// it can have a stopping distance and it can stop on a properly distance.
    /// </summary>
    [AddComponentMenu("Malbers/AI/AI Target")]
    public class AITarget : MonoBehaviour, IAITarget
    {
        public WayPointType pointType = WayPointType.Ground;
        [Tooltip("Distance for AI driven animals to stop when it has arrived to the gameobject when is set as a Target.")]
        public float stoppingDistance = 1f;
        [Tooltip("Offset to correct the Position of the Target")]
        [SerializeField] private Vector3 center;

        public WayPointType TargetType => pointType;

        /// <summary>Center of the Animal to be used for AI and Targeting  </summary>
        public Vector3 Center
        {
            private set => center = value; 
            get => transform.TransformPoint(center);
        }

        public void SetLocalCenter(Vector3 localCenter) => center = localCenter;

        public virtual Vector3 GetPosition() => transform.TransformPoint(center);

        public float StopDistance() => stoppingDistance * transform.localScale.y; //IMPORTANT For Scaled objects like the ball

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Gizmos.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(Center, transform.up, stoppingDistance * transform.localScale.y);
            UnityEditor.Handles.DrawSolidDisc(Center, transform.up, stoppingDistance* 0.066f * transform.localScale.y);
        }
#endif
    }
}