using MalbersAnimations.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Effect Modifiers/FireBall")]
    public class FireBallEffectM : EffectModifier
    {
        public float Power = 20;
        Rigidbody rb;

        public override void AwakeEffect(Effect effect) { }

        public override void StartEffect(Effect effect)
        {
            var aim = effect.Owner.GetComponent<LookAt>();                //Check if the owner has lookAt

            effect.Instance.SendMessage("SetOwner", effect.Owner, SendMessageOptions.DontRequireReceiver);

            var Direction = (aim != null && aim.IsAiming) ? aim.AimDirection.normalized : effect.Owner.transform.forward;

            var projectile = effect.Instance.GetComponent<IProjectile>();

            if (projectile != null)
            {
                projectile.Fire(Direction * Power);
            }
            else
            {
                rb = effect.Instance.GetComponent<Rigidbody>();         //Get the riggidbody of the effect
                rb.AddForce(Direction * Power);     //If it has look at take the direction from te lookat
            }
        }
    }
}