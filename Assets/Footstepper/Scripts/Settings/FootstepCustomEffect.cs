
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[System.Serializable]
	public class FootstepCustomEffect
	{
		[Tooltip("The name of the custom footstep effect - used to identify which custom effect will be used by matching names.")]
		public string customName = "";

		[Tooltip("Audio clips used for this custom footstep effect.")]
		public List<AudioClip> audioClips = new List<AudioClip>();

		[Tooltip("Prefabs used for this custom footstep effect.")]
		public List<FootstepPrefab> prefabs = new List<FootstepPrefab>();

		public FootstepCustomEffect()
		{

		}

		/// <summary>
		/// Returns the audio clip.
		/// </summary>
		/// <returns>The found audio clip.</returns>
		public virtual AudioClip GetClip()
		{
			if(this.audioClips.Count > 0)
			{
				return this.audioClips[Random.Range(0, this.audioClips.Count - 1)];
			}
			return null;
		}

		/// <summary>
		/// Returns the footstep prefab.
		/// </summary>
		/// <returns>The found footstep prefab.</returns>
		public virtual FootstepPrefab GetPrefab()
		{
			if(this.prefabs.Count > 0)
			{
				return this.prefabs[Random.Range(0, this.prefabs.Count - 1)];
			}
			return null;
		}
	}
}
