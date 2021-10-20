using UnityEngine;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.HAP
{
    /// <summary>This Enable the mounting System</summary> 
    [AddComponentMenu("Malbers/Riding/Mount Trigger")]
    public class MountTriggers : MonoBehaviour
    {

        [Tooltip("If true when the Rider enter the Trigger it will mount automatically")]
        public BoolReference AutoMount = new BoolReference(false);

        [Tooltip("Can be used also for dismounting")]
        public BoolReference Dismount = new BoolReference(true);

        /// <summary>Avoids Automount again after Dismounting and Automount was true</summary>
        public bool WasAutomounted { get; internal set; }

        ///// <summary>The name of the Animation we need to play to Mount the Animal</summary>
        //[Tooltip("The name of the Animation we need to play to Mount the Animal")]
        //public string MountAnimation = "Mount";

        /// <summary>The Transition ID value to dismount this kind of Montura.. (is Located on the Animator)</summary>
        [Tooltip("The Transition ID value to Mount the Animal, to Play the correct Mount Animation"),UnityEngine.Serialization.FormerlySerializedAs("DismountID")]
        public IntReference MountID;
        /// <summary>The Transition ID value to dismount this kind of Montura.. (is Located on the Animator)</summary>
        [Tooltip("The Transition ID value to Dismount the Animal, to Play the correct Mount Animation"), UnityEngine.Serialization.FormerlySerializedAs("DismountID")]
        public IntReference m_DismountID;

        public int DismountID => m_DismountID == 0 ? MountID : m_DismountID;

        [Tooltip("If the Rider has set the Dismount Option to Direction, it will use this parameter to find the Closest Direction")]
        /// <summary>The Local Direction of the Mount Trigger compared with the animal</summary>
        public Vector3Reference Direction;

        [CreateScriptableAsset] public TransformAnimation Adjustment;
        private Mount Montura;
        private MRider rider;
        private int col_amount;




        // Use this for initialization
        void OnEnable()
        {
            Montura = GetComponentInParent<Mount>(); //Get the Mountable in the parents
            col_amount = 0;
            rider = null;
        }

        private void OnDisable()
        {
            rider = null;
            col_amount = 0;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy ||  other.isTrigger) return; // Do not allow triggers

            GetAnimal(other);
        }
        

        private void GetAnimal(Collider other)
        {
            if (!Montura)
            {
                Debug.LogError("No Mount Script Found... please add one");
                return;
            }
            if (!Montura.Mounted && Montura.CanBeMounted)                       //If there's no other Rider on the Animal or the the Animal isn't death
            {
                rider = other.FindComponent<MRider>();

                if (rider != null)
                {
                    col_amount++;

                    if (rider.IsMountingDismounting) return;     //Means the Rider is already mounting an animal
                    rider.MountTriggerEnter(Montura,this); //Set Everything Requiered on the Rider in order to Mount

                    if (AutoMount.Value && !WasAutomounted)
                    {
                        rider.MountAnimal();
                    }
                }
            }
        }


        
        void OnTriggerExit(Collider other)
        {
            if (!gameObject.activeInHierarchy || other.isTrigger) return; // Do not allow triggers

            rider = other.FindComponent<MRider>();
          

            if (rider != null)
            {
                col_amount--;

                if (col_amount == 0)
                {
                    if (rider.IsMountingDismounting) return;                             //You Cannot Mount if you are already mounted

                    if (rider.MountTrigger == this && !Montura.Mounted)                 //When exiting if we are exiting From the Same Mount Trigger means that there's no mountrigger Nearby
                    {
                        rider.MountTriggerExit();
                    }

                    rider = null;
                    if (WasAutomounted) WasAutomounted = false;
                }
            }
        }
    }
}