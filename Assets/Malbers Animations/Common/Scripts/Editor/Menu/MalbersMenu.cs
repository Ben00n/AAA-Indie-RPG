using UnityEditor;
using UnityEngine;

namespace MalbersAnimations
{ 
    public class MalbersMenu : EditorWindow
    {
        const string URPL_Shader_Path = "Assets/Malbers Animations/Common/Shaders/URPL_MalbersShaders.unitypackage";
        const string HRPL_Shader_Path = "Assets/Malbers Animations/Common/Shaders/HRPL_MalbersShaders 2019 LTS.unitypackage";
        const string HRPL21_Shader_Path = "Assets/Malbers Animations/Common/Shaders/HRPL_MalbersShaders 2020.unitypackage";

        [MenuItem("Tools/Malbers Animations/Upgrade Malbers shaders to URPL", false, 0)]
        public static void UpgradeMaterialsURPL() => AssetDatabase.ImportPackage(URPL_Shader_Path, true);

        [MenuItem("Tools/Malbers Animations/Upgrade Malbers shaders to HRPL 2019 LTS", false, 0)]
        public static void UpgradeMaterialsHRPL() => AssetDatabase.ImportPackage(HRPL_Shader_Path, true);

        [MenuItem("Tools/Malbers Animations/Upgrade Malbers shaders to HRPL 2020+", false, 0)]
        public static void UpgradeMaterialsHRPL2020() => AssetDatabase.ImportPackage(HRPL21_Shader_Path, true);


        [MenuItem("Tools/Malbers Animations/Integrations", false, 2)]
        public static void OpenIntegrations() => Application.OpenURL("https://malbersanimations.gitbook.io/animal-controller/annex/integrations");


    }

        [CreateAssetMenu()]
        public class PrefabReferenceFixer : ScriptableObject
        {
            [MenuItem("Assets/Force Reserialize")]
            private static void ForceReserialize()
            {
                GameObject[] selection = Selection.gameObjects;
                string[] objectPaths = new string[selection.Length];

                for (int i = 0; i < selection.Length; ++i)
                {
                    objectPaths[i] = AssetDatabase.GetAssetPath(selection[i]);
                }

                AssetDatabase.ForceReserializeAssets(objectPaths);
            }
        }
}
