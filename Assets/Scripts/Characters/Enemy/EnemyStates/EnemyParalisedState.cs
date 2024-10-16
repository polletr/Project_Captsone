using UnityEngine;

public class EnemyParalisedState : EnemyBaseState
{
    public EnemyParalisedState(EnemyClass enemyClass, EnemyAnimator enemyAnim)
        : base(enemyClass, enemyAnim) { }

    public override void EnterState()
    {
        enemy.agent.ResetPath();

        enemy.agent.speed = enemy.ParalisedSpeed;

        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
        enemy.currentAudio.start();

    }
    public override void ExitState()
    {
        enemy.currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }

    public override void StateUpdate()
    {
        if (enemy.playerCharacter == null)
        {
            enemy.playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            enemy.playerCharacter.AddEnemyToChaseList(enemy);
        }

/*        if (enemy.agent.destination != chasePos)
        {
            enemy.agent.SetDestination(chasePos);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.SightRange && enemy.playerCharacter.GetComponent<PlayerController>().IsAlive())
        {
            chasePos = enemy.playerCharacter.transform.position;
            if (Vector3.Distance(enemy.transform.position, enemy.playerCharacter.transform.position) <= enemy.AttackRange)
            {
                enemy.agent.ResetPath();
                enemy.ChangeState(enemy.AttackState);
            }
        }
*/
        // Get the direction to the player
        Vector3 directionToPlayer = (enemy.playerCharacter.transform.position - enemy.transform.position).normalized;

        // Calculate the rotation towards the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z)); // Ignore y-axis to keep rotation flat

        // Smoothly rotate the enemy towards the player
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.RotationSpeed);
    }


    public override void StateFixedUpdate()
    {

    }

    protected override void VisionDetection() { }

    protected override void OnSoundDetected(Vector3 soundPosition, float soundRange) { }



}
