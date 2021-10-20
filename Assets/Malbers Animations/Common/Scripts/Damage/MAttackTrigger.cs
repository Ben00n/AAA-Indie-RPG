using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    /// <summary>Simple Script to make damage anything with a stat</summary>
    [AddComponentMenu("Malbers/Damage/Attack Trigger")]
    public class MAttackTrigger : MDamager
    {
        [RequiredField, Tooltip("Collider used for the Interaction")]
        public Collider Trigger;

        /// <summary>When the Attack Trigger is Enabled, Affect your stat</summary>
        [Tooltip("When the Attack Trigger is Enabled, Affect your stat")]
        public StatModifier SelfStatEnter;

        /// <summary>When the Attack Trigger is Disabled, Affect your stat</summary>
        [Tooltip("When the Attack Trigger is Disabled, Affect your stat")]
        public StatModifier SelfStatExit;

        /// <summary>When the Attack Trigger Exits an enemy, Affect his stats</summary>
        [Tooltip("When the Attack Trigger Exits an enemy, Affect his stats")]
        public StatModifier EnemyStatExit;

        public UnityEvent OnAttackBegin = new UnityEvent();
        public UnityEvent OnAttackEnd = new UnityEvent();


        public bool debug = true;
        public Color DebugColor = new Color(1, 0.25f, 0, 0.15f);

        /// <summary>Enemy that can be Damaged</summary>
        private IMDamage enemy;
        private Stats enemyStats;

        protected List<Collider> AlreadyHitted = new List<Collider>(); 
          
        [HideInInspector] public int Editor_Tabs1;

        void OnEnable()
        {
            if (Owner == null)
            {
                Owner = transform.root.gameObject;                         //Set which is the owner of this AttackTrigger
                //interactor = Owner.FindInterface<IInteractor>();
            }
            if (Trigger) Trigger.enabled = Trigger.isTrigger = true;
            AlreadyHitted = new List<Collider>();
            OnAttackBegin.Invoke();
            enemy = null;
            enemyStats = null;
        }

        public override void DoDamage(bool value) => Active = value;


        void OnDisable()
        {
            if (Trigger) Trigger.enabled = false;

            if (enemyStats) EnemyStatExit.ModifyStat(enemyStats); //Means the Colliders was disable before Exit Trigger

            AlreadyHitted = new List<Collider>();
            OnAttackEnd.Invoke();

            enemy = null;
            enemyStats = null;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsInvalid(other)) return;                                               //Check Layers and Don't hit yourself

            var Newenemy = other.GetComponentInParent<IMDamage>();                      //Get the Animal on the Other collider

            if (!AlreadyHitted.Find(col => col == other)) AlreadyHitted.Add(other);   //if the entering collider is not already on the list add it


            Direction = (Owner.transform.position - other.bounds.center).normalized;              //Calculate the direction of the attack

            TryInteract(other.gameObject);                                              //Get the interactable on the Other collider
            TryPhysics(other.attachedRigidbody, other, Direction, Force);               //If the other has a riggid body and it can be pushed

            if (enemy == Newenemy) return;                                              //if the animal is the same, do nothing we already in one of the Animal Colliders
            else                                                                        //Is a new Animal
            {
                if (enemy != null)
                    AlreadyHitted = new List<Collider>();                               //Clean the colliders if you had a previus animal

                enemy = Newenemy;                                                       //Get the Damager on the Other collider
                enemyStats = other.GetComponentInParent<Stats>();


                OnHit.Invoke(other.transform);

                TryDamage(enemy, statModifier); //if the other does'nt have the Damagable Interface dont send the Damagable stuff
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (IsInvalid(other)) return;

            if (enemy != other.GetComponentInParent<IMDamage>()) return;                //If is another animal exiting the trigger SKIP


            if (AlreadyHitted.Find(col => col == other))                                //Remove the collider from the list that is exiting the zone.
                AlreadyHitted.Remove(other);


            if (AlreadyHitted.Count == 0)                                               //When all the collides are removed from the list..
            {
                if (enemy != null)                                                      //if the other does'nt have the animal script skip
                {
                    EnemyStatExit.ModifyStat(enemyStats);
                    enemy = null;
                    enemyStats = null;
                }
            }
        }



#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            #region Find the Trigger
            Trigger = this.FindComponent<Collider>();
            if (!Trigger) Trigger = gameObject.AddComponent<BoxCollider>();
            Trigger.isTrigger = true;
            enabled = false;
            #endregion
        }


        void OnDrawGizmos()
        {
            MTools.DrawTriggers(transform, Trigger, DebugColor, true);
        }
#endif
    }

#if UNITY_EDITOR


    [CustomEditor(typeof(MAttackTrigger)), CanEditMultipleObjects]
    public class MAttackTriggerEd : MDamagerEd
    {
        SerializedProperty Trigger, EnemyStatExit, debug, DebugColor, OnAttackBegin, OnAttackEnd, Editor_Tabs1;
        protected string[] Tabs1 = new string[] { "General", "Damage", "Extras", "Events" };


        private void OnEnable()
        {
            FindBaseProperties();

            Trigger = serializedObject.FindProperty("Trigger");

            EnemyStatExit = serializedObject.FindProperty("EnemyStatExit");

            debug = serializedObject.FindProperty("debug");
            DebugColor = serializedObject.FindProperty("DebugColor");

            OnAttackBegin = serializedObject.FindProperty("OnAttackBegin");
            OnAttackEnd = serializedObject.FindProperty("OnAttackEnd");
            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");

        }


        protected override void DrawCustomEvents()
        {
            EditorGUILayout.PropertyField(OnAttackBegin);
            EditorGUILayout.PropertyField(OnAttackEnd);
        }

        protected override void DrawStatModifier(bool drawbox =true)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Enemy Stat", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(statModifier, new GUIContent("Enemy Stat Enter"), true);
            EditorGUILayout.PropertyField(EnemyStatExit, true);
            EditorGUILayout.PropertyField(pureDamage);
            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDescription("Attack Trigger Logic. By default should be Disabled.");


            Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, Tabs1);

            int Selection = Editor_Tabs1.intValue;

            if (Selection == 0) DrawGeneral();
            else if (Selection == 1) DrawDamage();
            else if (Selection == 2) DrawExtras();
            else if (Selection == 3) DrawEvents();


          

            //DrawGeneral();

            //DrawPhysics();

            //DrawCriticalDamage();
            //DrawStatModifier();

            //DrawMisc();
            //DrawEvents();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(debug);
                if (debug.boolValue)
                    EditorGUILayout.PropertyField(DebugColor, GUIContent.none);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawGeneral(bool drawbox = true)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(Trigger);
            }
            EditorGUILayout.EndVertical();
            base.DrawGeneral(true);
        }

        private void DrawDamage()
        {
            DrawStatModifier();
            DrawCriticalDamage();
        }

        private void DrawExtras()
        {
            DrawPhysics();
            DrawMisc();
        }
    }
#endif
}

