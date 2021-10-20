using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor.Presets;
using UnityEditor;
#endif

namespace MalbersAnimations
{
#if UNITY_EDITOR
    public static class MalbersEditor
    {
        public static GUIStyle BoldFoldout
        {
            get
            {
                var boldFoldout = new GUIStyle(EditorStyles.foldout);
                boldFoldout.fontStyle = FontStyle.Bold;
                return boldFoldout;
            }
        }



        #region Styles      
        public static GUIStyle StyleGray => MTools.StyleGray;
        public static GUIStyle StyleBlue => MTools.StyleBlue;
        public static GUIStyle StyleGreen => MTools.StyleGreen;
      
        #endregion

        public static Preset GetPreset(string name)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Preset).Name);  //FindAssets uses tags check documentation for more info
            Preset[] a = new Preset[guids.Length];

            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<Preset>(path);
            }
            var allInstances = a.ToList();

            return allInstances.Find(x => x.name == name);

        }

        
        public static bool CopyObjectSerialization(Object source, Object target)
        {
            Preset preset = new Preset(source);
            return preset.ApplyTo(target);
        }

        // This method creates a Preset from a given Object and save it as an asset in the project.
        public static void CreatePresetAsset(Object source, string name)
        {
            Preset preset = new Preset(source);
            AssetDatabase.CreateAsset(preset, "Assets/" + name + ".preset");
        } 

        public static string GetPropertyType(SerializedProperty property)
        {
            var type = property.type;
            var match = Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            return type;
        }

        public static System.Type[] GetTypesByName(string className)
        {
            List<System.Type> returnVal = new List<System.Type>();

            foreach (Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type[] assemblyTypes = a.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    if (assemblyTypes[j].Name == className)
                    {
                        returnVal.Add(assemblyTypes[j]);
                    }
                }
            }

            return returnVal.ToArray();
        }

        public static System.Type GetTypeByName(string className)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(t => t.Name == className);
        }

        /// <summary> Check if an Object is an Asset Folder </summary>
        public static bool IsAssetAFolder(Object obj)
        {
            string path = "";

            if (obj == null)
            {
                return false;
            }

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public static void AddParametersOnAnimator(UnityEditor.Animations.AnimatorController AnimController, UnityEditor.Animations.AnimatorController Mounted)
        {
            AnimatorControllerParameter[] parameters = AnimController.parameters;
            AnimatorControllerParameter[] Mountedparameters = Mounted.parameters;

            foreach (var param in Mountedparameters)
            {
                if (!SearchParameter(parameters, param.name))
                {
                    AnimController.AddParameter(param);
                }
            }
        }

        public static bool SearchParameter(AnimatorControllerParameter[] parameters, string name)
        {
            foreach (AnimatorControllerParameter item in parameters)
            {
                if (item.name == name) return true;
            }
            return false;
        }

        public static void DrawHeader(string title)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        public static void DrawSplitter()
        {
            EditorGUILayout.Space();
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 20f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }

        public static bool DrawHeaderFoldout(string title, bool state)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }
        public static void DrawCross(Transform m_transform)
        {
            var gizmoSize = 0.25f;
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.forward * -gizmoSize / m_transform.localScale.z));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.up * -gizmoSize / m_transform.localScale.y));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * gizmoSize / m_transform.localScale.x));
            Gizmos.DrawLine(m_transform.position, m_transform.position + m_transform.TransformVector(m_transform.root.right * -gizmoSize / m_transform.localScale.x));
        }

        public static void DrawLineHelpBox()
        {
            EditorGUILayout.BeginVertical(EditorStyles.textField);
            EditorGUILayout.EndVertical();
        }

        public static void DrawScript(MonoScript script)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }

        public static void DrawDescription(string v)
        {
            EditorGUILayout.BeginVertical(MTools.StyleBlue);
            EditorGUILayout.HelpBox(v, MessageType.None);
            EditorGUILayout.EndVertical();
        }

        public static void BoolButton(SerializedProperty prop, GUIContent content)
        {
            prop.boolValue = GUILayout.Toggle(prop.boolValue, content, EditorStyles.miniButton);
        }

        public static void Arrays(SerializedProperty prop, GUIContent content = null)
        {
            EditorGUI.indentLevel++;
            if (content != null)
                EditorGUILayout.PropertyField(prop, content, true);
            else
                EditorGUILayout.PropertyField(prop, true);
            EditorGUI.indentLevel--;
        }

        public static bool Foldout(SerializedProperty prop, string name, bool bold = false)
        {
            EditorGUI.indentLevel++;
            if (bold)
                prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, name, BoldFoldout);
            else
                prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, name);
            EditorGUI.indentLevel--;

            return prop.boolValue;
        }
    }
#endif
}