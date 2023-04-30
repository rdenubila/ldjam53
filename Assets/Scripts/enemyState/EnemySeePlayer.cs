using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeePlayer : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private GameController _gameController;
    float _startTime;

    public EnemySeePlayer(Animator animator, EnemyController enemyController, GameController gameController)
    {
        _animator = animator;
        _enemyController = enemyController;
        _gameController = gameController;
    }

    public void OnEnter()
    {
        _animator.SetTrigger("lookAround");
        _startTime = Time.time;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        CheckTimer();
    }

    void CheckTimer()
    {
        if (Time.time - _startTime > _enemyController.seeingInterval)
        {
            _enemyController.SetIsAttacking(true);
            _gameController.CallNearEnemies();
        }
    }
}
