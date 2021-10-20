using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyManager enemyManager;
    EnemyEffectsManager enemyEffectsManager;
    EnemyBossManager enemyBossManager;

    protected override void Awake()
    {
        base.Awake();
        animator.GetComponent<Animator>();
        enemyManager = GetComponent<EnemyManager>();
        enemyEffectsManager = GetComponent<EnemyEffectsManager>();
        enemyBossManager = GetComponent<EnemyBossManager>();
    }

    public void AwardExperienceOnDeath()
    {
        PlayerStatsManager playerStats = FindObjectOfType<PlayerStatsManager>();
        ExperienceCountBar experienceCountBar = FindObjectOfType<ExperienceCountBar>();

        if (playerStats != null)
        {
            playerStats.AddExperience(characterStatsManager.experienceAwardedOnDeath);

            if (experienceCountBar != null)
            {
                experienceCountBar.SetExperienceCountText(playerStats.experiencePoints);
            }
        }
    }

    public void InstantiateBossParticleFX()
    {
        BossFXTransform bossFxTransform = GetComponentInChildren<BossFXTransform>();

        GameObject phaseFX = Instantiate(enemyBossManager.particleFX, bossFxTransform.transform);
    }

    public void PlayWeaponTrailFX()
    {
        enemyEffectsManager.PlayWeaponFX(false);
    }


    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyManager.enemyRigidBody.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyManager.enemyRigidBody.velocity = velocity;

        if (enemyManager.isRotatingWithRootMotion)
        {
            enemyManager.transform.rotation *= animator.deltaRotation;
        }
    }

}
