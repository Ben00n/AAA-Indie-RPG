
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[System.Serializable]
	public class FootstepTextureEffect
	{
		[Tooltip("Select the textures that will use the defined footstep material.")]
		public List<Texture> texture = new List<Texture>();

		[Tooltip("Select the sprites that will use the defined footstep material.")]
		public List<Sprite> sprite = new List<Sprite>();

		[Space(10)]
		[Tooltip("The footstep material defines the footstep effects (audio clips and prefabs) for these textures.")]
		public FootstepMaterial material;

		public FootstepTextureEffect()
		{

		}

		public FootstepTextureEffect(Texture texture)
		{
			this.texture.Add(texture);
		}

		/// <summary>
		/// Checks if a provided texture is included.
		/// </summary>
		/// <param name="texture">The texture to check for.</param>
		/// <returns>true if the texture is included.</returns>
		public virtual bool Contains(Texture texture)
		{
			return this.texture.Contains(texture);
		}

		/// <summary>
		/// Checks if a provided sprite is included.
		/// </summary>
		/// <param name="sprite">The sprite to check for.</param>
		/// <returns>true if the sprite is included.</returns>
		public virtual bool Contains(Sprite sprite)
		{
			return this.sprite.Contains(sprite);
		}

		/// <summary>
		/// Returns the footstep effect for a provided effect tag.
		/// </summary>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public virtual FootstepEffect GetEffect(string effectTag)
		{
			if(this.material != null)
			{
				return this.material.GetEffect(effectTag);
			}
			return null;
		}
	}
}