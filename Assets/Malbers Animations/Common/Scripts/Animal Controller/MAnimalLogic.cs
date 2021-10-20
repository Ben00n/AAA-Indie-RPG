using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Utilities;
using UnityEngine.Events;
using MalbersAnimations.Events;
//using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

namespace MalbersAnimations.Controller
{
    public partial class MAnimal
    {
        void Awake()
        {
            if (Anim == null) Anim = GetComponentInParent<Animator>();   //Cache the Animator
            if (RB == null) RB = GetComponentInParent<Rigidbody>();      //Catche the Rigid Body  

            transform.parent = null; //IMPORTANT the animal cannot be parent of any game Object (Known Issue)

            if (Rotator != null && RootBone == null)
                RootBone = Rotator.GetChild(0);           //Find the First Rotator Child  THIS CAUSE ISSUES WITH TIMELINE!!!!!!!!!!!!

            GetHashIDs();

            foreach (var set in speedSets) set.CurrentIndex = set.StartVerticalIndex;

            if (RB)
            {
                RB.useGravity = false;
                RB.constraints = RigidbodyConstraints.FreezeRotation;
            }

            GetAnimalColliders();

            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] != null)
                {
                    if (CloneStates)
                    {
                        State instance = (State)ScriptableObject.CreateInstance(states[i].GetType());
                        instance = ScriptableObject.Instantiate(states[i]);                                 //Create a clone from the Original Scriptable Objects! IMPORTANT
                        instance.name = instance.name.Replace("(Clone)", "(C)");
                        states[i] = instance;
                    }

                    //statesD.Add(states[i].ID.ID, states[i]);        //Convert it to a Dictionary

                    states[i].SetAnimal(this);                                          //Awake all States
                   // states[i].Priority = states.Count - i;
                    states[i].AwakeState();
                }
            }



            //Awake all modes
            for (int i = 0; i < modes.Count; i++)
            {
                modes[i].Priority = modes.Count - i;
                modes[i].AwakeMode(this);
            }

            SetPivots();
            CalculateHeight();

            currentSpeedSet = new MSpeedSet() { Speeds = new List<MSpeed>(1) { new MSpeed("Default", 1, 4, 4) } }; //Create a Default Speed at Awake
            AlignUniqueID = UnityEngine.Random.Range(0, 99999);
        }

        public virtual void ResetController()
        {
            if (MainCamera == null) //Find the Camera
            {
                m_MainCamera.UseConstant = true;
                m_MainCamera.Value = MTools.FindMainCamera().transform;
            }

            foreach (var state in states)
            {
                state.InitializeState();
                state.InputValue = false;
                state.ResetState();
            }

            CheckIfGrounded(); //Make the first Alignment 

            MovementAxis = MovementAxisRaw = MovementAxisSmoothed = Vector3.zero; //Reset all movement
            UseRawInput = true; //Set the Raw Input as default.

            if (OverrideStartState != null)
            {
                State OverrideState = State_Get(OverrideStartState);
                if (OverrideState != null && !OverrideState.IsActiveState)
                {
                    activeState = OverrideState; //Set it as the Current Active State before Activating
                    OverrideState.Activate();
                }
                else Debug.LogWarning($"{name} cannot start with the State {OverrideStartState.name}. The state is not inlcuded.");
            }
            else
            {
                activeState = states[states.Count - 1]; //Set the First state as the active state (IMPORTANT TO BE THE FIRST THING TO DO)
                activeState.Activate();
            }

             
            activeState.IsPending = false;          //Force the active state to start without entering the animation.
            activeState.General.Modify(this);       //Force the active state to Modify all the Animal Settings
       

            Mode_Stop();

            //Set Start with Mode
            if (StartWithMode.Value != 0)
            {
                if (StartWithMode.Value / 1000 == 0)
                {
                    Mode_Activate(StartWithMode.Value);
                }
                else
                {
                    var mode = StartWithMode.Value / 1000;
                    var modeAb = StartWithMode.Value % 1000;
                    if (modeAb == 0) modeAb = -99;
                    Mode_Activate(mode, modeAb);
                }
            }

            ScaleFactor = transform.localScale.y;        //TOTALLY SCALABE animal

            if (Anim) Anim.speed = AnimatorSpeed;                         //Set the Global Animator Speed

            ResetGravityValues();


            LastPos = transform.position;
            AdditivePosition = Vector3.zero;
            InertiaPositionSpeed = Vector3.zero;

            UpdateDamagerSet();

            UpdateDirectionSpeed = true;
            UseAdditiveRot = true;
            UseAdditivePos = true;
            Grounded = true;
            Randomizer = true;
            AlwaysForward = AlwaysForward;         // Execute the code inside Always Forward .... Why??? Don't know ..something to do with the Input stuff
            Strafe = Strafe;                       // Execute the code inside Strafe
            Stance = currentStance;
            SpeedMultiplier = 1;
            CurrentFrame = 0;

            DefaultCameraInput = UseCameraInput;

            SetOptionalAnimParameter(Animator.StringToHash(m_Type), animalType); //This is only done once!
        }

        public void SetPivots()
        {
            Pivot_Hip = pivots.Find(item => item.name.ToUpper() == "HIP");
            Pivot_Chest = pivots.Find(item => item.name.ToUpper() == "CHEST");

            Has_Pivot_Hip = Pivot_Hip != null;
            Has_Pivot_Chest = Pivot_Chest != null;
            Starting_PivotChest = Has_Pivot_Chest;
        }

        void OnEnable()
        {
            if (Animals == null) Animals = new List<MAnimal>();
            Animals.Add(this);                                              //Save the the Animal on the current List


            UpdateInputSource(true); //Connect the Inputs


            if (isPlayer) SetMainPlayer();
            SetBoolParameter += SetAnimParameter;
            SetIntParameter += SetAnimParameter;
            SetFloatParameter += SetAnimParameter;

            if (!alwaysForward.UseConstant && alwaysForward.Variable != null)
                alwaysForward.Variable.OnValueChanged += Always_Forward;

            ResetController();
            Sleep = false;
        }

        void OnDisable()
        {
            if (Animals != null) Animals.Remove(this);       //Remove all this animal from the Overall AnimalList

            UpdateInputSource(false); //Disconnect the inputs

            DisableMainPlayer();


            MTools.ResetFloatParameters(Anim); //Reset all Anim Floats!!
            RB.velocity = Vector3.zero;

            SetBoolParameter -= SetAnimParameter;
            SetIntParameter -= SetAnimParameter;
            SetFloatParameter -= SetAnimParameter;


            if (!alwaysForward.UseConstant && alwaysForward.Variable != null)
                alwaysForward.Variable.OnValueChanged -= Always_Forward;

            foreach (var st in states) st.ExitState();
        }


        public void CalculateHeight()
        {
            if (height != 1) return; // means the height has already been calculated

            if (Has_Pivot_Hip)
            {
                height = Pivot_Hip.position.y;
                Center = Pivot_Hip.position; //Set the Center to be the Pivot Hip Position
            }
            else if (Has_Pivot_Chest)
            {
                height = Pivot_Chest.position.y;
                Center = Pivot_Chest.position;
            }


            if (Has_Pivot_Chest && Has_Pivot_Hip)
            {
                Center = (Pivot_Chest.position + Pivot_Hip.position) / 2;
            }
        }

        /// <summary>Update all the Attack Triggers Inside the Animal... In case there are more or less triggers</summary>
        public void UpdateDamagerSet()
        {
            Attack_Triggers = GetComponentsInChildren<IMDamager>(true).ToList();        //Save all Attack Triggers.
            foreach (var at in Attack_Triggers) at.Owner = (gameObject);                 //Tell to avery Damager that this Animal is the Owner
        }

        #region Animator Stuff
        protected virtual void GetHashIDs()
        {
            if (Anim == null) return;

            #region Main Animator Parameters
            //Movement
            hash_Vertical = Animator.StringToHash(m_Vertical);
            hash_Horizontal = Animator.StringToHash(m_Horizontal);
            hash_SpeedMultiplier = Animator.StringToHash(m_SpeedMultiplier);
            hash_Movement = Animator.StringToHash(m_Movement);
            hash_Grounded = Animator.StringToHash(m_Grounded);

            //States
            hash_State = Animator.StringToHash(m_State);
            hash_StateEnterStatus = Animator.StringToHash(m_StateStatus);
            hash_StateExitStatus = Animator.StringToHash(m_StateExitStatus);
            hash_LastState = Animator.StringToHash(m_LastState);
            hash_StateFloat = Animator.StringToHash(m_StateFloat);

            //Modes
            hash_Mode = Animator.StringToHash(m_Mode);
            hash_ModeStatus = Animator.StringToHash(m_ModeStatus);
            #endregion

            #region Optional Parameters
            //Movement
            hash_UpDown = Animator.StringToHash(m_UpDown);
            hash_Slope = Animator.StringToHash(m_Slope);
            hash_DeltaAngle = Animator.StringToHash(m_DeltaAngle);

            //States
            hash_StateTime = Animator.StringToHash(m_StateTime);
            hash_Strafe = Animator.StringToHash(m_Strafe);

            //Stance
            hash_Stance = Animator.StringToHash(m_Stance);
            hash_LastStance = Animator.StringToHash(m_LastStance);

            //Misc
            hash_Random = Animator.StringToHash(m_Random);
            hash_ModePower = Animator.StringToHash(m_ModePower);
            #endregion

            //Store all the Animator Parameter in a Dictionary
            animatorParams = new Hashtable();


            foreach (AnimatorControllerParameter parameter in Anim.parameters)
                animatorParams.Add(parameter.nameHash, parameter.name);
        }


        protected virtual void CacheAnimatorState()
        {
            m_PreviousCurrentState = m_CurrentState;
            m_PreviousNextState = m_NextState;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

            m_CurrentState = Anim.GetCurrentAnimatorStateInfo(0);
            m_NextState = Anim.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = Anim.IsInTransition(0);




            if (m_IsAnimatorTransitioning)
            {
                if (m_NextState.fullPathHash != 0)
                {
                    AnimStateTag = m_NextState.tagHash;
                    AnimState = m_NextState;
                }
            }
            else
            {
                if (m_CurrentState.fullPathHash != AnimState.fullPathHash)
                {
                    AnimStateTag = m_CurrentState.tagHash;
                }

                AnimState = m_CurrentState;
            }

            var lastStateTime = StateTime;
            StateTime = Mathf.Repeat(m_CurrentState.normalizedTime, 1);
            if (lastStateTime > StateTime) StateCycle?.Invoke(ActiveStateID); //Check if the Animation Started again.
        }

        /// <summary>Link all Parameters to the animator</summary>
        protected virtual void UpdateAnimatorParameters()
        {
            SetFloatParameter(hash_Vertical, VerticalSmooth);
            SetFloatParameter(hash_Horizontal, HorizontalSmooth);
            SetBoolParameter(hash_Movement, MovementDetected);

            SetOptionalAnimParameter(hash_UpDown, UpDownSmooth);
            SetOptionalAnimParameter(hash_DeltaAngle, DeltaAngle);
            SetOptionalAnimParameter(hash_Slope, SlopeNormalized);
            SetOptionalAnimParameter(hash_SpeedMultiplier, SpeedMultiplier);
            SetOptionalAnimParameter(hash_StateTime, StateTime);
        }

        #endregion

        #region Inputs

        internal void InputAxisUpdate()
        {
            if (UseRawInput)
            {
                var inputAxis = RawInputAxis;

                if (LockMovement || Sleep)
                {
                    MovementAxis = Vector3.zero;
                    return;
                }

                if (AlwaysForward) inputAxis.z = 1;


                if (MainCamera && UseCameraInput && !Strafe)
                {
                    var Cam_Forward = Vector3.ProjectOnPlane(MainCamera.forward, UpVector).normalized; //Normalize the Camera Forward Depending the Up Vector IMPORTANT!
                    var Cam_Right = Vector3.ProjectOnPlane(MainCamera.right, UpVector).normalized;


                    Vector3 UpInput;

                    if (UseCameraUp)
                    {
                        UpInput = (inputAxis.y * MainCamera.up);
                        UpInput += Vector3.Project(MainCamera.forward, UpVector) * inputAxis.z;
                    }
                    else
                    {
                        UpInput = (inputAxis.y * UpVector);

                        if (FreeMovement && inputAxis.y != 0 && inputAxis.z == 0)
                            inputAxis.z = 0.01f; //Hack to add a bit of Forward movement to find which direction is forward
                    }


                    if (Grounded) UpInput = Vector3.zero;            //Reset the UP Input in case is on the Ground
                    var m_Move = ((inputAxis.z * Cam_Forward) + (inputAxis.x * Cam_Right) + UpInput);


                    MoveDirection(m_Move);
                }
                else
                {
                    MoveWorld(inputAxis);
                }
            }
            else //Means that is Using a Direction Instead  Update every frame
            {
                MoveDirection(RawInputAxis);
            }
        }


        /// <summary>Get the Raw Input Axis from a source</summary>
        public virtual void SetInputAxis(Vector3 inputAxis)
        {
            UseRawInput = true;
            RawInputAxis = inputAxis; // Store the last current use of the Input
        }

        public virtual void SetInputAxis(Vector2 inputAxis) => SetInputAxis(new Vector3(inputAxis.x, 0, inputAxis.y));



        public virtual void SetInputAxisXY(Vector2 inputAxis) => SetInputAxis(new Vector3(inputAxis.x, inputAxis.y, 0));

        public virtual void SetInputAxisYZ(Vector2 inputAxis) => SetInputAxis(new Vector3(0, inputAxis.x, inputAxis.y));

        /// <summary>Use this for Custom UpDown Movement</summary>
        public virtual void SetUpDownAxis(float upDown) => SetInputAxis(new Vector3(RawInputAxis.x, upDown, RawInputAxis.z));

        /// <summary>Gets the movement from the World Coordinates</summary>
        /// <param name="move">World Direction Vector</param>
        protected virtual void MoveWorld(Vector3 move)
        {
            MoveWithDirection = false;

            if (!UseSmoothVertical && move.z > 0) move.z = 1;                      //It will remove slowing Stick push when rotating and going Forward
            Target_Direction = transform.TransformDirection(move).normalized;    //Convert from world to relative IMPORTANT

            SetMovementAxis(move);
        }

        private void SetMovementAxis(Vector3 move)
        {
            MovementAxis = move;
            MovementAxisRaw = MovementAxis;

            MovementDetected = MovementAxis.x != 0 || MovementAxis.z != 0 || MovementAxis.y != 0;
            MovementAxis.Scale(new Vector3(LockHorizontalMovement ? 0 : 1, LockUpDownMovement ? 0 : 1, LockForwardMovement ? 0 : 1));

            if (debugGizmos) Debug.DrawRay(transform.position, Target_Direction, Color.red);
        }

        /// <summary>Gets the movement from a Direction</summary>
        /// <param name="move">Direction Vector</param>
        protected virtual void MoveDirection(Vector3 move)
        {
            if (LockMovement)
            {
                MovementAxis = Vector3.zero;
                return;
            }

            MoveWithDirection = true;

            var UpDown = move.y;

            if (move.magnitude > 1f) move.Normalize();

            if (Grounded)
                move = Quaternion.FromToRotation(UpVector, SurfaceNormal) * move;    //Rotate with the ground Surface Normal


            Target_Direction = move;
            move = transform.InverseTransformDirection(move);               //Convert the move Input from world to Local

            float turnAmount = Mathf.Atan2(move.x, move.z);                 //Convert it to Radians
            float forwardAmount = move.z < 0 ? 0 : move.z;
           // float forwardAmount =  move.z;



            // Find the difference between the current rotation of the player and the desired rotation of the player in radians.
            float angleCurrent = Mathf.Atan2(Forward.x, Forward.z) * Mathf.Rad2Deg;
            float targetAngle = Mathf.Atan2(Target_Direction.x, Target_Direction.z) * Mathf.Rad2Deg;
            var Delta = Mathf.DeltaAngle(angleCurrent, targetAngle);
            DeltaAngle = MovementDetected ? Delta : 0;



            if (!UseSmoothVertical)                       //It will remove slowing Stick push when rotating and going Forward
            {
                forwardAmount = Mathf.Abs(move.z);
                forwardAmount = forwardAmount > 0 ? 1 : forwardAmount;
            }
            else
            {
                if (Mathf.Abs(DeltaAngle) < 120)
                    forwardAmount = Mathf.Clamp01(RawInputAxis.magnitude);

            }



            if (Rotate_at_Direction)
            {
                if (Mathf.Abs(Delta) <= 20)
                {
                    turnAmount = 0;
                    DeltaAngle = 0;
                }
                 
                forwardAmount = 0;
            }

            SetMovementAxis(new Vector3(turnAmount, UpDown, forwardAmount));

          //  Debug.Log($"MOV Axis"+MovementAxis);
        }

        /// <summary>Gets the movement from a Direction but it wont fo forward it will only rotate in place</summary>
        public virtual void RotateAtDirection(Vector3 direction)
        { 
            RawInputAxis = direction; // Store the last current use of the Input
            UseRawInput = false;
            Rotate_at_Direction = true;
            //Debug.Log($"RotateAtDirection");
        }
        #endregion

        #region Additional Speeds (Movement, Turn)

        /// <summary>Add more Rotations to the current Turn Animations  </summary>
        protected virtual void AdditionalTurn(float time)
        {
            float SpeedRotation = CurrentSpeedModifier.rotation;

            if (VerticalSmooth < 0.01 && !CustomSpeed && CurrentSpeedSet != null)
            {
                SpeedRotation = CurrentSpeedSet[0].rotation;
            }

            if (SpeedRotation < 0) return;          //Do nothing if the rotation is lower than 0

            if (MovementDetected)
            {
                float ModeRotation = (IsPlayingMode && !ActiveMode.AllowRotation) ? 0 : 1;//If the mode does not allow rotation set the multiplier to zero

                if (MoveWithDirection)
                {
                    var TargetLocalRot = Quaternion.Euler(0, DeltaAngle, 0);
                    Quaternion targetRotation = Quaternion.Slerp(Quaternion.identity, TargetLocalRot, (SpeedRotation + 1) / 4 * ((TurnMultiplier + 1) * time * ModeRotation));
                    AdditiveRotation *= targetRotation;
                }
                else
                {
                    float Turn = SpeedRotation * 10;           //Add Extra Multiplier
                    float TurnInput = Mathf.Clamp(HorizontalSmooth, -1, 1) * (MovementAxis.z >= 0 ? 1 : -1);  //Add +Rotation when going Forward and -Rotation when going backwards
                    AdditiveRotation *= Quaternion.Euler(0, Turn * TurnInput * time * ModeRotation, 0);
                    var TargetGlobal = Quaternion.Euler(0, TurnInput * (TurnMultiplier + 1), 0);
                    var AdditiveGlobal = Quaternion.Slerp(Quaternion.identity, TargetGlobal, time * (SpeedRotation + 1) * ModeRotation);
                    AdditiveRotation *= AdditiveGlobal;
                }
            }
        }

        /// <summary> Add more Speed to the current Move animations</summary>  
        protected virtual void AdditionalSpeed(float time)
        {
            var LPos = CurrentSpeedModifier.lerpPosition;

            InertiaPositionSpeed = (LPos > 0) ? Vector3.Lerp(InertiaPositionSpeed, TargetSpeed, time * LPos) :  TargetSpeed;
              
            if (ActiveMode != null && !ActiveMode.AllowMovement) //Remove Speeds if the Mode is Playing  a Mode
                InertiaPositionSpeed = Vector3.zero;

            if (debugGizmos)
            {
                Debug.DrawRay(transform.position, TargetSpeed.normalized, Color.cyan);
                Debug.DrawRay(transform.position, InertiaPositionSpeed * 10, Color.white);
            } 

            AdditivePosition += InertiaPositionSpeed;
        }

        #endregion

        internal void PlatformMovement()
        {
            if (platform == null) return;

            var DeltaPlatformPos = platform.position - platform_Pos;
            DeltaPlatformPos.y = 0;                                                                                       //the Y is handled by the Fix Position

            AdditivePosition += DeltaPlatformPos;                          // Keep the same relative position.

            Quaternion Inverse_Rot = Quaternion.Inverse(platform_Rot);
            Quaternion Delta = Inverse_Rot * platform.rotation;

            if (Delta != Quaternion.identity)                                        // no rotation founded.. Skip the code below
            {
                var pos = transform.DeltaPositionFromRotate(platform, Delta);
                AdditivePosition += pos;
            }

            AdditiveRotation *= Delta;

            platform_Pos = platform.position;
            platform_Rot = platform.rotation;


        }

        #region Terrain Alignment
        /// <summary>Raycasting stuff to align and calculate the ground from the animal ****IMPORTANT***</summary>
        internal virtual void AlignRayCasting()
        {
            MainRay = FrontRay = false;
            hit_Chest = new RaycastHit();                               //Clean the Raycasts every time 
            hit_Hip = new RaycastHit();                                 //Clean the Raycasts every time 
            hit_Chest.distance = hit_Hip.distance = Height;            //Reset the Distances to the Heigth of the animal

            //var Direction = Gravity;
            var Direction = -transform.up;



            if (Physics.Raycast(Main_Pivot_Point, Direction, out hit_Chest, Pivot_Multiplier, GroundLayer, QueryTriggerInteraction.Ignore))
            {
                FrontRay = true;

                var MainPivotSlope = Vector3.SignedAngle(hit_Chest.normal, UpVector, Right);


                if (MainPivotSlope > maxAngleSlope)  //Means that the Slope is higher thanthe Max slope so stop the animal from Going forward AND ONLY ON LOCOMOTION 
                {
                    if (MovementAxisRaw.z > 0 && !hit_Chest.transform.gameObject.CompareTag("Stair"))
                    {
                        AdditivePosition = Vector3.ProjectOnPlane(AdditivePosition, Forward); //Remove Forward Movement
                        MovementAxis.z = 0;
                    }
                }

                //    else
                {
                    if (debugGizmos)
                    {
                        Debug.DrawRay(hit_Chest.point, hit_Chest.normal * ScaleFactor * 0.2f, Color.green);
                        MTools.DrawWireSphere(Main_Pivot_Point + Direction * (hit_Chest.distance - RayCastRadius), Color.green, RayCastRadius * ScaleFactor);
                    }

                    if (platform == null || platform != hit_Chest.transform)               //Platforming logic
                    {
                        SetPlatform(hit_Chest.transform);
                    }

                    hit_Chest.collider.attachedRigidbody?.AddForceAtPosition(Gravity * (RB.mass / 2), hit_Chest.point, ForceMode.Force);
                }
            }
            else
            {
                platform = null;
            }

            if (Has_Pivot_Hip && Has_Pivot_Chest) //Ray From the Hip to the ground
            {
                var hipPoint = Pivot_Hip.World(transform) + DeltaVelocity;

                if (Physics.Raycast(hipPoint, Direction, out hit_Hip, ScaleFactor * Pivot_Hip.multiplier, GroundLayer, QueryTriggerInteraction.Ignore))
                {
                    MainRay = true;

                    if (debugGizmos)
                    {
                        Debug.DrawRay(hit_Hip.point, hit_Hip.normal * ScaleFactor * 0.2f, Color.green);
                        MTools.DrawWireSphere(hipPoint + Direction * (hit_Hip.distance - RayCastRadius), Color.green, RayCastRadius * ScaleFactor);
                    }

                    if (platform == null || platform != hit_Chest.transform)                 //Platforming logic
                    {
                        SetPlatform(hit_Chest.transform);
                    }

                    hit_Hip.collider.attachedRigidbody?.AddForceAtPosition(Gravity * (RB.mass / 2), hit_Hip.point, ForceMode.Force);

                    if (!FrontRay) hit_Chest = hit_Hip; //If there's no Front Ray
                }
                else
                {
                    platform = null;

                    if (FrontRay)
                    {
                        MovementAxis.z = 1; //Force going forward in case there's no Back Ray (HACK)
                        hit_Hip = hit_Chest;  //In case there's no Hip Ray
                    }
                }
            }
            else
            {
                MainRay = FrontRay; //Just in case you dont have HIP RAY IMPORTANT FOR HUMANOID CHARACTERS
                hit_Hip = hit_Chest;  //In case there's no Hip Ray
            }

            if (ground_Changes_Gravity)
                Gravity = -hit_Hip.normal;

            CalculateSurfaceNormal();
        }

        internal virtual void CalculateSurfaceNormal()
        {
            if (Has_Pivot_Hip)
            {
                Vector3 TerrainNormal;

                if (Has_Pivot_Chest)
                {
                    Vector3 direction = (hit_Chest.point - hit_Hip.point).normalized;
                    Vector3 Side = Vector3.Cross(UpVector, direction).normalized;
                    SurfaceNormal = Vector3.Cross(direction, Side).normalized;

                    //SurfaceNormal =  (hit_Chest.normal + hit_Hip.normal).normalized;
                    TerrainNormal = SurfaceNormal;
                }
                else
                {
                    SurfaceNormal = TerrainNormal = hit_Hip.normal;
                }

                TerrainSlope = Vector3.SignedAngle(TerrainNormal, UpVector, Right);
            }
            else
            {
                TerrainSlope = Vector3.SignedAngle(hit_Hip.normal, UpVector, Right);
                SurfaceNormal = UpVector;
            }
        }

        /// <summary>Align the Animal to Terrain</summary>
        /// <param name="align">True: Aling to UP, False Align to Terrain</param>
        internal virtual void AlignRotation(bool align, float time, float smoothness)
        {
            AlignRotation(align ? SurfaceNormal : UpVector, time, smoothness);
        }

        /// <summary>Align the Animal to a Custom </summary>
        /// <param name="align">True: Aling to UP, False Align to Terrain</param>
        internal virtual void AlignRotation(Vector3 alignNormal, float time, float smoothness)
        {
            Quaternion AlignRot = Quaternion.FromToRotation(transform.up, alignNormal) * transform.rotation;  //Calculate the orientation to Terrain 
            Quaternion Inverse_Rot = Quaternion.Inverse(transform.rotation);
            Quaternion Target = Inverse_Rot * AlignRot;
            Quaternion Delta = Quaternion.Lerp(Quaternion.identity, Target, time * smoothness); //Calculate the Delta Align Rotation

            // _transform.rotation = Quaternion.Lerp(_transform.rotation, AlignRot, time * smoothness);

            transform.rotation *= Delta;
            //AdditiveRotation *= Delta;
        }

        /// <summary>Snap to Ground with Smoothing</summary>
        internal void AlignPosition(float time)
        {
            if (!MainRay && !FrontRay) return;                                              //DO NOT ALIGN  IMPORTANT This caused the animals jumping upwards when falling down
            AlignPosition(hit_Hip.distance, time, AlignPosLerp * 2);
        }

        internal void AlignPosition(float distance, float time, float Smoothness)
        {
            //Debug.Log(distance);
            float difference = Height - distance;

            if (!Mathf.Approximately(distance, Height))
            {
                var DeltaDif = difference * time * Smoothness;

                Vector3 align = transform.rotation * new Vector3(0, DeltaDif, 0); //Rotates with the Transform to better alignment
                AdditivePosition += align;
                hit_Hip.distance += DeltaDif;
            }
        }

        /// <summary>Snap to Ground with no Smoothing</summary>
        internal virtual void AlignPosition_Distance(float distance)
        {
            float difference = Height - distance;
            AdditivePosition += transform.rotation * new Vector3(0, difference, 0); //Rotates with the Transform to better alignment
        }


        /// <summary>Snap to Ground with no Smoothing</summary>
        internal virtual void AlignPosition()
        {
            float difference = Height - hit_Hip.distance;
            AdditivePosition += transform.rotation * new Vector3(0, difference, 0); //Rotates with the Transform to better alignment
        }
        #endregion

        /// <summary> Movement Trot Walk Run (Velocity changes)</summary>
        internal void MovementSystem(float DeltaTime)
        {
            float maxspeedV = CurrentSpeedModifier.Vertical;
            float maxspeedH = 1;

            var LerpUpDown = DeltaTime * UpDownLerp.Value;
            var LerpVertical = DeltaTime * CurrentSpeedModifier.lerpPosAnim;
            var LerpTurn = DeltaTime * CurrentSpeedModifier.lerpRotAnim;
            var LerpAnimator = DeltaTime * CurrentSpeedModifier.lerpAnimator;

            if (Strafe)
            {
                maxspeedH = maxspeedV; //if the animal is strafing
                LerpVertical = LerpTurn = LerpUpDown = DeltaTime * CurrentSpeedModifier.lerpStrafe;
            }

            if (IsPlayingMode && !ActiveMode.AllowMovement) //Active mode and Isplaying Mode is failing!!**************
                MovementAxis = Vector3.zero;



            VerticalSmooth = LerpVertical > 0 ? Mathf.MoveTowards(VerticalSmooth, MovementAxis.z * maxspeedV, LerpVertical) : MovementAxis.z * maxspeedV;           //smoothly transitions bettwen Speeds
            HorizontalSmooth = LerpTurn > 0 ? Mathf.Lerp(HorizontalSmooth, MovementAxis.x * maxspeedH, LerpTurn) : MovementAxis.x * maxspeedH;               //smoothly transitions bettwen Directions
            UpDownSmooth = LerpVertical > 0 ? Mathf.Lerp(UpDownSmooth, MovementAxis.y, LerpUpDown) : MovementAxis.y;                                                //smoothly transitions bettwen Directions
            SpeedMultiplier = (LerpAnimator > 0) ? Mathf.MoveTowards(SpeedMultiplier, CurrentSpeedModifier.animator.Value, LerpAnimator) : CurrentSpeedModifier.animator.Value;  //Changue the velocity of the animator

        }

        /// <summary> Try Activate all other states </summary>
        protected void TryActivateState()
        {
            if (ActiveState.IsPersistent) return;        //If the State cannot be interrupted the ignored trying activating any other States
            if (JustActivateState) return;               //Do not try to activate a new state since there already a new one on Activation

            foreach (var trySt in states)
            {
                if (trySt.IsActiveState) continue;      //Skip Re-Activating yourself
                if (ActiveState.IgnoreLowerStates && ActiveState.Priority > trySt.Priority) return; //Do not check lower priority states

                if (trySt.OnActiveQueue && !trySt.OnQueue)  //Wake UP the State that is no longer on QUEUE and it was activated! (PRIORITY FOR THE QUEDED STATES)!
                {
                    trySt.ActivateQueued();
                    break;
                }

                if ((trySt.UniqueID + CurrentFrame) % trySt.TryLoop != 0) continue;   //Check the Performance Loop for the  trying state

                if (!ActiveState.IsPending && ActiveState.CanExit/* || ActiveState.CanExit && !ActiveState.IsPending*/)    //Means a new state can be activated
                {
                    if (trySt.Active && !trySt.OnExitCoolDown && !trySt.IsSleepFromState && !trySt.IsSleepFromMode && !trySt.OnQueue && trySt.TryActivate())
                    {
                        trySt.Activate();
                        break;
                    }
                }
            }
        }

        /// <summary>Check if the Active State can exit </summary>
        private void TryExitActiveState()
        {
            if (ActiveState.CanExit)
                ActiveState.TryExitState(DeltaTime);     //if is not in transition and is in the Main Tag try to Exit to lower States
        }

        /// <summary>Calculates the Pitch direction to Appy to the Rotator Transform</summary>
        internal void CalculatePitchDirectionVector()
        {
           PitchDirection = Vector3.Lerp( PitchDirection, Target_Direction, DeltaTime * UpDownLerp * 2);
           if (debugGizmos) Debug.DrawRay(transform.position, PitchDirection * 1f,  Color.yellow  );
        }

        void OnAnimatorMove()
        {
            if (Sleep)
            {
                Anim.ApplyBuiltinRootMotion();
                return;
            }
             
            CacheAnimatorState();
            ResetValues();

            if (ActiveState == null) return;

            bool AnimatePhysics = Anim.updateMode == AnimatorUpdateMode.AnimatePhysics;
            DeltaTime = AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;

            InputAxisUpdate(); //make sure when using Raw Input UPDATE the Calculations ****IMPORTANT****


            if (UpdateDirectionSpeed)
                DirectionalVelocity = FreeMovement ? PitchDirection : transform.forward; //Calculate the Direction Speed for the Additive Speed Position Direction


            if (UseAdditivePos) AdditionalSpeed(DeltaTime);
            if (UseAdditiveRot) AdditionalTurn(DeltaTime);
            RemoveHorizontalMovement();

             
            if (InputMode != null && InputMode.InputValue) 
                InputMode.TryActivate(); //FOR MODES  Still need to work this better yeah I know (IS WHEN THE INPUT IS PRESSED ON A MODE

            if (JustActivateState) //This definetely needs to be after the Try Activate!!!!!
            {
                if (LastState.ExitFrame) LastState.OnStateMove(DeltaTime);           //Play One Last Time the Last State
                //StartCoroutine(JustActivateStateClearNextFrame());
                JustActivateState = false;  
            }

            ApplyExternalForce();


            if (Grounded)
            {
                if (AlignLoop.Value <= 1 || (AlignUniqueID + CurrentFrame) % AlignLoop.Value == 0)
                    AlignRayCasting();

                AlignPosition(DeltaTime);

                if (!UseCustomAlign)
                    AlignRotation(UseOrientToGround, DeltaTime, AlignRotLerp);

                PlatformMovement();
            }
            else
            {
                MainRay = FrontRay = false;
                SurfaceNormal = UpVector;

                if (!UseCustomAlign)
                    AlignRotation(false, DeltaTime, AlignRotLerp); //Align to the Gravity Normal
                TerrainSlope = 0;
            }



            if (!FreeMovement)
            {
                if (Rotator != null)
                    Rotator.localRotation = Quaternion.Slerp(Rotator.localRotation, Quaternion.identity, DeltaTime * (AlignPosLerp / 2)); //Improve this!!


                PitchAngle = Mathf.Lerp(PitchAngle, 0, DeltaTime * UpDownLerp *2);
                Bank = Mathf.Lerp(Bank, 0, DeltaTime * (AlignPosLerp / 2));
            }
            else
                CalculatePitchDirectionVector();

            ActiveState.OnMoveState(DeltaTime);                                                     //UPDATE THE STATE BEHAVIOUR

            TryExitActiveState();

            TryActivateState();

            MovementSystem(DeltaTime);
            GravityLogic();


            LastPos = transform.position;

            if (float.IsNaN(AdditivePosition.x)) return;


            if (!DisablePositionRotation)
            {
                if (AnimatePhysics && RB)
                {

                    if (RB.isKinematic)
                    {
                        transform.position += AdditivePosition;
                    }
                    else
                    {
                        RB.velocity = Vector3.zero;
                        RB.angularVelocity = Vector3.zero;

                        if (DeltaTime > 0)
                        {
                            RB.velocity = AdditivePosition / DeltaTime;
                        }
                    }
                }
                else
                {
                    transform.position += AdditivePosition;
                }

                transform.rotation *= AdditiveRotation;
            }

            UpdateAnimatorParameters();              //Set all Animator Parameters
        }

        private void ApplyExternalForce()
        {
            var Acel = ExternalForceAcel > 0 ? (DeltaTime * ExternalForceAcel) : 1;
            CurrentExternalForce = Vector3.Lerp(CurrentExternalForce, ExternalForce, Acel);
            if (CurrentExternalForce != Vector3.zero) 
                AdditivePosition += CurrentExternalForce * DeltaTime;
        }

        private void GravityLogic()
        {
            if (UseGravity)
            {
                if (Grounded) return;  //Means has found the Ground

                var GTime = DeltaTime * GravityTime;

                GravityStoredVelocity = Gravity * m_gravityPower * (GTime * GTime / 2);
                AdditivePosition += GravityStoredVelocity * DeltaTime;                                         //Add Gravity if is in use
                GravityTime++;

                if (LimitGravityTime > 0 && LimitGravityTime < GravityTime) GravityTime--; //Make limit

                ///// Hack for Gravity Aceleration when is not falling !!
                ///REMOVED AND EVERYTHING ELSE WORKS!
                //bool GoingDown = Vector3.Dot(DeltaPos, Gravity) > 0;
                //if (GoingDown) //if the Animal is going down but suddenly has bump into somehting RESET the GRAVITY VALYES UGLY FIX
                //{
                //    var deltaPosY = Vector3.Project(DeltaPos, Gravity).sqrMagnitude / DeltaTime;
                //    if (deltaPosY < GravitySpeed && GravityTime > (m_gravityTime + 20) && GravitySpeed > 1)
                //    {
                //        Debug.Log("RESET Gravity");
                //        //Quick fix! it seems to work!!!
                //        GravityTime--;
                //        //Grounded = true;
                //        //ResetGravityValues();
                //        //ResetUPVector();
                //         InertiaPositionSpeed = Forward * ScaleFactor * DeltaTime * FallForward; //Force going forward
                //        GravitySpeed = 0;
                //        return;
                //    }
                //    GravitySpeed = deltaPosY;
                //}
                //else
                //{
                //    GravitySpeed = 0;
                //}
            }
        }
 
        /// <summary> Smooth Value between Vertical and Forward Used for Flying</summary>
        public float SmoothZY
        {
            get
            {
                if (!UpdateDirectionSpeed) return 1;   //Why???,  I dont Remember why?? but I can't remove it either  LOL
                if (IsPlayingMode && !ActiveMode.AllowMovement) return 0;

                return Mathf.Clamp01(Mathf.Max(Mathf.Abs(UpDownSmooth), Mathf.Abs(VerticalSmooth)));
            }
        }
 
        private void RemoveHorizontalMovement()
        {
            if (Remove_HMovement)
            {
                AdditivePosition = Vector3.Project(AdditivePosition, UpVector);
                MovementAxis.z = 0;
            }
            Remove_HMovement = false;
        }

        public virtual void FreeMovementRotator(float Ylimit, float bank, float deltatime)
        {
            Rotator.localEulerAngles = Vector3.zero;

            float NewAngle = 0;

            if (PitchDirection.magnitude > 0.001)                                                          //Rotation PITCH
            {
                NewAngle = 90 - Vector3.Angle(UpVector, PitchDirection);
                NewAngle = Mathf.Clamp(-NewAngle, -Ylimit, Ylimit);
            }

            PitchAngle = Mathf.Lerp(PitchAngle, NewAngle * SmoothZY, deltatime * UpDownLerp);

            Bank = Mathf.Lerp(Bank, -bank * Mathf.Clamp(HorizontalSmooth, -1, 1), deltatime * 5);

            var PitchRot = new Vector3(PitchAngle, 0, Bank);

            Rotator.localEulerAngles = PitchRot;
        }

        //IEnumerator JustActivateStateClearNextFrame()
        //{
        //    yield return null;
        //    JustActivateState = false;
        //}

        /// <summary> Resets Additive Rotation and Additive Position to their default</summary>
        void ResetValues()
        {
            // MainRay = FrontRay = false;
            AdditivePosition = RootMotion ? Anim.deltaPosition : Vector3.zero;
            AdditiveRotation = RootMotion ? Anim.deltaRotation : Quaternion.identity;


            // SurfaceNormal = UpVector;
            ScaleFactor = transform.localScale.y;                      //Keep Updating the Scale Every Frame
            DeltaPos = transform.position - LastPos;                    //DeltaPosition from the last frame
            CurrentFrame = (CurrentFrame + 1) % 999999999;


            var DeltaRB = RB.velocity * DeltaTime;
            DeltaVelocity = Grounded ? Vector3.ProjectOnPlane(DeltaRB, UpVector) : DeltaRB; //When is not grounded take the Up Vector this is the one!!!
        }
    }
}