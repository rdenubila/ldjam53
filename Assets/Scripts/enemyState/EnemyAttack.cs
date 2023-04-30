using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IState
{
    private Animator _animator;
    private EnemyController _enemyController;
    private GameController _gameController;
    private UiController _uiController;
    private CapsuleCollider _collider;
    private bool checkCollision = true;
    private GameObject icon;
    private float startTime;
    private float rotationTime = 1.5f;

    public EnemyAttack(Animator animator, EnemyController enemyController, GameController gameController, UiController uiController)
    {
        _animator = animator;
        _enemyController = enemyController;
        _gameController = gameController;
        _uiController = uiController;
        _collider = _enemyController.gameObject.GetComponent<CapsuleCollider>();
    }

    public void OnEnter()
    {
        icon = _uiController.AddIcon("Attack", _enemyController.gameObject, Vector3.up * 5f);
        _collider.enabled = false;
        checkCollision = true;
        startTime = Time.time;
        _animator.SetTrigger("attack");
    }

    public void OnExit()
    {
        _enemyController.DestroyObj(icon);
        _collider.enabled = true;
        _gameController.AttackFinished();
        _enemyController.ResetAttackTrigger();
    }

    public void Tick()
    {
        CheckTankCollision();
        LookToPlayer();
    }

    void CheckTankCollision()
    {
        if (checkCollision && _enemyController.IsTank())
        {
            if (_enemyController.CheckMeleeAttackCollision())
                checkCollision = false;
        }
    }

    void LookToPlayer()
    {
        if (checkCollision && Time.time - startTime < rotationTime)
            _enemyController.LookAtPlayer();
    }
}
