using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Preset/BlendShape Preset")]
    [HelpURL("https://malbersanimations.gitbook.io/animal-controller/utilities/blend-shapes/blend-shape-preset")]
    public class BlendShapePreset : ScriptableCoroutine
    {
        [Header("Smooth BlendShapes")]
        public FloatReference BlendTime = new FloatReference(1.5f);
        public AnimationCurve BlendCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

 

        [Space, Header("Blend Shapes Weights")]
        public float[] blendShapes;

        public void Load(SkinnedMeshRenderer mesh)
        {
            int Length = Mathf.Min(mesh.sharedMesh.blendShapeCount, blendShapes.Length);

            for (int i = 0; i < Length; i++)
            {
                mesh.SetBlendShapeWeight(i, blendShapes[i]);
            }
        }
        public virtual void SmoothBlend(SkinnedMeshRenderer mesh)
        {
            SetCoroutine(mesh.gameObject);
            Stop();

            coroutine.StartCoroutine(ICoroutine = C_SmoothBlend(mesh));
        }


        protected IEnumerator C_SmoothBlend(SkinnedMeshRenderer mesh)
        {
            var BSScript = coroutine.GetComponentInParent<BlendShape>();

            float elapsedTime = 0;
            int Length = Mathf.Min(mesh.sharedMesh.blendShapeCount, blendShapes.Length);

            float[] StartBlends = new float[mesh.sharedMesh.blendShapeCount];

            for (int i = 0; i < Length; i++)
            {
                StartBlends[i] = mesh.GetBlendShapeWeight(i);
            }


            while ((BlendTime > 0) && (elapsedTime <= BlendTime))
            {
                float result = BlendCurve.Evaluate(elapsedTime / BlendTime);             //Evaluation of the Pos curve

                for (int i = 0; i < Length; i++)
                {
                    var newWeight = Mathf.Lerp(StartBlends[i], blendShapes[i], result);

                    if (BSScript)
                    {
                        BSScript.blendShapes[i] = newWeight;
                        BSScript.UpdateBlendShapes();
                    }
                    else
                    {
                        mesh.SetBlendShapeWeight(i, newWeight);
                    }
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            if (BSScript)
            {
                BSScript.LoadPreset(this);
            }
            else
            {
                Load(mesh);
            }
          
            if (BSScript) BSScript.SetShapesCount();

            yield return null;
        }
    }
}