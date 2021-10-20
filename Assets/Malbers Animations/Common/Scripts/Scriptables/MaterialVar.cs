using UnityEngine;
using System.Collections.Generic;

namespace MalbersAnimations.Scriptables
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Material", order = 2000)]
    public class MaterialVar : ScriptableObject
    {
        /// <summary> The current value</summary>
        [SerializeField] private Material value;


        /// <summary>Value of the String Scriptable variable</summary>
        public Material Value
        {
            get => value;
            set =>  this.value = value;
        }

        public virtual void SetValue(MaterialVar var)
        { Value = var.Value; }

        public virtual void SetValue(Material var)
        { Value = var; }
    }
}