using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToStartState : State
{
    public IdleState idleState;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }
    public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        float distanceFromTarget = Vector3.Distance(startingPosition, enemyManager.transform.position);
        enemyManager.navmeshAgent.SetDestination(startingPosition);
        enemyManager.currentTarget = null;
        enemyStats.currentHealth = enemyStats.maxHealth; 

        HandleRotateTowardsTarget(enemyManager);

        if (distanceFromTarget <= enemyManager.maximumAggroRadius)
        {
            enemyManager.transform.rotation = startingRotation;
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0f, Time.deltaTime);
            return idleState;
        }
        else
            return this;

    }

    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = startingPosition - transform.position; 
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navmeshAgent.enabled = true;
            enemyManager.navmeshAgent.SetDestination(startingPosition);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
