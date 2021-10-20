    using UnityEngine;
using System.Collections;
using MalbersAnimations.Scriptables;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif


namespace MalbersAnimations.Utilities
{
    [AddComponentMenu("Malbers/Events/Messages")] 
    public class Messages : MonoBehaviour
    {
        public MesssageItem[] messages;                                     //Store messages to send it when Enter the animation State
        public bool UseSendMessage = true;
        public bool nextFrame = false;

        public virtual void SendMessage(Component component)
        {
            SendMessage(component.gameObject);
        }

        public virtual void SendMessage(GameObject go)
        {
            foreach (var m in messages)
            {
                if (m.message == string.Empty || !m.Active) break;          //If the messaje is empty or disabled break.... ignore it

                if (nextFrame)
                {
                    StartCoroutine(CNextFrame(m, go));
                }
                else
                {
                    Deliver(m, go);
                }
            }
        }


        IEnumerator CNextFrame(MesssageItem m, GameObject component)
        {
            yield return null;
            Deliver(m, component);
        }

        private void Deliver(MesssageItem m, GameObject component)
        {
            if (UseSendMessage)
                DeliverMessage(m, component.transform.root.gameObject);
            else
            {
                var listeners = component.transform.root.GetComponentsInChildren<IAnimatorListener>();
                if (listeners != null && listeners.Length > 0)
                {
                    foreach (var list in listeners)
                        DeliverListener(m, list);
                }
            }
        }

        public static void DeliverMessage(MesssageItem m, GameObject component)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    component.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Int:
                    component.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Float:
                    component.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.String:
                    component.SendMessage(m.message, m.stringValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Void:
                    component.SendMessage(m.message, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.IntVar:
                    component.SendMessage(m.message, (int)m.intVarValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Transform:
                    component.SendMessage(m.message, m.transformValue, SendMessageOptions.DontRequireReceiver);
                    break;
                default:
                    break;
            }
        }

        public static void DeliverListener(MesssageItem m, IAnimatorListener listener)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    listener.OnAnimatorBehaviourMessage(m.message, m.boolValue);
                    break;
                case TypeMessage.Int:
                    listener.OnAnimatorBehaviourMessage(m.message, m.intValue);
                    break;
                case TypeMessage.Float:
                    listener.OnAnimatorBehaviourMessage(m.message, m.floatValue);
                    break;
                case TypeMessage.String:
                    listener.OnAnimatorBehaviourMessage(m.message, m.stringValue);
                    break;
                case TypeMessage.Void:
                    listener.OnAnimatorBehaviourMessage(m.message, null);
                    break;
                case TypeMessage.IntVar:
                    listener.OnAnimatorBehaviourMessage(m.message, (int)m.intVarValue);
                    break;
                case TypeMessage.Transform:
                    listener.OnAnimatorBehaviourMessage(m.message, m.transformValue);
                    break;

                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class MesssageItem
    {
        public string message;
        public TypeMessage typeM;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public IntVar intVarValue;
        public Transform transformValue;

        public float time;
        public bool sent;
        public bool Active = true;

        public MesssageItem()
        {
            message = string.Empty;
            Active = true;
        }
    }



    //INSPECTOR

#if UNITY_EDITOR
    [CustomEditor(typeof(Messages))]
    public class MessagesEd : Editor
    {
        private ReorderableList list;

        private Messages MMessage;
        private SerializedProperty sp_messages;

        private MonoScript script;

        private void OnEnable()
        {
            MMessage = ((Messages)target);
            script = MonoScript.FromMonoBehaviour(MMessage);
            sp_messages = serializedObject.FindProperty("messages");

            list = new ReorderableList(serializedObject, sp_messages, true, true, true, true);

            list.drawElementCallback = drawElementCallback1;
            list.drawHeaderCallback = HeaderCallbackDelegate1;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Send Messages to all the MonoBehaviours that uses the Interface |IAnimatorListener| ");

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginVertical(MTools.StyleGray);
                {
                    MalbersEditor.DrawScript(script);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    list.DoLayoutList();
                    EditorGUILayout.EndVertical();

                    var UseSendMessage = serializedObject.FindProperty("UseSendMessage");
                    UseSendMessage.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Use Send Message", "Uses the SendMessage() instead"), UseSendMessage.boolValue);
                    var nextFrame = serializedObject.FindProperty("nextFrame");
                    nextFrame.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Next Frame", "Send the Message the frame after"), nextFrame.boolValue);
                }

                EditorGUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Messages Inspector");
            }

            serializedObject.ApplyModifiedProperties();
        }

        void HeaderCallbackDelegate1(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 10, rect.y, (rect.width / 3) + 30, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + 10 + ((rect.width) / 3) + 5 + 30, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");

            Rect R_5 = new Rect(rect.x + 10 + ((rect.width) / 3) * 2 + 5 + 15, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");
        }

        void drawElementCallback1(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.messages[index];
            var sp_element = sp_messages.GetArrayElementAtIndex(index);

            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            element.Active = EditorGUI.Toggle(R_0, element.Active);

            Rect R_1 = new Rect(rect.x + 15, rect.y, (rect.width / 3) + 15, EditorGUIUtility.singleLineHeight);
            element.message = EditorGUI.TextField(R_1, element.message);


            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 5 + 30, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            element.typeM = (TypeMessage)EditorGUI.EnumPopup(R_3, element.typeM);

            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5 + 15, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            switch (element.typeM)
            {
                case TypeMessage.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
                    break;
                case TypeMessage.Int:
                    EditorGUI.PropertyField(R_5, sp_element.FindPropertyRelative("intValue"), GUIContent.none);
                    break;
                case TypeMessage.Float:
                    EditorGUI.PropertyField(R_5, sp_element.FindPropertyRelative("floatValue"), GUIContent.none);
                    break;
                case TypeMessage.String:
                    EditorGUI.PropertyField(R_5, sp_element.FindPropertyRelative("stringValue"), GUIContent.none);
                    break;
                case TypeMessage.IntVar:
                    EditorGUI.PropertyField(R_5, sp_element.FindPropertyRelative("intVarValue"), GUIContent.none);
                    break;
                case TypeMessage.Transform:
                    EditorGUI.PropertyField(R_5, sp_element.FindPropertyRelative("transformValue"), GUIContent.none);
                    break;
                default:
                    break;
            }
        }
    }
#endif
}