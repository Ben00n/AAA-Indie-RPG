using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/fly")]
    public class Fly : State
    {
        public enum FlyInput { Toggle, Press, None}

        [Header("Fly Parameters")]
        [Range(0, 90),Tooltip("Bank amount used when turning")]
        public float Bank = 30;
        [Range(0, 90), Tooltip("Limit to go Up and Down")]
        public float Ylimit = 80;

        [Tooltip("Bank amount used when turning while straffing")]
        public float BankStrafe = 0; 
        [Tooltip("Limit to go Up and Down while straffing")]
        public float YlimitStrafe = 0;

        //[Space, Tooltip("Type of Fly Input for Activating Flying. \nToggle: Press the Input Down to Start Flying. Press when Flying to Stop Flying.\nPress: As long as the Input is Pressed the Animal will keep Flying")]
        //public FlyInput flyInput = FlyInput.Toggle;



        [Space,Tooltip("The animal will move forward while flying, without the need to push the W Key, or Move forward Input")]
        public BoolReference AlwaysForward = new BoolReference(false);
        private bool LastAlwaysForward;

        [Tooltip("When the Animal is close to the Ground it will automatically Land")]
        public BoolReference canLand = new BoolReference( true);
        [Tooltip("Layers to Land on")]
        public LayerMask LandOn = (1);
        //[Tooltip("Doesn't allow landing after certain time")]
        //public FloatReference landTime = new FloatReference(1f);
        //private float currentFlyElapsedTime;

        [Tooltip("Ray Length multiplier to check for ground and automatically land (increases or decreases the MainPivot Lenght for the Fall Ray")]
        public FloatReference LandMultiplier = new FloatReference(1f);
        [Tooltip("When Entering the Fly State... The animal will keep the Velocity from the last State if this value is greater than zero")]
        [FormerlySerializedAs("InertiaTime")]
        public FloatReference InertiaLerp = new FloatReference(1);
        
        
        [Header("Avoid Water"),Tooltip("Avoids Water when Flying")]
        public bool AvoidWater = false;
        [Tooltip("Radius of the spherecast for Finding Water")]
        public float WRadius = 0.1f;
        [Tooltip("Distance for spherecast Ray for Finding Water")]
        public float WDistance = 0.5f;

        private int WaterLayer;


        //[Tooltip("Uses the Rotator on the Animal to Apply Rotations. If the Animations Rotates  the Animal. Disable this")]
        //public BoolReference UsePitchRotation = new BoolReference(true);


        [Header("Gliding")]
        public BoolReference GlideOnly = new BoolReference(false);
        public BoolReference AutoGlide = new BoolReference(true);
        [MinMaxRange(0, 10)]
        public RangedFloat GlideChance = new RangedFloat(0.8f, 4);
        [MinMaxRange(0, 10)]
        public RangedFloat FlapChange = new RangedFloat(0.5f, 4);

        public int FlapSpeed = 1;
        public int GlideSpeed = 2;
        [Tooltip("Variation to make Random Flap and Glide Animation")]
        public float Variation = 0.3f;

        protected bool isGliding = false;
        protected float FlyStyleTime = 1;
       // private float DistanceToGround;
        private bool FoundLand;
        private bool GoingDown;

        protected float AutoGlide_CurrentTime = 1;
        RaycastHit[] LandHit = new RaycastHit[1];

        [Header("Down Acceleration")]
        public FloatReference GravityDrag = new FloatReference(0);
        public FloatReference DownAcceleration = new FloatReference(0.5f);
        private float acceleration = 0;

        protected Vector3 verticalInertia;

        [Header("Bone Blocking Landing"),Tooltip("Somethimes the Head blocks the Landing Ray.. this will solve the landing by raycasting a ray from the Bone that is blocking the Logic")]
        /// <summary>If the Animal is a larger one sometimes </summary>
        public bool BoneBlockingLanding = false;
        [Hide("BoneBlockingLanding", true),Tooltip("Name of the blocker bone")]
        public string BoneName = "Head";
        [Hide("BoneBlockingLanding", true),Tooltip("Local Offset from the Blocker Bone")]
        public Vector3 BoneOffsetPos = Vector3.zero;
        [Hide("BoneBlockingLanding", true),Tooltip("Distance of the Landing Ray from the blocking Bone")]
        public float BlockLandDist = 0.4f;
        private Transform BlockingBone;

        //public override void StatebyInput()
        //{
        //    if (InputValue && !IsActiveState)                       //Enable fly if is not already active
        //    {
        //        InputValue = !(flyInput == FlyInput.Toggle);        //Reset the Input to false if is set to toggle
        //        Activate();
        //    }
        //}


        public override void InitializeState()
        {
            LandHit = new RaycastHit[1];
            AutoGlide_CurrentTime = Time.time;
            FlyStyleTime = GlideChance.RandomValue;
            WaterLayer = LayerMask.GetMask("Water");
            SearchForContactBone();
        }

        /// <summary>When using Contact bone Find it on the Animal that is using it</summary>
        void SearchForContactBone()
        {
            BlockingBone = null;

            if (BoneBlockingLanding) 
                BlockingBone = animal.transform.FindGrandChild(BoneName);
        }

        public override void Activate()
        {
            base.Activate();
            LastAlwaysForward = animal.AlwaysForward;
            animal.AlwaysForward = AlwaysForward;
            InputValue = true; //Make sure the Input is set to True when the flying is not being activated by an input player
        }

 
        public override void EnterCoreAnimation()
        {
            verticalInertia = Vector3.Project(animal.DeltaPos, animal.UpVector);
            animal.PitchDirection = animal.Forward;

            acceleration = 0;
            animal.LastState = this; //IMPORTANT for Modes that changes the Last state enter ?????????????????????????

            animal.InertiaPositionSpeed = Vector3.ProjectOnPlane(animal.DeltaPos, animal.Up); //Keep the Speed from the take off

            if (GlideOnly.Value)
            {
                animal.currentSpeedModifier.Vertical = GlideSpeed;
                animal.UseSprintState = false;
                animal.Speed_Change_Lock(true);
            }
            else
            {
                animal.currentSpeedModifier.Vertical = FlapSpeed;
                isGliding = true;
            }
        }

        public override void OnStateMove(float deltatime)
        {
            if (InCoreAnimation) //While is flying
            {

                var limit = Ylimit;
                var bank = Bank;

                if (animal.Strafe)
                {
                    limit = YlimitStrafe;
                    bank = BankStrafe;
                }

                GoingDown = animal.UpDownSmooth <= 0;

                if (GlideOnly && !GoingDown)
                {
                    RemoveUpDown();
                    limit = 0;
                }
                else if (AutoGlide)
                    AutoGliding();


                GravityPush(deltatime); //Add artificial gravity to the Fly

                if (BlockingBone && animal.MovementAxis.y < 0)
                {
                    var HitPoint = BlockingBone.TransformPoint(BoneOffsetPos);

                    if (debug) Debug.DrawRay(HitPoint, animal.Gravity * BlockLandDist * animal.ScaleFactor, Color.magenta);

                    bool Hit = Physics.RaycastNonAlloc(HitPoint, animal.Gravity, LandHit, BlockLandDist * animal.ScaleFactor, animal.GroundLayer, QueryTriggerInteraction.Ignore) > 0;

                    if (Hit)
                    {
                        RemoveUpDown();
                        limit = 0;
                    }
                } 
               

                if (General.FreeMovement)
                    animal.FreeMovementRotator(limit, bank, deltatime);


                if (AvoidWater)
                {
                    var WaterPos = transform.position + animal.AdditivePosition;
                    var Dist = WDistance * animal.ScaleFactor;
                    var Gravity = animal.Gravity;
               


                    if (Physics.Raycast(WaterPos, Gravity, out RaycastHit hit, Dist, WaterLayer))
                    {
                        Color findWater = Color.cyan;

                        if (animal.MovementAxis.y < 0) animal.MovementAxis.y = 0;

                        if (hit.distance < Dist * 0.75f)
                        {
                            animal.AdditivePosition += Gravity * -(Dist * 0.75f - hit.distance);
                        }

                        if (debug) Debug.DrawRay(WaterPos, Gravity * Dist, findWater);
                        return;
                    }
                }

                if (InertiaLerp.Value > 0)
                    animal.AddInertia(ref verticalInertia, InertiaLerp);
            }
        }
        public override void TryExitState(float DeltaTime)
        {
            if (!InputValue) AllowExit();

          

            if (canLand.Value && GoingDown)
            {
                var MainPivot = animal.Main_Pivot_Point + animal.AdditivePosition;

                float LandDistance = (animal.Height * LandMultiplier) / animal.ScaleFactor;

                if (debug) Debug.DrawRay(MainPivot, animal.Gravity * LandDistance, Color.yellow);

                if (Physics.Raycast(MainPivot, animal.Gravity, out RaycastHit LandHit, 100f, LandOn))
                {
                    //DistanceToGround = LandHit.distance;
                    FoundLand = true;
                    if (LandHit.distance < LandDistance)
                    {
                        Debugging($"[AllowExit] Can Land. Touching <{LandHit.collider.name}>");
                        AllowExit();
                    }
                }
                else
                {
                    //Means that has lost the RayCastHit that it had
                    if (FoundLand)
                    {
                        // Debug.LogWarning("The Animal Tried to go below the terrain.... Unity Physic Bug  :( ");
                        animal.Teleport(animal.LastPos); //HACK WHEN THE ANIMAL Goes UnderGround
                        animal.ResetUPVector();
                    }
                }
            }
        }

        private void RemoveUpDown()
        {
            animal.ResetUPVector();
            animal.MovementAxis.y = 0;
            animal.MovementAxisRaw.y = 0;
        }

        void GravityPush(float deltaTime)
        {
            var Gravity = animal.Gravity;
            //Add more speed when going Down
            float downAcceleration = DownAcceleration * animal.ScaleFactor;

            if (animal.MovementAxis.y < 0f)
            {
                acceleration += downAcceleration * deltaTime;
            }
            else
            {
                acceleration = Mathf.MoveTowards(acceleration, 0, deltaTime * 2);            //Deacelerate slowly all the acceleration you earned..
            }


            if (acceleration != 0) animal.AdditivePosition += animal.InertiaPositionSpeed.normalized * acceleration * deltaTime; //USE INERTIA SPEED INSTEAD OF TARGET POSITION

            if (GravityDrag > 0)
            {
                animal.AdditivePosition += Gravity * (GravityDrag * animal.ScaleFactor) * deltaTime;
            }
        }

        void AutoGliding()
        {
            if (MTools.ElapsedTime(FlyStyleTime, AutoGlide_CurrentTime))
            {
                AutoGlide_CurrentTime = Time.time;
                isGliding ^= true;

                FlyStyleTime = isGliding ? GlideChance.RandomValue : FlapChange.RandomValue;

                var newGlideSpeed = Random.Range(GlideSpeed - Variation, GlideSpeed);
                var newFlapSpeed = Random.Range(FlapSpeed, FlapSpeed + Variation);

                animal.currentSpeedModifier.Vertical = (isGliding && !animal.Strafe) ? newGlideSpeed : newFlapSpeed;
            }
        }
      
        public override void ResetStateValues()
        {
            verticalInertia = Vector3.zero;
            acceleration = 0;
            isGliding = false;
            LandHit = new RaycastHit[1];
            InputValue = false;
            FoundLand = false;
            //DistanceToGround = float.MaxValue;
        }

        public override void RestoreAnimalOnExit()
        {
            animal.FreeMovement = false;
            //animal.currentSpeedModifier.lerpAnimator = 20; //Restore the Speed on the animal ??????????????????????/
            animal.AlwaysForward = LastAlwaysForward;
            animal.Speed_Change_Lock(false);
            animal.InputSource?.SetInput(ID.name, false); //Hack to reset the toggle when it exit on Grounded
        }





        /// <summary>Allow the State to be Replaced by lower States</summary>
        public override void AllowExit()
        {
            if (CanExit)
            {
                IgnoreLowerStates = false;
                IsPersistent = false;
                 base.InputValue = false;  //release the base Input value
            }
        }

        public override bool InputValue //lets override to Allow exit when the Input Changes
        {
            get => base.InputValue;
            set
            {
                base.InputValue = value; 

                if (InCoreAnimation && IsActiveState && !value && CanExit) //When the Fly Input is false then allow exit
                {
                    AllowExit();
                }
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            ID = MTools.GetInstance<StateID>("Fly");
            Input = "Fly";

            General = new AnimalModifier()
            {
                RootMotion = true,
                Grounded = false,
                Sprint = true,
                OrientToGround = false,
                CustomRotation = false,
                IgnoreLowerStates = true,
                Gravity = false,
                modify = (modifier)(-1),
                AdditivePosition = true, 
                AdditiveRotation = true, 
                FreeMovement = true, 
            };
        }
#endif
    }
}
