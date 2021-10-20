using UnityEngine;
using MalbersAnimations.Scriptables;
using System;

namespace MalbersAnimations.Controller
{
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/main-components/manimal-controller/states/fall")]
    public class Fall : State
    {
        public enum FallBlending { DistanceNormalized, Distance , VerticalVelocity }

        /// <summary>Air Resistance while falling</summary>
        [Header("Fall Parameters")]
        [Tooltip("Can the Animal be controller while falling?")]
        public BoolReference AirControl = new BoolReference(true);
        [Tooltip("Rotation while falling")]
        public FloatReference AirRotation = new FloatReference(10);
        [Tooltip("Maximum Movement while falling")]
        public FloatReference AirMovement = new FloatReference(0);
        [Tooltip("Lerp value for the Air Movement adjusment")]
        public FloatReference airSmooth = new FloatReference(2);

        [Space]
        [Tooltip("Forward Offset Position of the Fall Ray")]
        public FloatReference FallRayForward = new FloatReference(0.1f);
        [Tooltip("Multiplier for the Fall Ray Length")]
        public FloatReference fallRayMultiplier = new FloatReference(1f);
        [Tooltip("RayHits Allowed on the Raycast NonAloc")]
        public IntReference RayHits = new IntReference(2);


        /// <summary>Used to Set fallBlend to zero before reaching the ground</summary>
        [Space, Tooltip("Used to Set fallBlend to zero before reaching the ground")]
        public FloatReference LowerBlendDistance;


        // public float UpImpulseLimit = 1;
        public float AirDrag = 1;


        public FallBlending BlendFall = FallBlending.DistanceNormalized;

        /// <summary>Distance to Apply a Fall Hard Animation</summary>
        [Space,Header("Fall Damage")]
        public StatID AffectStat;
        [Tooltip("Minimum Distance to Apply a Soft Land Animation")]
        public FloatReference FallMinDistance = new FloatReference(5f);
        [Tooltip("Maximun Distance to Apply a Hard Land Animation")]
        public FloatReference FallMaxDistance = new FloatReference(15f);

        [Tooltip("The Fall State will set the StateStatus Depending the Fall Distance (X: Distance Y:Status Value)")]
        public Vector2[] landStatus;
        /// <summary>Stores the max heigth before going Down</summary>
        public float MaxHeight { get; set; }
       
        /// <summary>Acumulated Fall Distance</summary>
        public float FallCurrentDistance { get; set; }

        protected Vector3 fall_Point;
        private RaycastHit[] FallHits;
        //  protected Vector3 HorizontalInertia;
        private RaycastHit FallRayCast;

        /// <summary>While Falling this is the distance to the ground</summary>
        private float DistanceToGround;
        /// <summary>Distance difference front the last frame to this one</summary>
        private float LastDeltaDistance;

        /// <summary> Normalized Value of the Height </summary>
        float FallBlend;
        public Vector3 UpImpulse { get; set; }

        private MSpeed FallSpeed = MSpeed.Default;

        public Vector3 FallPoint { get; private set; }

        /// <summary> UP Impulse was going UP </summary>
        public bool GoingUPImpulse { get; private set; }
        
        
        private bool GoingDown;
        private bool DeltaGoingDown;
        private bool FoundLand;

        private int Hits;

        public override bool TryActivate()
        { 
            float SprintMultiplier = (animal.VerticalSmooth);
            var fall_Pivot = animal.Main_Pivot_Point + (animal.Forward * SprintMultiplier * FallRayForward * animal.ScaleFactor); //Calculate ahead the falling ray

            fall_Pivot += animal.DeltaPos; //Check for the Next Frame
           
            float Multiplier = animal.Pivot_Multiplier * fallRayMultiplier;
            return TryFallRayCasting(fall_Pivot, Multiplier);
        }

