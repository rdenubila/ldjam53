using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float distanceToTarget = 15f;
    Vector3 offset = Vector3.up * 2f;

    GameObject _enemyToAttack = null;

    // Update is called once per frame
    void Update()
    {
        CheckTarget();
    }

    public GameObject GetEnemyToAttack() => _enemyToAttack;

    void CheckTarget()
    {
        float[] angles = new float[] { 0f, -15f, 15f, -30f, 30f };
        foreach (float angle in angles)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + offset, cameraDirection(angle));
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distanceToTarget, Color.red);
            if (Physics.Raycast(ray, out hit, distanceToTarget))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                _enemyToAttack = hit.collider.gameObject;
                return;
            }
        }

        _enemyToAttack = null;
    }

    Vector3 cameraDirection(float angle)
    {
        Vector2 input = Vector2.up + Vector2.right * angle / 360;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        return cameraForward * input.y + cameraRight * input.x;
    }
}
