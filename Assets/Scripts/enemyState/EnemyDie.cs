using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : IState
{
    private Animator _animator;
    private EnemyController _enemyController;


    public EnemyDie(Animator animator, EnemyController enemyController)
    {
        _animator = animator;
        _enemyController = enemyController;
    }

    public void OnEnter()
    {
        _animator.SetTrigger("die");
        _enemyController.Die();
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
