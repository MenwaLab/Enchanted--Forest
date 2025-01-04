using System;

class Program
{ 
    static List<Player> players = new List<Player>();
    static void Main(string[] args)
    {
        bool playAgain=true;
        while(playAgain==true)
        {
            playAgain=startGame();
        }
        Console.WriteLine("Muchas gracias por jugar Enchanted Forest. Hasta la próxima! -Meyli");
    }
    static bool startGame()
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
         
        while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || choice1 > tokens.Length)
        {
            Console.WriteLine($"Por favor introduzca un número válido para su ficha (entre 1 y {tokens.Length}): ");
        }
        choice1--;  // Adjust for 0-based indexing

        var player1Position=GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player1 = new Player("Player 1", tokens[choice1], player1Position.x, player1Position.y, generatorMaze);
        generatorMaze.SetPlayer1Position(player1Position.x, player1Position.y);



        Console.WriteLine("Player 2, choose your token by entering its number: ");
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > tokens.Length)
        {
            Console.WriteLine($"Por favor introduzca un número válido para su ficha (entre 1 y {tokens.Length}): ");
        }
        choice2--;

        var player2Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player2 = new Player("Player 2", tokens[choice2], player2Position.x, player2Position.y, generatorMaze);
        generatorMaze.SetPlayer2Position(player2Position.x, player2Position.y);

        Console.WriteLine($"Player 1 chose {player1.Token.Name}");
        Console.WriteLine($"Player 2 chose {player2.Token.Name}. Empezemos el juego!!!");

        //List<Player> players = new List<Player> { player1, player2 };
        players = new List<Player> { player1, player2 };
        generatorMaze.GenerateTeleportationPortal(); 



          while (true)
        {
            foreach(var player in players)
            {
                generatorMaze.PrintMazeSpectre();
                Console.WriteLine($"{player.Name}, it's your turn. Tu posicion es {player.Position}");
                if (player.SkipTurns > 0)
                {
                    Console.WriteLine($"{player.Name} is skipping a turn.");
                    player.SkipTurns--;  // Decrease the skip count
                    continue; 
                }
                player.Token.ReduceCooldown();
                player.CheckCooldownAndRestoreSpeed();

                Console.WriteLine("Do you want to use your ability? (Y/N): ");
                string? input = Console.ReadLine();

                while (input == null || input.ToUpper() != "Y" && input.ToUpper() != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' for Yes or 'N' for No:");
                    input = Console.ReadLine(); // Keep reading until valid input
                }
                player.HasUsedAbility = input.ToUpper() == "Y";
                
                if (player.HasUsedAbility)
                {
                    Player target = player == players[0] ? players[1] : players[0];
                    player.Token.UseAbility(player, target);
                }
                    //Console.WriteLine("Muevase de acuerdo a las teclas");
                    HandleMovement(player, generatorMaze,input);

                
                player.Token.Speed = player.Token.Speed > player.Token.BaseSpeed
                    ? player.Token.BaseSpeed
                    : player.Token.Speed;

                string tileType;
if (generatorMaze.IsBeneficialTile(player.Position.x,player.Position.y, out tileType))
{
    switch (tileType)
    {
        case "Cooldown Reduction":
    if (player.Token.CooldownTime > 0)
    {
        player.Token.CooldownTime = Math.Max(0, player.Token.CooldownTime - 2);
        Console.WriteLine($"{player.Name} reduced cooldown by 2! Current cooldown: {player.Token.CooldownTime}");
    }
    break;


        case "Speed Increase":
            player.Token.Speed += 1;
            Console.WriteLine($"{player.Name} increased speed by 1!");
            break;
    }
}
            
            
            // Check victory condition after Player 1's turn
            string? winner = Win(player, generatorMaze.exit);
            if (winner != null)
            {
                Console.WriteLine($"{winner} has reached the exit and won the game!");
                Console.WriteLine("Quieres jugar otra vez? S/N");

                string? jugarOtravez=Console.ReadLine();
                while (jugarOtravez == null || jugarOtravez.ToUpper() != "S" && jugarOtravez.ToUpper() != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'S' for Si or 'N' for No:");
                    jugarOtravez = Console.ReadLine(); // Keep reading until valid input
                }
                
                return jugarOtravez.ToUpper() == "S";
            }
        }
    }
}

public static void HandleMovement(Player player, MazeGeneration generatorMaze, string input)
{
    while (true) // Retry until the player successfully moves or changes direction
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

        // Attempt to move the player
        if (TryMovePlayer(player, dx, dy, player.Token.Speed, generatorMaze))
        {
            //generatorMaze.PrintMaze();
            break;
        }  
             // Move successful; exit the loop
        else
            Console.WriteLine("No valid moves in that direction. Try again.");
    }
}


