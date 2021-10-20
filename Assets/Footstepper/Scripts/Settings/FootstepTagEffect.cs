
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[System.Serializable]
	public class FootstepTagEffect
	{
		[Tooltip("The effect tag of this footstep effect - is used by footsteppers with a matching effect tag.")]
		public string tag = "";

		[Tooltip("The footstep effect of this effect tag.")]
		public FootstepEffect effect = new FootstepEffect();

		public FootstepTagEffect()
		{

		}
	}
}