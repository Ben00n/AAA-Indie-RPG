using MalbersAnimations.Events;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/AI/Waypoint")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/ai/mwaypoint")]
    public class MWayPoint : MonoBehaviour, IWayPoint
    {
        public static List<MWayPoint> WayPoints;

        public WayPointType pointType = WayPointType.Ground;
        [Tooltip("Distance for AI driven animals to stop when it has arrived to the gameobject when is set as a Target.")]
        public float stoppingDistance = 1f;

        [MinMaxRange(0, 60), Tooltip("Waytime range to go to the next destination")]
        public RangedFloat m_WaitTime = new RangedFloat(1, 5);

        public Color DebugColor = Color.red;
        public float WaitTime => m_WaitTime.RandomValue;

        public WayPointType TargetType => pointType;

        public virtual Vector3 GetPosition() => transform.position;

        public float StopDistance() => stoppingDistance;

        public Transform WPTransform => base.transform;

        [SerializeField] private List<Transform> nextWayPoints;
        public List<Transform> NextTargets { get => nextWayPoints; set => nextWayPoints = value; }

        [Space]
        public GameObjectEvent OnTargetArrived = new GameObjectEvent();

        void OnEnable()
        {
            if (WayPoints == null) WayPoints = new List<MWayPoint>();

            WayPoints.Add(this);
        }

        void OnDisable()
        {
            WayPoints.Remove(this);
        }

        public void TargetArrived(GameObject target) => OnTargetArrived.Invoke(target);

        public Transform NextTarget()
        {
            return NextTargets.Count > 0 ? NextTargets[UnityEngine.Random.Range(0, NextTargets.Count)] : null;
        }

        /// <summary>Returns a Random Waypoint from the Global WaypointList</summary>
        public static Transform GetWaypoint()
        {
            return (WayPoints != null && WayPoints.Count > 1) ? WayPoints[Random.Range(0, WayPoints.Count)].WPTransform : null;
        }

        /// <summary>Returns a Random Waypoint from the Global WaypointList by its type (Ground, Air, Water)</summary>
        public static Transform GetWaypoint(WayPointType pointType)
        {
            if (WayPoints != null && WayPoints.Count > 1)
            {
                var MWayPoint = WayPoints.Find(item => item.pointType == pointType);

                return MWayPoint ? MWayPoint.WPTransform : null;
            }
            return null;
        }

#if UNITY_EDITOR
        /// <summary>DebugOptions</summary>

        void OnDrawGizmos()
        {
            UnityEditor.Handles.color = DebugColor;
            Gizmos.color = DebugColor;

            if (pointType == WayPointType.Air)
            {
                Gizmos.DrawWireSphere(base.transform.position, stoppingDistance);
            }
            else
            {
                UnityEditor.Handles.DrawWireDisc(base.transform.position, transform.up, stoppingDistance);
            }

            Gizmos.color = DebugColor;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance * 0.25f);

            var col = DebugColor;
            col.a = 0.3f;
            Gizmos.color = col;
            Gizmos.DrawSphere(transform.position, stoppingDistance * 0.25f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance * 0.25f);

            Gizmos.color = DebugColor;

            if (nextWayPoints != null)
            {
                foreach (var item in nextWayPoints)
                {
                    if (item) Gizmos.DrawLine(transform.position, item.position);
                }
            }
        }
#endif
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MWayPoint))]
    [UnityEditor.CanEditMultipleObjects]
    public class MWayPointEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty
            pointType, stoppingDistance, WaitTime, nextWayPoints, DebugColor, OnTargetArrived;

        private void OnEnable()
        {
            pointType = serializedObject.FindProperty("pointType");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            WaitTime = serializedObject.FindProperty("m_WaitTime");
            nextWayPoints = serializedObject.FindProperty("nextWayPoints");
            DebugColor = serializedObject.FindProperty("DebugColor");
            OnTargetArrived = serializedObject.FindProperty("OnTargetArrived");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Uses this Transform position as the destination point for AI Driven characters");
            UnityEditor.EditorGUILayout.BeginVertical(MTools.StyleGray);
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                {
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    UnityEditor.EditorGUILayout.PropertyField(pointType);
                    UnityEditor.EditorGUILayout.PropertyField(DebugColor, GUIContent.none, GUILayout.Width(40));
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    UnityEditor.EditorGUILayout.PropertyField(stoppingDistance);
                    UnityEditor.EditorGUILayout.PropertyField(WaitTime);
                }
                UnityEditor.EditorGUILayout.EndVertical();

                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                {
                    UnityEditor.EditorGUI.indentLevel++;
                    UnityEditor.EditorGUILayout.PropertyField(nextWayPoints, true);
                    UnityEditor.EditorGUI.indentLevel--;
                }
                UnityEditor.EditorGUILayout.EndVertical();
                    UnityEditor.EditorGUILayout.PropertyField(OnTargetArrived);
            }
            UnityEditor.EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
