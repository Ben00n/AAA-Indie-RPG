// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.IO;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This is the Quest Machine welcome window. It provides easy shortcuts to
    /// tools, documentation, and support.
    /// </summary>
    [InitializeOnLoad]
    public class WelcomeWindow : EditorWindow
    {

        private const string ShowOnStartEditorPrefsKey = "PixelCrushers.QuestMachine.ShowWelcomeWindowOnStart";

        private static bool showOnStartPrefs
        {
            get { return EditorPrefs.GetBool(ShowOnStartEditorPrefsKey, true); }
            set { EditorPrefs.SetBool(ShowOnStartEditorPrefsKey, value); }
        }

        [MenuItem("Tools/Pixel Crushers/Quest Machine/Welcome Window", false, 0)]
        public static WelcomeWindow ShowWindow()
        {
            var window = GetWindow<WelcomeWindow>(false, "Welcome");
            window.minSize = new Vector2(300, 352);
            window.showOnStart = true; // Can't check EditorPrefs when constructing window. Check in first EditorApplication.update.
            return window;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            RegisterWindowCheck();
        }

        private static void RegisterWindowCheck()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += CheckShowWelcomeWindow;
            }
        }

        private static void CheckShowWelcomeWindow()
        {
            EditorApplication.update -= CheckShowWelcomeWindow;
            if (showOnStartPrefs)
            {
                ShowWindow();
            }
        }

        public bool showOnStart = true;

        private string m_version = null;

        private void OnGUI()
        {
            DrawBanner();
            DrawButtons();
            DrawDefines();
            DrawFooter();
        }

        private void DrawBanner()
        {
            if (QuestEditorStyles.logo == null)
            {
                EditorGUILayout.LabelField("Quest Machine", EditorStyles.boldLabel);
            }
            else
            {
                GUI.DrawTexture(new Rect(5, 5, QuestEditorStyles.logo.width, QuestEditorStyles.logo.height), QuestEditorStyles.logo);
            }
            if (m_version == null) m_version = GetVersion();
            if (!string.IsNullOrEmpty(m_version))
            {
                var versionSize = EditorStyles.label.CalcSize(new GUIContent(m_version));
                GUI.Label(new Rect(position.width - (versionSize.x + 5), 47 - versionSize.y, versionSize.x, versionSize.y), m_version);
            }
        }

        private const float ButtonWidth = 68;

        private void DrawButtons()
        {
            GUILayout.BeginArea(new Rect(5, 56, position.width - 10, position.height - 56));
            try
            {
                EditorGUILayout.HelpBox("Welcome to Quest Machine!\n\nThe buttons below are shortcuts to common functions.", MessageType.None);
                GUILayout.Label("Help", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                try
                {
                    if (GUILayout.Button(new GUIContent("\nManual\n", "Open the Quest Machine manual."), GUILayout.Width(ButtonWidth)))
                    {
                        Application.OpenURL("file://" + Application.dataPath + "/Plugins/Pixel Crushers/Quest Machine/Documentation/Quest_Machine_Manual.pdf");
                    }
                    if (GUILayout.Button(new GUIContent("\nVideos\n", "Open the video tutorial list."), GUILayout.Width(ButtonWidth)))
                    {
                        Application.OpenURL("http://www.pixelcrushers.com/quest-machine-video-tutorials/");
                    }
                    if (GUILayout.Button(new GUIContent("Scripting\nReference\n", "Open the scripting & API reference."), GUILayout.Width(ButtonWidth)))
                    {
                        Application.OpenURL("http://pixelcrushers.com/quest_machine/api/html");
                    }
                    if (GUILayout.Button(new GUIContent("\nForum\n", "Go to the Pixel Crushers forum."), GUILayout.Width(ButtonWidth)))
                    {
                        Application.OpenURL("http://www.pixelcrushers.com/phpbb");
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
                GUILayout.Label("Editors", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                try
                {
                    if (GUILayout.Button(new GUIContent("Quest\nEditor\n", "Open the Quest Editor."), GUILayout.Width(ButtonWidth)))
                    {
                        QuestEditorWindow.ShowWindow();
                    }
                    if (GUILayout.Button(new GUIContent("Quest\nGenerator\n", "Open the Quest Generator Editor."), GUILayout.Width(ButtonWidth)))
                    {
                        QuestGeneratorEditorWindow.ShowWindow();
                    }
                    if (GUILayout.Button(new GUIContent("Quest\nReference\n", "Open the Quest Reference utility window."), GUILayout.Width(ButtonWidth)))
                    {
                        QuestReferenceEditorWindow.ShowWindow();
                    }
                    if (GUILayout.Button(new GUIContent("Text\nTable\nEditor", "Open the Text Table editor."), GUILayout.Width(ButtonWidth)))
                    {
                        TextTableEditorWindow.ShowWindow();
                    }
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        private const string USE_PHYSICS2D = "USE_PHYSICS2D";
        private const string USE_NEW_INPUT = "USE_NEW_INPUT";
        private const string TMP_PRESENT = "TMP_PRESENT";

        private void DrawDefines()
        {
            GUILayout.BeginArea(new Rect(5, 240, position.width - 10, position.height - 240));
            EditorGUILayout.LabelField("Current Build Target: " + ObjectNames.NicifyVariableName(EditorUserBuildSettings.activeBuildTarget.ToString()), EditorStyles.boldLabel);

            var define_USE_PHYSICS2D = false;
            var define_USE_NEW_INPUT = false;
            var define_TMP_PRESENT = false;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            for (int i = 0; i < defines.Length; i++)
            {
                if (string.Equals(USE_PHYSICS2D, defines[i].Trim())) define_USE_PHYSICS2D = true;
                if (string.Equals(USE_NEW_INPUT, defines[i].Trim())) define_USE_NEW_INPUT = true;
                if (string.Equals(TMP_PRESENT, defines[i].Trim())) define_TMP_PRESENT = true;
            }
#if EVALUATION_VERSION || !UNITY_2018_1_OR_NEWER
            define_USE_PHYSICS2D = true;
            define_USE_NEW_INPUT = false;
            define_TMP_PRESENT = false;
#endif

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(new GUIContent("Enable support for:", "NOTE: Enables Dialogue System support. You must still enable each package in Package Manager."));
#if UNITY_2018_1_OR_NEWER && !EVALUATION_VERSION
            var new_USE_PHYSICS2D = EditorGUILayout.ToggleLeft("2D Physics (USE_PHYSICS2D)", define_USE_PHYSICS2D);
            var new_USE_NEW_INPUT = EditorGUILayout.ToggleLeft("New Input System (USE_NEW_INPUT)", define_USE_NEW_INPUT);
#else
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ToggleLeft(new GUIContent("2D Physics (USE_PHYSICS2D)", "Support is built in for evaluation version or Unity 2017 and earlier."), define_USE_PHYSICS2D);
            EditorGUILayout.ToggleLeft(new GUIContent("New Input System (USE_NEW_INPUT)", "New Input System support not available in evaluation version."), define_USE_NEW_INPUT);
            EditorGUI.EndDisabledGroup();
            var new_USE_PHYSICS2D = define_USE_PHYSICS2D;
            var new_USE_NEW_INPUT = define_USE_NEW_INPUT;
#endif

#if EVALUATION_VERSION
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ToggleLeft(new GUIContent("TextMesh Pro (TMP_PRESENT)", "TextMesh Pro support not available in evaluation version."), define_TMP_PRESENT);
            EditorGUI.EndDisabledGroup();
            var new_TMP_PRESENT = define_TMP_PRESENT;
#else
            var new_TMP_PRESENT = EditorGUILayout.ToggleLeft(new GUIContent("TextMesh Pro (TMP_PRESENT)", "Enable Dialogue System support for TextMesh Pro. You must still enable TextMesh Pro in Package Manager."), define_TMP_PRESENT);
#endif
            var changed = EditorGUI.EndChangeCheck();

            if (new_USE_PHYSICS2D != define_USE_PHYSICS2D) MoreEditorUtility.ToggleScriptingDefineSymbol(USE_PHYSICS2D, new_USE_PHYSICS2D);
            if (new_USE_NEW_INPUT != define_USE_NEW_INPUT) MoreEditorUtility.ToggleScriptingDefineSymbol(USE_NEW_INPUT, new_USE_NEW_INPUT);
            if (new_TMP_PRESENT != define_TMP_PRESENT) MoreEditorUtility.ToggleScriptingDefineSymbol(TMP_PRESENT, new_TMP_PRESENT, true);

            //EditorWindowTools.DrawHorizontalLine();
            GUILayout.EndArea();

            if (changed) MoreEditorUtility.RecompileScripts();
        }

        private void DrawFooter()
        {
            var newShowOnStart = EditorGUI.ToggleLeft(new Rect(5, position.height - 5 - EditorGUIUtility.singleLineHeight, position.width - 90, EditorGUIUtility.singleLineHeight), "Show at start", showOnStart);
            if (newShowOnStart != showOnStart)
            {
                showOnStart = newShowOnStart;
                showOnStartPrefs = newShowOnStart;
            }
            if (GUI.Button(new Rect(position.width - 80, position.height - 5 - EditorGUIUtility.singleLineHeight, 70, EditorGUIUtility.singleLineHeight), new GUIContent("Support", "Contact the developer for support")))
            {
                Application.OpenURL("http://www.pixelcrushers.com/support-form/");
            }
        }

        private string GetVersion()
        {
            try
            {
                var filename = Application.dataPath + "/Plugins/Pixel Crushers/Quest Machine/Documentation/_RELEASE_NOTES.txt";
                if (File.Exists(filename))
                {
                    var lines = File.ReadAllLines(filename);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        if (line.StartsWith("Version"))
                        {
                            return line.Replace("Version ", string.Empty).Replace(":", string.Empty);
                        }
                    }
                }
                return string.Empty;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

    }

}
