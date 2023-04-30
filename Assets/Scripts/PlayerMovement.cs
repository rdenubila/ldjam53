using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{

    Animator _anim;
    private CharacterController _characterController;
    private PlayerController _playerController;
    public float gravity = 20.0f;
    bool isRunning = false;
    Vector2 desiredDirection = Vector2.zero;
    float desiredVelocity = 0f;
    float movementVelocity = 0f;
    float easing = .05f;
    bool freezeMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!freezeMovement)
        {
            setAnimVelocity();
            rotateToCamera();
            applyGravity();
        }
    }

    void applyGravity()
    {
        Vector3 moveDirection = Vector3.down * (gravity * Time.deltaTime);
        _characterController.Move(moveDirection * Time.deltaTime);
    }

    void setAnimVelocity()
    {
        easeVelocity();
        _anim.SetFloat("velocity", desiredVelocity);
    }

    void easeVelocity()
    {
        desiredVelocity = Mathf.Lerp(desiredVelocity, getVelocity(), easing);
    }

    float getVelocity()
    {
        float velocity = movementVelocity;
        if (isRunning) velocity *= 2f;
        return velocity;
    }

    void rotateToCamera()
    {
        Vector2 input = this.desiredDirection;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 desiredDirection = cameraForward * input.y + cameraRight * input.x;
        if (desiredDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(desiredDirection), easing);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        desiredDirection = direction;
        movementVelocity = direction.magnitude;
    }

    public void Run(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            _anim.SetTrigger("roll");
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (freezeMovement) return;

        GameObject enemy = _playerController.GetEnemyToAttack();
        if (enemy)
        {
            AttackEnemy(enemy);
            enemy.GetComponent<EnemyController>().SetIsTakingDamage(true);
        }
    }

    void AttackEnemy(GameObject enemy)
    {
        freezeMovement = true;
        _anim.SetTrigger("attack");
        _anim.applyRootMotion = false;
        transform.DOMove(enemy.transform.position - Camera.main.transform.forward * 1f, .5f).SetDelay(.5f);
        transform.DORotateQuaternion(Quaternion.LookRotation(enemy.transform.position - transform.position), .5f);
    }

    public void RestorePlayerMovement()
    {
        freezeMovement = false;
        _anim.applyRootMotion = true;
        OnEnemyStopTakingDamage.Invoke();
    }

    public delegate void EnemyStopTakingDamage();
    public static event EnemyStopTakingDamage OnEnemyStopTakingDamage;
}
