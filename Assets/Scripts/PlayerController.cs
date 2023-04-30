using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float distanceToTarget = 15f;
    Vector3 offset = Vector3.up * 2f;
    GameObject _enemyToAttack = null;
    Animator _anim;
    public int health = 5;
    private PlayerMovement _playerMovement;

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        _anim = GetComponent<Animator>();
        CheckTarget();
    }

    public GameObject GetEnemyToAttack() => _enemyToAttack;

    public void TakeDamage(GameObject from)
    {
        _playerMovement.RestorePlayerMovement();
        transform.LookAt(from.transform);
        _anim.SetTrigger("takeDamage");
        health--;
    }

    void CheckTarget()
    {
        _enemyToAttack = CheckTarget(Camera.main.transform);
        if (!_enemyToAttack)
            CheckTarget(transform);
    }

    GameObject CheckTarget(Transform objTransform)
    {
        float[] angles = new float[] { 0f, -15f, 15f, -30f, 30f };
        foreach (float angle in angles)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + offset, cameraDirection(objTransform, angle));
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distanceToTarget, Color.red);
            if (Physics.Raycast(ray, out hit, distanceToTarget))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    Vector3 cameraDirection(Transform objTransform, float angle)
    {
        Vector2 input = Vector2.up + Vector2.right * angle / 360;
        Vector3 cameraForward = objTransform.forward;
        Vector3 cameraRight = objTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        return cameraForward * input.y + cameraRight * input.x;
    }
}
