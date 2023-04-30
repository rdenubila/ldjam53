using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class EnemyMoveToPlayer : IState
{

    private NavMeshAgent _navAgent;
    private PlayerController _playerController;
    EnemyController _enemyController;
    Vector3 _circlePos;

    public EnemyMoveToPlayer(NavMeshAgent navAgent, PlayerController playerController, EnemyController enemyController)
    {
        _navAgent = navAgent;
        _playerController = playerController;
        _enemyController = enemyController;
    }
    public void OnEnter()
    {
        _circlePos = GetCirclePos(_enemyController.followDistance * Random.Range(1f, .65f));
        _navAgent.enabled = true;
        _navAgent.isStopped = false;
        followPlayer();
    }

    public void OnExit()
    {
        _navAgent.isStopped = true;
        _navAgent.ResetPath();
        _navAgent.enabled = false;
    }

    public void Tick()
    {
        followPlayer();
    }

    void followPlayer()
    {
        _navAgent.SetDestination(GetRandomPointAroundObject());
    }

    Vector3 GetRandomPointAroundObject()
    {
        Vector3 spawnPos = _playerController.transform.position + _circlePos;
        return spawnPos;
    }

    Vector3 GetCirclePos(float radius)
    {
        float angle = Random.Range(0f, 360f);
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector3 spawnPos = new Vector3(x, 0f, z);
        return spawnPos;
    }
}
