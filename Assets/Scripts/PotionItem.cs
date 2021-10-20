using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables")]
public class PotionItem : ConsumableItem
{
    [Header("Potion Type")]
    public bool healthPotion;

    [Header("Recovery Amount")]
    public int healthRecoveryAmount;

    [Header("Recovery FX")]
    public GameObject recoveryFX;

    public override void AttemptToConsumeItem(PlayerAnimatorManager playerAnimatorManager,PlayerWeaponSlotManager weaponSlotManager,PlayerEffectsManager playerEffectsManager)
    {
        base.AttemptToConsumeItem(playerAnimatorManager, weaponSlotManager, playerEffectsManager);
        GameObject potion = Instantiate(itemModel, weaponSlotManager.rightHandSlot.transform);
        playerEffectsManager.currentParticleFX = recoveryFX;
        playerEffectsManager.amountToBeHealed = healthRecoveryAmount;
        playerEffectsManager.instantiatedFXModel = potion;
        weaponSlotManager.rightHandSlot.UnloadWeapon();
    }
}
