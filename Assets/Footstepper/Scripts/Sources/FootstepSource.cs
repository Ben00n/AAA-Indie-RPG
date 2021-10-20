
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	public abstract class FootstepSource : MonoBehaviour
	{
		/// <summary>
		/// Returns the footstep effect for a provided position and effect tag.
		/// </summary>
		/// <param name="position">The position to check for.</param>
		/// <param name="effectTag">The effect tag to check for.</param>
		/// <returns>The found footstep effect.</returns>
		public abstract FootstepEffect GetFootstepAt(Vector3 position, string effectTag);
	}
}
