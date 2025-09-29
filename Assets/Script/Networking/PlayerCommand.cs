[System.Serializable]
public class PlayerCommand
{
    public float MovementX;
    public float MovementY;
    public bool IsRunning;
    public bool IsJumping;
    public bool IsRotatingClockwise;
    public bool IsRotatingAntiClockwise;

    public PlayerCommand()
    {
        MovementX = 0f;
        MovementY = 0f;
        IsRunning = false;
        IsJumping = false;
        IsRotatingClockwise = false;
        IsRotatingAntiClockwise = false;
    }

    public PlayerCommand(float movementX, float movementY, bool isRunning = false, bool isJumping = false, bool isRotatingClockwise = false, bool isRotatingAntiClockwise = false)
    {
        MovementX = movementX;
        MovementY = movementY;
        IsRunning = isRunning;
        IsJumping = isJumping;
        IsRotatingClockwise = isRotatingClockwise;
        IsRotatingAntiClockwise = isRotatingAntiClockwise;
    }
}