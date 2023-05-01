using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyTakeDamage : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private float delay = 1f;
    private CapsuleCollider _collider;

    private GameController _gameController;
    float currentTime;

    public EnemyTakeDamage(Animator animator, EnemyController enemyController, GameController gameController)
    {
        _animator = animator;
        _enemyController = enemyController;
        _collider = _enemyController.gameObject.GetComponent<CapsuleCollider>();
        _gameController = gameController;
    }

    public void OnEnter()
    {
        currentTime = Time.time;
        _animator.SetTrigger("stopAnims");
        _collider.enabled = false;
        _enemyController.transform.DORotateQuaternion(
            Quaternion.LookRotation(_enemyController.player().transform.position - _enemyController.transform.position),
            .25f
        )
        .SetDelay(.8f)
        .OnComplete(() =>
        {
            SetTakingDamage(true);
            _enemyController.DecreaseHealth();
        });
    }

    void CheckIncrementBlood()
    {
        if (Time.time - currentTime > 1f)
        {
            currentTime = Time.time;
            _gameController.gameStats.AddBlood();
        }
    }

    public void Tick()
    {
        CheckIncrementBlood();
     }

    public void OnExit()
    {
        _enemyController.SetIsAttacking(true);
        _enemyController.ResetAttackTrigger();
        _collider.enabled = true;
        SetTakingDamage(false);
    }

    void SetTakingDamage(bool value) => _animator.SetBool("takingDamage", value);

}