public static bool TryMovePlayer(Player player, int dx, int dy, int steps, MazeGeneration generatorMaze)
{
    int startX = player.Position.x;
    int startY = player.Position.y;

    int maxSteps = player.Token.Speed;
    steps = Math.Min(steps, maxSteps);

    // If no steps to take, return false
    if (steps == 0)
    {
        Console.WriteLine("No valid moves in that direction. Please change direction.");
        return false;
    }

    int totalStepsMoved = 0;

    // Try moving step by step
    for (int step = 1; step <= steps; step++)
    {
        int nextX = startX + dx * step;
        int nextY = startY + dy * step;

        Console.WriteLine($"Player {player.Name} is at ({startX}, {startY}). Checking move to ({nextX}, {nextY}).");

        // Check if the new position is within bounds and not a wall
        if (!IsValidMove(nextX, nextY, generatorMaze))
        {
            Console.WriteLine($"Blocked by a wall at ({nextX}, {nextY}).");

            // If we're not at the last step, stop and return the current valid position
            if (step > 1)
            {
                //Console.WriteLine($"Stopping at ({startX + dx * (step - 1)}, {startY + dy * (step - 1)})");
                player.Position = (startX + dx * (step - 1), startY + dy * (step - 1)); // Update position to the last valid point
                UpdatePlayerPositionInMaze(player, generatorMaze);
                generatorMaze.CheckTeleportation(player);
                //return true;
                if (player == players[0])
                {
                    generatorMaze.SetPlayer1Position(player.Position.x, player.Position.y); // Update Player 1's position
                }
                else
                {
                    generatorMaze.SetPlayer2Position(player.Position.x, player.Position.y); // Update Player 2's position
                }
                return true;
            }
            else
            {
                // If the first step is blocked, stop the movement entirely
                Console.WriteLine("No valid moves in this direction. You are stuck!");
                return false;
            }
        }

        totalStepsMoved++; // Successfully moved a step
    }

    // Update the player's position after successful movement
    int finalX = startX + dx * totalStepsMoved;
    int finalY = startY + dy * totalStepsMoved;

    // Ensure the final position is within bounds and not a wall
    if (IsValidMove(finalX, finalY, generatorMaze))
    {
        // Update the player's position
        player.Position = (finalX, finalY);
        Console.WriteLine($"{player.Name} finished moving to ({finalX}, {finalY}). Speed is {player.Token.Speed}");
        UpdatePlayerPositionInMaze(player, generatorMaze);
        // Update player position in the maze after the move
        if (player == players[0])
        {
            generatorMaze.SetPlayer1Position(player.Position.x, player.Position.y); // Update Player 1's position
        }
        else
        {
            generatorMaze.SetPlayer2Position(player.Position.x, player.Position.y); // Update Player 2's position
        }
        // Check for traps at the final position
        Trap? trap = generatorMaze.IsTrapAtPosition(finalX, finalY);
        
        if (trap != null)
{
    Console.WriteLine($"Trap {trap.Name} found at ({finalX}, {finalY})");
    if (player.Token.Name == "Elf" && player.Token.CurrentCooldown == 0 && player.HasUsedAbility)
    {
        // Elf triggers the trap but doesn't suffer the effect
        Console.WriteLine($"Trap {trap.Name} triggered, but {player.Name}'s Elf ability nullifies its effect.");
    }
    else
    {
        trap.ApplyEffect(player);  // Only call ApplyEffect if trap is not null
    }
}
else{
    Console.WriteLine("No trap at the final position.");
}
        generatorMaze.CheckTeleportation(player);
        return true; // Movement successful
    }
    else
    {
        Console.WriteLine($"Blocked by a wall at ({finalX}, {finalY}). Cannot move there.");
        return false; // If final position is invalid, return false
    }

}

 
// Validates if the player can move to the new position
public static bool IsValidMove(int newX, int newY, MazeGeneration generatorMaze)
{
    if (newX < 0 || newY < 0 || newX >= generatorMaze.Size || newY >= generatorMaze.Size)
    {
        //Console.WriteLine($"Position ({newX}, {newY}) is outside the maze bounds.");
        return false; // Out of bounds check

    }
        
    if (generatorMaze.IsWall(newX, newY))
    {
        return false; // Wall check

    }
     Trap? trapAtPosition = generatorMaze.IsTrapAtPosition(newX, newY);
    if (trapAtPosition != null)
    {
        // If the trap has been triggered, it's still considered open for movement
        if (trapAtPosition.Triggered)
        {
            Console.WriteLine($"Trap {trapAtPosition.Name} has been triggered but the path is open for movement.");
            return true; // Allow movement even though the trap is triggered
        }
        else
        {
            //Console.WriteLine($"Trap {trapAtPosition.Name} is not triggered yet.");
            return true; // Allow movement to a trap that hasn't been triggered yet
        }
    }
        
    return true;
}
private static void UpdatePlayerPositionInMaze(Player player, MazeGeneration generatorMaze)
{
    if (player == players[0])
    {
        generatorMaze.SetPlayer1Position(player.Position.x, player.Position.y);
    }
    else
    {
        generatorMaze.SetPlayer2Position(player.Position.x, player.Position.y);
    }
}

public static string? Win(Player player, (int x, int y) exit)
{
    if (player.Position.x == exit.x && player.Position.y == exit.y)
    {
        return player.Name; // Return the player's name who won
    }
    return null; // No winner yet
}


public static (int x, int y) GetRandomValidPosition(MazeGeneration maze, (int x, int y) exit)
{
    Random random = new Random();
    int size = maze.Size;

    while (true)
    {
        int x = random.Next(0, size);
        int y = random.Next(0, size);

        // Ensure the position is not a wall, trap, or the exit
        if (!maze.IsWall(x, y) && maze.IsTrapAtPosition(x, y) == null && (x, y) != exit)
        {
            return (x, y);
        }
    }
}
public static void CheckTeleportation(Player player, (int x, int y) backToStartPortal, (int x, int y) randomTeleportPortal, (int x, int y) startPosition, MazeGeneration generatorMaze)
{
    if (player.Position == backToStartPortal)
    {
        Console.WriteLine($"{player.Name} stepped on the back-to-start portal! Teleporting back to start...");
        player.Position = startPosition; // Teleport back to the start
    }
    else if (player.Position == randomTeleportPortal)
    {
        Console.WriteLine($"{player.Name} stepped on the random teleport portal! Teleporting to a random position...");
        player.Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit); // Teleport to a random valid position
    }
}




}
