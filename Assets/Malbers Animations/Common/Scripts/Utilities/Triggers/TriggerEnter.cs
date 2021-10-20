using UnityEngine;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.Utilities
{
    /// <summary> This is used when the collider is in a different gameObject and you need to check the Trigger Events
    /// Create this component at runtime and subscribe to the UnityEvents </summary>
    [AddComponentMenu("Malbers/Utilities/Colliders/Trigger Enter")]
    public class TriggerEnter : UnityUtils
    {
        [SerializeField] private BoolReference active = new BoolReference(true);
        public LayerReference HitLayer =  new LayerReference(-1);

        public ColliderEvent onTriggerEnter = new ColliderEvent();

        public bool Active { get => active; set => active.Value = value; }
       

        void OnTriggerEnter(Collider other)
        {
            if (!active.Value) return;
            if (!MTools.Layer_in_LayerMask(other.gameObject.layer, HitLayer.Value)) return;

            onTriggerEnter.Invoke(other);
        }

        private void Reset()
        {
            var collider = GetComponent<Collider>();
            Active = true;

            if (collider)
            {
                collider.isTrigger = true;
            }
            else
            {
                Active = false;
                Debug.LogError("This Script requires a Collider, please add any type of collider");
            }
        }
    }
}