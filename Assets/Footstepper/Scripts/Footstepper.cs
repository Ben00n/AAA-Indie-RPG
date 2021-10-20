
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	public enum FootstepperMode { Enabled, Disabled, OnlyAudio, OnlyPrefab }
	public enum FootstepperSpeedUpdateType { None, Update, LateUpdate, FixedUpdate }

	[AddComponentMenu("Footstepper/Footstepper")]
	public class Footstepper : MonoBehaviour
	{
		public static string VERSION = "1.4.0";

		[Tooltip("Select in which mode this footstepper operates:\n" +
			"- Enabled: Uses both audio clips and prefabs from footstep sources.\n" +
			"- Disabled: Isn't used at all.\n" +
			"- Only Audio: Only uses audio clips from footstep sources.\n" +
			"- Only Prefab: Only uses prefabs from footstep sources.\n")]
		public FootstepperMode mode = FootstepperMode.Enabled;

		[Tooltip("Define a tag to find a matching footstep effect, " +
			"leave empty to use the material's default effect.\n" +
			"You can use tags to create different effects for e.g. 'heavy' and 'light' footsteppers.")]
		public string effectTag = "";

		[Tooltip("Prevent footstep effects when first initializing this footstepper (e.g. after loading a scene or spawning).\n" +
			"Time in seconds.")]
		public float initialTimeout = 0;

		[Tooltip("The time in seconds between allowing 2 footsteps.\n" +
			"E.g. usefull when using animation blending, where mutliple animations might play an animation event.")]
		public float timeBetween = 0.05f;

		[Tooltip("The minimum weight an animation must have to play a footstep effect using animation events.\n" +
			"This is only used for footstep effects (walk, run, sprint), not for jump or land effects.")]
		[Range(0.0f, 1.0f)]
		public float minAnimationWeight = 0.4f;

		[Tooltip("Only play the footstep effect of the animation with the highest weight.\n" +
			"This only applies to animation events that are fired in the same frame and " +
			"is only used for footstep effects (walk, run, sprint), not for jump or land effects.")]
		public bool onlyHighestAnimationWeight = true;


		// feet
		[Header("Feet Settings")]
		[Tooltip("Define the feet that will be used - you can set up as many feet as you want.")]
		public List<Transform> feet = new List<Transform>();


		// speed
		[Header("Speed Settings")]
		[Tooltip("Select when the movement speed is calculated, this should match the method used to move the game object:\n" +
			"- None: No automatic speed calculation, e.g. use 'Speed' property to set the speed from your control component.\n" +
			"- Update: In the 'Update' function (i.e. each frame).\n" +
			"- Late Update: In the 'LateUpdate' function (i.e. each frame after all 'Update' functions have finished).\n" +
			"- Fixed Update: In the 'FixedUpdate' function (i.e. each physics frame).")]
		public FootstepperSpeedUpdateType speedUpdateType = FootstepperSpeedUpdateType.FixedUpdate;

		[Tooltip("Check the movement speed to use the correct volume.\n" +
			"When using the dedicated functions for walking, running and sprinting footsteps, " +
			"this will only play the footstep if the speed is correct.")]
		public bool useSpeedCheck = true;

		public float minSpeed = 0;

		[Tooltip("The speed that determines if running is used.\n" +
			"Anything below this speed is considered walking.")]
		public float runSpeed = 4;

		[Tooltip("The speed that determines if sprinting is used.\n" +
			"The speed below this speed (and above the run speed) is considered running.")]
		public float sprintSpeed = 7;


		// autoplay
		[Header("Autoplay Footsteps")]
		[Tooltip("Automatically play the footstep effects at determined inveralls when the game object is moving.\n" +
			"The game object's movement speed determines which effect (and timeout) is used based on the run/sprint speed settings.\n" +
			"Use this option if you don't want (or can't) use other methods like animation events.")]
		public bool autoPlay = false;

		[Tooltip("The time in seconds between playing two walk footstep effects.")]
		public float walkTimeout = 0.5f;

		[Tooltip("The time in seconds between playing two run footstep effects.")]
		public float runTimeout = 0.3f;

		[Tooltip("The time in seconds between playing two sprint footstep effects.")]
		public float sprintTimeout = 0.1f;


		// audio settings
		[Header("Audio Settings")]
		[Tooltip("The audio source used to play the audio clips.")]
		public AudioSource source;

		[Range(0.0f, 1.0f)]
		[Tooltip("Randomizes the pitch when playing an audio clip.\n" +
			"Uses the audio sources starting pitch + a random value between + and - the pitch variation, " +
			"e.g. a pitch of 1 and variation of 0.2 will use a pitch between 0.8 and 1.2.")]
		public float pitchVariation = 0.1f;

		// volume
		[Header("Audio Volumes")]
		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used at walking speed.")]
		public float walkVolume = 0.1f;

		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used at running speed.")]
		public float runVolume = 0.2f;

		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used at sprinting speed.")]
		public float sprintVolume = 0.3f;

		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used for jumping.")]
		public float jumpVolume = 0.3f;

		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used at landing.")]
		public float landVolume = 0.3f;

		[Range(0.0f, 1.0f)]
		[Tooltip("The volume used for custom effects.")]
		public float customEffectVolume = 0.5f;


		// raycast
		[Header("Raycast Settings")]
		[Tooltip("Select if 3D or 2D raycasting is used.")]
		public RaycastMode raycastMode = RaycastMode.Raycast3D;

		[Tooltip("Finding the footstep material below a foot (or the game object) uses raycasting.\n" +
			"The layer mask defines which layers will be checked for footstep sources.")]
		public LayerMask layerMask = -1;

		[Tooltip("The distance used for raycasting.")]
		public float rayDistance = 0.3f;

		[Tooltip("The offset to the foot's (or game object's) position when raycasting.")]
		public Vector3 rayOffset = Vector3.zero;

		[Tooltip("The offset is added in the local space of the foot, otherwise in local space of this game object.")]
		public bool inFootSpace = false;


		// auto find
		[Header("Auto Find")]
		[Tooltip("Search for tilemaps on the hit game object, using the hit position's tile sprite to find a footstep effect.\n" +
			"Requires a 'Footstep Manager' in the scene.")]
		public bool searchTilemaps = true;

		[Tooltip("Search for renderers on the hit game object, using the renderer's main texture/sprite to find a footstep effect.\n" +
			"Requires a 'Footstep Manager' in the scene.")]
		public bool searchRenderers = true;


		// fallback effect
		[Header("Fallback Effect")]
		[Tooltip("The fallback material is used if no footstep source was found.\n" +
			"This still requires the raycast to hit something.")]
		public FootstepMaterial fallbackMaterial;

		[Tooltip("Use the fallback material even if the raycast didn't hit anything.")]
		public bool noRaycastFallback = false;


		// in-game
		protected bool isGrounded = true;

		protected float timeout = 0;

		protected float startPitch = 1.0f;

		protected Vector2 speed = Vector2.zero;

		protected Vector3 lastPosition = Vector3.zero;

		protected int autoPlayIndex = 0;

		protected float autoPlayTimeout = 0.1f;

		protected List<FootstepSource> overrideSources = new List<FootstepSource>();

		protected AnimationEvent highestWeightEvent;

		protected System.Action<int> highestWeightCall;

		protected System.Action<int, string> highestWeightCustomCall;

		protected virtual void Start()
		{
			this.lastPosition = this.transform.position;
			if(this.source != null)
			{
				this.startPitch = this.source.pitch;
			}
			this.timeout = this.initialTimeout;
		}

		protected virtual void OnDisable()
		{
			this.ClearOverrideSources();
		}

		protected virtual void Update()
		{
			if(this.timeout > 0)
			{
				this.timeout -= Time.deltaTime;
			}

			if(FootstepperSpeedUpdateType.Update == this.speedUpdateType)
			{
				this.CalculateSpeed();
			}

			if(this.autoPlay && this.speed.x > 0)
			{
				this.autoPlayTimeout -= Time.deltaTime;
				if(this.autoPlayTimeout <= 0)
				{
					this.FootstepIndex(this.autoPlayIndex++);
					if(this.autoPlayIndex >= this.feet.Count)
					{
						this.autoPlayIndex = 0;
					}
				}
			}
		}

		protected virtual void LateUpdate()
		{
			if(FootstepperSpeedUpdateType.LateUpdate == this.speedUpdateType)
			{
				this.CalculateSpeed();
			}

			if(this.onlyHighestAnimationWeight &&
				this.highestWeightEvent != null)
			{
				if(this.highestWeightCustomCall != null)
				{
					this.highestWeightCustomCall(this.highestWeightEvent.intParameter, this.highestWeightEvent.stringParameter);
				}
				else if(this.highestWeightCall != null)
				{
					this.highestWeightCall(this.highestWeightEvent.intParameter);
				}

				this.highestWeightEvent = null;
				this.highestWeightCall = null;
				this.highestWeightCustomCall = null;
			}
		}

		protected virtual void FixedUpdate()
		{
			if(FootstepperSpeedUpdateType.FixedUpdate == this.speedUpdateType)
			{
				this.CalculateSpeed();
			}
		}

		public virtual void CalculateSpeed()
		{
			Vector3 velocity = (this.transform.position - this.lastPosition) / Time.fixedDeltaTime;
			this.speed.y = velocity.y;
			velocity.y = 0;
			this.speed.x = velocity.magnitude;

			this.lastPosition = this.transform.position;
		}


		/*
		============================================================================
		Override source functions
		============================================================================
		*/
		/// <summary>
		/// Add a footstep source as override source.
		/// </summary>
		/// <param name="footstepSource">The footstep source.</param>
		public virtual void AddOverrideSource(FootstepSource footstepSource)
		{
			this.overrideSources.Add(footstepSource);
		}

		/// <summary>
		/// Remove a footstep source from override sources.
		/// </summary>
		/// <param name="footstepSource">The footstep source</param>
		public virtual void RemoveOverrideSource(FootstepSource footstepSource)
		{
			this.overrideSources.Remove(footstepSource);
		}

		/// <summary>
		/// Removes all current override sources.
		/// </summary>
		public virtual void ClearOverrideSources()
		{
			this.overrideSources.Clear();
		}

		/// <summary>
		/// Returns the current override footstep effect for a position.
		/// </summary>
		/// <param name="position">The position to check for.</param>
		/// <returns>The footstep effect, or null if no effect was found.</returns>
		public virtual FootstepEffect GetOverrideEffect(Vector3 position)
		{
			FootstepEffect effect = null;
			for(int i = this.overrideSources.Count - 1; i >= 0; i--)
			{
				if(this.overrideSources[i].enabled)
				{
					effect = this.overrideSources[i].GetFootstepAt(position, this.effectTag);
					if(effect != null)
					{
						return effect;
					}
				}
			}
			return null;
		}


		/*
		============================================================================
		Tool functions
		============================================================================
		*/
		/// <summary>
		/// Get the enabled state of this footstepper.
		/// This property used by the footstepper to check if it is enabled, override it in custom footsteppers to implement custom enabled checks.
		/// </summary>
		public virtual bool IsEnabled
		{
			get { return FootstepperMode.Disabled != this.mode; }
		}

		/// <summary>
		/// Get the audio enabled state of this footstepper.
		/// This property used by the footstepper to check if audio clips will be played, override it in custom footsteppers to implement custom enabled checks.
		/// </summary>
		public virtual bool IsAudioEnabled
		{
			get { return FootstepperMode.Enabled == this.mode || FootstepperMode.OnlyAudio == this.mode; }
		}

		/// <summary>
		/// Get the prefab enabled state of this footstepper.
		/// This property used by the footstepper to check if prefabs will be spawned, override it in custom footsteppers to implement custom enabled checks.
		/// </summary>
		public virtual bool IsPrefabEnabled
		{
			get { return FootstepperMode.Enabled == this.mode || FootstepperMode.OnlyPrefab == this.mode; }
		}

		/// <summary>
		/// Set the footstepper to be grounded.
		/// Footsteps (walk, run, sprint) are only played if grounded is set to true.
		/// Use this function to e.g. let the footstepper know if it's grounded from your control script.
		/// </summary>
		/// <param name="grounded">true if the footstepper should be grounded.</param>
		public virtual void SetGrounded(bool grounded)
		{
			this.isGrounded = grounded;
		}

		/// <summary>
		/// Returns the foot for an index in the list (e.g. 0 for the first foot in the list).
		/// If the index exceeds the defined feet, this game object's transform is returned.
		/// </summary>
		/// <param name="index">The index that will be used.</param>
		/// <returns>The foot of the used index, or the transform of this game object if not found.</returns>
		public virtual Transform GetFoot(int index)
		{
			if(index >= 0 &&
				index < this.feet.Count &&
				this.feet[index] != null)
			{
				return this.feet[index];
			}
			return this.transform;
		}

		/// <summary>
		/// Get the volume and foodstep type for the current movement speed.
		/// </summary>
		/// <param name="volume">The volume that will be set.</param>
		/// <param name="type">The footstep type that will be set.</param>
		public virtual void GetSpeedType(ref float volume, ref FootstepType type)
		{
			volume = this.runVolume;
			type = FootstepType.Run;
			this.autoPlayTimeout = this.runTimeout;

			if(this.useSpeedCheck)
			{
				if(this.speed.x < this.runSpeed)
				{
					volume = this.walkVolume;
					type = FootstepType.Walk;
					this.autoPlayTimeout = this.walkTimeout;
				}
				else if(this.speed.x >= this.sprintSpeed)
				{
					volume = this.sprintVolume;
					type = FootstepType.Sprint;
					this.autoPlayTimeout = this.sprintTimeout;
				}
			}
		}

		/// <summary>
		/// The current movement speed.
		/// X-axis is the horizontal speed.
		/// Y-axis is the vertical speed.
		/// </summary>
		public virtual Vector2 Speed
		{
			get { return this.speed; }
			set { this.speed = value; }
		}


		/*
		============================================================================
		Play functions
		============================================================================
		*/
		/// <summary>
		/// Plays a footstep effect.
		/// </summary>
		/// <param name="foot">The foot that will be used.</param>
		/// <param name="volume">The volume used for audio clips.</param>
		/// <param name="type">The footstep type that will be used.</param>
		public virtual void PlayFootstep(Transform foot, float volume, FootstepType type, string customName)
		{
			if(this.timeout <= 0)
			{
				Vector3 hitPosition;
				Vector3 hitNormal;
				FootstepEffect effect = this.FindFootstep(foot, out hitPosition, out hitNormal);
				if(effect != null)
				{
					if(this.IsAudioEnabled)
					{
						AudioClip clip = effect.GetClip(type, customName);
						if(clip != null &&
							this.source != null &&
							volume > 0)
						{
							this.source.pitch = this.startPitch + Random.Range(-this.pitchVariation, this.pitchVariation);
							this.source.PlayOneShot(clip, volume);
						}
					}

					if(this.IsPrefabEnabled)
					{
						FootstepPrefab prefab = effect.GetPrefab(type, customName);
						if(prefab != null &&
							prefab.prefab != null)
						{
							this.StartCoroutine(prefab.CreatePrefab(this.transform, foot, hitPosition, hitNormal));
						}
					}

					this.timeout = this.timeBetween;
				}
			}
		}

		/// <summary>
		/// Plays a footstep effect from a provided footstep source.
		/// </summary>
		/// <param name="foot">The foot that will be used.</param>
		/// <param name="footstepSource">The footstep source that will be used.</param>
		/// <param name="raycastPosition">true if a raycast should be used to find the ground position.</param>
		public virtual void PlayFootstep(Transform foot, FootstepSource footstepSource, bool raycastPosition)
		{
			if(this.IsEnabled &&
				this.timeout <= 0)
			{
				Vector3 hitPosition = foot.position;
				Vector3 hitNormal = Vector3.up;
				if(raycastPosition)
				{
					RaycastResult result = this.Raycast(foot);
					if(result != null)
					{
						hitPosition = result.point;
						hitNormal = result.normal;
					}
				}

				FootstepEffect effect = null;
				if(this.overrideSources.Count > 0)
				{
					effect = this.GetOverrideEffect(hitPosition);
				}
				if(effect == null)
				{
					effect = footstepSource.GetFootstepAt(hitPosition, this.effectTag);
				}

				if(effect != null)
				{
					FootstepType type = FootstepType.Run;

					if(this.IsAudioEnabled)
					{
						float volume = this.runVolume;
						this.GetSpeedType(ref volume, ref type);

						AudioClip clip = effect.GetClip(type, "");
						if(clip != null &&
							this.source != null &&
							volume > 0)
						{
							this.source.pitch = this.startPitch + Random.Range(-this.pitchVariation, this.pitchVariation);
							this.source.PlayOneShot(clip, volume);
						}
					}

					if(this.IsPrefabEnabled)
					{
						FootstepPrefab prefab = effect.GetPrefab(type, "");
						if(prefab != null &&
							prefab.prefab != null)
						{
							this.StartCoroutine(prefab.CreatePrefab(this.transform, foot, hitPosition, hitNormal));
						}
					}

					this.timeout = this.timeBetween;
				}
			}
		}

		/// <summary>
		/// Returns the footstep effect below a foot.
		/// </summary>
		/// <param name="foot">The foot that will be used.</param>
		/// <param name="hitPosition">Stores the ground position (from raycasting).</param>
		/// <param name="hitNormal">Stores the ground normal (from raycasting).</param>
		/// <returns></returns>
		public virtual FootstepEffect FindFootstep(Transform foot, out Vector3 hitPosition, out Vector3 hitNormal)
		{
			hitPosition = foot.position;
			hitNormal = Vector3.up;

			if(this.overrideSources.Count > 0)
			{
				FootstepEffect effect = this.GetOverrideEffect(foot.position);
				if(effect != null)
				{
					RaycastResult result2 = this.Raycast(foot);
					if(result2 != null)
					{
						hitPosition = result2.point;
						hitNormal = result2.normal;
					}
					return effect;
				}
			}

			RaycastResult result = this.Raycast(foot);
			if(result != null)
			{
				hitPosition = result.point;
				hitNormal = result.normal;

				// hit source
				FootstepSource footstepSource = result.transform.GetComponentInParent<FootstepSource>();
				if(footstepSource != null)
				{
					FootstepEffect effect = footstepSource.GetFootstepAt(foot.position, this.effectTag);
					if(effect != null)
					{
						return effect;
					}
				}

				// texture/sprite
				if(FootstepManager.Instance != null)
				{
#if UNITY_2017_2_OR_NEWER
					// tilemap
					if(this.searchTilemaps)
					{
						UnityEngine.Tilemaps.Tilemap[] tilemaps = result.transform.GetComponentsInChildren<UnityEngine.Tilemaps.Tilemap>();
						if(tilemaps != null &&
							tilemaps.Length > 0)
						{
							for(int i = 0; i < tilemaps.Length; i++)
							{
								Sprite sprite = tilemaps[i].GetSprite(tilemaps[i].WorldToCell(result.point));
								if(sprite != null)
								{
									FootstepEffect effect = FootstepManager.Instance.GetFootstepFor(sprite, this.effectTag);
									if(effect != null)
									{
										return effect;
									}
								}
							}
						}
					}
#endif

					// from texture
					if(this.searchRenderers)
					{
						Renderer renderer = result.transform.GetComponentInParent<Renderer>();
						if(renderer != null)
						{
							if(renderer is SpriteRenderer)
							{
								FootstepEffect effect = FootstepManager.Instance.GetFootstepFor(((SpriteRenderer)renderer).sprite, this.effectTag);
								if(effect != null)
								{
									return effect;
								}
							}
							else
							{
								FootstepEffect effect = FootstepManager.Instance.GetFootstepFor(renderer.material.mainTexture, this.effectTag);
								if(effect != null)
								{
									return effect;
								}
							}
						}
					}
				}

				// fallback
				if(this.fallbackMaterial != null)
				{
					return this.fallbackMaterial.GetEffect(this.effectTag);
				}
			}
			// fallback
			else if(this.noRaycastFallback &&
				this.fallbackMaterial != null)
			{
				return this.fallbackMaterial.GetEffect(this.effectTag);
			}

			return null;
		}


		/*
		============================================================================
		Footstep functions
		============================================================================
		*/
		/// <summary>
		/// Called by animation events using the 'Footstep' function name.
		/// Tries to play a footstep effect below a foot (index provided by the event).
		/// Uses the volume and type matching the current horizontal speed.
		/// </summary>
		/// <param name="evt">The animation event calling the function.</param>
		public virtual void Footstep(AnimationEvent evt)
		{
			if(this.IsEnabled)
			{
				if(evt.isFiredByAnimator)
				{
					if(evt.animatorClipInfo.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animatorClipInfo.weight < evt.animatorClipInfo.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepIndex(evt.intParameter);
						}
					}
				}
				else if(evt.isFiredByLegacy)
				{
					if(evt.animationState.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animationState.weight < evt.animationState.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepIndex(evt.intParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tries to play the footstep effect below a foot.
		/// Uses the volume and type matching the current horizontal speed.
		/// </summary>
		/// <param name="index">The index of the foot that will be used.</param>
		public virtual void FootstepIndex(int index)
		{
			if(this.isGrounded &&
				this.IsEnabled &&
				(!this.useSpeedCheck ||
					this.speed.x >= this.minSpeed))
			{
				float volume = this.runVolume;
				FootstepType type = FootstepType.Run;
				this.GetSpeedType(ref volume, ref type);
				this.PlayFootstep(this.GetFoot(index), volume, type, "");
			}
		}

		/// <summary>
		/// Called by animation events using the 'FootstepWalk' function name.
		/// Tries to play a walk footstep effect below a foot (index provided by the event).
		/// </summary>
		/// <param name="evt">The animation event calling the function.</param>
		public virtual void FootstepWalk(AnimationEvent evt)
		{
			if(this.IsEnabled)
			{
				if(evt.isFiredByAnimator)
				{
					if(evt.animatorClipInfo.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animatorClipInfo.weight < evt.animatorClipInfo.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepWalkIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepWalkIndex(evt.intParameter);
						}
					}
				}
				else if(evt.isFiredByLegacy)
				{
					if(evt.animationState.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animationState.weight < evt.animationState.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepWalkIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepWalkIndex(evt.intParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tries to play the walk footstep effect below a foot.
		/// </summary>
		/// <param name="index">The index of the foot that will be used.</param>
		public virtual void FootstepWalkIndex(int index)
		{
			if(this.isGrounded &&
				this.IsEnabled &&
				(!this.useSpeedCheck ||
					(this.speed.x >= this.minSpeed &&
						this.speed.x < this.runSpeed)))
			{
				this.PlayFootstep(this.GetFoot(index),
					this.walkVolume, FootstepType.Walk, "");
			}
		}

		/// <summary>
		/// Called by animation events using the 'FootstepRun' function name.
		/// Tries to play a run footstep effect below a foot (index provided by the event).
		/// </summary>
		/// <param name="evt">The animation event calling the function.</param>
		public virtual void FootstepRun(AnimationEvent evt)
		{
			if(this.IsEnabled)
			{
				if(evt.isFiredByAnimator)
				{
					if(evt.animatorClipInfo.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animatorClipInfo.weight < evt.animatorClipInfo.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepRunIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepRunIndex(evt.intParameter);
						}
					}
				}
				else if(evt.isFiredByLegacy)
				{
					if(evt.animationState.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animationState.weight < evt.animationState.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepRunIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepRunIndex(evt.intParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tries to play the run footstep effect below a foot.
		/// </summary>
		/// <param name="index">The index of the foot that will be used.</param>
		public virtual void FootstepRunIndex(int index)
		{
			if(this.isGrounded &&
				this.IsEnabled &&
				(!this.useSpeedCheck ||
					(this.speed.x >= this.runSpeed &&
						this.speed.x < this.sprintSpeed)))
			{
				this.PlayFootstep(this.GetFoot(index),
					this.runVolume, FootstepType.Run, "");
			}
		}

		/// <summary>
		/// Called by animation events using the 'FootstepSprint' function name.
		/// Tries to play a sprint footstep effect below a foot (index provided by the event).
		/// </summary>
		/// <param name="evt">The animation event calling the function.</param>
		public virtual void FootstepSprint(AnimationEvent evt)
		{
			if(this.IsEnabled)
			{
				if(evt.isFiredByAnimator)
				{
					if(evt.animatorClipInfo.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animatorClipInfo.weight < evt.animatorClipInfo.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepSprintIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepSprintIndex(evt.intParameter);
						}
					}
				}
				else if(evt.isFiredByLegacy)
				{
					if(evt.animationState.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animationState.weight < evt.animationState.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCall = this.FootstepSprintIndex;
								this.highestWeightCustomCall = null;
							}
						}
						else
						{
							this.FootstepSprintIndex(evt.intParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tries to play the sprint footstep effect below a foot.
		/// </summary>
		/// <param name="index">The index of the foot that will be used.</param>
		public virtual void FootstepSprintIndex(int index)
		{
			if(this.isGrounded &&
				this.IsEnabled &&
				(!this.useSpeedCheck ||
					this.speed.x >= this.sprintSpeed))
			{
				this.PlayFootstep(this.GetFoot(index),
					this.runVolume, FootstepType.Sprint, "");
			}
		}


		/*
		============================================================================
		Jump/land functions
		============================================================================
		*/
		/// <summary>
		/// Plays the jump footstep effect below a foot.
		/// </summary>
		/// <param name="index">The foot that will be used.</param>
		public virtual void Jump(int index)
		{
			if(this.IsEnabled)
			{
				this.PlayFootstep(this.GetFoot(index),
					this.jumpVolume, FootstepType.Jump, "");
			}
		}

		/// <summary>
		/// Plays the land footstep effect below a foot.
		/// </summary>
		/// <param name="index">The foot that will be used.</param>
		public virtual void Land(int index)
		{
			if(this.IsEnabled)
			{
				this.PlayFootstep(this.GetFoot(index),
					this.landVolume, FootstepType.Land, "");
			}
		}


		/*
		============================================================================
		Custom effect functions
		============================================================================
		*/
		/// <summary>
		/// Called by animation events using the 'FootstepCustom' function name.
		/// Plays a custom footstep effect below a foot (index and name provided by the event).
		/// </summary>
		/// <param name="evt">The animation event calling the function.</param>
		public virtual void FootstepCustom(AnimationEvent evt)
		{
			if(this.IsEnabled)
			{
				if(evt.isFiredByAnimator)
				{
					if(evt.animatorClipInfo.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animatorClipInfo.weight < evt.animatorClipInfo.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCustomCall = this.FootstepCustomIndex;
								this.highestWeightCall = null;
							}
						}
						else
						{
							this.FootstepCustomIndex(evt.intParameter, evt.stringParameter);
						}
					}
				}
				else if(evt.isFiredByLegacy)
				{
					if(evt.animationState.weight >= this.minAnimationWeight)
					{
						if(this.onlyHighestAnimationWeight)
						{
							if(this.highestWeightEvent == null ||
								this.highestWeightEvent.animationState.weight < evt.animationState.weight)
							{
								this.highestWeightEvent = evt;
								this.highestWeightCustomCall = this.FootstepCustomIndex;
								this.highestWeightCall = null;
							}
						}
						else
						{
							this.FootstepCustomIndex(evt.intParameter, evt.stringParameter);
						}
					}
				}
			}
		}

		/// <summary>
		/// Plays a custom footstep effect below a foot
		/// </summary>
		/// <param name="index">The index of the foot that will be used.</param>
		/// <param name="customName">The name of the custom footstep effect that will be used.</param>
		public virtual void FootstepCustomIndex(int index, string customName)
		{
			if(this.IsEnabled)
			{
				this.PlayFootstep(this.GetFoot(index),
					this.customEffectVolume, FootstepType.Custom, customName);
			}
		}


		/*
		============================================================================
		Raycast functions
		============================================================================
		*/
		/// <summary>
		/// Uses a raycast with the component's raycast settings to find the ground below a foot.
		/// Depending on the raycast mode, uses 2D or 3D raycasting.
		/// </summary>
		/// <param name="index">The foot that will be used.</param>
		/// <returns>The result of the raycast.</returns>
		public virtual RaycastResult Raycast(Transform foot)
		{
			if(RaycastMode.Raycast3D == this.raycastMode)
			{
				return RaycastResult.Raycast3D(
					this.inFootSpace ?
						foot.TransformPoint(this.rayOffset) :
						foot.position + this.transform.rotation * this.rayOffset,
					this.rayDistance, this.layerMask);
			}
			else
			{
				return RaycastResult.Raycast2D(
					this.inFootSpace ?
						foot.TransformPoint(this.rayOffset) :
						foot.position + this.transform.rotation * this.rayOffset,
					this.rayDistance, this.layerMask);
			}
		}


		/*
		============================================================================
		Gizmo functions
		============================================================================
		*/
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			if(this.feet.Count == 0)
			{
				Vector3 position = this.transform.TransformPoint(this.rayOffset);
				RaycastResult result = this.Raycast(this.transform);
				if(result != null)
				{
					Gizmos.DrawLine(position, result.point);
				}
				else
				{
					Gizmos.DrawLine(position, position + Vector3.down * this.rayDistance);
				}
			}
			else
			{
				for(int i = 0; i < this.feet.Count; i++)
				{
					if(this.feet[i] != null)
					{
						Vector3 position = this.inFootSpace ?
							this.feet[i].TransformPoint(this.rayOffset) :
							this.feet[i].position + this.transform.rotation * this.rayOffset;
						RaycastResult result = this.Raycast(this.feet[i]);
						if(result != null)
						{
							Gizmos.DrawLine(position, result.point);
						}
						else
						{
							Gizmos.DrawLine(position, position + Vector3.down * this.rayDistance);
						}
					}
				}
			}
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/Footstepper Icon.png");
		}
	}
}
