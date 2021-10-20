using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    PlayerAnimatorManager playerAnimatorManager;
    PlayerEquipmentManager playerEquipmentManager;
    PlayerManager playerManager;
    PlayerStatsManager playerStatsManager;
    PlayerInventoryManager playerInventoryManager;
    InputHandler inputHandler;
    PlayerWeaponSlotManager playerWeaponSlotManager;
    PlayerEffectsManager playerEffectsManager;

    public string lastAttack;

    LayerMask backStabLayer = 1 << 12;

    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerManager = GetComponent<PlayerManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (playerStatsManager.currentStamina <= 0)
            return;

        if (inputHandler.comboFlag)
        {
            playerAnimatorManager.animator.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OH_Light_Attack_1)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                lastAttack = weapon.OH_Light_Attack_2;
                GameObject instantiatedFX = Instantiate(weapon.OH_Light_Attack_2_FX, playerAnimatorManager.transform);
                AudioManager.Instance.PlaySound(weapon.OH_Light_Attack_2_Sound, gameObject);
                Destroy(instantiatedFX, 3f);
            }
            else if (lastAttack == weapon.OH_Light_Attack_2)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_3, true);
                lastAttack = weapon.OH_Light_Attack_3;
                GameObject instantiatedFX = Instantiate(weapon.OH_Light_Attack_3_FX, playerAnimatorManager.transform);
                AudioManager.Instance.PlaySound(weapon.OH_Light_Attack_3_Sound, gameObject);
                Destroy(instantiatedFX, 3f);
            }
        }
    }

    public void HandleDualWeaponCombo(WeaponItem weapon)
    {
        if (playerStatsManager.currentStamina <= 0)
            return;

        if (inputHandler.comboFlag)
        {
            playerAnimatorManager.animator.SetBool("canDoCombo", false);
            if (lastAttack == weapon.Dual_Light_Attack_1)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Light_Attack_2, true);
                lastAttack = weapon.Dual_Light_Attack_2;
            }
            else if (lastAttack == weapon.Dual_Light_Attack_2)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Light_Attack_3, true);
                lastAttack = weapon.Dual_Light_Attack_3;
            }
            else if(lastAttack == weapon.Dual_Light_Attack_3)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Light_Attack_4, true);
                lastAttack = weapon.Dual_Light_Attack_4;
            }
            else if(lastAttack == weapon.Dual_Heavy_Attack_1)
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Heavy_Attack_2, true);
                lastAttack = weapon.Dual_Heavy_Attack_2;
            }
        }
    }

    public void HandleDualAttack(WeaponItem weapon)
    {
        if (playerStatsManager.currentStamina <= 0)
            return;

        playerWeaponSlotManager.attackingWeapon = weapon;
        playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Light_Attack_1, true);
        lastAttack = weapon.Dual_Light_Attack_1;
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        if (playerStatsManager.currentStamina <= 0)
            return;

        playerWeaponSlotManager.attackingWeapon = weapon;
        playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
        lastAttack = weapon.OH_Light_Attack_1;
        GameObject instantiatedFX = Instantiate(weapon.OH_Light_Attack_1_FX, playerAnimatorManager.transform);
        AudioManager.Instance.PlaySound(weapon.OH_Light_Attack_1_Sound, gameObject);
        Destroy(instantiatedFX, 3f);
    }


    public void HandleDualHeavyAttack(WeaponItem weapon)
    {
        if (playerManager.isInteracting)
            return;

        if (playerStatsManager.currentStamina <= 0)
            return;

        playerWeaponSlotManager.attackingWeapon = weapon;
        playerAnimatorManager.PlayTargetAnimation(weapon.Dual_Heavy_Attack_1, true);
        lastAttack = weapon.Dual_Heavy_Attack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (playerManager.isInteracting)
            return;

        if (playerStatsManager.currentStamina <= 0)
            return;

        playerWeaponSlotManager.attackingWeapon = weapon;
        playerAnimatorManager.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
        lastAttack = weapon.OH_Heavy_Attack_1;
        GameObject instantiatedFX = Instantiate(weapon.OH_Heavy_Attack_1_FX, playerAnimatorManager.transform);
        AudioManager.Instance.PlaySound(weapon.OH_Heavy_Attack_1_Sound, gameObject);
        Destroy(instantiatedFX, 3f);
    }

    #region Input Actions
    public void HandleRBAction()
    {
        if(playerInventoryManager.rightWeapon.isMeleeWeapon && playerInventoryManager.leftWeapon.isUnarmed)
        {
            PerformRBMeleeAction();
        }
    }

    public void HandleRTAction()
    {
        if(playerInventoryManager.rightWeapon.isMeleeWeapon && playerInventoryManager.leftWeapon.isUnarmed)
        {
            PerformRTMeleeAction();
        }
    }

    public void HandleDualAction()
    {
        if(playerInventoryManager.rightWeapon.isMeleeWeapon && playerInventoryManager.leftWeapon.isMeleeWeapon)
        {
            PerformDualMeleeAction();
        }
    }

    public void HandleDualHeavyAction()
    {
        if (playerInventoryManager.rightWeapon.isMeleeWeapon && playerInventoryManager.leftWeapon.isMeleeWeapon)
        {
            PerformRTDualAction();
        }
    }

    public void HandleQAction()
    {
        PerformQBlockAction();
    }

    #endregion

    #region Attack Actions

    private void PerformDualMeleeAction()
    {
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleDualWeaponCombo(playerInventoryManager.rightWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;
            if (playerManager.canDoCombo)
                return;

            if(playerInventoryManager.rightWeapon != playerInventoryManager.rightWeapon.isUnarmed && playerInventoryManager.leftWeapon != playerInventoryManager.leftWeapon.isUnarmed)
            {
                playerAnimatorManager.animator.SetBool("isUsingRightHand", true);
                playerAnimatorManager.animator.SetBool("isUsingLeftHand", true);
                HandleDualAttack(playerInventoryManager.rightWeapon);
            }
        }
    }
    private void PerformRBMeleeAction()
    {
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventoryManager.rightWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;
            if (playerManager.canDoCombo)
                return;

            if (playerInventoryManager.rightWeapon != playerInventoryManager.rightWeapon.isUnarmed)
            {
                playerAnimatorManager.animator.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerInventoryManager.rightWeapon);
            }
        }
        playerEffectsManager.PlayWeaponFX(false);
    }

    private void PerformRTDualAction()
    {
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleDualWeaponCombo(playerInventoryManager.rightWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;
            if (playerManager.canDoCombo)
                return;

            if (playerInventoryManager.rightWeapon != playerInventoryManager.rightWeapon.isUnarmed && playerInventoryManager.leftWeapon != playerInventoryManager.leftWeapon.isUnarmed)
            {
                playerAnimatorManager.animator.SetBool("isUsingRightHand", true);
                playerAnimatorManager.animator.SetBool("isUsingLeftHand", true);
                HandleDualHeavyAttack(playerInventoryManager.rightWeapon);
            }
        }
    }
    private void PerformRTMeleeAction()
    {
        if (playerManager.isInteracting)
            return;

        playerAnimatorManager.animator.SetBool("isUsingRightHand", true);
        HandleHeavyAttack(playerInventoryManager.rightWeapon);
    }

    #region Defense Actions
    private void PerformQBlockAction()
    {
        if (playerManager.isInteracting)
            return;

        if (playerManager.isBlocking)
            return;

        playerAnimatorManager.PlayTargetAnimation("Block", false, true);
        playerEquipmentManager.OpenBlockingCollider();
        playerManager.isBlocking = true;
    }
    #endregion

    public void AttemptBackStab()
    {
        if (playerInventoryManager.rightWeapon.isUnarmed)
            return;

        RaycastHit hit;

        if(Physics.Raycast(inputHandler.backStabRayCastStartPoint.position,transform.TransformDirection(Vector3.forward),out hit, 0.5f, backStabLayer))
        {
            EnemyStatsManager enemyStats = hit.transform.gameObject.GetComponentInParent<EnemyStatsManager>();
            CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
            DamageCollider rightWeapon = playerWeaponSlotManager.rightHandDamageCollider;

            if (enemyStats.isDead)
                return;

            if (enemyCharacterManager != null)
            {
                playerManager.transform.position = enemyCharacterManager.backStabCollider.backStabberStandPoint.position;

                Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                rotationDirection = hit.transform.position - playerManager.transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                playerManager.transform.rotation = targetRotation;

                int criticalDamage = playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.currentWeaponDamage;
                enemyCharacterManager.pendingDamage = criticalDamage;

                playerAnimatorManager.PlayTargetAnimation("Backstab", true);
                enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Back_Stabbed", true);
            }
        }
    }

    #endregion
}
