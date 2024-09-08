using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour, IDamageable
{
    public Vector3 PatrolCenterPos { get; set; }
    public Animator animator { get; set; }
    public GameObject playerCharacter { get; set; }

    public float PatrolRange { get { return patrolRange; } }
    public float MaxIdleTime { get { return maxIdleTime; } }
    public float MinIdleTime { get { return minIdleTime; } }
    public float PatrolSpeed { get { return patrolSpeed; } }
    public float ChaseSpeed { get { return chaseSpeed; } }
    public float HearingMultiplier { get { return hearingMultiplier; } }
    public float SightRange { get { return sightRange; } }
    public float SightAngle { get { return sightAngle; } }
    public float AttackRange { get { return attackRange; } }
    public float AttackDamage { get { return attackDamage; } }


    [HideInInspector]
    public EnemyBaseState currentState;

    [HideInInspector]
    public NavMeshAgent agent;

    [SerializeField] private float patrolRange;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField, Range(0f, 2f)] private float hearingMultiplier = 1f;
    [SerializeField, Range(0.2f, 15f)] private float sightRange = 5f;
    [SerializeField, Range(20f, 90f)] private float sightAngle = 45f;
    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 1;
    [SerializeField] private float health = 3;


    public GameEvent Event;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        PatrolCenterPos = transform.position;
        ChangeState(new EnemyIdleState());
    }

    private void FixedUpdate() => currentState?.StateFixedUpdate();
    private void Update() => currentState?.StateUpdate();

    public void GetDamaged(float attackDamage)
    {
        health -= attackDamage;
    }


    #region ChangeState
    public void ChangeState(EnemyBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.enemy = this;
        currentState.EnterState();
    }
    #endregion

}
