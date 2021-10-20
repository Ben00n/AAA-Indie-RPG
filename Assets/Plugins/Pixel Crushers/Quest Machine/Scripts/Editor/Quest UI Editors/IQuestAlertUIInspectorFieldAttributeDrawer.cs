// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom drawer that allows assignment of the C# interface IQuestAlertUI.
    /// </summary>
    [CustomPropertyDrawer(typeof(IQuestAlertUIInspectorFieldAttribute), true)]
    public class IQuestAlertUIInspectorFieldAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(IQuestAlertUI), true);
        }

        public static void DoLayout(SerializedProperty property, GUIContent label)
        {
            if (property == null) return;
            property.objectReferenceValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue, typeof(IQuestAlertUI), true);
        }

    }
}
