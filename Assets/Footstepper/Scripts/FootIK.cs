
using UnityEngine;
using System.Collections.Generic;

namespace GamingIsLove.Footsteps
{
	[AddComponentMenu("Footstepper/Foot IK")]
	public class FootIK : MonoBehaviour
	{
		[Tooltip("The animator that will be used.")]
		public Animator animator;

		[Tooltip("Enables IK foot placement.")]
		[Space(10)]
		public bool enableIK = true;

		[Tooltip("Smooth the transition between IK placement and non-placement (e.g. when the raycast didn't hit the ground).")]
		[Range(0.0f, 1.0f)]
		public float smoothing = 0.1f;


		// right foot
		[Header("Feet Settings")]
		[Tooltip("The IK position weight of the right foot.")]
		[Range(0.0f, 1.0f)]
		public float rightPositionWeight = 1;

		[Tooltip("The IK rotation weight of the right foot.")]
		[Range(0.0f, 1.0f)]
		public float rightRotationWeight = 1;

		[Tooltip("The offset added to the right foot's IK position.")]
		public Vector3 rightOffset = Vector3.zero;

		[Space(10)]
		[Tooltip("The IK position weight of the left foot.")]
		[Range(0.0f, 1.0f)]
		public float leftPositionWeight = 1;

		[Tooltip("The IK rotation weight of the left foot.")]
		[Range(0.0f, 1.0f)]
		public float leftRotationWeight = 1;

		[Tooltip("The offset added to the left foot's IK position.")]
		public Vector3 leftOffset = Vector3.zero;


		// raycast
		[Header("Raycast Settings")]
		[Tooltip("Select if 3D or 2D raycasting is used.")]
		public RaycastMode raycastMode = RaycastMode.Raycast3D;

		[Tooltip("Finding the ground below a foot uses raycasting.\n" +
			"The layer mask defines which layers will be checked.")]
		public LayerMask layerMask = -1;

		[Tooltip("The distance used for raycasting.")]
		public float rayDistance = 0.6f;

		[Tooltip("The offset to the foot's (or game object's) position when raycasting.")]
		public Vector3 rayOffset = new Vector3(0, 0.5f, 0);

		[Tooltip("The offset is added in the local space of the foot, otherwise in local space of this game object.")]
		public bool inFootSpace = false;


		// in-game
		protected Smooth rightSmooth = new Smooth();

		protected Smooth leftSmooth = new Smooth();

		protected virtual void Reset()
		{
			this.animator = this.GetComponent<Animator>();
		}


		/*
		============================================================================
		Raycast functions
		============================================================================
		*/
		public virtual RaycastResult Raycast(Vector3 position, Quaternion rotation)
		{
			if(RaycastMode.Raycast3D == this.raycastMode)
			{
				return RaycastResult.Raycast3D(
					this.inFootSpace ?
						position + rotation * this.rayOffset :
						position + this.transform.rotation * this.rayOffset,
					this.rayDistance, this.layerMask);
			}
			else
			{
				return RaycastResult.Raycast2D(
					this.inFootSpace ?
						position + rotation * this.rayOffset :
						position + this.transform.rotation * this.rayOffset,
					this.rayDistance, this.layerMask);
			}
		}


		/*
		============================================================================
		IK functions
		============================================================================
		*/
		protected virtual void OnAnimatorIK()
		{
			if(this.animator != null)
			{
				if(this.enableIK)
				{
					this.SetIK(AvatarIKGoal.RightFoot, this.rightPositionWeight,
						this.rightRotationWeight, this.rightOffset, this.rightSmooth);
					this.SetIK(AvatarIKGoal.LeftFoot, this.leftPositionWeight,
						this.leftRotationWeight, this.leftOffset, this.leftSmooth);
				}
				else
				{
					this.rightSmooth.target = 0;
					this.leftSmooth.target = 0;
					this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
					this.animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
					this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
					this.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
				}
			}
		}

		protected virtual void SetIK(AvatarIKGoal goal, float positionWeight, float rotationWeight, Vector3 offset, Smooth smooth)
		{
			RaycastResult result = this.Raycast(
				this.animator.GetIKPosition(goal),
				this.animator.GetIKRotation(goal));
			if(result != null)
			{
				smooth.target = 1;
				this.animator.SetIKPosition(goal, result.point + offset);

				if(rotationWeight > 0)
				{
					this.animator.SetIKRotation(goal,
						Quaternion.LookRotation(
							Vector3.ProjectOnPlane(transform.forward, result.normal),
							result.normal));
				}
			}
			else
			{
				smooth.target = 0;
			}

			this.animator.SetIKPositionWeight(goal, positionWeight * smooth.current);
			this.animator.SetIKRotationWeight(goal, rotationWeight * smooth.current);
		}


		/*
		============================================================================
		Smoothing functions
		============================================================================
		*/
		protected virtual void Update()
		{
			this.rightSmooth.Update(this.smoothing);
			this.leftSmooth.Update(this.smoothing);
		}

		public class Smooth
		{
			public float current = 0;

			public float target = 0;

			public float velocity = 0;

			public Smooth()
			{

			}

			public void Update(float smoothTime)
			{
				if(smoothTime > 0)
				{
					this.current = Mathf.SmoothDamp(this.current, this.target, ref this.velocity, smoothTime);
				}
				else
				{
					this.current = this.target;
				}
			}
		}


		/*
		============================================================================
		Gizmo functions
		============================================================================
		*/
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(this.transform.position, "/GamingIsLove/Footsteps/FootIK Icon.png");
		}
	}
}
