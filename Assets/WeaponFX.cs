using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFX : MonoBehaviour
{
    [Header("Header FX")]
    public ParticleSystem normalWeaponTrail;

    public void PlayWeaponFX()
    {
        /*normalWeaponTrail.Stop();

        if(normalWeaponTrail.isStopped)
        {
            normalWeaponTrail.Play();
        }*/
        normalWeaponTrail.Play();
    }
}
