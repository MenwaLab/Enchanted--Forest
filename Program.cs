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
                string? input = Console.ReadLine(); // Read and convert to uppercase to simplify checking
                while (input == null || input.ToUpper() != "Y" && input.ToUpper() != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' for Yes or 'N' for No:");
                    input = Console.ReadLine(); // Keep reading until valid input
                }
                if (input.ToUpper() == "Y")
                {
                    player1.Token.UseAbility(player1, player2);
                }
                    //Console.WriteLine("Muevase de acuerdo a las teclas");
                    HandleMovement(player1, generatorMaze);
            }
            // Check victory condition after Player 1's turn
    string? winner = Win(player1, generatorMaze.exit);
    if (winner != null)
    {
        Console.WriteLine($"{winner} has reached the exit and won the game!");
        break; // End the game
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
                string? input = Console.ReadLine(); // Read and convert to uppercase to simplify checking
                while (input == null || input.ToUpper() != "Y" && input.ToUpper() != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' for Yes or 'N' for No:");
                    input = Console.ReadLine(); // Keep reading until valid input
                }
                if (input.ToUpper() == "Y")
                {
                    player2.Token.UseAbility(player1, player2);
                }
                    HandleMovement(player2, generatorMaze);
            }
            // Check victory condition after Player 1's turn
     winner = Win(player2, generatorMaze.exit);

    if (winner != null)
    {
        Console.WriteLine($"{winner} has reached the exit and won the game!");
        break; // End the game
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
                    dx = -1;
                    break;
                case ConsoleKey.DownArrow:
                    dx = 1;
                    break;
                case ConsoleKey.LeftArrow:
                    dy = -1;
                    break;
                case ConsoleKey.RightArrow:
                    dy = 1;
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
    int startX = player.Position.x;
    int startY = player.Position.y;

    int maxSteps = player.Token.Speed -1;
    steps = Math.Min(steps, maxSteps); 

    if (steps == 0)
    {
        Console.WriteLine("No valid moves in that direction. Please change direction.");
        return false;
    }

    for (int step = 1; step <= steps; step++)
    {
        int nextX = startX + dx * step;
        int nextY = startY + dy * step;

        Console.WriteLine($"Player {player.Name} is at ({startX}, {startY}). Checking move to ({nextX}, {nextY}).");

        // Check if the new position is within bounds and not a wall
        if (!IsValidMove(nextX, nextY, generatorMaze))
        {
            Console.WriteLine($"Blocked by a wall at ({nextX}, {nextY}). Retrying with fewer steps.");
            return TryMovePlayer(player, dx, dy, steps - 1, generatorMaze); // Retry with fewer steps
        }
    }

    // Final valid position after successful movement
    int finalX = startX + dx * steps;
    int finalY = startY + dy * steps;

    // Update player's position
    player.Position = (finalX, finalY);
    Console.WriteLine($"{player.Name} finished moving to ({finalX}, {finalY}). speed is {player.Token.Speed}");

    // Check for traps at the final position
    Trap? trap = generatorMaze.IsTrapAtPosition(finalX, finalY);
    if (trap != null)
    {
        trap.ApplyEffect(player);
    }

    return true; // Movement successful
}


public static bool ExploreAlternativePaths(Player player, int dx, int dy, int remainingSteps, MazeGeneration generatorMaze)
{
    // Try moving left or right (alternating dy or dx based on current direction)
    int[] altDx = { 0, 0, 1, -1 };
    int[] altDy = { 1, -1, 0, 0 };

    for (int i = 0; i < altDx.Length; i++)
    {
        int newDx = altDx[i];
        int newDy = altDy[i];

        // Avoid retrying the same direction
        if (newDx == dx && newDy == dy) continue;

        int nextX = player.Position.x + newDx * remainingSteps;
        int nextY = player.Position.y + newDy * remainingSteps;

        if (IsValidMove(nextX, nextY, generatorMaze))
        {
            Console.WriteLine($"Alternative path found to ({nextX}, {nextY}).");
            player.Position = (nextX, nextY);
            return true;
        }
    }

    Console.WriteLine("No alternative paths found.");
    return false;
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

public static string? Win(Player player, (int x, int y) exit)
{
    if (player.Position.x == exit.x && player.Position.y == exit.y)
    {
        return player.Name; // Return the player's name who won
    }
    return null; // No winner yet
}

}
