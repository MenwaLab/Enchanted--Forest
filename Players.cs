public class Player
{
    public string Name { get; set; }
    public Token Token { get; set; }
    public (int x, int y) Position { get; set; }
    public int SkipTurns { get; set; } 
    private MazeGeneration maze; // Add this to store the maze reference

    public Player(string name, Token token, int startX, int startY, MazeGeneration maze)
    {
        Name = name;
        Token = token;
        Position = (startX, startY);
        SkipTurns = 0;
        this.maze = maze; 
    }

public bool Move(int dx, int dy)
{
    int remainingSpeed = Token.Speed; // The speed of the player
    int currentX = Position.x;
    int currentY = Position.y;

    while (remainingSpeed > 0)
    {
        int nextX = currentX + dx;
        int nextY = currentY + dy;

        // Check if the next position is a wall
        if (maze.IsWall(nextX, nextY))
        {
            Console.WriteLine($"Blocked by a wall at ({nextX}, {nextY}). Adjusting movement.");
            break; // Stop movement when a wall is encountered
        }

        // Move to the next position if it's valid
        currentX = nextX;
        currentY = nextY;
        remainingSpeed--;

        Console.WriteLine($"{Name} moved to ({currentX}, {currentY})");
    }

    // Final position reached
    Position = (currentX, currentY);

    // Check for traps at the final position
    Trap? trap = maze.IsTrapAtPosition(Position.x, Position.y);
    if (trap != null)
    {
        trap.ApplyEffect(this);
    }

    return true;
}

    public override string ToString()
    {
        return $"{Name} at {Position}, Token: {Token.Name}";
    }

    public void CheckCooldownAndRestoreSpeed()
{
    if (Token.CurrentCooldown == 0)
    {
        // Restore speed if it was reduced
        Token.Speed += 1;  // Assuming you reduced speed by 1 previously
    }
}
}