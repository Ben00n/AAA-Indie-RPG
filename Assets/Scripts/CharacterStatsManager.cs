using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    public int healthLevel = 10;
    public float maxHealth;
    public float currentHealth;

    public int staminaLevel = 10;
    public float maxStamina;
    public float currentStamina;

    public int focusLevel = 10;
    public float maxFocusPoints;
    public float currentFocusPoints;

    [Header("Poise")]
    public float totalPoiseDefense; // total poise after calculation
    public float offensivePoiseBonus; //The poise you gain during an attack with weapon
    public float armorPoiseBonus; // the poise you gain from wearing your armor
    public float totalPoiseResetTime = 15;
    public float poiseResetTimer = 0;

    [Header("Armor Absorptions")]
    public float physicalDamageAbsorptionHead;
    public float physicalDamageAbsorptionShoulders;
    public float physicalDamageAbsorptionTorso;
    public float physicalDamageAbsorptionHands;
    public float physicalDamageAbsorptionHips;
    public float physicalDamageAbsorptionShoes;
    public float physicalDamageAbsorptionBack;

    public int experiencePoints = 0;
    public int experienceAwardedOnDeath = 50;

    public bool isDead;

    protected virtual void Update()
    {
        HandlePoiseResetTimer();
    }

    private void Start()
    {
        totalPoiseDefense = armorPoiseBonus;
    }

    public virtual void TakeDamage(int physicalDamage, string damageAnimation = "Damage_01")
    {
        if (isDead)
            return;

        float totalPhysicalDamageAbsorption = 1 -
            (1 - physicalDamageAbsorptionHead / 100) *
            (1 - physicalDamageAbsorptionShoulders / 100) *
            (1 - physicalDamageAbsorptionTorso / 100) *
            (1 - physicalDamageAbsorptionHands / 100) *
            (1 - physicalDamageAbsorptionHips / 100) *
            (1 - physicalDamageAbsorptionShoes / 100) *
            (1 - physicalDamageAbsorptionBack / 100);

        physicalDamage = Mathf.RoundToInt(physicalDamage - (physicalDamage * totalPhysicalDamageAbsorption));

        Debug.Log("Total Damage Absorption is " + totalPhysicalDamageAbsorption + "%");

        float finalDamage = physicalDamage; // + firedamage + magicdamage ....

        currentHealth = Mathf.RoundToInt(currentHealth - finalDamage);

        Debug.Log("Total Damage Dealt is " + finalDamage);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }

    public virtual void HandlePoiseResetTimer()
    {
        if(poiseResetTimer > 0)
        {
            poiseResetTimer = poiseResetTimer - Time.deltaTime;
        }
        else
        {
            totalPoiseDefense = armorPoiseBonus;
        }
    }

    public virtual void TakeDamageNoAnimation(int damage)
    {
        currentHealth = currentHealth - damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }
}
