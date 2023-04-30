using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private GameController _gameController;
    private CapsuleCollider _collider;
    private bool checkCollision = true;


    public EnemyAttack(Animator animator, EnemyController enemyController, GameController gameController)
    {
        _animator = animator;
        _enemyController = enemyController;
        _gameController = gameController;
        _collider = _enemyController.gameObject.GetComponent<CapsuleCollider>();
    }

    public void OnEnter()
    {
        _collider.enabled = false;
        checkCollision = true;
        _enemyController.LookAtPlayer();
        _animator.SetTrigger("attack");
    }

    public void OnExit()
    {
        _collider.enabled = true;
        _gameController.AttackFinished();
        _enemyController.ResetAttackTrigger();
    }

    public void Tick()
    {
        if (checkCollision && _enemyController.IsTank())
        {
            if (_enemyController.CheckMeleeAttackCollision())
                checkCollision = false;
        }
    }
}
