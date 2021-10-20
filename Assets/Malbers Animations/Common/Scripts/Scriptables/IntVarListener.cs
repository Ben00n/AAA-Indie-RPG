using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/Int Listener")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class IntVarListener : VarListener
    {
        public IntReference value;
        public IntEvent Raise = new IntEvent();

        public virtual int Value
        {
            get => value;
            set
            {
                this.value.Value = value;
                Raise.Invoke(value);
            }
        }

        void OnEnable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged += InvokeInt;
            Raise.Invoke(value);
        }

        void OnDisable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged -= InvokeInt;
        }

        public virtual void InvokeInt(int value)
        { if (Enable) Raise.Invoke(value);}
    }

    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(IntVarListener)), UnityEditor.CanEditMultipleObjects]
    public class IntVarListenerEditor : VarListenerEditor
    {
        private UnityEditor.SerializedProperty Raise;

        void OnEnable()
        {
            base.SetEnable();
            Raise = serializedObject.FindProperty("Raise");
        }

        protected override void DrawEvents()
        {
            UnityEditor.EditorGUILayout.PropertyField(Raise);
            base.DrawEvents();
        }
    }
#endif
}