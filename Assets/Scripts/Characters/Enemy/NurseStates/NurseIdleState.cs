using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseIdleState : NurseBaseState
{
    public NurseIdleState(NurseEnemy enemyClass, EnemyAnimator enemyAnim)
    : base(enemyClass, enemyAnim) { }

    public override void EnterState() { }

    public override void ExitState() { }

    public override void StateFixedUpdate() { }

    public override void StateUpdate() { }
}
