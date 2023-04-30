using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EnemyIdle : IState
{
    private EnemyController _enemyController;
    private Animator _animator;

    public EnemyIdle(Animator animator, EnemyController enemyController)
    {
        _animator = animator;
        _enemyController = enemyController;
    }

    public void OnEnter()
    {
        _animator.SetFloat("horizontalVelocity", 0f);
        _animator.SetFloat("verticalVelocity", 0f);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (_enemyController.IsAttacking())
        {
            _enemyController.LookAtPlayer();
        }
    }
}
