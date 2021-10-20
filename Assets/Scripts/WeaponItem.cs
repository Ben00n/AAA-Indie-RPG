using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("Damage")]
    public int baseDamage = 25;
    public float heavyDamageMultiplier = 2;
    public int criticalDamageMultiplier = 4;

    [Header("Absorption")]
    public float physicalDamageAbsorption;

    [Header("Idle Animations")]
    public string right_Hand_Idle;
    public string left_Hand_Idle;
    

    [Header("One Handed Attack Animations")]
    public string OH_Light_Attack_1;
    public string OH_Light_Attack_2;
    public string OH_Light_Attack_3;

    public string OH_Heavy_Attack_1;

    [Header("Dual Attack Animation")]
    public string Dual_Light_Attack_1;
    public string Dual_Light_Attack_2;
    public string Dual_Light_Attack_3;
    public string Dual_Light_Attack_4;

    public string Dual_Heavy_Attack_1;
    public string Dual_Heavy_Attack_2;

    [Header("FX's")]
    public GameObject OH_Light_Attack_1_FX;
    public GameObject OH_Light_Attack_2_FX;
    public GameObject OH_Light_Attack_3_FX;
    public GameObject OH_Heavy_Attack_1_FX;

    [Header("Sounds")]
    public AudioClip OH_Light_Attack_1_Sound;
    public AudioClip OH_Light_Attack_2_Sound;
    public AudioClip OH_Light_Attack_3_Sound;
    public AudioClip OH_Heavy_Attack_1_Sound;

    [Header("Stamina Costs")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;

    [Header("Weapon Type")]
    public bool isMeleeWeapon;

}
