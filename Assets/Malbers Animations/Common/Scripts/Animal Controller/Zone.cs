using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Scriptables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    /// <summary>When an animal Enter a Zone this will activate a new State or a new Mode </summary>
    [AddComponentMenu("Malbers/Animal Controller/Zone")]
    public class Zone : MonoBehaviour 
    { 
        /// <summary>Set the Action Zone to Automatic</summary>
        public bool automatic;
        ///// <summary>if Automatic is set to true this will be the time to disable temporarly the Trigger</summary>
        //public float AutomaticDisabled = 10f;
        //private float ZoneActivationTime;
        /// <summary>Use the Trigger for Heads only</summary>
        public bool HeadOnly;
        public string HeadName = "Head";


        public ZoneType zoneType = ZoneType.Mode;
        public StateAction stateAction = StateAction.Activate;
        public StanceAction stanceAction = StanceAction.Enter;

        public LayerReference Layer = new LayerReference(1048576);
        public IntReference stateStatus = new IntReference(-1);
        
        [SerializeField] private List<Tag> tags;

        public ModeID modeID;
        public StateID stateID;
      
        public StanceID stanceID;
        public MAction ActionID;
        /// <summary> Mode Index Value</summary>
        [SerializeField] private IntReference modeIndex =  new IntReference(0);

        /// <summary> Mode Index Value</summary> 
        public int ModeIndex => modeID.ID == 4 ? ActionID.ID : modeIndex.Value;


        public FloatReference Force = new FloatReference(10);
        public FloatReference EnterDrag = new FloatReference(2);
        public FloatReference ExitDrag = new FloatReference(4);
        public FloatReference Bounce = new FloatReference(8);
         

        /// <summary>Current Animal the Zone is using </summary>
        public MAnimal CurrentAnimal { get; internal set; }
        protected List<Collider> animal_Colliders = new List<Collider>();

        public float ActionDelay = 0;
        [Tooltip("Value Assigned to the Mode Float Value when using the Mode Zone")]
        public float ModeFloat = 0;
        public bool RemoveAnimalOnActive = false;
         
  
        public AnimalEvent OnEnter = new AnimalEvent();
        public AnimalEvent OnExit = new AnimalEvent();
        public AnimalEvent OnZoneActivation = new AnimalEvent();

        protected Collider ZoneCollider;
     //   protected Stats AnimalStats;
 
        /// <summary>Keep a Track of all the Zones on the Scene </summary>
        public static List<Zone> Zones;
 
        /// <summary>Retuns the ID of the Zone regarding the Type of Zone(State,Stance,Mode) </summary>
        public int GetID
        {
            get
            {
                switch (zoneType)
                {
                    case ZoneType.Mode:
                        return modeID;
                    case ZoneType.State:
                        return stateID;
                    case ZoneType.Stance:
                        return stateID;
                    case ZoneType.Force:
                        return 100;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>Is the zone a Mode Zone</summary>
        public bool IsMode => zoneType == ZoneType.Mode;

        /// <summary>Is the zone a Mode Zone</summary>
        public bool IsState => zoneType == ZoneType.State;

        /// <summary>Is the zone a Mode Zone</summary>
        public bool IsStance => zoneType == ZoneType.Stance;

        public List<Tag> Tags { get => tags; set => tags = value; }

        void OnTriggerEnter(Collider other)
        {
            if (IgnoreCollider(other)) return;                             //If the collider does not fill the requirements skip

            if (Tags != null && Tags.Count > 0)                             //Check if we are using Tags and if the entering animal does not have that tag the this zone is not for that animal
            {
                bool hasTag = false;
                foreach (var t in tags)
                {
                    if (t != null && other.transform.HasMalbersTagInParent(t))
                    {
                        hasTag = true;
                        break;
                    }
                }

                if (!hasTag)
                {
                    Debug.LogWarning($"The Zone:<B>[{name}]</B> cannot be activated by <B>[{other.transform.root.name}]</B>. The Zone is using Tags and <B>[{other.transform.root.name}]</B> does not have any.");
                    return;
                }
            }

            MAnimal newAnimal = other.GetComponentInParent<MAnimal>();               //Get the animal on the entering collider

            if (!newAnimal || newAnimal.Sleep || !newAnimal.enabled) return;         //If there's no animal, or is Sleep or disabled do nothing

            if (!animal_Colliders.Contains(other))                //if the entering collider is not already on the list add it
                animal_Colliders.Add(other);

            if (newAnimal == CurrentAnimal) return;                     //if the animal is the same do nothing (when entering two animals on the same Zone)
            else
            {
                if (CurrentAnimal)
                    animal_Colliders = new List<Collider>();                            //Clean the colliders

                CurrentAnimal = newAnimal;                                             //Set a new Animal
                OnEnter.Invoke(CurrentAnimal);

                if (automatic)
                {
                    ActivateZone();
                }
                else
                {
                    PrepareZone();
                }
            }
        }
        void OnTriggerExit(Collider other)
        { 
            if (IgnoreCollider(other)) return;                             //If the collider does not fill the requirements skip

            MAnimal exit_animal = other.GetComponentInParent<MAnimal>();

            if (!exit_animal) return;                                            //If there's no animal script found skip all
            if (exit_animal != CurrentAnimal) return;                            //If is another animal exiting the zone SKIP

            if (animal_Colliders.Contains(other))                       //Remove the collider from the list that is exiting the zone.
                animal_Colliders.Remove(other);

            if (animal_Colliders.Count == 0)                                        //When all the collides are removed from the list..
            {
                OnExit.Invoke(CurrentAnimal);                                      //Invoke On Exit when all animal's colliders has exited the Zone
                ResetStoredAnimal();
            }
        }

        private bool IgnoreCollider(Collider other) =>
            !isActiveAndEnabled ||                                          //Check if the Zone is Active
            other.isTrigger ||                                              //No Triggers
            !MTools.CollidersLayer(other, Layer.Value) ||                   //Just accept animal layer only
            HeadOnly && !other.name.ToLower().Contains(HeadName.ToLower()); //If is Head Only and no head was found Skip


        /// <summary>Activate the Zone depending the Zone Type</summary>
        /// <param name="forced"></param>
        public virtual void ActivateZone()
        {
            if (CurrentAnimal)
            {
                var isZoneActive = false;
                CurrentAnimal.IsOnZone = true;
                switch (zoneType)
                {
                    case ZoneType.Mode:
                        isZoneActive = ActivateModeZone();
                        break;
                    case ZoneType.State:
                        isZoneActive = ActivateStateZone(); //State Zones does not require to be delay or prepared to be activated
                        break;
                    case ZoneType.Stance:
                        isZoneActive = ActivateStanceZone(); //State Zones does not require to be delay or prepared to be activated
                        break;
                    case ZoneType.Force:
                        isZoneActive = SetForceZone(true); //State Zones does not require to be delay or prepared to be activated
                        break;
                }
                if (isZoneActive) OnZoneActive();
            }
        }


        protected virtual void PrepareZone()
        {
            if (CurrentAnimal)
            {
                CurrentAnimal.IsOnZone = true;
                switch (zoneType)
                {
                    case ZoneType.Mode:

                     
                        var PreMode = CurrentAnimal.Mode_Get(modeID);

                        if (PreMode == null || !PreMode.HasAbilityIndex(ModeIndex)) //If the Animal does not have that mode or that Ability Index exti
                        {
                            Debug.LogWarning($"<B>[{name}]</B> cannot be activated by <B>[{CurrentAnimal.name}]</B>. It does not have The <B>[Mode {modeID}]</B> or <B>[ModeIndex {ModeIndex}]</B>");
                            return;
                        }
                        if (PreMode != null)
                        {
                            PreMode.AbilityIndex = ModeIndex;
                            PreMode.OnEnterMode.AddListener(OnZoneActive);
                        }

                        break;
                    case ZoneType.State:
                        break;
                    case ZoneType.Stance:
                        break;
                    case ZoneType.Force:
                        break;
                }
            }
        }


        /// <summary>Enables the Zone using the State</summary>
        private bool ActivateStateZone()
        {
            var Succesful = false;
            switch (stateAction)
            {
                case StateAction.Activate:
                    if (CurrentAnimal.ActiveStateID != stateID)
                    {
                        CurrentAnimal.State_Activate(stateID);
                        Succesful = true;
                    }
                    break;
                case StateAction.AllowExit:
                    if (CurrentAnimal.ActiveStateID == stateID)
                    {
                        CurrentAnimal.ActiveState.AllowExit();
                        Succesful = true;
                    }
                    break;
                case StateAction.ForceActivate:
                    CurrentAnimal.State_Force(stateID);
                    Succesful = true;
                    break;
                case StateAction.Enable:
                    CurrentAnimal.State_Enable(stateID);
                    Succesful = true;
                    break;
                case StateAction.Disable:
                    CurrentAnimal.State_Disable(stateID);
                    Succesful = true;
                    break;
                case StateAction.ChangeEnterStatus:
                    if (CurrentAnimal.ActiveStateID == stateID)
                    {
                        CurrentAnimal.State_SetStatus(stateStatus);
                        Succesful = true;
                    }
                    break;
                case StateAction.ChangeExitStatus:
                    if (CurrentAnimal.ActiveStateID == stateID)
                    {
                        CurrentAnimal.State_SetExitStatus(stateStatus);
                        Succesful = true;
                    }
                    break;
                default:
                    break;
            }
            return Succesful;
        }


        private bool ActivateModeZone()
        {
            if (!CurrentAnimal.IsPlayingMode)
            {
                CurrentAnimal.Mode_SetPower(ModeFloat); //Set the correct height for the Animal Animation 
                return CurrentAnimal.Mode_TryActivate(modeID.ID, ModeIndex);
            }
            
            return false;
        }

        /// <summary>Enables the Zone using the Stance</summary>
        private bool ActivateStanceZone()
        {
            switch (stanceAction)
            {
                case StanceAction.Enter:
                    CurrentAnimal.Stance_Set(stanceID);
                    break;
                case StanceAction.Exit:
                    CurrentAnimal.Stance_Reset();
                    break;
                case StanceAction.Toggle:
                    CurrentAnimal.Stance_Toggle(stanceID);
                    break;
                case StanceAction.Stay:
                    CurrentAnimal.Stance_Set(stanceID);
                    break;
                default:
                    break;
            }

            return true;
        }

        private bool SetForceZone(bool value)
        {
            if (value) //ENTERING THE FORCE ZONE!!!
            {
                var StartExtForce = CurrentAnimal.CurrentExternalForce + CurrentAnimal.GravityStoredVelocity;


                if (StartExtForce.magnitude > Bounce)
                    StartExtForce = StartExtForce.normalized * Bounce;

                CurrentAnimal.CurrentExternalForce = StartExtForce;
                CurrentAnimal.ExternalForce = transform.up * Force;
                CurrentAnimal.ExternalForceAcel = EnterDrag;

                CurrentAnimal.Grounded = false;
                CurrentAnimal.UseGravity = false;

                if (CurrentAnimal.ActiveState.ID == StateEnum.Fall)
                {
                    var fall = CurrentAnimal.ActiveState as Fall;
                    fall.FallCurrentDistance = 0;
                }

                if (forceNoGravity != null) StopCoroutine(forceNoGravity);  //Done for FALL STATE WHICH ENABLE THE GRAVITY!!
                forceNoGravity = StartCoroutine(FORCENOGRAVITY());
            }
            else
            {
                if (forceNoGravity != null) StopCoroutine(forceNoGravity);


                if (CurrentAnimal.ActiveState.ID == StateEnum.Fall) CurrentAnimal.UseGravity = true;



                if (ExitDrag > 0)
                {
                    CurrentAnimal.ExternalForceAcel = ExitDrag;
                    CurrentAnimal.ExternalForce = Vector3.zero;
                }

            }
            return value;
        }

        private void OnZoneActive()
        {
            OnZoneActivation.Invoke(CurrentAnimal);
            if (RemoveAnimalOnActive) ResetStoredAnimal();
        }

        public virtual void ResetStoredAnimal()
        {
            CurrentAnimal.IsOnZone = false; //Tell the Animal is no longer on a Zone

            switch (zoneType)
            {
                case ZoneType.Mode:

                    var mode = CurrentAnimal.Mode_Get(modeID);
                   
                    if (mode != null) //Means we found the current Active mode
                    {
                        if (mode.AbilityIndex == ModeIndex) mode.ResetAbilityIndex(); //Only reset when it has the same Index... works for zones near eachother 
                        mode.OnEnterMode.RemoveListener(OnZoneActive);
                    }

                    break;
                case ZoneType.State:

                    break;
                case ZoneType.Stance:
                    if (stanceAction == StanceAction.Stay && CurrentAnimal.Stance == stanceID.ID) CurrentAnimal.Stance_Reset();
                    break;
                case ZoneType.Force:
                    SetForceZone(false);
                    break;
                default:
                    break;
            } 

            CurrentAnimal = null;
           // AnimalStats = null;
            animal_Colliders = new List<Collider>();                            //Clean the colliders
        }

 
        private Coroutine forceNoGravity;

        IEnumerator FORCENOGRAVITY()
        {
            while (true)
            {
                CurrentAnimal.UseGravity = false;
                yield return null;
            }
        }


        void OnEnable()
        {
            if (Zones == null) Zones = new List<Zone>();
            ZoneCollider = GetComponent<Collider>();                          //Get the reference for the collider
            ZoneCollider.isTrigger = true;                                    //Force Trigger
            Zones.Add(this);                                                  //Save the the Action Zones on the global Action Zone list
            //ZoneActivationTime = -AutomaticDisabled;
        }

        void OnDisable()
        {
            Zones.Remove(this);                                              //Remove the the Action Zones on the global Action Zone list

            if (CurrentAnimal)  ResetStoredAnimal();
        }

        /// <summary>  Used when an AI animal arrives to the Zone, Automatically activates the zone </summary>
        /// <param name="target"></param>
        public void TargetArrived(GameObject target)
        {
            CurrentAnimal = target.FindComponent<MAnimal>();
            ActivateZone();
        }

        [HideInInspector] public bool EditorShowEvents = false;
        [HideInInspector] public bool ShowStatModifiers = false;
    }

    public enum StateAction
    {
        /// <summary>Tries to Activate the State of the Zone</summary>
        Activate,
        /// <summary>If the Animal is already on the state of the zone it will allow to exit and activate states below the Active one</summary>
        AllowExit,
        /// <summary>Force the State of the Zone to be enable even if it cannot be activate at the moment</summary>
        ForceActivate,
        /// <summary>Enable a  Disabled State </summary>
        Enable,
        /// <summary>Disable State </summary>
        Disable,
        /// <summary>Set a State Status on a State</summary>
        ChangeEnterStatus,
        /// <summary>Set a State Status on a State</summary>
        ChangeExitStatus,

    }
    public enum StanceAction
    {
        /// <summary>Enters a Stance</summary>
        Enter,
        /// <summary>Exits a Stance</summary>
        Exit,
        /// <summary>Toggle a Stance</summary>
        Toggle,
        /// <summary>While the Animal is inside the collider the Animal will stay on the Stance</summary>
        Stay,
    }
    public enum ZoneType
    {
        Mode,
        State,
        Stance,
        Force
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(Zone))/*, CanEditMultipleObjects*/]
    public class ZoneEditor : Editor
    {
        private Zone M;

        SerializedProperty
            HeadOnly, stateAction, HeadName, zoneType, stateID, modeID, modeIndex, ActionID, auto,
            //StatModifierOnEnter, StatModifierOnExit, ShowStatModifiers, statModifier,
         
            stanceAction, layer, stanceID, RemoveAnimalOnActive, m_tag, ModeFloat, Force, EnterAceleration, ExitAceleration, stateStatus, Bounce;



        MonoScript script;
        private void OnEnable()
        {
            M = ((Zone)target);
            script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);

            HeadOnly = serializedObject.FindProperty("HeadOnly");
            RemoveAnimalOnActive = serializedObject.FindProperty("RemoveAnimalOnActive");
            HeadName = serializedObject.FindProperty("HeadName");
            layer = serializedObject.FindProperty("Layer");
            stateStatus = serializedObject.FindProperty("stateStatus");


            Force = serializedObject.FindProperty("Force");
            EnterAceleration = serializedObject.FindProperty("EnterDrag");
            ExitAceleration = serializedObject.FindProperty("ExitDrag");
            Bounce = serializedObject.FindProperty("Bounce");


            m_tag = serializedObject.FindProperty("tags");
            ModeFloat = serializedObject.FindProperty("ModeFloat");
            zoneType = serializedObject.FindProperty("zoneType");
            stateID = serializedObject.FindProperty("stateID");
            stateAction = serializedObject.FindProperty("stateAction");
            stanceAction = serializedObject.FindProperty("stanceAction");
            modeID = serializedObject.FindProperty("modeID");
            stanceID = serializedObject.FindProperty("stanceID");
            modeIndex = serializedObject.FindProperty("modeIndex");
            ActionID = serializedObject.FindProperty("ActionID");
            auto = serializedObject.FindProperty("automatic");
           // AutomaticDisabled = serializedObject.FindProperty("AutomaticDisabled");

            //statModifier = serializedObject.FindProperty("StatModifierOnActive");
            //StatModifierOnEnter = serializedObject.FindProperty("StatModifierOnEnter");
            //StatModifierOnExit = serializedObject.FindProperty("StatModifierOnExit");
            //ShowStatModifiers = serializedObject.FindProperty("ShowStatModifiers");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Area to modify States, Stances or Modes on an Animal");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(auto, new GUIContent("Automatic", "As soon as the animal enters the zone it will execute the logic. If False then Call the Method Zone.Activate()"));
                //if (auto.boolValue)
                //{
                //    EditorGUILayout.PropertyField(AutomaticDisabled, new GUIContent("Disabled", "The zone will be disable after x seconds"));
                //    if (AutomaticDisabled.floatValue < 0) AutomaticDisabled.floatValue = 0;
                //}
                EditorGUILayout.PropertyField(zoneType, new GUIContent("Zone Type", "Choose between a Mode or a State for the Zone"));
                EditorGUILayout.PropertyField(layer, new GUIContent("Animal Layer", "What Layer to detect"));

                ZoneType zone = (ZoneType)zoneType.intValue;


                switch (zone)
                {
                    case ZoneType.Mode:
                        EditorGUILayout.PropertyField(modeID, new GUIContent("Mode ID", "Which Mode to Set when entering the Zone"));

                        serializedObject.ApplyModifiedProperties();


                        if (M.modeID != null && M.modeID == 4)
                        {
                            EditorGUILayout.PropertyField(ActionID, new GUIContent("Action Index", "Which Action to Set when entering the Zone"));

                            if (ActionID.objectReferenceValue == null)
                            {
                                EditorGUILayout.HelpBox("Please Select an Action ID", MessageType.Error);
                            }
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(modeIndex, new GUIContent("Ability Index", "Which Ability to Set when entering the Zone"));
                            if (ActionID.objectReferenceValue == null)
                            {
                                EditorGUILayout.HelpBox("Please Select an Ability ID", MessageType.Error);
                            }
                        }

                        EditorGUILayout.PropertyField(ModeFloat);
                      
                        break;
                    case ZoneType.State:
                        EditorGUILayout.PropertyField(stateID, new GUIContent("State ID", "Which State will Activate when entering the Zone"));
                        EditorGUILayout.PropertyField(stateAction, new GUIContent("Action", "Set what action for State the animal will apply when entering the zone"));

                        int stateaction = stateAction.intValue;
                        if (stateaction == (int)StateAction.ChangeEnterStatus || stateaction == (int)StateAction.ChangeExitStatus)
                        {
                            EditorGUILayout.PropertyField(stateStatus, new GUIContent("Status", "Set the State Status"));
                        }

                        if (stateID.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("Please Select an State ID", MessageType.Error);
                        }
                        break;
                    case ZoneType.Stance:
                        EditorGUILayout.PropertyField(stanceID, new GUIContent("Stance ID", "Which Stance will Activate when entering the Zone"));
                        EditorGUILayout.PropertyField(stanceAction, new GUIContent("Action", "Set what action for stance the animal will apply when entering the zone"));
                        if (stanceID.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("Please Select an Stance ID", MessageType.Error);
                        }
                        break;
                    case ZoneType.Force:
                        EditorGUILayout.PropertyField(Force);
                        EditorGUILayout.PropertyField(EnterAceleration);
                        EditorGUILayout.PropertyField(ExitAceleration);
                        EditorGUILayout.PropertyField(Bounce);
                        break;
                    default:
                        break;
                }



                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    if (zone != ZoneType.Force)
                        EditorGUILayout.PropertyField(RemoveAnimalOnActive, new GUIContent("Remove Animal on Active", "Remove the Stored Animal on the Zone when the zones gets Active, Reseting the zone to its default state"));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_tag, new GUIContent("Tags", "Set this parameter if you want the zone to Interact only with gameObject with that tag"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(HeadOnly, new GUIContent("Bone Only", "Activate when a bone enter the Zone.\nThat Bone needs a collider!!"));
                    if (HeadOnly.boolValue)
                    {
                        EditorGUILayout.PropertyField(HeadName, new GUIContent("Bone Name", "Name for the Bone you need to check if it has enter the zone"));
                    }
                }
                EditorGUILayout.EndVertical();


                //if (zone == ZoneType.Mode)
                //{

                //}


                //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //{
                //    EditorGUI.indentLevel++;
                //    ShowStatModifiers.boolValue = EditorGUILayout.Foldout(ShowStatModifiers.boolValue, "Stat Modifiers");
                //    EditorGUI.indentLevel--;

                //    if (ShowStatModifiers.boolValue)
                //    {
                //        EditorGUILayout.PropertyField(StatModifierOnEnter, new GUIContent("On Enter", "Modify the Stat entering the Zone"), true);
                //        EditorGUILayout.PropertyField(StatModifierOnExit, new GUIContent("On Exit", "Modify the Stat exiting the Zone"), true);
                //        EditorGUILayout.PropertyField(statModifier, new GUIContent("On Active", "Modify the Stat when the Zone is Active"), true);
                //    }
                //}
                //EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                M.EditorShowEvents = EditorGUILayout.Foldout(M.EditorShowEvents, "Events");
                EditorGUI.indentLevel--;

                if (M.EditorShowEvents)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEnter"), new GUIContent("On Animal Enter Zone"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnExit"), new GUIContent("On Animal Exit Zone"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnZoneActivation"), new GUIContent("On Zone Active"));
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Zone Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
