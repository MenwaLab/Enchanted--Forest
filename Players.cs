public class Player
{
    public string Name { get; set; }
    public Token Token { get; set; }
    public (int x, int y) Position { get; set; }
    public int SkipTurns { get; set; } 
    private MazeGeneration maze; // Add this to store the maze reference
    public bool HasUsedAbility { get; set; }

    public Player(string name, Token token, int startX, int startY, MazeGeneration maze)
    {
        Name = name;
        Token = token;
        Position = (startX, startY);
        SkipTurns = 0;
        this.maze = maze; 
        HasUsedAbility = false;
    }

public bool Move(int dx, int dy)
{
    return Program.TryMovePlayer(this, dx, dy, Token.Speed, maze);
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
        if(Token.Speed > Token.BaseSpeed)  // Assuming you reduced speed by 1 previously
        {
            Token.Speed=Token.BaseSpeed;
        }
    }
}
}