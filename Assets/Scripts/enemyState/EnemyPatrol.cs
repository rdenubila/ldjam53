using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : IState
{

    private NavMeshAgent _navAgent;
    private PlayerController _playerController;
    EnemyController _enemyController;
    Vector3 _circlePos;
    List<GameObject> _enemySpawnPoints;

    public EnemyPatrol(NavMeshAgent navAgent, PlayerController playerController, EnemyController enemyController, List<GameObject> enemySpawnPoints)
    {
        _navAgent = navAgent;
        _playerController = playerController;
        _enemyController = enemyController;
        _enemySpawnPoints = enemySpawnPoints;
    }
    public void OnEnter()
    {
        _navAgent.speed = 1f;
        _navAgent.enabled = true;
        _navAgent.isStopped = false;
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        _navAgent.SetDestination(FindPoint().transform.position);
    }

    GameObject FindPoint()
    {
        GameObject point = _enemySpawnPoints
            .Where(item => Vector3.Distance(item.transform.position, _enemyController.transform.position) > 10f)
            .OrderBy(item => Random.Range(0, _enemySpawnPoints.Count()))
            .First();
        return point;
    }

    public void OnExit()
    {
        _navAgent.isStopped = true;
        _navAgent.ResetPath();
        _navAgent.enabled = false;
    }

    public void Tick()
    {
        if (_enemyController.ReachDestination())
        {
            GoToNextPoint();
        }
    }
}
