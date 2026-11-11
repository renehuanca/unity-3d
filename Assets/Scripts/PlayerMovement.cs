using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 moveInput;

    public float gravity = -9.8f;
    private float verticalVelocity;

    public Transform modelTransform;
    public Transform cameraPivot;
    private Animator animator;

    private bool isRunning;
    private bool isMoving;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    public float jumpForce = 5f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = (isRunning && isMoving) ? runSpeed : walkSpeed;

        if (controller.isGrounded && verticalVelocity<0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDir = (forward * moveInput.y + right * moveInput.x);

        if (desiredMoveDir.magnitude > 0.1f)
        {
            Quaternion targetRotaion = Quaternion.LookRotation(desiredMoveDir);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotaion, 15f * Time.deltaTime);
        }
        
        Vector3 velocity = desiredMoveDir * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        UpdateAnimations();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    public void OnDance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Dance");
        }
    }

    public void UpdateAnimations()
    {
        isMoving = moveInput.magnitude > 0.1f;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning && isMoving);
        animator.SetBool("Grounded", controller.isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

}
