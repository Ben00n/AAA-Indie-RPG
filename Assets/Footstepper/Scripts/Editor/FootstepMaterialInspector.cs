
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[CustomEditor(typeof(FootstepMaterial))]
	[CanEditMultipleObjects]
	public class FootstepMaterialInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// default effect
			SerializedProperty defaultEffect = serializedObject.FindProperty("defaultEffect");
			if(defaultEffect != null)
			{
				EditorGUILayout.HelpBox(
					"The default effect is used when the footstepper doesn't use an effect tag or no matching tag is defined in the tag effects.",
					MessageType.Info);

				EditorGUILayout.PropertyField(defaultEffect, true);
			}

			// tag effects
			SerializedProperty tagEffects = serializedObject.FindProperty("tagEffects");
			if(tagEffects != null)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.HelpBox(
					"The effect of the matching tag is used if the footstepper defined an effect tag.\n" +
					"You can use tags to create different effects for e.g. 'heavy' and 'light' footsteppers.\n" +
					"Tags must be unique, you can't define effects with the same tag.",
					MessageType.Info);

				bool sameTags = false;
				HashSet<string> tags = new HashSet<string>();
				for(int i = 0; i < this.targets.Length; i++)
				{
					if(this.targets[i] is FootstepMaterial)
					{
						tags.Clear();
						FootstepMaterial material = (FootstepMaterial)this.targets[i];
						for(int j = 0; j < material.tagEffects.Count; j++)
						{
							if(!tags.Contains(material.tagEffects[j].tag))
							{
								tags.Add(material.tagEffects[j].tag);
							}
							else
							{
								sameTags = true;
								i = this.targets.Length;
								break;
							}
						}
					}
				}
				if(sameTags)
				{
					EditorGUILayout.HelpBox(
						"A tag is used multiple times - only use a tag once!",
						MessageType.Error);
				}

				EditorGUILayout.PropertyField(tagEffects, true);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
