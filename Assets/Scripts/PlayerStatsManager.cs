using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{

    PlayerManager playerManager;
    HealthBar healthBar;
    StaminaBar staminaBar;
    FocusPointBar focusPointsBar;
    PlayerAnimatorManager playerAnimatorManager;

    public float healthRegenerationAmount = 0.5f;
    public float healthRegenTimer = 0;

    public float staminaRegenerationAmount = 30;
    public float staminaRegenTimer = 0;

    public float focusRegenerationAmount = 1;
    public float focusRegenTimer = 0;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        healthBar = FindObjectOfType<HealthBar>();
        staminaBar = FindObjectOfType<StaminaBar>();
        focusPointsBar = FindObjectOfType<FocusPointBar>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);

        maxFocusPoints = SetMaxFocusFromFocusLevel();
        currentFocusPoints = maxFocusPoints;
        focusPointsBar.SetMaxFocus(maxFocusPoints);
        focusPointsBar.SetCurrentFocus(currentFocusPoints);
    }

    public override void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer = poiseResetTimer - Time.deltaTime;
        }
        else if(poiseResetTimer <= 0 && !playerManager.isInteracting)
        {
            totalPoiseDefense = armorPoiseBonus;
        }
    }

    private float SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private float SetMaxStaminaFromStaminaLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
    }

    private float SetMaxFocusFromFocusLevel()
    {
        maxFocusPoints = focusLevel * 10;
        return maxFocusPoints;
    }

    public override void TakeDamage(int damage, string damageAnimation = "Damage_01")
    {
        if (playerManager.isInvulnerable)
            return;

        base.TakeDamage(damage, damageAnimation = "Damage_01");
        healthBar.SetCurrentHealth(currentHealth);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            playerAnimatorManager.PlayTargetAnimation("Dead_02", true);
            isDead = true;
            //HandlePlayerDeath
        }
        else
        {
            playerAnimatorManager.PlayTargetAnimation(damageAnimation, true);
        }
    }

    public override void TakeDamageNoAnimation(int damage)
    {
        base.TakeDamageNoAnimation(damage);
        healthBar.SetCurrentHealth(currentHealth);
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void RegenerateStamina()
    {
        if(playerManager.isInteracting)
        {
            staminaRegenTimer = 0;
        }
        else
        {
            staminaRegenTimer += Time.deltaTime;

            if (currentStamina < maxStamina && staminaRegenTimer > 1f)
            {
                currentStamina += staminaRegenerationAmount * Time.deltaTime;
                staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
            }
        }
    }

    public void RegenerateHealth()
    {
        if(playerManager.isInteracting)
        {
            healthRegenTimer = 0;
        }
        else
        {
            healthRegenTimer += Time.deltaTime;
            if (currentHealth < maxHealth && healthRegenTimer > 2f)
            {
                currentHealth += healthRegenerationAmount * Time.deltaTime;
                healthBar.SetCurrentHealth(Mathf.RoundToInt(currentHealth));
            }
        }
    }

    public void RegenerateFocus()
    {
        if (playerManager.isInteracting)
        {
            focusRegenTimer = 0;
        }
        else
        {
            focusRegenTimer += Time.deltaTime;
            if (currentFocusPoints < maxFocusPoints && focusRegenTimer > 1f)
            {
                currentFocusPoints += focusRegenerationAmount * Time.deltaTime;
                focusPointsBar.SetCurrentFocus(Mathf.RoundToInt(currentFocusPoints));
            }
        }
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth = currentHealth + healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;

        }
        healthBar.SetCurrentHealth(currentHealth);
    }

    public void DeductFocusPoints(int focusPoints)
    {
        currentFocusPoints = currentFocusPoints - focusPoints;

        if(currentFocusPoints < 0)
        {
            currentFocusPoints = 0;
        }

        focusPointsBar.SetCurrentFocus(currentFocusPoints);
    }

    public void AddExperience(int experience)
    {
        experiencePoints = experiencePoints + experience;
    }

}
