// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom drawer that allows assignment of the C# interface IQuestJournalUI.
    /// </summary>
    [CustomPropertyDrawer(typeof(IQuestJournalUIInspectorFieldAttribute), true)]
    public class IQuestJournalUIInspectorFieldAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(IQuestJournalUI), true);
        }

        public static void DoLayout(SerializedProperty property, GUIContent label)
        {
            if (property == null) return;
            property.objectReferenceValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue, typeof(IQuestJournalUI), true);
        }

    }
}
