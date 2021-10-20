
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[AddComponentMenu("Footstepper/Trigger Footstep Source")]
	public class TriggerFootstepSource : FootstepSource
	{
		[Tooltip("The footstep material defines the footstep effects (audio clips and prefabs) of this trigger.")]
		public FootstepMaterial material;


		// in-game
		protected List<Footstepper> inTrigger = new List<Footstepper>();

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

		protected virtual void OnTriggerEnter(Collider other)
		{
			Footstepper footsteps = other.transform.GetComponent<Footstepper>();
			if(footsteps != null)
			{
				footsteps.AddOverrideSource(this);
				this.inTrigger.Add(footsteps);
			}
		}

		protected virtual void OnTriggerExit(Collider other)
		{
			Footstepper footsteps = other.transform.GetComponent<Footstepper>();
			if(footsteps != null)
			{
				footsteps.RemoveOverrideSource(this);
				this.inTrigger.Remove(footsteps);
			}
		}

		protected virtual void OnTriggerEnter2D(Collider2D other)
		{
			Footstepper footsteps = other.transform.GetComponent<Footstepper>();
			if(footsteps != null)
			{
				footsteps.AddOverrideSource(this);
				this.inTrigger.Add(footsteps);
			}
		}

		protected virtual void OnTriggerExit2D(Collider2D other)
		{
			Footstepper footsteps = other.transform.GetComponent<Footstepper>();
			if(footsteps != null)
			{
				footsteps.RemoveOverrideSource(this);
				this.inTrigger.Remove(footsteps);
			}
		}

		protected virtual void OnDisable()
		{
			for(int i = 0; i < this.inTrigger.Count; i++)
			{
				if(this.inTrigger[i] != null)
				{
					this.inTrigger[i].RemoveOverrideSource(this);
				}
			}
			this.inTrigger.Clear();
		}


		/*
		============================================================================
		Gizmo functions
		============================================================================
		*/
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/TriggerFootstepSource Icon.png");
		}
	}
}