        private bool TryFallRayCasting(Vector3 fall_Pivot, float Multiplier)
        {
            FallHits = new RaycastHit[RayHits];

            var Direction = animal.TerrainSlope < 0 ? animal.Gravity : -transform.up;

            var Radius = animal.RayCastRadius * animal.ScaleFactor;
            Hits = Physics.SphereCastNonAlloc(fall_Pivot, Radius, Direction, FallHits, Multiplier, animal.GroundLayer, QueryTriggerInteraction.Ignore);

            if (Hits > 0)
            {
                if (animal.Grounded)
                {
                    foreach (var hit in FallHits)
                    {
                        if (hit.collider != null)
                        {
                            var slope = Vector3.Angle(hit.normal, animal.UpVector);
                            slope *= Vector3.Dot(animal.UpVector, animal.Forward) < 0 ? -1 : 1;            //Calcualte the Fall Angle Positive or Negative

                            if (slope > -animal.maxAngleSlope && slope <= animal.maxAngleSlope)
                            {
                                FallRayCast = hit;

                                if (debug)
                                {
                                    Debug.DrawRay(fall_Pivot, Direction * Multiplier, Color.magenta);
                                    Debug.DrawRay(FallRayCast.point, FallRayCast.normal * animal.ScaleFactor * 0.2f, Color.magenta);
                                    MTools.DrawWireSphere(fall_Pivot + Direction * DistanceToGround, Color.magenta, Radius);
                                }
                                break;
                            }
                        }
                    }

                    DistanceToGround = FallRayCast.distance;

                    var TerrainSlope = Vector3.Angle(FallRayCast.normal, animal.UpVector);
                    TerrainSlope *= Vector3.Dot(animal.UpVector, animal.Forward) < 0 ? -1 : 1;            //Calcualte the Fall Angle Positive or Negative

                    if (TerrainSlope < -animal.maxAngleSlope || animal.DeepSlope)
                    {
                        Debugging($"[Try] Slope is too deep [{FallRayCast.collider}] | Hits: {Hits} | Slope : {TerrainSlope:F2} T:{Time.time:F3}");
                        return true;
                    }
                }
                else   //If the Animal is in the air  NOT GROUNDED
                {
                    FallRayCast = FallHits[0];
                    DistanceToGround = FallRayCast.distance;

                    var FallSlope = Vector3.Angle(FallRayCast.normal, animal.UpVector);

                    if (FallSlope > animal.maxAngleSlope)
                    {
                        Debugging($"[Try] The Animal is on the Air and the angle SLOPE of the ground Hitted is too Deep");
                      
                        return true;
                    }
                    if (animal.Height >= DistanceToGround ) //If the distance to ground is very small means that we are very close to the ground
                    {

                        if (animal.ExternalForce != Vector3.zero) return true; //Hack for external forces

                        Debugging($"[Try Failed] Distance to the ground is very small means that we are very close to the ground. CHECK IF GROUNDED");
                        animal.CheckIfGrounded();//Hack IMPORTANT HACK!!!!!!!
                        return false;
                    }
                }
            }
            else
            {
                Debugging($"[Try] There's no Ground beneath the Animal"); 
                //    Debug.Break();
                return true;
            }

          //  animal.CheckIfGrounded(); //Hack IMPORTANT HACK!!!!!!!

            return false;
        }

        public override void Activate()
        {
            base.Activate();
            animal.State_SetFloat(0);
            ResetStateValues();
        }

        public override void EnterCoreAnimation()
        {
            SetStatus(0);
            if (animal.LastState.ID.ID != ID.ID) animal.State_SetFloat(0); //Only Reset when it comes from Locomotino

            UpImpulse = Vector3.Project(animal.DeltaPos,animal.UpVector);   //Clean the Vector from Forward and Horizontal Influence    
            GoingUPImpulse = Vector3.Dot(UpImpulse, animal.UpVector) > 0;

            if (MTools.CompareOR(animal.LastState.ID, 0, 1, 4, StateEnum.Climb) && GoingUPImpulse || animal.HasExternalForce) //means it was on locomotion or idle //Remove Up Impulse HACK
                UpImpulse = Vector3.zero; 

            IgnoreLowerStates = false;
             

            FallSpeed = new MSpeed(animal.CurrentSpeedModifier)
            {
                name = "FallSpeed",
                position = animal.HorizontalSpeed + animal.ExternalForceHSpeed,
                strafeSpeed = animal.HorizontalSpeed + animal.ExternalForceHSpeed,
                animator = 1,
                rotation = AirRotation.Value, 
            };

            animal.UpdateDirectionSpeed = AirControl; //Set the Directional Speed to be Updated depending the Air Control var IMPORTANT

            if (animal.HasExternalForce) //???
            {
                animal.DirectionalVelocity = Vector3.ProjectOnPlane(animal.ExternalForce, animal.UpVector).normalized;
            }

            animal.SetCustomSpeed(FallSpeed, true);

            GoingUPImpulse = Vector3.Dot(UpImpulse, animal.UpVector) > 0;

                //means it was on locomotion or idle //Remove Up Impulse HACK
            if (MTools.CompareOR( animal.LastState.ID,0,1,4,StateEnum.Climb) && GoingUPImpulse || animal.HasExternalForce)
                UpImpulse = Vector3.zero;
        }

