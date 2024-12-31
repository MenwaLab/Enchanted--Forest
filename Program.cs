using System;
//using MazeGeneration; ??

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Maze Game!");
        Console.WriteLine("Por favor introduzca el tamaño del laberinto con el que desea jugar!: ");
        int size;

        while (!int.TryParse(Console.ReadLine(), out size) || size < 5)
        {
            Console.WriteLine("Por favor introduzca un tamaño válido para su laberinto: ");
        }

        // Create the maze
        MazeGeneration generatorMaze = new MazeGeneration(size);// Create the maze
        Token[] tokens = TokenFactory.GetAvailableTokens();

        Console.WriteLine("Available tokens:");
        for (int i = 0; i < tokens.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {tokens[i]}");
        }

        Console.WriteLine("Player 1, choose your token by entering its number: ");
        int choice1;
         
        while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || choice1 > 5)
        {
            Console.WriteLine("Por favor introduzca un número válido para su ficha (entre 1 y 5): ");
        }
        choice1--;  // Adjust for 0-based indexing
        Player player1 = new Player("Player 1", tokens[choice1], 0, 0, generatorMaze);


        Console.WriteLine("Player 2, choose your token by entering its number: ");
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > 5)
        {
            Console.WriteLine("Por favor introduzca un número válido para su ficha (entre 1 y 5): ");
        }
        choice2--;
        Player player2 = new Player("Player 2", tokens[choice2], 0, 0,generatorMaze);

        Console.WriteLine($"Player 1 chose {player1.Token.Name}");
        Console.WriteLine($"Player 2 chose {player2.Token.Name}. Empezemos el juego!!!");

        
        generatorMaze.PrintMaze();
        while (true)
        {
            Console.WriteLine($"{player1.Name}, it's your turn.");
             if (player1.SkipTurns > 0)
            {
                Console.WriteLine($"{player1.Name} is skipping a turn.");
                player1.SkipTurns--;  // Decrease the skip count
            }
            else
            {
                player1.Token.ReduceCooldown();
                player1.CheckCooldownAndRestoreSpeed();
                Console.WriteLine("Do you want to use your ability? (Y/N): ");
                if (Console.ReadLine()!.ToUpper() == "Y")
                {
                    player1.Token.UseAbility(player1, player2);
                }
                    Console.WriteLine("Muevase de acuerdo a las teclas");
                    HandleMovement(player1, generatorMaze);
            }

            Console.WriteLine($"{player2.Name}, it's your turn.");
            if (player2.SkipTurns > 0)
            {
                Console.WriteLine($"{player1.Name} is skipping a turn.");
                player1.SkipTurns--;  // Decrease the skip count
            }
            else
            {
                player2.Token.ReduceCooldown();
                player2.CheckCooldownAndRestoreSpeed();
                Console.WriteLine("Do you want to use your ability? (Y/N): ");
                if (Console.ReadLine()!.ToUpper() == "Y")
                {
                    player2.Token.UseAbility(player2, player1);
                }
                    Console.WriteLine("Muevase de acuerdo a las teclas");
                    HandleMovement(player2, generatorMaze);
            }
        }
    }
    public static void HandleMovement(Player player,MazeGeneration generatorMaze)
    {
        while (true) // Retry until the player successfully moves
        {
            Console.WriteLine("Press an arrow key to move:");
            ConsoleKeyInfo key = Console.ReadKey(true); // Get key press

            int dx = 0, dy = 0;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    dy = -1;
                    break;
                case ConsoleKey.DownArrow:
                    dy = 1;
                    break;
                case ConsoleKey.LeftArrow:
                    dx = -1;
                    break;
                case ConsoleKey.RightArrow:
                    dx = 1;
                    break;
                default:
                    Console.WriteLine("Invalid key. Try again.");
                    continue;
            }
            if (TryMovePlayer(player, dx, dy, player.Token.Speed, generatorMaze))
                break; // Move successful; exit the loop
            else
                Console.WriteLine("No valid moves in that direction. Try again.");
        }
    }
    
    public static bool TryMovePlayer(Player player, int dx, int dy, int steps, MazeGeneration generatorMaze)
{
    int currentX = player.Position.x;
    int currentY = player.Position.y;

    for (int step = 1; step <= steps; step++)
    {
        int nextX = currentX + dx * step;
        int nextY = currentY + dy * step;

        Console.WriteLine($"Player {player.Name} is at ({currentX}, {currentY}). Checking move to ({nextX}, {nextY}).");

        // Check if the new position is within bounds and not a wall
        if (generatorMaze.IsWall(nextX, nextY))
        {
            Console.WriteLine($"Blocked by a wall at ({nextX}, {nextY}). Trying fewer steps.");
            if (step > 1) // Reduce the steps by one if it's not the first step
            {
                return TryMovePlayer(player, dx, dy, step - 1, generatorMaze); // Retry with fewer steps
            }
            return false; // If it's the first step, movement is blocked
        }

        // Move to the next valid position
        currentX = nextX;
        currentY = nextY;
        Console.WriteLine($"{player.Name} moved to ({currentX}, {currentY}).");
    }

    // Update player's final position after the full movement
    player.Position = (currentX, currentY);
    Console.WriteLine($"{player.Name} finished moving to ({currentX}, {currentY}).");

    // Check for traps at the final position
    Trap? trap = generatorMaze.IsTrapAtPosition(currentX, currentY);
    if (trap != null)
    {
        trap.ApplyEffect(player);
    }

    return true; // Movement successful
}


// Validates if the player can move to the new position
public static bool IsValidMove(int newX, int newY, MazeGeneration generatorMaze)
{
    if (newX < 0 || newY < 0 || newX >= generatorMaze.Size || newY >= generatorMaze.Size)
    {
        return false; // Out of bounds check

    }
        
    if (generatorMaze.IsWall(newX, newY))
    {
        return false; // Wall check

    }
        
    return true;
}
}
