using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.HAP
{
    [AddComponentMenu("Malbers/Riding/Mount")]
    public class Mount : MonoBehaviour, IAnimatorListener
    {
        #region References
        /// <summary>Input for the Mount</summary>
        public IInputSource MountInput { get; private set; }
        public bool debug;
        #endregion

        #region General
        /// <summary>Enable Disable the Mount Logic</summary>
        public BoolReference active = new BoolReference(true);
        /// <summary>Works for the ID of the Mount (EX Wagon</summary>
        public IntReference ID;


        /// <summary>if true then it will ignore the Mounting Animations</summary>
        public BoolReference instantMount = new BoolReference(false);
        public string mountIdle = "Idle";

        /// <summary>The Rider can only Mount when the Animal is on any of these states on the list</summary>
        public bool MountOnly;
        /// <summary>The Rider can only Dismount when the Animal is on any of these states on the list</summary>
        public bool DismountOnly;
        /// <summary>The Rider is Forced to dismount if the animal is on any of these states</summary>
        public bool ForceDismount;
        public List<StateID> MountOnlyStates = new List<StateID>();
        public List<StateID> DismountOnlyStates = new List<StateID>();
        public List<StateID> ForceDismountStates = new List<StateID>();

        public bool DisableMountTriggers = true;

        /// <summary>Reference for the Animator Update Mode</summary>
        public AnimatorUpdateMode DefaultAnimUpdateMode { get; set; }

        //If the Animal have been mounted  
        /// <summary>There's a Rider Inside the MountTriggers </summary>
        internal bool NearbyRider;
        #endregion

        #region Straight Mount
        public BoolReference straightSpine;                              //Activate this only for other animals but the horse 
        public BoolReference UseSpeedModifiers;
        public Vector3 pointOffset = new Vector3(0, 0, 3);
        public Vector3 MonturaSpineOffset => StraightSpineOffsetTransform.TransformPoint(pointOffset);

        //public float LowLimit = 45;
        //public float HighLimit = 135;

        public float smoothSM = 0.5f;
        #endregion

        #region Events
        public UnityEvent OnMounted = new UnityEvent();
        public UnityEvent OnDismounted = new UnityEvent();
        public BoolEvent OnCanBeMounted = new BoolEvent();

        /// <summary>Velocity changes for diferent Animation Speeds... used on other animals</summary>
        public List<SpeedTimeMultiplier> SpeedMultipliers;
        #endregion

        public Transform MountPoint;     // Reference for the RidersLink Bone  
        public Transform MountBase;     // Reference for the RidersLink Bone  
        public Transform FootLeftIK;     // Reference for the LeftFoot correct position on the mount
        public Transform FootRightIK;    // Reference for the RightFoot correct position on the mount
        public Transform KneeLeftIK;     // Reference for the LeftKnee correct position on the mount
        public Transform KneeRightIK;    // Reference for the RightKnee correct position on the mount
        #region Properties

        /// <summary>Straighen the Spine bone while mounted depends on the Mount</summary>
        public bool StraightSpine { get => straightSpine; set => straightSpine.Value = value; }


        /// <summary>Straighen the Spine bone while mounted depends on the Mount</summary>
        public Transform StraightSpineOffsetTransform;
        private bool defaultStraightSpine;

        public Animator Anim { get; private set; }  //Reference for the Animator 


        /// <summary>Reference for the AI Animal Control</summary>

        public IAIControl AI { get; internal set; }  //Reference for the AI Animal Controll 

        public List<MountTriggers> MountTriggers { get; private set; }

        protected bool mounted;
        /// <summary> Is the animal Mounted</summary>
        public bool Mounted
        {
            get => mounted;

            set
            {
                if (value != mounted)
                {
                    mounted = value;
                    if (mounted)
                        OnMounted.Invoke();    //Invoke the Event
                    else OnDismounted.Invoke();
                }
            }
        }

        /// <summary>Reference of the Animal</summary>
        public MAnimal Animal;

        /// <summary> Dismount only when the Animal is Still on place </summary>
        public virtual bool CanDismount => Mounted;

        public virtual string MountIdle { get => mountIdle; set => mountIdle = value; }

        /// <summary>Animal Mountable Script 'Enabled/Disabled'</summary>
        public virtual bool CanBeMounted { get => active; set => active.Value = value; }


        /// <summary>If "Mount Only" is enabled, this will capture the State the animal is at, in order to Mount</summary>
        public bool CanBeMountedByState { get; set; }
        /// <summary>If "Mount Only" is enabled, this will capture the State the animal is at, in order to Mount</summary>
        public bool CanBeDismountedByState { get; set; }

        /// <summary>Active Ride the Montura. is setted by the Rider Script </summary>
        public MRider Rider { get; set; }

        /// <summary> Ignore Mounting Animations </summary>
        public bool InstantMount { get => instantMount; set => instantMount.Value = value; }
        #endregion

        /// <summary>Enable the Input for the Mount</summary>
        public virtual void EnableInput(bool value)
        {
            MountInput?.Enable(value);
            Animal?.StopMoving();
        }

        void OnEnable()
        {
            Animal?.OnSpeedChange.AddListener(SetAnimatorSpeed);
            Animal?.OnStateChange.AddListener(AnimalStateChange);
        }

        void OnDisable()
        {
            Animal?.OnSpeedChange.RemoveListener(SetAnimatorSpeed);
            Animal?.OnStateChange.RemoveListener(AnimalStateChange);
        }

        internal void Awake()
        {
            if (Animal == null) Animal = GetComponent<MAnimal>();

            AI = gameObject.FindInterface<IAIControl>();
            
            Anim = Animal.GetComponent<Animator>();
            MountInput = Animal.GetComponent<IInputSource>();

            MountTriggers = GetComponentsInChildren<MountTriggers>(true).ToList(); //Catche all the MountTriggers of the Mount

            CanBeDismountedByState = CanBeMountedByState = true; //Set as true can be mounted and canbe dismounted by state
            defaultStraightSpine = StraightSpine;
            if (Anim) DefaultAnimUpdateMode = Anim.updateMode;

            if (!StraightSpineOffsetTransform)
            {
                StraightSpineOffsetTransform = transform;
            }
        }

        /// <summary>Used for Aiming while on the horse.... Straight Spine needs to be pause </summary>
        public void PauseStraightSpine(bool value) => StraightSpine = !value && defaultStraightSpine;

        /// <summary>Used for Aiming while on the horse.... Straight Spine needs to be pause </summary>
        public void EnableMountTriggers(bool value)
        {
            foreach (var mt in MountTriggers)
                mt.gameObject.SetActive(value);
        }

        private void AnimalStateChange(int StateID)
        {
            var ActiveState = Animal.ActiveStateID;

            if (MountOnly)
            {
                CanBeMountedByState = MountOnlyStates.Contains(ActiveState);   //Set MountOnly by State
            }

            if (DismountOnly)
            {
                CanBeDismountedByState = DismountOnlyStates.Contains(ActiveState);   //Set DimountOnly by State
            }

            if (Rider)
            {
                Rider.UpdateCanMountDismount();

                if (ForceDismount) //Means the Rider is forced to dismount
                {
                    if (ForceDismountStates.Contains(ActiveState))
                        Rider.ForceDismount();
                }
            }
        }

        public void SetAnimatorSpeed()
        {
            SetAnimatorSpeed(Animal.currentSpeedModifier);
        }

        /// <summary>Align the Animator Speed of the Mount  with the Rider Speed</summary>
        private void SetAnimatorSpeed(MSpeed SpeedModifier)
        {
            if (!Rider || !Rider.IsRiding) return;                            //if there's No Rider Skip

            if (UseSpeedModifiers)
            {
                var speed = SpeedMultipliers.Find(s => s.name == SpeedModifier.name); //Find the Curren Animal Speed

                float TargetAnimSpeed = speed != null ? speed.AnimSpeed * SpeedModifier.animator * Animal.AnimatorSpeed : 1f;

                Rider.TargetSpeedMultiplier = TargetAnimSpeed;
            }
        }

        /// <summary>Enable/Disable the StraightMount Feature </summary>
        public virtual void StraightMount(bool value)
        {
            StraightSpine = value;
        }

        public virtual bool OnAnimatorBehaviourMessage(string message, object value)
        { return this.InvokeWithParams(message, value); }

        [HideInInspector] public int Editor_Tabs1;
        [HideInInspector] public int Editor_Tabs2;


#if UNITY_EDITOR
        private void Reset()
        {
            Animal = GetComponent<MAnimal>();
            StraightSpineOffsetTransform = transform;

            MEvent RiderMountUIE = MTools.GetInstance<MEvent>("Rider Mount UI");

            if (RiderMountUIE != null)
            {
                UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<Transform>(OnCanBeMounted, RiderMountUIE.Invoke, transform);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(OnCanBeMounted, RiderMountUIE.Invoke);
            }
        }

        /// <summary> Debug Options </summary>
        void OnDrawGizmos()
        {
            if (!debug) return;

            Gizmos.color = Color.red;
            if (StraightSpineOffsetTransform)
            {
                Gizmos.DrawSphere(MonturaSpineOffset, 0.125f);
            }
            else
            {
                StraightSpineOffsetTransform = transform;
            }
        }
#endif
    }

    [System.Serializable]
    public class SpeedTimeMultiplier
    {
        /// <summary>Name of the Speed the on the animal to apply the AnimSpeed</summary>
        public string name = "SpeedName";

        /// <summary>Speed Modifier multiplier for the Rider</summary>
        public float AnimSpeed = 1f;
    }

    #region INSPECTOR
#if UNITY_EDITOR

    [CanEditMultipleObjects, CustomEditor(typeof(Mount))]
    public class MountEd : Editor
    {
        bool helpUseSpeeds;
        bool helpEvents;
        Mount M;

        SerializedProperty 
            UseSpeedModifiers, MountOnly, DismountOnly, active, mountIdle, instantMount, straightSpine, ID, StraightSpineOffsetTransform,
            DisableMountTriggers,pointOffset, Animal, smoothSM, mountPoint, rightIK,rightKnee, leftIK, leftKnee, SpeedMultipliers,
            OnMounted, Editor_Tabs1, Editor_Tabs2,OnDismounted, OnCanBeMounted, MountOnlyStates, DismountOnlyStates, MountBase,
            ForceDismountStates, ForceDismount, debug;

        private void OnEnable()
        {
            M = (Mount)target;

            UseSpeedModifiers = serializedObject.FindProperty("UseSpeedModifiers");
            //syncAnimators = serializedObject.FindProperty("syncAnimators");
            Animal = serializedObject.FindProperty("Animal");
            // ShowLinks = serializedObject.FindProperty("ShowLinks");
            debug = serializedObject.FindProperty("debug");
            ID = serializedObject.FindProperty("ID");
            DisableMountTriggers = serializedObject.FindProperty("DisableMountTriggers");

            MountOnly = serializedObject.FindProperty("MountOnly");
            DismountOnly = serializedObject.FindProperty("DismountOnly");
            active = serializedObject.FindProperty("active");
            mountIdle = serializedObject.FindProperty("mountIdle");
            instantMount = serializedObject.FindProperty("instantMount");
            straightSpine = serializedObject.FindProperty("straightSpine");
            //HighLimit = serializedObject.FindProperty("HighLimit");
            //LowLimit = serializedObject.FindProperty("LowLimit");
            smoothSM = serializedObject.FindProperty("smoothSM");

            mountPoint = serializedObject.FindProperty("MountPoint");
            MountBase = serializedObject.FindProperty("MountBase");
            rightIK = serializedObject.FindProperty("FootRightIK");
            rightKnee = serializedObject.FindProperty("KneeRightIK");
            leftIK = serializedObject.FindProperty("FootLeftIK");
            leftKnee = serializedObject.FindProperty("KneeLeftIK");

            Editor_Tabs1 = serializedObject.FindProperty("Editor_Tabs1");
            Editor_Tabs2 = serializedObject.FindProperty("Editor_Tabs2");

            SpeedMultipliers = serializedObject.FindProperty("SpeedMultipliers");
            //  DebugSync = serializedObject.FindProperty("DebugSync");
            OnMounted = serializedObject.FindProperty("OnMounted");
            pointOffset = serializedObject.FindProperty("pointOffset");
            StraightSpineOffsetTransform = serializedObject.FindProperty("StraightSpineOffsetTransform");

            OnDismounted = serializedObject.FindProperty("OnDismounted");
            OnCanBeMounted = serializedObject.FindProperty("OnCanBeMounted");
            MountOnlyStates = serializedObject.FindProperty("MountOnlyStates");
            DismountOnlyStates = serializedObject.FindProperty("DismountOnlyStates");

            ForceDismountStates = serializedObject.FindProperty("ForceDismountStates");
            ForceDismount = serializedObject.FindProperty("ForceDismount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Makes this GameObject mountable. Requires Mount Triggers and Moint Points");

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginChangeCheck();
                {
                    Editor_Tabs1.intValue = GUILayout.Toolbar(Editor_Tabs1.intValue, new string[] { "General", "Links", "Custom Mount" });
                    if (Editor_Tabs1.intValue != 3) Editor_Tabs2.intValue = 3;

                    Editor_Tabs2.intValue = GUILayout.Toolbar(Editor_Tabs2.intValue, new string[] { "M/D States", "Events", "Debug" });
                    if (Editor_Tabs2.intValue != 3) Editor_Tabs1.intValue = 3;


                    //First Tabs
                    int Selection = Editor_Tabs1.intValue;

                    if (Selection == 0) ShowGeneral();
                    else if (Selection == 1) ShowLinks();
                    else if (Selection == 2) ShowCustom();

                    //2nd Tabs
                    Selection = Editor_Tabs2.intValue;

                    if (Selection == 0) ShowStates();
                    else if (Selection == 1) ShowEvents();
                    else if (Selection == 2) ShowDebug();
                }

                EditorGUILayout.EndVertical();


                if (M.MountPoint == null)
                {
                    EditorGUILayout.HelpBox("'Mount Point'  is empty, please set a reference", MessageType.Warning);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Mount Inspector");
                //EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowDebug()
        {
            EditorGUILayout.PropertyField(debug);
        }


        private void ShowEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
                    helpEvents = GUILayout.Toggle(helpEvents, "?", EditorStyles.miniButton, GUILayout.Width(18));
                }
                EditorGUILayout.EndHorizontal();
                if (helpEvents) EditorGUILayout.HelpBox("On Mounted: Invoked when the rider start to mount the animal\nOn Dismounted: Invoked when the rider start to dismount the animal\nInvoked when the Mountable has an available Rider Nearby", MessageType.None);

                EditorGUILayout.PropertyField(OnMounted);
                EditorGUILayout.PropertyField(OnDismounted);
                EditorGUILayout.PropertyField(OnCanBeMounted);
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowStates()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Mount/Dismount States", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(MountOnly, new GUIContent("Mount Only", "The Rider can only Mount when the Animal is on any of these states"));

                if (MountOnly.boolValue) MalbersEditor.Arrays(MountOnlyStates);

                EditorGUILayout.PropertyField(DismountOnly, new GUIContent("Dismount Only", "The Rider can only Dismount when the Animal is on any of these states"));

                if (DismountOnly.boolValue) MalbersEditor.Arrays(DismountOnlyStates);


                EditorGUILayout.PropertyField(ForceDismount, new GUIContent("Force Dismount", "The Rider is forced to dismount when the Animal is on any of these states"));

                if (ForceDismount.boolValue) MalbersEditor.Arrays(ForceDismountStates);
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowCustom()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(straightSpine, new GUIContent("Straight Spine", "Straighten the Mount Point to fix the Rider Animation"));

                if (M.StraightSpine)
                {
                    EditorGUILayout.PropertyField(StraightSpineOffsetTransform, new GUIContent("Transf Ref", "Transform to use for the Point Offset Calculation"));
                    EditorGUILayout.PropertyField(pointOffset, new GUIContent("Point Offset", "Point in front of the Mount to Straight the Spine of the Rider"));
                    EditorGUILayout.PropertyField(smoothSM, new GUIContent("Smoothness", "Smooth changes between the rotation and the straight Mount"));
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(UseSpeedModifiers, new GUIContent("Animator Speeds", "Use this for other animals but the horse"));
                    helpUseSpeeds = GUILayout.Toggle(helpUseSpeeds, "?", EditorStyles.miniButton, GUILayout.Width(18));
                }
                EditorGUILayout.EndHorizontal();

                if (M.UseSpeedModifiers)
                {
                    if (helpUseSpeeds) EditorGUILayout.HelpBox("Changes the Speed on the Rider's Animator to Sync with the Animal Animator.\nThe Original Riding Animations are meant for the Horse. Only change the Speeds for other creatures", MessageType.None);
                    MalbersEditor.Arrays(SpeedMultipliers, new GUIContent("Animator Speed Multipliers", "Velocity changes for diferent Animation Speeds... used on other animals"));
                }
            }
            EditorGUILayout.EndVertical();

        }

        private void ShowLinks()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.HelpBox("'Mount Point' is obligatory, the rest are optional", MessageType.None);

                EditorGUILayout.PropertyField(MountBase, new GUIContent("Mount Base", "Reference for the Mount Base, Parent of the Mount Point, used for Straight movement for the mount"));
                EditorGUILayout.PropertyField(mountPoint, new GUIContent("Mount Point", "Reference for the Mount Point"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(rightIK, new GUIContent("Right Foot", "Reference for the Right Foot correct position on the mount"));
                EditorGUILayout.PropertyField(rightKnee, new GUIContent("Right Knee", "Reference for the Right Knee correct position on the mount"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(leftIK, new GUIContent("Left Foot", "Reference for the Left Foot correct position on the mount"));
                EditorGUILayout.PropertyField(leftKnee, new GUIContent("Left Knee", "Reference for the Left Knee correct position on the mount"));
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowGeneral()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(active, new GUIContent("Active", "If the animal can be mounted. Deactivate if the mount is death or destroyed or is not ready to be mountable"));
                EditorGUILayout.PropertyField(Animal, new GUIContent("Animal", "Animal Reference for the Mounting System"));
                EditorGUILayout.PropertyField(ID, new GUIContent("ID", "Default should be 0.... change this and the Stance parameter on the Rider will change to that value... alowing other types of mounts like Wagon"));
                EditorGUILayout.PropertyField(instantMount, new GUIContent("Instant Mount", "Ignores the Mounting Animations"));
                EditorGUILayout.PropertyField(mountIdle, new GUIContent("Mount Idle", "Animation to Play directly when instant mount is enabled"));
                EditorGUILayout.PropertyField(DisableMountTriggers, new GUIContent("Disable M Triggers", "Disable Mount Triggers when the rider stars to mount the animal and enable them back when finish dismounting"));
            }
            EditorGUILayout.EndVertical();
        }
    }



    [CustomPropertyDrawer(typeof(SpeedTimeMultiplier))]
    public class SpeedTimeMultiplierDrawer : PropertyDrawer
    {
        // Use this for initialization
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            label = EditorGUI.BeginProperty(position, label, property);
            // position = EditorGUI.PrefixLabel(position, label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.BeginChangeCheck();

            var name = property.FindPropertyRelative("name");
            var AnimSpeed = property.FindPropertyRelative("AnimSpeed");
            var height = EditorGUIUtility.singleLineHeight;
            var line = position;
            line.height = height;

            //line.x += 4;
            //line.width -= 8;


            var MainRect = new Rect(line.x, line.y, line.width / 2, height);
            var lerpRect = new Rect(line.x + line.width / 2, line.y, line.width / 2, height);

            EditorGUIUtility.labelWidth = 45f;
            EditorGUI.PropertyField(MainRect, name, new GUIContent("Name", "Name of the Speed to modify for the Rider"));
            EditorGUIUtility.labelWidth = 75f;
            EditorGUI.PropertyField(lerpRect, AnimSpeed, new GUIContent(" Speed Mult", "Anim Speed Multiplier"));
            if (name.stringValue == string.Empty) name.stringValue = "SpeedName";
            EditorGUIUtility.labelWidth = 0;

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
#endif

    #endregion
}