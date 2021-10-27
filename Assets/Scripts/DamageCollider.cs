using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;

    [Header("Poise")]
    public float poiseBreak;
    public float offensivePoiseBonus;

    [Header("Damage")]
    public int currentWeaponDamage = 25;


    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerStatsManager playerStats = collision.GetComponent<PlayerStatsManager>();
            CharacterManager playerCharacterManager = collision.GetComponent<CharacterManager>();
            CharacterEffectsManager playerEffectsManager = collision.GetComponent<CharacterEffectsManager>();
            BlockingCollider shield = collision.transform.GetComponentInChildren<BlockingCollider>();

            if (playerCharacterManager != null)
            {
                if (shield != null && playerCharacterManager.isBlocking)
                {
                    float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption) / 100;

                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block Guard");
                        return;
                    }
                }
            }

            if (playerStats != null)
            {
                // DETECT WHERE ON THE COLLIDER MY WEAPON HIT
                Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                playerEffectsManager.PlayBloodSplatterFX(contactPoint);

                playerStats.poiseResetTimer = playerStats.totalPoiseResetTime;
                playerStats.totalPoiseDefense = playerStats.totalPoiseDefense - poiseBreak;

                if (playerStats.totalPoiseDefense > poiseBreak)
                {
                    playerStats.TakeDamageNoAnimation(currentWeaponDamage);
                }
                else
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }
        }

        if (collision.tag == "Enemy")
        {
            EnemyStatsManager enemyStats = collision.GetComponent<EnemyStatsManager>();
            PlayerCombatManager playerAttacker = GetComponentInParent<PlayerCombatManager>();
            CharacterEffectsManager enemyEffectsManager = collision.GetComponent<CharacterEffectsManager>();
            PlayerInventoryManager playerInventory = GetComponentInParent<PlayerInventoryManager>();


            float heavyAttackDamage = currentWeaponDamage * playerInventory.rightWeapon.heavyDamageMultiplier;

            // DETECT WHERE ON THE COLLIDER MY WEAPON HIT
            Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            enemyEffectsManager.PlayBloodSplatterFX(contactPoint);

            if (enemyStats != null)
            {
                enemyStats.poiseResetTimer = enemyStats.totalPoiseResetTime;
                enemyStats.totalPoiseDefense = enemyStats.totalPoiseDefense - poiseBreak;

                if (enemyStats.totalPoiseDefense > poiseBreak)
                {
                    enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                }
                else
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }

              /*  if (playerAttacker.lastAttack == "OH_Magic_Sword_Heavy_01" || playerAttacker.lastAttack == "OH_Basic_Sword_Heavy_01" ||
                     playerAttacker.lastAttack == "OH_Heavy_Attack_01" || playerAttacker.lastAttack == "OH_Darkness_Sword_Heavy_01" ||
                     playerAttacker.lastAttack == "OH_Heavy_Attack_Blood_01")
                {
                    enemyStats.TakeDamage(Mathf.RoundToInt(heavyAttackDamage));
                }
                else
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }*/

            }
        }

        if (collision.tag == "Illusionary Wall")
        {
            IllusionaryWall illusionaryWall = collision.GetComponent<IllusionaryWall>();

            illusionaryWall.wallHasBeenHit = true;
        }
    }
}