        public override void OnStateMove(float deltaTime)
        {
            if (InCoreAnimation)
            {
                animal.AdditivePosition += UpImpulse;
              
                if (GoingUPImpulse)  UpImpulse = Vector3.Lerp(UpImpulse, Vector3.zero, deltaTime * AirDrag); //Clean the Up impulse with air Drag

                //Change the Speed to the maximum Speed when AirMovement is enable
                if (AirControl && AirMovement > 0 && AirMovement > CurrentSpeedPos)
                    CurrentSpeedPos = Mathf.Lerp(CurrentSpeedPos, AirMovement, deltaTime * airSmooth);
            }
        }


        public override void ExitState()
        {
            if (landStatus != null && landStatus.Length >= 1)
            {
                var status = 0;

                foreach (var ls in landStatus)
                    if (ls.x < FallCurrentDistance) status = (int)ls.y;

                SetStatus(status);  //Set the Landing Status!! IMPORTANT for Multiple Landing Animations
            }
          


            if (AffectStat != null && FallCurrentDistance > FallMinDistance.Value && animal.Grounded) //Meaning if we are on the safe minimun distance we do not get damage from falling
            {
                var StatFallValue = (FallCurrentDistance) * 100 / FallMaxDistance;

                var animalStats = animal.GetComponent<Stats>();
                animalStats?.Stat_ModifyValue(AffectStat, StatFallValue, StatOption.ReduceByPercent);
            }

            base.ExitState();
        }


