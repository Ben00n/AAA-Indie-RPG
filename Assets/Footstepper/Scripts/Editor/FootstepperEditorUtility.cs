
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	public static class FootstepperEditorUtility
	{
		[MenuItem("GameObject/Footstep Manager", false, 10)]
		public static void CreateFootstepManager(MenuCommand menuCommand)
		{
			GameObject gameObject = new GameObject("Footstep Manager");
			gameObject.AddComponent<FootstepManager>();
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			Selection.activeObject = gameObject;
		}

		[MenuItem("Component/Footstepper/Add Footstepper (3D)", true)]
		[MenuItem("Component/Footstepper/Add Footstepper (2D)", true)]
		public static bool ValidateAddFootstepper()
		{
			return Selection.activeGameObject != null;
		}

		[MenuItem("Component/Footstepper/Add Footstepper (3D)", false, 10)]
		public static void AddFootstepper3D()
		{
			if(Selection.activeGameObject != null)
			{
				Footstepper footstepper = Selection.activeGameObject.GetComponent<Footstepper>();
				if(footstepper == null)
				{
					footstepper = Selection.activeGameObject.AddComponent<Footstepper>();
					footstepper.raycastMode = RaycastMode.Raycast3D;

					AudioSource audioSource = Selection.activeGameObject.GetComponent<AudioSource>();
					if(audioSource == null)
					{
						audioSource = Selection.activeGameObject.AddComponent<AudioSource>();
					}
					footstepper.source = audioSource;
				}
			}
		}

		[MenuItem("Component/Footstepper/Add Footstepper (2D)", false, 10)]
		public static void AddFootstepper2D()
		{
			if(Selection.activeGameObject != null)
			{
				Footstepper footstepper = Selection.activeGameObject.GetComponent<Footstepper>();
				if(footstepper == null)
				{
					footstepper = Selection.activeGameObject.AddComponent<Footstepper>();
					footstepper.raycastMode = RaycastMode.Raycast2D;

					AudioSource audioSource = Selection.activeGameObject.GetComponent<AudioSource>();
					if(audioSource == null)
					{
						audioSource = Selection.activeGameObject.AddComponent<AudioSource>();
					}
					footstepper.source = audioSource;
				}
			}
		}
	}
}