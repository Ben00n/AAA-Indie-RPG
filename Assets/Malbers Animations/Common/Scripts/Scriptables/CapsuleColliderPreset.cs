using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Preset/Capsule Collider Preset")]
    public class CapsuleColliderPreset : ScriptableObject
    {
       public OverrideCapsuleCollider modifier;

        public void Modify(CapsuleCollider collider) => modifier.Modify(collider);
    }
}