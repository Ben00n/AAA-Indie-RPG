
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	public enum FootstepType { None, Walk, Run, Sprint, Jump, Land, Custom };

	[System.Serializable]
	public class FootstepEffect
	{
		// walk
		[Header("Walk (Fallback: Run > Sprint)")]
		[Tooltip("Audio clips used at walking speed.\n" +
			"If no clips are defined (i.e. size 0), uses run and sprint clips as fallback (in that order).")]
		public List<AudioClip> walkAudioClips = new List<AudioClip>();

		[Tooltip("Prefabs used at walking speed.\n" +
			"If no prefabs are defined (i.e. size 0), uses run and sprint prefabs as fallback (in that order).")]
		public List<FootstepPrefab> walkPrefabs = new List<FootstepPrefab>();


		// run
		[Header("Run (Fallback: Walk > Sprint)")]
		[Tooltip("Audio clips used at running speed.\n" +
			"If no clips are defined (i.e. size 0), uses walk and sprint clips as fallback (in that order).")]
		public List<AudioClip> runAudioClips = new List<AudioClip>();

		[Tooltip("Prefabs used at running speed.\n" +
			"If no prefabs are defined (i.e. size 0), uses walk and sprint prefabs as fallback (in that order).")]
		public List<FootstepPrefab> runPrefabs = new List<FootstepPrefab>();


		// sprint
		[Header("Sprint (Fallback: Run > Walk)")]
		[Tooltip("Audio clips used at sprinting speed.\n" +
			"If no clips are defined (i.e. size 0), uses run and walk clips as fallback (in that order).")]
		public List<AudioClip> sprintAudioClips = new List<AudioClip>();

		[Tooltip("Prefabs used at sprinting speed.\n" +
			"If no prefabs are defined (i.e. size 0), uses run and walk prefabs as fallback (in that order).")]
		public List<FootstepPrefab> sprintPrefabs = new List<FootstepPrefab>();


		// jump
		[Header("Jump")]
		[Tooltip("Audio clips used on jumping.")]
		public List<AudioClip> jumpAudioClips = new List<AudioClip>();

		[Tooltip("Prefabs used on jumping.")]
		public List<FootstepPrefab> jumpPrefabs = new List<FootstepPrefab>();


		// land
		[Header("Land")]
		[Tooltip("Audio clips used on landing.")]
		public List<AudioClip> landAudioClips = new List<AudioClip>();

		[Tooltip("Prefabs used on landing.")]
		public List<FootstepPrefab> landPrefabs = new List<FootstepPrefab>();


		// custom
		[Header("Custom Effects")]
		[Tooltip("Custom footstep effects can be played by using a matching custom effect name.")]
		public List<FootstepCustomEffect> customEffects = new List<FootstepCustomEffect>();

		public FootstepEffect()
		{

		}

		/// <summary>
		/// Returns the audio clip for a footstep type.
		/// </summary>
		/// <param name="type">The footstep type.</param>
		/// <returns>The found audio clip.</returns>
		public virtual AudioClip GetClip(FootstepType type, string customName)
		{
			if(FootstepType.Walk == type)
			{
				if(this.walkAudioClips.Count > 0)
				{
					return this.walkAudioClips[Random.Range(0, this.walkAudioClips.Count - 1)];
				}
				else if(this.runAudioClips.Count > 0)
				{
					return this.runAudioClips[Random.Range(0, this.runAudioClips.Count - 1)];
				}
				else if(this.sprintAudioClips.Count > 0)
				{
					return this.sprintAudioClips[Random.Range(0, this.sprintAudioClips.Count - 1)];
				}
			}
			else if(FootstepType.Run == type)
			{
				if(this.runAudioClips.Count > 0)
				{
					return this.runAudioClips[Random.Range(0, this.runAudioClips.Count - 1)];
				}
				else if(this.walkAudioClips.Count > 0)
				{
					return this.walkAudioClips[Random.Range(0, this.walkAudioClips.Count - 1)];
				}
				else if(this.sprintAudioClips.Count > 0)
				{
					return this.sprintAudioClips[Random.Range(0, this.sprintAudioClips.Count - 1)];
				}
			}
			else if(FootstepType.Sprint == type)
			{
				if(this.sprintAudioClips.Count > 0)
				{
					return this.sprintAudioClips[Random.Range(0, this.sprintAudioClips.Count - 1)];
				}
				else if(this.runAudioClips.Count > 0)
				{
					return this.runAudioClips[Random.Range(0, this.runAudioClips.Count - 1)];
				}
				else if(this.walkAudioClips.Count > 0)
				{
					return this.walkAudioClips[Random.Range(0, this.walkAudioClips.Count - 1)];
				}
			}
			else if(FootstepType.Jump == type)
			{
				if(this.jumpAudioClips.Count > 0)
				{
					return this.jumpAudioClips[Random.Range(0, this.jumpAudioClips.Count - 1)];
				}
			}
			else if(FootstepType.Land == type)
			{
				if(this.landAudioClips.Count > 0)
				{
					return this.landAudioClips[Random.Range(0, this.landAudioClips.Count - 1)];
				}
			}
			else if(FootstepType.Custom == type)
			{
				if(this.customEffects.Count > 0)
				{
					for(int i = 0; i < this.customEffects.Count; i++)
					{
						if(this.customEffects[i].customName == customName)
						{
							return this.customEffects[i].GetClip();
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the footstep prefab for a footstep type.
		/// </summary>
		/// <param name="type">The footstep type.</param>
		/// <returns>The found footstep prefab.</returns>
		public virtual FootstepPrefab GetPrefab(FootstepType type, string customName)
		{
			if(FootstepType.Walk == type)
			{
				if(this.walkPrefabs.Count > 0)
				{
					return this.walkPrefabs[Random.Range(0, this.walkPrefabs.Count - 1)];
				}
				else if(this.runPrefabs.Count > 0)
				{
					return this.runPrefabs[Random.Range(0, this.runPrefabs.Count - 1)];
				}
				else if(this.sprintPrefabs.Count > 0)
				{
					return this.sprintPrefabs[Random.Range(0, this.sprintPrefabs.Count - 1)];
				}
			}
			else if(FootstepType.Run == type)
			{
				if(this.runPrefabs.Count > 0)
				{
					return this.runPrefabs[Random.Range(0, this.runPrefabs.Count - 1)];
				}
				else if(this.walkPrefabs.Count > 0)
				{
					return this.walkPrefabs[Random.Range(0, this.walkPrefabs.Count - 1)];
				}
				else if(this.sprintPrefabs.Count > 0)
				{
					return this.sprintPrefabs[Random.Range(0, this.sprintPrefabs.Count - 1)];
				}
			}
			else if(FootstepType.Sprint == type)
			{
				if(this.sprintPrefabs.Count > 0)
				{
					return this.sprintPrefabs[Random.Range(0, this.sprintPrefabs.Count - 1)];
				}
				else if(this.runPrefabs.Count > 0)
				{
					return this.runPrefabs[Random.Range(0, this.runPrefabs.Count - 1)];
				}
				else if(this.walkPrefabs.Count > 0)
				{
					return this.walkPrefabs[Random.Range(0, this.walkPrefabs.Count - 1)];
				}
			}
			else if(FootstepType.Jump == type)
			{
				if(this.jumpPrefabs.Count > 0)
				{
					return this.jumpPrefabs[Random.Range(0, this.jumpPrefabs.Count - 1)];
				}
			}
			else if(FootstepType.Land == type)
			{
				if(this.landPrefabs.Count > 0)
				{
					return this.landPrefabs[Random.Range(0, this.landPrefabs.Count - 1)];
				}
			}
			else if(FootstepType.Custom == type)
			{
				if(this.customEffects.Count > 0)
				{
					for(int i = 0; i < this.customEffects.Count; i++)
					{
						if(this.customEffects[i].customName == customName)
						{
							return this.customEffects[i].GetPrefab();
						}
					}
				}
			}
			return null;
		}
	}
}
