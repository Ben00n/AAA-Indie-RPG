
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[System.Serializable]
	public class FootstepPrefab
	{
		[Tooltip("The prefab that will be used.")]
		public GameObject prefab;


		// position
		[Space(10)]
		[Tooltip("The prefab will be placed at the position that was hit by the raycast, " +
			"otherwise the position of the foot is used.")]
		public bool atHitPosition = true;

		[Tooltip("Offset added to the prefab's position.")]
		public Vector3 positionOffset = Vector3.zero;


		// rotation
		[Space(10)]
		[Tooltip("Use the rotation of the foot, otherwise uses the rotation of the footstepper's game object.")]
		public bool useFootRotation = false;

		[Tooltip("Uses the normal of the position that was hit by the racast, aligning the prefab with the surface.")]
		public bool useHitNormal = true;

		[Tooltip("Offset added to the prefab's rotation.")]
		public Vector3 rotationOffset = Vector3.zero;


		// remove
		[Space(10)]
		[Tooltip("The time in seconds after which particle systems on the prefab are stopped.\n" +
			"Set to 0 or below to not stop particles.")]
		public float stopAfter = 1.0f;

		[Tooltip("The time in seconds after which the prefab will be removed (disabled when using pooling, destroyed otherwise).\n" +
			"Counted after stopping the prefab's particles - or instantiation if not stopping particles.")]
		public float removeAfter = 0.0f;

		public FootstepPrefab()
		{

		}

		/// <summary>
		/// Creates an instance of the prefab using the footstep prefab's setup.
		/// This handles the complete lifecycle of the prefab - i.e. also stopping particle effects and destroying/disabling the game object.
		/// When a footstep manager is used with pooling enabled, this'll reuse previously spawned prefabs.
		/// </summary>
		/// <param name="origin">The transform on which the footstepper is attached.</param>
		/// <param name="foot">The foot that causes the prefab spawn.</param>
		/// <param name="hitPosition">The ground position (found via raycasting).</param>
		/// <param name="hitNormal">The ground normal (found via raycasting).</param>
		/// <returns>Enumerator used by coroutines.</returns>
		public virtual IEnumerator CreatePrefab(Transform origin, Transform foot, Vector3 hitPosition, Vector3 hitNormal)
		{
			// get pool for prefab
			Queue<GameObject> pool = FootstepManager.Instance != null ?
				FootstepManager.Instance.GetPool(this.prefab) : null;

			// get prefab instance
			GameObject instance = pool != null && pool.Count > 0 ? pool.Dequeue() : null;
			Quaternion rotation = this.useHitNormal ?
				Quaternion.FromToRotation(Vector3.up, hitNormal) *
					Quaternion.Euler((this.useFootRotation ? foot.eulerAngles : origin.eulerAngles) + this.rotationOffset) :
				Quaternion.Euler((this.useFootRotation ? foot.eulerAngles : origin.eulerAngles) + this.rotationOffset);

			// create new
			if(instance == null)
			{
				if(FootstepManager.Instance != null)
				{
					instance = GameObject.Instantiate(this.prefab,
						(this.atHitPosition ? hitPosition : foot.position) + this.positionOffset,
						rotation, FootstepManager.Instance.transform);
				}
				else
				{
					instance = GameObject.Instantiate(this.prefab,
						(this.atHitPosition ? hitPosition : foot.position) + this.positionOffset,
						rotation);
				}
			}
			// from pool
			else
			{
				instance.transform.SetPositionAndRotation(
					(this.atHitPosition ? hitPosition : foot.position) + this.positionOffset,
					rotation);
				instance.SetActive(true);
			}

			// stop particles
			if(this.stopAfter > 0)
			{
				yield return new WaitForSeconds(this.stopAfter);

				ParticleSystem[] particle = instance.GetComponentsInChildren<ParticleSystem>();
				if(particle != null)
				{
					for(int i = 0; i < particle.Length; i++)
					{
						particle[i].Stop(true);
					}
				}
			}

			// remove
			yield return new WaitForSeconds(this.removeAfter >= 0 ? this.removeAfter : 0);

			if(pool != null)
			{
				instance.SetActive(false);
				pool.Enqueue(instance);
			}
			else
			{
				GameObject.Destroy(instance);
			}
		}
	}
}
