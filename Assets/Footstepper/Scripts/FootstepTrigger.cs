using UnityEngine;
using System.Collections;

namespace GamingIsLove.Footsteps
{
	[AddComponentMenu("Footstepper/Footstep Trigger")]
	public class FootstepTrigger : MonoBehaviour
	{
		[Tooltip("The footstepper used to play footstep effects.")]
		public Footstepper footstepper;

		[Tooltip("The time in seconds between allowing 2 footsteps from this trigger.")]
		public float timeBetween = 0.1f;

		[Tooltip("Use the footstepper's raycast to determine the position and normal of the surface.\n" +
			"Otherwise uses the position of this game object.")]
		public bool raycastPosition = true;

		// layer limitation
		[Tooltip("Limit the layers that can cause footsteps.")]
		[Space(10)]
		public bool limitLayers = false;

		[Tooltip("Select the layers that can cause footsteps (only used when 'Limit Layers' is enabled).")]
		public LayerMask layerMask = -1;


		// in-game
		private float timeout = 0;

		protected virtual void Start()
		{
			if(this.footstepper == null)
			{
				this.enabled = false;
			}
		}

		protected virtual void Update()
		{
			if(this.timeBetween > 0)
			{
				this.timeout -= Time.deltaTime;
			}
		}

		protected virtual bool CheckLayer(GameObject gameObject)
		{
			return !this.limitLayers || (this.layerMask.value & 1 << gameObject.layer) != 0;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			if(this.timeout <= 0 && 
				this.CheckLayer(other.gameObject))
			{
				FootstepSource footstepSource = other.gameObject.GetComponentInParent<FootstepSource>();
				if(footstepSource != null)
				{
					this.timeout = this.timeBetween;
					this.footstepper.PlayFootstep(this.transform, footstepSource, this.raycastPosition);
				}
			}
		}

		protected virtual void OnTriggerEnter2D(Collider2D other)
		{
			if(this.timeout <= 0 &&
				this.CheckLayer(other.gameObject))
			{
				FootstepSource footstepSource = other.gameObject.GetComponentInParent<FootstepSource>();
				if(footstepSource != null)
				{
					this.timeout = this.timeBetween;
					this.footstepper.PlayFootstep(this.transform, footstepSource, this.raycastPosition);
				}
			}
		}


		/*
		============================================================================
		Gizmo functions
		============================================================================
		*/
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/FootstepTrigger Icon.png");
		}
	}
}
