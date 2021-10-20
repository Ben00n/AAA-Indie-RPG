
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	public enum RaycastMode { Raycast3D, Raycast2D };

	public class RaycastResult
	{
		public Transform transform;

		public Vector3 point;

		public Vector3 normal;

		public RaycastResult(Transform transform, Vector3 point, Vector3 normal)
		{
			this.transform = transform;
			this.point = point;
			this.normal = normal;
		}

		public static RaycastResult Raycast3D(Vector3 position, float distance, LayerMask layerMask)
		{
			RaycastHit hitInfo;
			if(Physics.Raycast(new Ray(position, Vector3.down), out hitInfo, distance, layerMask))
			{
				return new RaycastResult(hitInfo.transform, hitInfo.point, hitInfo.normal);
			}
			return null;
		}

		public static RaycastResult Raycast2D(Vector3 position, float distance, LayerMask layerMask)
		{
			RaycastHit2D hitInfo = Physics2D.Raycast(position, Vector2.down, distance, layerMask);
			if(hitInfo.collider != null)
			{
				return new RaycastResult(hitInfo.transform, hitInfo.point, hitInfo.normal);
			}
			return null;
		}
	}
}
