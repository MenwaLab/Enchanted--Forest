using System.Resources;
public class Player
{
    static ResourceManager resourceManager3 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Player).Assembly);
    public string Name { get; set; }
    public Token Token { get; set; }
    public (int x, int y) Position { get; set; }
    public int SkipTurns { get; set; } 
    private MazeGeneration maze; //guardar la referencia del tablero
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
        return $"{Name} en {Position}, Ficha: {Token.Name}";
    }

    public void CheckCooldownAndRestoreSpeed()
    {
        if (Token.CurrentCooldown == 0)
        {
            // Normalizar la velocidad si fue reducida
            if(Token.Speed > Token.BaseSpeed)  
            {
                Token.Speed=Token.BaseSpeed;
            }
        }
    }
    public static void SwapPlayerPositions(Player player1, Player player2)
    {
    var tempPosition = player1.Position;
    player1.Position = player2.Position;
    player2.Position = tempPosition;

    string? swappedPositionsMessage = resourceManager3.GetString("PlayerSwappedPositions");
        if (!string.IsNullOrEmpty(swappedPositionsMessage))
        {
            Console.WriteLine(string.Format(swappedPositionsMessage, player1.Name, player2.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'PlayerSwappedPositions' not found.");
        }
    }
}