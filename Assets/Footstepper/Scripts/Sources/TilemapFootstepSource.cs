
#if UNITY_2017_2_OR_NEWER
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[AddComponentMenu("Footstepper/Tilemap Footstep Source")]
	public class TilemapFootstepSource : FootstepSource
	{
		[Tooltip("The tilemap that will be used.")]
		public Tilemap[] tilemap;

		[Tooltip("The texture materials define the footstep effects (audio clips and prefabs) that are linked to the tilemap's sprites.\n" +
			"The sprite of the tile at the position is used to find the correct effect.")]
		public List<FootstepTextureMaterial> textureMaterials = new List<FootstepTextureMaterial>();

		protected virtual void Reset()
		{
			this.tilemap = this.GetComponentsInChildren<Tilemap>();
		}

		/// <summary>
		/// Returns the footstep effect for a provided position and effect tag.
		/// Checks for sprite of a tile at the position.
		/// </summary>
		/// <param name="position">The position to check for.</param>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public override FootstepEffect GetFootstepAt(Vector3 position, string effectTag)
		{
			if(this.tilemap != null &&
				this.tilemap.Length > 0)
			{
				Sprite sprite = this.GetSpriteAt(position);
				if(sprite != null)
				{
					for(int i = 0; i < this.textureMaterials.Count; i++)
					{
						FootstepEffect effect = this.textureMaterials[i].GetEffect(sprite, effectTag);
						if(effect != null)
						{
							return effect;
						}
					}

					if(FootstepManager.Instance != null)
					{
						return FootstepManager.Instance.GetFootstepFor(sprite, effectTag);
					}
				}
			}
			return null;
		}

		public virtual Sprite GetSpriteAt(Vector3 position)
		{
			for(int i = 0; i < this.tilemap.Length; i++)
			{
				Sprite sprite = this.tilemap[i].GetSprite(this.tilemap[i].WorldToCell(position));
				if(sprite != null)
				{
					return sprite;
				}
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
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/TilemapFootstepSource Icon.png");
		}
	}
}
#endif
