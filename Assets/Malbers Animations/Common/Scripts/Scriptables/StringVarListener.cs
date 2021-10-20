using MalbersAnimations.Scriptables;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Variables/String Listener")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class StringVarListener : VarListener
    {
        public StringReference value;
        public StringEvent Raise = new StringEvent();

        public virtual string Value
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

        public virtual void InvokeInt(string value)
        { if (Enable) Raise.Invoke(value);}
    }

    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(StringVarListener)), UnityEditor.CanEditMultipleObjects]
    public class StringVarListenerEditor : VarListenerEditor
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