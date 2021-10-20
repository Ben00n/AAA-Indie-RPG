
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[CreateAssetMenu(fileName = "New FootstepTextureMaterial", menuName = "Footstepper/Footstep Texture Material")]
	public class FootstepTextureMaterial : ScriptableObject
	{
		[Tooltip("Set up which footstep material will be used for which textures to allow terrains to find the correct effects.")]
		public List<FootstepTextureEffect> textureData = new List<FootstepTextureEffect>();

		/// <summary>
		/// Loads textures from a terrain and adds them to the texture data.
		/// This will remove any previously added data!
		/// </summary>
		/// <param name="terrain">The terrain that will be used.</param>
		public virtual void LoadFromTerrain(Terrain terrain)
		{
			if(terrain != null)
			{
				this.textureData.Clear();

#if UNITY_2018_3_OR_NEWER
				for(int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
				{
					Texture texture = terrain.terrainData.terrainLayers[i].diffuseTexture;
					if(texture != null)
					{
						this.textureData.Add(new FootstepTextureEffect(texture));
					}
				}
#else
				for(int i = 0; i < terrain.terrainData.splatPrototypes.Length; i++)
				{
					Texture texture = terrain.terrainData.splatPrototypes[i].texture;
					if(texture != null)
					{
						this.textureData.Add(new FootstepTextureEffect(texture));
					}
				}
#endif
			}
		}

		/// <summary>
		/// Returns the footstep effect for a provided texture and effect tag.
		/// </summary>
		/// <param name="texture">The texture to check for.</param>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public virtual FootstepEffect GetEffect(Texture texture, string effectTag)
		{
			for(int i = 0; i < this.textureData.Count; i++)
			{
				if(this.textureData[i].Contains(texture))
				{
					return this.textureData[i].GetEffect(effectTag);
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the footstep effect for a provided sprite and effect tag.
		/// </summary>
		/// <param name="sprite">The sprite to check for.</param>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public virtual FootstepEffect GetEffect(Sprite sprite, string effectTag)
		{
			for(int i = 0; i < this.textureData.Count; i++)
			{
				if(this.textureData[i].Contains(sprite))
				{
					return this.textureData[i].GetEffect(effectTag);
				}
			}
			return null;
		}
	}
}
