using UnityEngine;
using System.Collections;




namespace MalbersAnimations.Utilities
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Material Property Lerp", order = 2000)]
    public class MaterialPropertyLerpSO : ScriptableCoroutine
    {
        [Tooltip("Index of the Material")]
        public int materialIndex = 0;
        public float time = 1f;
        public AnimationCurve curve = new AnimationCurve(MTools.DefaultCurve);


        public string propertyName;
        public MaterialPropertyType propertyType = MaterialPropertyType.Float;
       
        public float FloatValue = 1f;
        public Color ColorValue = Color.white;
        [ColorUsage(true, true)]
        public Color ColorHDRValue = Color.white;




        //protected override void OnDisable()
        //{
        //    base.OnDisable();
        //    MatBlock = null;
        //}


 

        public virtual void Lerp(Renderer mesh)
        {
            SetCoroutine(mesh.gameObject);
            //Stop();

            //if (MatBlock == null) MatBlock = new MaterialPropertyBlock();
            //mesh.GetPropertyBlock(MatBlock, materialIndex);


            switch (propertyType)
            {
                case MaterialPropertyType.Float:
                    coroutine.StartCoroutine(ICoroutine = LerperFloat(mesh));
                    break;
                case MaterialPropertyType.Color:
                    coroutine.StartCoroutine(ICoroutine = LerperColor(mesh, ColorValue));
                    break;
                case MaterialPropertyType.HDRColor:
                    coroutine.StartCoroutine(ICoroutine = LerperColor(mesh, ColorHDRValue));
                    break;
                default:
                    break;
            }
        }


        IEnumerator LerperFloat(Renderer mesh)
        {
            float elapsedTime = 0;
            var mat = mesh.materials[materialIndex];

            while (elapsedTime <= time)
            {
                float value = curve.Evaluate(elapsedTime / time);
                mat.SetFloat(propertyName, value * FloatValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            mat.SetFloat(propertyName, curve.Evaluate(1f));

            yield return null;
        }

        IEnumerator LerperColor(Renderer mesh, Color FinalColor)
        {
            float elapsedTime = 0;

            var mat = mesh.materials[materialIndex];


            Color StartingColor = mat.GetColor(propertyName);
            Color ElapsedColor;

            while (elapsedTime <= time)
            {
                float value = curve.Evaluate(elapsedTime / time);

                ElapsedColor = Color.LerpUnclamped(StartingColor, FinalColor, value);

                mat.SetColor(propertyName, ElapsedColor);
               
                elapsedTime += Time.deltaTime;
               

                yield return null;
            } 
            yield return null;
        }


        public MaterialPropertyBlock MatBlock { get; internal set; }
        IEnumerator LerperFloatMPB(Renderer mesh)
        {
            float elapsedTime = 0;

            while (elapsedTime <= time)
            {
                float value = curve.Evaluate(elapsedTime / time);
                MatBlock.SetFloat(propertyName, value * FloatValue);
                mesh.SetPropertyBlock(MatBlock); // apply back onto renderer
                elapsedTime += Time.deltaTime;
                yield return null;
            }


            MatBlock.SetFloat(propertyName, curve.Evaluate(1f));
            mesh.SetPropertyBlock(MatBlock); // apply back onto renderer

            yield return null;
        }


        IEnumerator LerperColorMPB(Renderer mesh, Color FinalColor)
        {
            float elapsedTime = 0;

            Color StartingColor = MatBlock.GetColor(propertyName);
            Color ElapsedColor;

            while (elapsedTime <= time)
            {
                float value = curve.Evaluate(elapsedTime / time);

                ElapsedColor = Color.LerpUnclamped(StartingColor, FinalColor, value);

                MatBlock.SetColor(propertyName, ElapsedColor);
                mesh.SetPropertyBlock(MatBlock); // apply back onto renderer
                elapsedTime += Time.deltaTime;
                yield return null;
            } 
            yield return null;
        }
    }

    [System.Serializable]
    public class MaterialPropertyInternal
    {
        public string propertyName;
        public MaterialPropertyType propertyType = MaterialPropertyType.Float;
        public float FloatValue = 1f;
        public Color ColorValue = Color.white;
        [ColorUsage(true, true)]
        public Color ColorHDRValue = Color.white;

        [HideInInspector] public bool isFloat; 
        [HideInInspector] public bool isColor; 
        [HideInInspector] public bool isColorHDR; 
    }

    public enum MaterialPropertyType
    {
        Float,
        Color,
        HDRColor
    }

    //Inspector

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MaterialPropertyLerpSO)),UnityEditor.CanEditMultipleObjects]
    public class MaterialPropertyLerpSOEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty propertyName, materialIndex, propertyType, time, FloatValue, ColorValue, ColorHDRValue, curve;//, UseMaterialPropertyBlock, shared;

        private void OnEnable()
        {
            propertyName = serializedObject.FindProperty("propertyName");
            materialIndex = serializedObject.FindProperty("materialIndex");
            propertyType = serializedObject.FindProperty("propertyType");
            time = serializedObject.FindProperty("time");
            FloatValue = serializedObject.FindProperty("FloatValue");
            ColorValue = serializedObject.FindProperty("ColorValue");
            ColorHDRValue = serializedObject.FindProperty("ColorHDRValue");
            curve = serializedObject.FindProperty("curve");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(propertyName);
            UnityEditor.EditorGUILayout.PropertyField(materialIndex);
            UnityEditor.EditorGUILayout.PropertyField(time);

            UnityEditor.EditorGUILayout.PropertyField(propertyType);

            var pType = (MaterialPropertyType)propertyType.intValue;

            switch (pType)
            {
                case MaterialPropertyType.Float:
                    UnityEditor.EditorGUILayout.PropertyField(FloatValue);
                    break;
                case MaterialPropertyType.Color:
                    UnityEditor.EditorGUILayout.PropertyField(ColorValue);
                    break;
                case MaterialPropertyType.HDRColor:
                    UnityEditor.EditorGUILayout.PropertyField(ColorHDRValue);
                    break;
                default:
                    break;
            }


            UnityEditor.EditorGUILayout.PropertyField(curve);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

  
}

