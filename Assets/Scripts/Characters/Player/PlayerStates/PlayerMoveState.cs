using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerAnimator animator, PlayerController playerController, InputManager inputM)
        : base(animator, playerController, inputM) { }

    public override void EnterState()
    {
        playerAnimator.animator.Play(playerAnimator.IdleHash);
    }
    public override void ExitState()
    {

    }

    public override void StateFixedUpdate() { }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Ray ray = new Ray(player.Camera.position, player.Camera.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, player.Settings.MaxEnemyDistance))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out EnemyClass enemy))
            {
                player.AddEnemyToChaseList(enemy);
            }
        }

        if (Physics.Raycast(ray, out hit, player.InteractionRange))
        {
            var obj = hit.collider.gameObject;
            if (obj.TryGetComponent(out IInteractable thing))
            {
                player.interactableObj = thing;
            }
        }
        else
        {
            player.interactableObj = null;
        }
    }

    public override void HandleChangeAbility(int d)
    {
        player.flashlight.ChangeSelectedAbility(d);
    }

    public override void HandleInteract()
    {
        if (player.interactableObj != null)
            player.ChangeState(player.InteractState);
    }

}
