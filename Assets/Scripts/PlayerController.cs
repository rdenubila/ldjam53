using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    float distanceToTarget = 15f;
    Vector3 offset = Vector3.up * 2f;
    GameObject _enemyToAttack = null;
    Animator _anim;
    public int health = 5;
    private PlayerMovement _playerMovement;
    private GameController _gameController;
    public Transform bloodTransform;

    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _gameController = FindObjectOfType<GameController>();

        GameStats.OnBloodCountUpdated += UpdateBloodStats;
    }

    void Update()
    {
        _anim = GetComponent<Animator>();
        CheckTarget();
    }

    void UpdateBloodStats(int current, int total, int limit)
    {
        bloodTransform?.transform?.DOScaleZ((float)current / (float)limit * 100f, 1f);
    }

    public GameObject GetEnemyToAttack() => _enemyToAttack;

    public void TakeDamage(GameObject from)
    {
        _playerMovement.RestorePlayerMovement();
        transform.LookAt(from.transform);
        _anim.SetTrigger("takeDamage");
        _gameController.gameStats.TakeDamage();
    }

    void CheckTarget()
    {
        _enemyToAttack = CheckTarget(Camera.main.transform);
        if (!_enemyToAttack)
            _enemyToAttack = CheckTarget(transform);

        _gameController?.HandleIconVisibility(_enemyToAttack);
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
                if (hit.collider.CompareTag("Hunter"))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.green);
                    return hit.collider.gameObject;
                }
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