        public override void TryExitState(float DeltaTime)
        {
            var scaleFactor = animal.ScaleFactor;
            var Radius = animal.RayCastRadius * scaleFactor;

            DeltaGoingDown = GoingDown;
            var Gravity = animal.Gravity;
            GoingDown = Vector3.Dot(animal.DeltaPos, Gravity) > 0; //Check if is falling down


            //GoingDown = Vector3.Dot(animal.DeltaPos, Gravity) > 0; //Check if is falling down
            float DeltaDistance = 0;

            if (GoingDown)
            {
                DeltaDistance = Vector3.Project(animal.DeltaPos, Gravity).magnitude;
                FallCurrentDistance += DeltaDistance;
            }

            FallPoint = animal.Main_Pivot_Point;
            //FallPoint += Vector3.Project(animal.AdditivePosition, Gravity); //To check ahead that the animal can land..

            if (animal.debugGizmos && debug)
            {
                MTools.DrawWireSphere(FallPoint, Color.magenta, Radius);
                MTools.DrawWireSphere(FallPoint + Gravity * animal.Height, (Color.red + Color.blue) / 2, Radius);
                Debug.DrawRay(FallPoint, Gravity * 100f, Color.magenta);
            }

            if (Physics.Raycast(FallPoint, Gravity, out FallRayCast, 100f, animal.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                DistanceToGround = FallRayCast.distance;

                FoundLand = true;

                if (animal.debugGizmos && debug)
                {
                    MTools.DrawWireSphere(FallRayCast.point, (Color.blue + Color.red) / 2, Radius);
                    MTools.DrawWireSphere(FallPoint, (Color.red), Radius);
                }

                switch (BlendFall)
                {
                    case FallBlending.DistanceNormalized:
                        {
                            var realDistance = DistanceToGround - animal.Height;
                            if (MaxHeight < realDistance)
                                MaxHeight = realDistance; //get the Highest Distance the first time you touch the ground
                            else
                            {
                                realDistance -= LowerBlendDistance;

                                FallBlend = Mathf.Lerp(FallBlend, realDistance / MaxHeight, DeltaTime * 10); //Small blend in case there's a new ground found
                                animal.State_SetFloat(1 - FallBlend); //Blend between High and Low Fall
                            }
                        }
                        break;
                    case FallBlending.Distance:
                        animal.State_SetFloat(FallCurrentDistance);
                        break;
                    case FallBlending.VerticalVelocity:
                        var UpInertia = Vector3.Project(animal.DeltaPos, animal.UpVector).magnitude;   //Clean the Vector from Forward and Horizontal Influence    
                        animal.State_SetFloat(UpInertia / animal.DeltaTime * (GoingDown ? 1 : -1));
                        break;
                    default:
                        break;
                }


        
            
                if (animal.Height > DistanceToGround || ((DistanceToGround - DeltaDistance) < 0)) //Means has touched the ground
                {
                    var angl = Vector3.Angle(FallRayCast.normal, animal.UpVector);
                    var DeepSlope = angl > animal.maxAngleSlope;

                  
                    if (!DeepSlope) //Check if we are not on a deep slope
                    {
                        AllowExit();
                        animal.Grounded = true;
                        animal.UseGravity = false;
                        var GroundedPos = Vector3.Project(FallRayCast.point - animal.transform.position, Gravity);  //IMPORTANT HACk FOR 

                        animal.Teleport(animal.transform.position + GroundedPos); //SUPER IMPORTANT!!!
                        animal.ResetUPVector(); //IMPORTAAANT!

                        UpImpulse = Vector3.zero;
                        Debugging($"[Try Exit] (Grounded) + [Terrain Angle ={angl}]");
                    }
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

            if (!animal.IsOnZone && !animal.HasExternalForce)
            {
                if ((DeltaGoingDown && !GoingDown))
                {
                    if (LastDeltaDistance > DeltaDistance && DeltaDistance != 0)
                    {
                        //Means that is still trying to fall but it cant because something is bloking the fall soo lets push it forward
                        animal.ResetUPVector();
                        animal.GravityTime = animal.StartGravityTime;
                        animal.MovementAxis.z = 1; //Force going forward HACK
                        animal.InertiaPositionSpeed = animal.Forward * animal.ScaleFactor * DeltaTime * animal.FallForward;
                        Debug.Log("GoingForward "+ DeltaDistance);
                    }
                }
            }

            LastDeltaDistance = DeltaDistance;
           
        }

        public override void ResetStateValues()
        { 
            MaxHeight = float.NegativeInfinity; //Resets MaxHeight
            GoingDown =
            DeltaGoingDown =
            FoundLand = false;
            DistanceToGround = float.PositiveInfinity;
            FallSpeed = new MSpeed();
            FallBlend = 1;
            FallRayCast = new RaycastHit();
            FallHits = new RaycastHit[RayHits];
            UpImpulse = Vector3.zero;
            FallCurrentDistance = 0;
            LastDeltaDistance = 0; 
        }

        public override void RestoreAnimalOnExit()
        {
            animal.UpdateDirectionSpeed = true; //Reset the Rotate Direction to the Default value
        }


#if UNITY_EDITOR

        /// <summary>This is Executed when the Asset is created for the first time </summary>
        private void Reset()
        {
            ID = MTools.GetInstance<StateID>("Fall");
            General = new AnimalModifier()
            {
                RootMotion = false,
                AdditivePosition = true,
                AdditiveRotation = true,
                Grounded = false,
                Sprint = false,
                OrientToGround = false,
                
                Gravity = true,
                CustomRotation = false,
                modify = (modifier)(-1),
            };

            LowerBlendDistance = 0.1f;
            FallRayForward = 0.1f;
            fallRayMultiplier = 1f;

            FallSpeed.name = "FallSpeed";

            ExitFrame = false; //IMPORTANT
        }
#endif
    }
}