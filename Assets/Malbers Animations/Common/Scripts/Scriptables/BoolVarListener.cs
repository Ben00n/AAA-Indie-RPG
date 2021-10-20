using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;


namespace MalbersAnimations
{
    [DefaultExecutionOrder(750)]
    [AddComponentMenu("Malbers/Variables/Bool Listener")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/secondary-components/variable-listeners-and-comparers")]
    public class BoolVarListener : VarListener
    {
        public BoolReference value = new BoolReference();
        public UnityEvent OnTrue = new UnityEvent();
        public UnityEvent OnFalse = new UnityEvent();

        public bool Value
        {
            get => value;
            set
            {
                this.value.Value = value;
                Invoke(value);

            }
        }

        void OnEnable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged += Invoke;
            Invoke(value);
        }

        void OnDisable()
        {
            if (value.Variable != null) value.Variable.OnValueChanged -= Invoke;
        }

        public virtual void Invoke(bool value)
        {
            if (Enable)
            { 
                if (value) OnTrue.Invoke();
                else OnFalse.Invoke();

#if UNITY_EDITOR
                if (debug) Debug.Log($"BoolVar: ID [{ID.Value}] -> [{name}] -> [{value}]");
#endif
            }
        }

        public virtual void Invoke_Toogle()
        {
            if (Enable)
                Value ^= true; //toogle the value
        }


        /// <summary> Used to use turn Objects to True or false </summary>
        public virtual void Invoke(Object value)
        {
            if (Enable)
            {
                if (value != null)
                    OnTrue.Invoke();
                else 
                    OnFalse.Invoke();
            }
        }

        public void ShowCursor(bool value) => UnityUtils.ShowCursor(value);
    }




    //INSPECTOR
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(BoolVarListener)), UnityEditor.CanEditMultipleObjects]
    public class BoolVarListenerEditor : VarListenerEditor
    {
        private UnityEditor.SerializedProperty OnTrue, OnFalse;

        private void OnEnable()
        {
            base.SetEnable();
            OnTrue = serializedObject.FindProperty("OnTrue");
            OnFalse = serializedObject.FindProperty("OnFalse");
        }

        protected override void DrawEvents()
        {
            UnityEditor.EditorGUILayout.PropertyField(OnTrue);
            UnityEditor.EditorGUILayout.PropertyField(OnFalse);
            base.DrawEvents();
        }
    }
#endif
}
