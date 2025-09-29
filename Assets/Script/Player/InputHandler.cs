using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Server Settings")]
    public bool EnableServerControl = true;
    public KeyCode ToggleControlModeKey = KeyCode.T;

    [Header("Input Settings")]
    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode BackwardKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode RunKey = KeyCode.LeftShift;
    public KeyCode RotateClockwiseKey = KeyCode.E;
    public KeyCode RotateAntiClockwiseKey = KeyCode.Q;

    [Header("Input Values")]
    public Vector2 MovementInput;
    public bool IsRunning;
    public bool IsRotatingClockwise;
    public bool IsRotatingAntiClockwise;

    public static event Action<Vector2> OnMovementInput;
    public static event Action OnJumpPressed;
    public static event Action<bool> OnRunToggle;
    public static event Action<bool> OnRotateClockwiseToggle;
    public static event Action<bool> OnRotateAntiClockwiseToggle;

    private Vector2 _previousMovementInput;
    private bool _previousRunState;
    private bool _previousRotateClockwiseState;
    private bool _previousRotateAntiClockwiseState;

    // Server input override
    private Vector2 _serverMovementInput;
    private bool _serverIsRunning;
    private bool _serverIsJumping;
    private bool _serverIsRotatingClockwise;
    private bool _serverIsRotatingAntiClockwise;
    private bool _hasServerInput;

    void Start()
    {
        HttpServer.OnCommandReceived += ProcessServerCommand;
    }

    void Update()
    {
        HandleToggleInput();
        HandleMovementInput();
        HandleActionInput();
    }

    void HandleToggleInput()
    {
        if (Input.GetKeyDown(ToggleControlModeKey))
        {
            EnableServerControl = !EnableServerControl;
            Debug.Log($"Control mode switched to: {(EnableServerControl ? "Server Control" : "Keyboard Control")}");

            // Clear server input when switching to keyboard control
            if (!EnableServerControl)
            {
                _hasServerInput = false;
                _serverMovementInput = Vector2.zero;
                _serverIsRunning = false;
                _serverIsJumping = false;
                _serverIsRotatingClockwise = false;
                _serverIsRotatingAntiClockwise = false;
            }
        }
    }

    void HandleMovementInput()
    {
        Vector2 currentInput;

        if (EnableServerControl && _hasServerInput)
        {
            currentInput = _serverMovementInput;
        }
        else
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(ForwardKey)) vertical = 1f;
            if (Input.GetKey(BackwardKey)) vertical = -1f;
            if (Input.GetKey(RightKey)) horizontal = 1f;
            if (Input.GetKey(LeftKey)) horizontal = -1f;

            currentInput = new Vector2(horizontal, vertical).normalized;
        }

        MovementInput = currentInput;

        if (MovementInput != _previousMovementInput)
        {
            OnMovementInput?.Invoke(MovementInput);
            _previousMovementInput = MovementInput;
        }
    }

    void HandleActionInput()
    {
        bool currentJumpState = false;
        bool currentRunState = false;
        bool currentRotateClockwiseState = false;
        bool currentRotateAntiClockwiseState = false;

        if (EnableServerControl && _hasServerInput)
        {
            if (_serverIsJumping)
            {
                OnJumpPressed?.Invoke();
                _serverIsJumping = false; // Reset jump after triggering
            }

            currentRunState = _serverIsRunning;
            currentRotateClockwiseState = _serverIsRotatingClockwise;
            currentRotateAntiClockwiseState = _serverIsRotatingAntiClockwise;
        }
        else
        {
            if (Input.GetKeyDown(JumpKey))
            {
                OnJumpPressed?.Invoke();
            }

            currentRunState = Input.GetKey(RunKey);
            currentRotateClockwiseState = Input.GetKey(RotateClockwiseKey);
            currentRotateAntiClockwiseState = Input.GetKey(RotateAntiClockwiseKey);
        }

        IsRunning = currentRunState;
        if (IsRunning != _previousRunState)
        {
            OnRunToggle?.Invoke(IsRunning);
            _previousRunState = IsRunning;
        }

        IsRotatingClockwise = currentRotateClockwiseState;
        if (IsRotatingClockwise != _previousRotateClockwiseState)
        {
            OnRotateClockwiseToggle?.Invoke(IsRotatingClockwise);
            _previousRotateClockwiseState = IsRotatingClockwise;
        }

        IsRotatingAntiClockwise = currentRotateAntiClockwiseState;
        if (IsRotatingAntiClockwise != _previousRotateAntiClockwiseState)
        {
            OnRotateAntiClockwiseToggle?.Invoke(IsRotatingAntiClockwise);
            _previousRotateAntiClockwiseState = IsRotatingAntiClockwise;
        }
    }

    void OnDestroy()
    {
        HttpServer.OnCommandReceived -= ProcessServerCommand;

        OnMovementInput = null;
        OnJumpPressed = null;
        OnRunToggle = null;
        OnRotateClockwiseToggle = null;
        OnRotateAntiClockwiseToggle = null;
    }

    void ProcessServerCommand(string commandJson)
    {
        try
        {
            PlayerCommand command = JsonUtility.FromJson<PlayerCommand>(commandJson);

            _serverMovementInput = new Vector2(command.MovementX, command.MovementY);
            _serverIsRunning = command.IsRunning;
            _serverIsJumping = command.IsJumping;
            _serverIsRotatingClockwise = command.IsRotatingClockwise;
            _serverIsRotatingAntiClockwise = command.IsRotatingAntiClockwise;
            _hasServerInput = true;

            Debug.Log($"Server command received: Movement({command.MovementX}, {command.MovementY}), Run: {command.IsRunning}, Jump: {command.IsJumping}, RotateCW: {command.IsRotatingClockwise}, RotateACW: {command.IsRotatingAntiClockwise}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse server command: {e.Message}");
        }
    }

    public Vector2 GetMovementInput() => MovementInput;
    public bool GetRunInput() => IsRunning;
}
