using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingCollider : MonoBehaviour
{
    public BoxCollider blockingCollider;

    public float blockingPhysicalDamageAbsorption;

    private void Awake()
    {
        blockingCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderDamageAbsorption(WeaponItem weapon)
    {
        if(weapon != null)
        {
            blockingPhysicalDamageAbsorption = weapon.physicalDamageAbsorption;
        }
    }

    public void EnableBlockingCollider()
    {
        blockingCollider.enabled = true;
    }

    public void DisableBlockingCollider()
    {
        blockingCollider.enabled = false;
    }
}
