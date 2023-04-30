using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    EnemyController enemyController;
    Animator _anim;
    NavMeshAgent _navAgent;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        _anim = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.enabled = false;

        _navAgent.updatePosition = false;
        _navAgent.updateRotation = false;
    }

    void Update()
    {
        _anim.SetFloat("verticalVelocity", _navAgent.velocity.magnitude);

        _navAgent.nextPosition = transform.position;

        if (_navAgent.enabled)
        {
            Vector3 direction = (_navAgent.destination - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
