
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[CustomEditor(typeof(FootstepTextureMaterial))]
	[CanEditMultipleObjects]
	public class FootstepTextureMaterialInspector : Editor
	{
		private Terrain terrain;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// load textues from terrain
			if(!serializedObject.isEditingMultipleObjects)
			{
				this.terrain = (Terrain)EditorGUILayout.ObjectField("Load From Terrain", this.terrain, typeof(Terrain), true);
				if(this.terrain != null)
				{
					EditorGUILayout.HelpBox(
						"This will remove all currently added data!",
						MessageType.Warning);
					if(GUILayout.Button("Load Textures From Terrain"))
					{
						((FootstepTextureMaterial)this.target).LoadFromTerrain(this.terrain);
						this.terrain = null;
					}
					EditorGUILayout.Separator();
				}
			}

			EditorGUILayout.HelpBox(
				"Set up which footstep material will be used for which textures to allow terrains to find the correct effects.",
				MessageType.Info);

			this.DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
