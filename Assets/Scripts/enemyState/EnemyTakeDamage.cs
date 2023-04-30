using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyTakeDamage : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private float delay = 1f;

    public EnemyTakeDamage(Animator animator, EnemyController enemyController)
    {
        _animator = animator;
        _enemyController = enemyController;
    }

    public void OnEnter()
    {

        _enemyController.transform.DORotateQuaternion(
            Quaternion.LookRotation(_enemyController.player().transform.position - _enemyController.transform.position),
            .25f
        )
        .SetDelay(1f)
        .OnComplete(() =>
        {
            SetTakingDamage(true);
            _enemyController.DecreaseHealth();
        });
    }

    public void Tick() { }

    public void OnExit()
    {
        SetTakingDamage(false);
    }

    void SetTakingDamage(bool value) => _animator.SetBool("takingDamage", value);

}
