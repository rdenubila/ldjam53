using System;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    axe,
    pistol,
    tank
}

public class EnemyController : MonoBehaviour
{
    public int health = 3;
    public EnemyType enemyType = EnemyType.axe;
    public float followDistance = 10f;
    public float giveUpDistance = 20f;
    public float seeingInterval = 5f;
    public float boxCastDistance = 10f;
    public Vector3 boxCastSize = new Vector3(1f, 1f, 1f);

    public bool isTakingDamage = false;
    public bool isAttacking = false;
    public bool attackTrigger = false;
    PlayerController _playerController;
    // PlayerMovement _playerMovement;
    GameController _gameController;
    private StateMachine _stateMachine;
    private Animator _anim;
    private NavMeshAgent _navAgent;

    void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _playerController = FindObjectOfType<PlayerController>();
        _stateMachine = new StateMachine();
        _anim = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();

        var idle = new EnemyIdle(_anim, this);
        var moveToPlayer = new EnemyMoveToPlayer(_navAgent, _playerController, this);
        var seeingPlayer = new EnemySeePlayer(_anim, this, _gameController);
        var takeDamage = new EnemyTakeDamage(_anim, this);
        var attack = new EnemyAttack(_anim, this, _gameController);
        var die = new EnemyDie(_anim, this);

        At(idle, moveToPlayer, () => FollowPlayerIfIsAttacking() && !StartAttack());
        At(seeingPlayer, moveToPlayer, () => IsAttacking());
        At(moveToPlayer, idle, () => ReachDestination());
        At(idle, seeingPlayer, () => IsSeeingPlayer());
        At(takeDamage, idle, () => !isTakingDamage);
        At(idle, attack, () => StartAttack());
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
    }

    void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    bool FollowPlayerIfIsAttacking() => IsFarFromPlayer() && IsAttacking();
    bool IsFarFromPlayer() => DistanceFromPlayer() > followDistance;
    bool IsNearToPlayer() => DistanceFromPlayer() < followDistance;
    bool IsSeeingPlayer() => !isAttacking && CheckVisibilityToPlayer();
    public bool IsAttacking() => isAttacking;
    public float DistanceFromPlayer() => Vector3.Distance(transform.position, player().position);
    public Transform player() => _playerController?.transform;
    public void LookAtPlayer() => transform.LookAt(player().transform);
    public void SetIsAttacking(bool _isAttacking) => isAttacking = _isAttacking;
    public void SetIsTakingDamage(bool _isTakingDamage) => isTakingDamage = _isTakingDamage;
    public bool ReachDestination() => _navAgent.enabled && Vector3.Distance(transform.position, _navAgent.destination) < _navAgent.stoppingDistance;

    public bool StartAttack() => attackTrigger;

    public void Die()
    {
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
        if (
            enemyType == EnemyType.pistol
            || enemyType == EnemyType.tank
            )
        {
            _anim.SetLayerWeight(1, 1f);
        }
    }

}