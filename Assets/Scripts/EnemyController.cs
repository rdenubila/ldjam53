using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    melee,
    pistol,
    tank
}

public class EnemyController : MonoBehaviour
{
    public int health = 3;
    public EnemyType enemyType = EnemyType.melee;
    public float followDistance = 10f;
    float giveUpDistance = 50f;
    public float seeingInterval = 5f;
    public float boxCastDistance = 10f;
    public Vector3 boxCastSize = new Vector3(1f, 1f, 1f);

    public bool isTakingDamage = false;
    public bool isAttacking = false;
    public bool attackTrigger = false;
    PlayerController _playerController;
    GameController _gameController;
    UiController _uiController;
    private StateMachine _stateMachine;
    [SerializeField] private Animator _anim;
    private NavMeshAgent _navAgent;

    void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _uiController = FindObjectOfType<UiController>();
        _playerController = FindObjectOfType<PlayerController>();
        _stateMachine = new StateMachine();
        _anim = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        List<GameObject> enemySpawnPoints = GameObject.FindGameObjectsWithTag("HunterSpawnPoint").ToList<GameObject>();

        var idle = new EnemyIdle(_anim, this);
        var moveToPlayer = new EnemyMoveToPlayer(_navAgent, _playerController, this);
        var patrol = new EnemyPatrol(_navAgent, _playerController, this, enemySpawnPoints);
        var seeingPlayer = new EnemySeePlayer(_anim, this, _gameController);
        var takeDamage = new EnemyTakeDamage(_anim, this, _gameController);
        var attack = new EnemyAttack(_anim, this, _gameController, _uiController);
        var die = new EnemyDie(_anim, this);

        At(idle, moveToPlayer, () => FollowPlayerIfIsAttacking());
        At(idle, seeingPlayer, () => IsSeeingPlayer());
        At(patrol, seeingPlayer, () => IsSeeingPlayer());
        At(idle, attack, () => StartAttack());
        At(idle, patrol, () => !IsAttacking());
        At(seeingPlayer, moveToPlayer, () => IsAttacking());
        At(moveToPlayer, idle, () => ReachDestination());
        At(takeDamage, idle, () => !isTakingDamage);
        At(moveToPlayer, attack, () => StartAttack());
        At(attack, idle, () => !StartAttack());

        _stateMachine.AddAnyTransition(takeDamage, () => isTakingDamage);
        _stateMachine.AddAnyTransition(die, () => health == 0);

        _stateMachine.SetState(idle);

        PlayerMovement.OnEnemyStopTakingDamage += () => SetIsTakingDamage(false);
        _anim.SetInteger("enemyType", ((int)enemyType));
        HandleUpperBodyLayer();
    }

    private void Update()
    {
        _stateMachine.Tick();

        if (IsFarFromPlayer())
        {
            // SetIsAttacking(false);
        }
    }

    void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    bool FollowPlayerIfIsAttacking() => IsFarFromPlayer() && IsAttacking() && !StartAttack();
    bool IsFarFromPlayer() => DistanceFromPlayer() > followDistance;
    bool IsNearToPlayer() => DistanceFromPlayer() < followDistance;
    bool IsSeeingPlayer() => !isAttacking && CheckVisibilityToPlayer();
    public bool IsAttacking() => isAttacking;
    public bool IsTank() => enemyType == EnemyType.tank;
    public float DistanceFromPlayer() => Vector3.Distance(transform.position, player().position);
    public Transform player() => _playerController?.transform;
    public void LookAtPlayer() => transform.LookAt(player().transform);
    public void SetIsAttacking(bool _isAttacking) => isAttacking = _isAttacking;
    public void SetIsTakingDamage(bool _isTakingDamage)
    {
        isTakingDamage = _isTakingDamage;
        if (!isTakingDamage)
        {
            // this._anim.ResetTrigger("takingDamage");
        }
    }
    public bool ReachDestination() => _navAgent.enabled && Vector3.Distance(transform.position, _navAgent.destination) < _navAgent.stoppingDistance;

    public bool StartAttack() => attackTrigger;

    public void Die()
    {
        _navAgent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        Destroy(gameObject, 5f);
    }

    public void Attack()
    {
        attackTrigger = true;
    }

    public void ResetAttackTrigger()
    {
        attackTrigger = false;
    }

    public void DecreaseHealth() => health--;

    bool CheckVisibilityToPlayer()
    {
        Vector3 direction = player().position - transform.position;
        RaycastHit hit;

        if (Physics.BoxCast(transform.position, boxCastSize, direction, out hit, transform.rotation, boxCastDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void HandleUpperBodyLayer()
    {
        if (enemyType == EnemyType.tank)
        {
            _anim.SetLayerWeight(1, 1f);
        }
    }

    public void CheckAttackCollision()
    {
        switch (enemyType)
        {
            case EnemyType.melee:
                CheckMeleeAttackCollision();
                break;
            case EnemyType.pistol:
                CheckPistolAttackCollision();
                break;
        }
    }

    public bool CheckMeleeAttackCollision()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.5f, transform.forward, 1.5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(gameObject);
                return true;
            }
        }
        return false;
    }

    void CheckPistolAttackCollision()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 2f, transform.forward);
        print("CheckAttackCollision");
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10f, Color.red, 3f);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(gameObject);
            }
        }
    }

    public void DestroyObj(GameObject obj) => Destroy(obj);

}
