using UnityEngine;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations
{
    public class MessagesBehavior : StateMachineBehaviour
    {
        public bool UseSendMessage;
        public bool NormalizeTime = true;
        public bool debug;

        public MesssageItem[] onEnterMessage;   //Store messages to send it when Enter the animation State
        public MesssageItem[] onExitMessage;    //Store messages to send it when Exit  the animation State
        public MesssageItem[] onTimeMessage;    //Store messages to send on a specific time  in the animation State

        IAnimatorListener[] listeners;         //To all the MonoBehavious that Have this 


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            listeners = animator.GetComponents<IAnimatorListener>();


            foreach (MesssageItem ontimeM in onTimeMessage)  //Set all the messages Ontime Sent = false when start
            {
                ontimeM.sent = false;
            }

            foreach (MesssageItem onEnterM in onEnterMessage)
            {
                if (onEnterM.Active && !string.IsNullOrEmpty(onEnterM.message))
                {
                    if (UseSendMessage)
                        DeliverMessage(onEnterM, animator);
                    else
                        foreach (var item in listeners) DeliverListener(onEnterM, item);
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (MesssageItem onExitM in onExitMessage)
            {
                if (onExitM.Active && !string.IsNullOrEmpty(onExitM.message))
                {
                    if (UseSendMessage)
                        DeliverMessage(onExitM, animator);
                    else
                        foreach (var item in listeners) DeliverListener(onExitM, item);
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.fullPathHash == animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash) return; //means is transitioning to itself

            foreach (MesssageItem onTimeM in onTimeMessage)
            {
                if (onTimeM.Active && !string.IsNullOrEmpty(onTimeM.message))
                {
                    // float stateTime = stateInfo.loop ? stateInfo.normalizedTime % 1 : stateInfo.normalizedTime;
                    float stateTime = NormalizeTime ? stateInfo.normalizedTime % 1 : stateInfo.normalizedTime;

                    if (!onTimeM.sent && (stateTime >= onTimeM.time))
                    {
                        onTimeM.sent = true;

                        //  Debug.Log(onTimeM.message + ": "+stateTime);

                        if (UseSendMessage)
                            DeliverMessage(onTimeM, animator);
                        else
                            foreach (var item in listeners) DeliverListener(onTimeM, item);
                    }
                }
            }
        }

        /// <summary>  Using Message to the Monovehaviours asociated to this animator delivery with Send Message  </summary>
        void DeliverMessage(MesssageItem m, Animator anim)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    anim.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Int:
                    anim.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Float:
                    anim.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.String:
                    anim.SendMessage(m.message, m.stringValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Void:
                    anim.SendMessage(m.message, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.IntVar:
                    anim.SendMessage(m.message, (int)m.intVarValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Transform:
                    anim.SendMessage(m.message, m.transformValue, SendMessageOptions.DontRequireReceiver);
                    break;
                default:
                    break;
            }

            if (debug) Debug.Log($"{name}:<color=white> <b>[Msg Behaviour] [Animator:{anim.name}] [Msg:{m.message} | Type: {m.typeM}]</b> </color>");  //Debug
        }



        /// <summary> Send messages to all scripts with IBehaviourListener to this animator   </summary>
        void DeliverListener(MesssageItem m, IAnimatorListener listener)
        {
            string val = "";
            bool succesful = false;
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, m.boolValue);
                    val = m.boolValue.ToString();
                    break;
                case TypeMessage.Int:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, m.intValue);
                    val = m.intValue.ToString();
                    break;
                case TypeMessage.Float:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, m.floatValue);
                    val = m.floatValue.ToString();
                    break;
                case TypeMessage.String:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, m.stringValue);
                    val = m.stringValue.ToString();
                    break;
                case TypeMessage.Void:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, null);
                    val = "Void";
                    break;
                case TypeMessage.IntVar:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, (int)m.intVarValue);
                    val = m.intVarValue.name.ToString();
                    break;
                case TypeMessage.Transform:
                    succesful = listener.OnAnimatorBehaviourMessage(m.message, m.transformValue);
                    val = m.transformValue.name.ToString();
                    break;
                default:
                    break;
            }

            if (debug && succesful) Debug.Log($"<b>[Msg: {m.message}->{val}] [{m.typeM}]</b> T:{Time.time:F3}");  //Debug

        }
    }
   

    //INSPECTOR

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MesssageItem))]
    public class MessageDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // position.y += 2;

            EditorGUI.BeginProperty(position, label, property);
            //GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var height = EditorGUIUtility.singleLineHeight;

            //PROPERTIES

            var Active = property.FindPropertyRelative("Active");
            var message = property.FindPropertyRelative("message");
            var typeM = property.FindPropertyRelative("typeM");

            var rect = new Rect(position);

            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(R_0, Active, GUIContent.none);

            Rect R_1 = new Rect(rect.x + 15, rect.y, (rect.width / 3) + 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(R_1, message, GUIContent.none);


            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 5 + 30, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(R_3, typeM, GUIContent.none);


            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5 + 15, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            var TypeM = (TypeMessage)typeM.intValue;

            SerializedProperty messageValue = property.FindPropertyRelative("boolValue");

            switch (TypeM)
            {
                case TypeMessage.Bool:
                    messageValue = property.FindPropertyRelative("boolValue");
                    messageValue.boolValue = EditorGUI.ToggleLeft(R_5, messageValue.boolValue ? " True" : " False", messageValue.boolValue);
                    break;
                case TypeMessage.Int:
                    messageValue = property.FindPropertyRelative("intValue");
                    break;
                case TypeMessage.Float:
                    messageValue = property.FindPropertyRelative("floatValue");
                    break;
                case TypeMessage.String:
                    messageValue = property.FindPropertyRelative("stringValue");
                    break;
                case TypeMessage.IntVar:
                    messageValue = property.FindPropertyRelative("intVarValue");
                    break;
                case TypeMessage.Transform:
                    messageValue = property.FindPropertyRelative("transformValue");
                    break;
                default:
                    break;
            }

            if (TypeM != TypeMessage.Void && TypeM != TypeMessage.Bool)
            {
                EditorGUI.PropertyField(R_5, messageValue, GUIContent.none);
            }


            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }


    [CustomEditor(typeof(MessagesBehavior))]
    public class MessageBehaviorsEd : Editor
    {
        private ReorderableList listOnEnter, listOnExit, listOnTime;
        bool OnEnter, OnExit, OnTime;

        private MessagesBehavior MMessage;
        private SerializedProperty sp_messagesONEXIT, onEnterMessage, onTimeMessage, UseSendMessage, NormalizeTime, debug;
        private MonoScript script;

        private void OnEnable()
        {

            MMessage = ((MessagesBehavior)target);
            sp_messagesONEXIT = serializedObject.FindProperty("onExitMessage");
            onEnterMessage = serializedObject.FindProperty("onEnterMessage");
            onTimeMessage = serializedObject.FindProperty("onTimeMessage");
            UseSendMessage = serializedObject.FindProperty("UseSendMessage");
            NormalizeTime = serializedObject.FindProperty("NormalizeTime");
            debug = serializedObject.FindProperty("debug");

            script = MonoScript.FromScriptableObject(MMessage);

            listOnEnter = new ReorderableList(serializedObject, onEnterMessage, true, true, true, true);
            listOnExit = new ReorderableList(serializedObject, sp_messagesONEXIT, true, true, true, true);
            listOnTime = new ReorderableList(serializedObject, onTimeMessage, true, true, true, true);

            listOnEnter.drawElementCallback = drawElementCallback1;
            listOnEnter.drawHeaderCallback = HeaderCallbackDelegate1;

            listOnExit.drawElementCallback = drawElementCallback2;
            listOnExit.drawHeaderCallback = HeaderCallbackDelegate1;

            listOnTime.drawElementCallback = drawElementCallback3;
            listOnTime.drawHeaderCallback = HeaderCallbackDelegate2;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Notifies the Animator Asociate Scripts\nWorks exactly like Animation Events");


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(MTools.StyleGray);
            {
                MalbersEditor.DrawScript(script);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUI.indentLevel++;
                if (listOnEnter.count > 0) OnEnter = true;
                OnEnter = EditorGUILayout.Foldout(OnEnter, "On Enter (" + listOnEnter.count + ")");
                EditorGUI.indentLevel--;

                if (OnEnter)
                    listOnEnter.DoLayoutList();

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                if (listOnExit.count > 0) OnExit = true;
                OnExit = EditorGUILayout.Foldout(OnExit, "On Exit (" + listOnExit.count + ")");
                EditorGUI.indentLevel--;

                if (OnExit)
                    listOnExit.DoLayoutList();

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                if (listOnTime.count > 0) OnTime = true;
                OnTime = EditorGUILayout.Foldout(OnTime, "On Time (" + listOnTime.count + ")");
                EditorGUI.indentLevel--;

                if (OnTime)
                    listOnTime.DoLayoutList();

                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(UseSendMessage, new GUIContent("Use Send Message", "Uses the SendMessage() instead"));
                EditorGUILayout.PropertyField(NormalizeTime, new GUIContent("NomalizeState", "Update State Time will be normalized "));
                EditorGUILayout.PropertyField(debug);
            }

            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Message Behaviour Inspector");
                //EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate1(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 10, rect.y, (rect.width / 3) + 30, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + 10 + ((rect.width) / 3) + 5 + 30, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");

            Rect R_5 = new Rect(rect.x + 10 + ((rect.width) / 3) * 2 + 5 + 15, rect.y, ((rect.width) / 3) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");
        }

        void HeaderCallbackDelegate2(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 10, rect.y, (rect.width / 4) + 30, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + 10 + ((rect.width) / 4) + 5 + 30, rect.y, ((rect.width) / 4) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");
            Rect R_4 = new Rect(rect.x + 10 + ((rect.width) / 4) * 2 + 5 + 20, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_4, "Time");
            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 5 + 10, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");

        }
        //ON ENTER
        void drawElementCallback1(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onEnterMessage[index];
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
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case TypeMessage.Float:
                    element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                case TypeMessage.String:
                    element.stringValue = EditorGUI.TextField(R_5, element.stringValue);
                    break;
                case TypeMessage.IntVar:
                    element.intVarValue = (IntVar)EditorGUI.ObjectField(R_5, element.intVarValue, typeof(IntVar), false);
                    break;
                default:
                    break;
            }

        }

        //ON EXIT
        void drawElementCallback2(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onExitMessage[index];
            var sp_element = sp_messagesONEXIT.GetArrayElementAtIndex(index);

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

        //ON Time
        void drawElementCallback3(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onTimeMessage[index];
            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            element.Active = EditorGUI.Toggle(R_0, element.Active);

            Rect R_1 = new Rect(rect.x + 15, rect.y, (rect.width / 4) + 15, EditorGUIUtility.singleLineHeight);
            element.message = EditorGUI.TextField(R_1, element.message);

            Rect R_3 = new Rect(rect.x + ((rect.width) / 4) + 5 + 30, rect.y, ((rect.width) / 4) - 5 - 5, EditorGUIUtility.singleLineHeight);
            element.typeM = (TypeMessage)EditorGUI.EnumPopup(R_3, element.typeM);

            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 2 + 5 + 25, rect.y, ((rect.width) / 4) - 5 - 15, EditorGUIUtility.singleLineHeight);

            element.time = EditorGUI.FloatField(R_4, element.time);

            //if (element.time > 1) element.time = 1;
             if (element.time < 0) element.time = 0;

            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);
            switch (element.typeM)
            {
                case TypeMessage.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
                    break;
                case TypeMessage.Int:
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case TypeMessage.Float:
                    element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                case TypeMessage.String:
                    element.stringValue = EditorGUI.TextField(R_5, element.stringValue);
                    break;
                case TypeMessage.IntVar:
                    element.intVarValue = (IntVar)EditorGUI.ObjectField(R_5, element.intVarValue, typeof(IntVar), false);
                    break;
                default:
                    break;
            }

        }

    }
#endif
}