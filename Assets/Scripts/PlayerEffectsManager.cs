using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    InputHandler inputHandler;
    PlayerStatsManager playerStatsManager;
    PlayerWeaponSlotManager playerWeaponSlotManager;

    public GameObject currentParticleFX;
    public GameObject instantiatedFXModel;
    public AudioClip potionDrink;

    public int amountToBeHealed;

    private void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        playerStatsManager = GetComponentInParent<PlayerStatsManager>();
        playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
    }
    public void HealPlayerFromEffect()
    {
        playerStatsManager.HealPlayer(amountToBeHealed);
        AudioManager.Instance.PlaySound(potionDrink, instantiatedFXModel);
        GameObject healParticles = Instantiate(currentParticleFX, playerStatsManager.transform);
        Destroy(instantiatedFXModel.gameObject,1f);
        Destroy(healParticles.gameObject, 3f);
        StartCoroutine(waitForTimeToLoadWeaponAndDestroyFX());
    }

    private IEnumerator waitForTimeToLoadWeaponAndDestroyFX()
    {
        yield return new WaitForSeconds(1.5f);
        playerWeaponSlotManager.LoadBothWeaponsOnSlots();
        inputHandler.consumingFlag = false;
    }
}
