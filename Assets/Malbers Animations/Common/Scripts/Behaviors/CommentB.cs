using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>  Adding Coments on the the Animator</summary>
    public class CommentB : StateMachineBehaviour
    {[Multiline] public string text; }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CommentB))]
    public class CommentBEd : UnityEditor.Editor
    {
        private CommentB Script => target as CommentB;
        GUIStyle style;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.Space();
            style = new GUIStyle(UnityEditor.EditorStyles.helpBox);
            style.fontSize = 13;
            //style.fontStyle = FontStyle.Bold;
            //style.font.
            UnityEditor.EditorGUILayout.BeginVertical(MTools.StyleGreen);
            string text = UnityEditor.EditorGUILayout.TextArea(Script.text, style);
            UnityEditor.EditorGUILayout.EndVertical();
            if (text != Script.text)
            {
                UnityEditor.Undo.RecordObject(Script, "Edit Comments");
                Script.text = text;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}