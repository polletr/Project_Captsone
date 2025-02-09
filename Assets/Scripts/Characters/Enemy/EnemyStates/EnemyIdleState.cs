using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    
    public EnemyIdleState(ShadowEnemy enemyClass, EnemyAnimator enemyAnim) 
        : base(enemyClass,enemyAnim) { }

    public override void EnterState()
    {
        enemy.agent.ResetPath();
        enemyAnimator.animator.CrossFade(enemyAnimator.IdleHash, enemyAnimator.animationCrossFade);

        // Randomize the idle time between a range of seconds
        time = Random.Range(enemy.MinIdleTime, enemy.MaxIdleTime);
        timer = 0f; // Reset the timer
        if (enemy.playerCharacter != null)
        {
            enemy.playerCharacter.RemoveEnemyFromChaseList(enemy);
            enemy.playerCharacter = null;
        }

        PLAYBACK_STATE playbackState;
        enemy.currentAudio.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            enemy.currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
            enemy.currentAudio.start();
        }

    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate()
    {

    }

    public override void StateUpdate()
    {
        VisionDetection();
        if (enemy.playerCharacter != null)
        {
            RotateToPlayer();
        }

        /*        timer += Time.deltaTime;

                if (timer >= time)
                {
                    enemy.ChangeState(enemy.PatrolState);
                }
        */
    }

}
