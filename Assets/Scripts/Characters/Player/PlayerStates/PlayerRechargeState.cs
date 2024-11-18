using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRechargeState : PlayerBaseState
{
    public PlayerRechargeState
        (PlayerController playerController) : base(playerController)
    {
    }

    private float progress; // Reference to the progress bar UI.
    private float lerpSpeed;
    private float ButtonMashBoost; // Extra progress per button press.
    private float maxTime; // Max value for the progress bar.


    public override void EnterState()
    {
        progress = 0;
        lerpSpeed = 2f;
        maxTime = Player.flashlight.MaxBatteryLife;
        ButtonMashBoost = Player.Settings.FlashlightReloadTime;

        Player.playerAnimator.animator.enabled = true;
        CamController.LookAtTarget(Player.RechargeRotation);
        Player.playerAnimator.animator.Play(Player.playerAnimator.RechargeHash);
        Debug.Log("Recharge state");

        // Player.playerAnimator.animator.CrossFade(Player.playerAnimator.RechargeHash, 0.5f);
        Player.ReloadAnimation = Player.StartCoroutine(ReloadAnimation());

        Player.playerFootsteps.getPlaybackState(out var playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            Player.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public override void ExitState()
    {

       CamController.RotateBack();
        Player.StopCoroutine(Player.ReloadAnimation);

        // player.PlayerCam.transform.parent = player.CameraHolder;
        Player.Event.OnFinishRecharge?.Invoke();
    }

    public override void StateFixedUpdate()
    {
    }

    public override void StateUpdate()
    {
        if (progress >= maxTime)
        {
            Player.playerAnimator.animator.Play(Player.playerAnimator.IdleHash);
            Player.StartCoroutine(BackIdle());

            Player.ChangeState(Player.MoveState);
        }

        progress += Player.Settings.FlashlightReloadTime * Time.deltaTime;

        if (Player.playerAnimator.animator.speed > 1f)
        {
            // Lerp the speed back to 1 slowly
            Player.playerAnimator.animator.speed =
                Mathf.Lerp(Player.playerAnimator.animator.speed, 1f, lerpSpeed * Time.deltaTime);
        }

     
        /*
        var targetRotation = Quaternion.Euler(50, 0, 0); // Target rotation as a Quaternion (local space)
        var currentRotation = Player.PlayerCam.transform.localRotation;

// Check if the rotation is close to the target (prevent jitter)
        if (Quaternion.Angle(currentRotation, targetRotation) < 0.01f) return;

// Spherically interpolate towards the target rotation
        Player.PlayerCam.transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, lerpSpeed * Time.deltaTime);
        */


    }


    private IEnumerator ReloadAnimation()
    {
        while (true)
        {
            Player.Event.SetTutorialText?.Invoke("Recharging: " + progress.ToString("F0") + "\n" +
                                                 "Tap R to boost recharge");
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator BackIdle()
    {
        while (Player.playerAnimator.animator.enabled)
        {
            yield return new WaitForSeconds(0.1f);
            Player.playerAnimator.animator.enabled = false;
        }
    }

    public override void HandleMovement(Vector2 dir)
    {
    }

    public override void HandleLookAround(Vector2 dir, InputDevice device)
    {
    }


    public override void HandleAttack(bool held)
    {
    }

    protected override void HandleFlashlightSphereCast()
    {
    }

    public override void HandleFlashlightPower()
    {
    }

    public override void HandleRecharge()
    {
        progress += ButtonMashBoost + Player.playerInventory.CrankCollected;
        Player.playerAnimator.animator.speed = 2f;
    }
}