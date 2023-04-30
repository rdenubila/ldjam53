using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private GameController _gameController;


    public EnemyAttack(Animator animator, EnemyController enemyController, GameController gameController)
    {
        _animator = animator;
        _enemyController = enemyController;
        _gameController = gameController;
    }

    public void OnEnter()
    {
        _animator.SetTrigger("attack");
        _enemyController.ResetAttackTrigger();
    }

    public void OnExit()
    {
        _gameController.AttackFinished();
    }

    public void Tick()
    {
    }
}
