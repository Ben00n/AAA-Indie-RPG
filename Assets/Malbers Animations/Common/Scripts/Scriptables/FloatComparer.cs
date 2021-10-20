using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/Float Comparer")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class FloatComparer : FloatVarListener
    {
        public List<AdvancedFloatEvent> compare = new List<AdvancedFloatEvent>();

        /// <summary>Set the first value on the comparer </summary>
        public float CompareFirst { get => compare[0].Value.Value; set => compare[0].Value.Value = value; }

        public override float Value
        {
            set
            {
                base.Value = value;
                Compare();
            }
        }

        public float this[int index]
        {
            get => compare[index].Value.Value;
            set => compare[index].Value.Value = value;
        }

        void OnEnable()
        {
            if (value.Variable)
            {
                value.Variable.OnValueChanged += Compare;
                value.Variable.OnValueChanged += InvokeFloat;
            }

            Raise.Invoke(Value);
        }

        void OnDisable()
        {
            if (value.Variable)
            {
                value.Variable.OnValueChanged -= Compare;
                value.Variable.OnValueChanged -= InvokeFloat;
            }
        }

        /// <summary>Compares the Int parameter on this Component and if the condition is made then the event will be invoked</summary>
        public virtual void Compare()
        {
            if (enabled)
            foreach (var item in compare)
                item.ExecuteAdvanceFloatEvent(value);
        }


        /// <summary>Compares an given int Value and if the condition is made then the event will be invoked</summary>
        public virtual void Compare(float value)
        {
            if (enabled)
                foreach (var item in compare)
                item.ExecuteAdvanceFloatEvent(value);
        }

        /// <summary>Compares an given intVar Value and if the condition is made then the event will be invoked</summary>
        public virtual void Compare(FloatVar value)
        {
            if (enabled)
                foreach (var item in compare)
                item.ExecuteAdvanceFloatEvent(value.Value);
        }
    }


    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(FloatComparer))]
    public class FloatComparerListenerEditor : IntCompareEditor {}
#endif
}