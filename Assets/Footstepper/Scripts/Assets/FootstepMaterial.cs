
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[CreateAssetMenu(fileName = "New FootstepMaterial", menuName = "Footstepper/Footstep Material")]
	public class FootstepMaterial : ScriptableObject
	{
		[Tooltip("The default effect is used when the footstepper doesn't use an effect tag or no matching tag is defined in the tag effects.")]
		public FootstepEffect defaultEffect = new FootstepEffect();

		[Tooltip("The effect of the matching tag is used if the footstepper defined an effect tag.\n" +
			"You can use tags to create different effects for e.g. 'heavy' and 'light' footsteppers.\n" +
			"Tags must be unique, you can't define effects with the same tag.")]
		public List<FootstepTagEffect> tagEffects = new List<FootstepTagEffect>();


		// in-game
		private Dictionary<string, FootstepEffect> lookup;

		/// <summary>
		/// Returns the footstep effect for a provided effect tag.
		/// </summary>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public virtual FootstepEffect GetEffect(string effectTag)
		{
			if(this.tagEffects.Count > 0)
			{
				FootstepEffect effect;
				if(this.Lookup.TryGetValue(effectTag, out effect))
				{
					return effect;
				}
			}
			return this.defaultEffect;
		}

		protected virtual Dictionary<string, FootstepEffect> Lookup
		{
			get
			{
				if(this.lookup == null)
				{
					this.lookup = new Dictionary<string, FootstepEffect>();
					for(int i = 0; i < this.tagEffects.Count; i++)
					{
						this.lookup.Add(this.tagEffects[i].tag, this.tagEffects[i].effect);
					}
				}
				return this.lookup;
			}
		}
	}
}
