using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float WalkSpeed = 5f;
    public float RunSpeed = 8f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundCheckDistance = 0.1f;

    [Header("References")]
    public Transform GroundCheck;
    public LayerMask GroundMask = 1;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;
    private Vector2 _currentMovementInput;
    private bool _isCurrentlyRunning;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        if (GroundCheck == null)
        {
            GroundCheck = transform;
        }

        InputHandler.OnMovementInput += HandleMovementInput;
        InputHandler.OnJumpPressed += HandleJump;
        InputHandler.OnRunToggle += HandleRunToggle;
    }

    void Update()
    {
        CheckGrounded();
        ApplyMovement();
        ApplyGravity();
        
        _characterController.Move(_velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        InputHandler.OnMovementInput -= HandleMovementInput;
        InputHandler.OnJumpPressed -= HandleJump;
        InputHandler.OnRunToggle -= HandleRunToggle;
    }

    void CheckGrounded()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckDistance, GroundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }

    void HandleMovementInput(Vector2 input)
    {
        _currentMovementInput = input;
    }

    void HandleJump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
    }

    void HandleRunToggle(bool isRunning)
    {
        _isCurrentlyRunning = isRunning;
    }

    void ApplyMovement()
    {
        float currentSpeed = _isCurrentlyRunning ? RunSpeed : WalkSpeed;
        
        Vector3 move = transform.right * _currentMovementInput.x + transform.forward * _currentMovementInput.y;
        _characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        _velocity.y += Gravity * Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckDistance);
        }
    }
}
