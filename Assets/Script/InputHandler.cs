using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Input Values")]
    public Vector2 MovementInput;
    public bool IsRunning;

    public static event Action<Vector2> OnMovementInput;
    public static event Action OnJumpPressed;
    public static event Action<bool> OnRunToggle;

    private Vector2 _previousMovementInput;
    private bool _previousRunState;

    void Update()
    {
        HandleMovementInput();
        HandleActionInput();
    }

    void HandleMovementInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(forwardKey)) vertical = 1f;
        if (Input.GetKey(backwardKey)) vertical = -1f;
        if (Input.GetKey(rightKey)) horizontal = 1f;
        if (Input.GetKey(leftKey)) horizontal = -1f;

        MovementInput = new Vector2(horizontal, vertical).normalized;

        if (MovementInput != _previousMovementInput)
        {
            OnMovementInput?.Invoke(MovementInput);
            _previousMovementInput = MovementInput;
        }
    }

    void HandleActionInput()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            OnJumpPressed?.Invoke();
        }

        IsRunning = Input.GetKey(runKey);
        if (IsRunning != _previousRunState)
        {
            OnRunToggle?.Invoke(IsRunning);
            _previousRunState = IsRunning;
        }
    }

    void OnDestroy()
    {
        OnMovementInput = null;
        OnJumpPressed = null;
        OnRunToggle = null;
    }

    public Vector2 GetMovementInput() => MovementInput;
    public bool GetRunInput() => IsRunning;
}
