using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Timeline.TimelinePlaybackControls;


[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    #region Variables: Movement
    private Vector2 playerInput;
    private CharacterController characterController;
    public Vector3 direction;

    [SerializeField] private float speed;

    [SerializeField] private Movement movement;
    #endregion

    #region Variable: Gravity
    private float gravity = -9.81f;
    [SerializeField] private float gravMod;
    private float velocity;
    #endregion

    #region Variables: Jump
    [SerializeField] private float jumpForce;
    #endregion

    [SerializeField] private float stamina = 2.5f;

    public Vector2 deltaInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
    }

    private void Update()
    {
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
    }


    // Generates the movement to the object.
    private void ApplyMovement()
    {
        var targetSpeed = movement.isSprinting && stamina > 0.0f ? movement.speed * movement.multiplier : movement.speed;
        movement.currentSpeed = Mathf.MoveTowards(movement.speed, targetSpeed, movement.currentSpeed);

        if(stamina < 0)
        {
            movement.currentSpeed = movement.speed;
            StartCoroutine(StaminaReset());
        }

        if(movement.currentSpeed > movement.speed)
        {
            DrainStamina();
        }
        if(stamina < 2.5f && movement.currentSpeed < movement.speed * movement.multiplier)
        {
            StaminUp();
        }

        characterController.Move(direction * movement.currentSpeed * Time.deltaTime);
    }


    //Generates the gravity for the object.
    private void ApplyGravity()
    {
        if(IsGrounded() && velocity < 0.0f)
        {
            velocity = -1f;
        }
        else
        {
            velocity += gravity * gravMod * Time.deltaTime;
        }

        direction.y = velocity;
    }



    private void ApplyRotation()
    {
        direction = Quaternion.Euler(0.0f, cameraTransform.eulerAngles.y, 0.0f) * new Vector3(playerInput.x, 0.0f, playerInput.y);
        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Now apply the rotation to the character
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    //Assign the values of the input from the player.
    public void Move(InputAction.CallbackContext context)
    {
        playerInput = context.ReadValue<Vector2>();
        direction = new Vector3(playerInput.x, 0, playerInput.y);
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded()) return;

        velocity += jumpForce;
    }

    public void Look(InputAction.CallbackContext context)
    {
        deltaInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (CanRun())
        {
            movement.isSprinting = context.performed;
        }
    }


    private bool IsGrounded() => characterController.isGrounded;

    private void DrainStamina()
    {
        stamina -= Time.deltaTime;
        if (stamina == 0.0f)
        {
            StartCoroutine(StaminaReset());
        }
    }

    private void StaminUp()
    {
        stamina += Time.deltaTime;
        if (stamina == 2.5f)
        {
            return;
        }
    }

    private IEnumerator StaminaReset()
    {
        yield return new WaitForSeconds(1.5f);
    }


    private bool CanRun()
    {
        if (stamina > 0.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[Serializable]
public struct Movement
{
    public float speed;
    public float multiplier;
    public float accel;

    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}
