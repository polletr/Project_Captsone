using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState 
{
    protected Vector3 chasePos;

    protected float time;
    protected float timer;

    public EnemyAnimator enemyAnimator { get; private set; }
    public EnemyClass enemy { get; set; }
    
    public EnemyBaseState(EnemyClass enemyClass,EnemyAnimator enemyAnim)
    {
        enemy = enemyClass;
        enemyAnimator = enemyAnim;
    }
    

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void StateFixedUpdate() { }
    public virtual void StateUpdate() { }
    public virtual void HandleAttack() { }
    public virtual void HandleStun() 
    {
        enemy.ChangeState(enemy.StunState);
    }

    public virtual void HandleParalise() 
    {
        if (!enemy.Paralised)
        {
            enemy.Paralised = true;
            enemy.ChangeState(enemy.ParalisedState);
        }

    }

    public virtual void HandleChase()
    {
        enemy.ChangeState(enemy.ChaseState);
    }

    protected virtual void OnSoundDetected(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(enemy.transform.position, soundPosition);
        if (distance <= soundRange * enemy.HearingMultiplier)
        {
            NavMeshHit navHit;
            // Check if there's a valid NavMesh path to the sound position
            if (NavMesh.SamplePosition(soundPosition, out navHit, 1.0f, NavMesh.AllAreas))
            {
                // If the path is clear, proceed with chasing the player
                if (NavMesh.Raycast(enemy.transform.position, soundPosition, out NavMeshHit navMHit, NavMesh.AllAreas))
                {
                    if (navMHit.mask == 0) // No obstacles found
                    {
                        chasePos = soundPosition;
                        enemy.ChangeState(enemy.ChaseState);
                    }
                }
            }
        }
    }

    protected virtual void VisionDetection()
    {
        float detectionRadius = enemy.SightRange;

        // Detect all targets within the detection radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(enemy.transform.position, detectionRadius);

        // If the enemy already sees the player, stop checking
        if (enemy.playerCharacter != null)
        {
            return;
        }

        foreach (Collider target in targetsInViewRadius)
        {
            // Perform a raycast to ensure there are no obstacles between the enemy and the target
            Vector3 directionToTarget = (target.transform.position - enemy.transform.position).normalized;
            RaycastHit hit;

            if (Physics.Raycast(enemy.transform.position, directionToTarget, out hit, detectionRadius))
            {
                if (hit.collider == target && hit.collider.CompareTag("Player"))
                {
                    // Check if there's a valid NavMesh path to the player
                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(target.transform.position, out navHit, 1.0f, NavMesh.AllAreas))
                    {
                        if (!NavMesh.Raycast(enemy.transform.position, target.transform.position, out navHit, NavMesh.AllAreas))
                        {
                            Debug.Log("Player Detected");
                            chasePos = target.transform.position;
                            enemy.playerCharacter = hit.collider.GetComponent<PlayerController>();
                            enemy.playerCharacter.AddEnemyToChaseList(enemy);
                            enemy.ChangeState(enemy.ChaseState);
                        }
                    }
                }
            }
        }
    }

}