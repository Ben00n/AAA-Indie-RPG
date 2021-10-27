using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsManager : CharacterStatsManager
{
    public GameObject particlesOnDeath;
    private GameObject instantiatedObj;
    public GameObject gameObjectForQuest;
    private float time = 1.8f;

    public AudioClip getHitSound;

    EnemyManager enemyManager;
    WorldEventManager worldEventManager;
    EventColliderBeginBossFight eventCollider;
    EnemyAnimatorManager enemyAnimatorManager;
    EnemyBossManager enemyBossManager;
    public UIEnemyHealthBar enemyHealthBar;

    public bool isBoss;

    private void Awake()
    {
        worldEventManager = FindObjectOfType<WorldEventManager>();
        eventCollider = FindObjectOfType<EventColliderBeginBossFight>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyBossManager = GetComponent<EnemyBossManager>();
        enemyManager = GetComponent<EnemyManager>();
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if(!isBoss)
        {
            enemyHealthBar.SetMaxHealth(maxHealth);
        }
    }

    public override void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer = poiseResetTimer - Time.deltaTime;
        }
        else if (poiseResetTimer <= 0 && !enemyManager.isInteracting)
        {
            totalPoiseDefense = armorPoiseBonus;
        }
    }

    private float SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public override void TakeDamageNoAnimation(int damage) //used for backstab
    {
        if (isDead)
            return;

        base.TakeDamageNoAnimation(damage);

        if(!isBoss)
        {
            enemyHealthBar.SetHealth(currentHealth);
        }
        else if(isBoss && enemyBossManager != null)
        {
            enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
        }

        AudioManager.Instance.PlaySound(getHitSound, gameObject);

        if (currentHealth <= 0)
        {
            HandleDeathNoAnim();
        }
    }

    public override void TakeDamage(int damage,string damageAnimation = "Damage_01")
    {
        if (isDead)
            return;

        base.TakeDamage(damage, damageAnimation = "Damage_01");

        if (!isBoss)
        {
            enemyHealthBar.SetHealth(currentHealth);
        }
        else if (isBoss && enemyBossManager != null)
        {
            enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
        }

        enemyAnimatorManager.PlayTargetAnimation(damageAnimation,true); //get hit anim disabled due to cancel animation
        AudioManager.Instance.PlaySound(getHitSound, gameObject);

        if (currentHealth <= 0)
        {
            HandleDeath();
            if(isBoss)
            {
                Destroy(eventCollider.gameObject);
                worldEventManager.BossHasBeenDefeated();
            }
        }
    }

    private void HandleDeath()
    {
        enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
        StartCoroutine(Destroy());
    }

    private void HandleDeathNoAnim()
    {
        StartCoroutine(Destroy());
    }

    public IEnumerator Destroy()
    {
        Destroy(gameObjectForQuest);
        yield return new WaitForSeconds(7f);
        instantiatedObj = (GameObject)Instantiate(particlesOnDeath, transform.position - new Vector3(0f,0.6f,0f), transform.rotation);
        Destroy(instantiatedObj, time);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
