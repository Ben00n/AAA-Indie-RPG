
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[AddComponentMenu("Footstepper/Object Footstep Source")]
	public class ObjectFootstepSource : FootstepSource
	{
		[Tooltip("The footstep material defines the footstep effects (audio clips and prefabs) of this game object.")]
		public FootstepMaterial material;

		/// <summary>
		/// Returns the footstep effect for a provided effect tag.
		/// </summary>
		/// <param name="position">Not used.</param>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public override FootstepEffect GetFootstepAt(Vector3 position, string effectTag)
		{
			if(this.material != null)
			{
				return this.material.GetEffect(effectTag);
			}
			return null;
		}


		/*
		============================================================================
		Gizmo functions
		============================================================================
		*/
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/ObjectFootstepSource Icon.png");
		}
	}
}
