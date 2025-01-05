
    
using System;
using System.Formats.Asn1;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Globalization;
using System.Resources;
using System.Reflection;

class Program
{ 
    static List<Player> players = new List<Player>();

    static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Program).Assembly);


    static void Main(string[] args)
    {
        Console.WriteLine("Select language / Seleccione idioma:");
    Console.WriteLine("1. English");
    Console.WriteLine("2. Español");

    string? languageChoice = Console.ReadLine();
    
    while (true)
    {
        languageChoice = Console.ReadLine();
        if (languageChoice == "1")
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");
            break;  // Exit the loop once a valid input is received
        }
        else if (languageChoice == "2")
        {
            CultureInfo.CurrentCulture = new CultureInfo("es");
            break;  // Exit the loop once a valid input is received
        }
        else
        {
            // If input is invalid, show an error and prompt again
            Console.WriteLine("Invalid input. Please enter '1' for English or '2' for Español.");
        }
    }
    
    
        bool playAgain=true;
        while(playAgain==true)
        {
            playAgain=startGame();
        }
        Console.WriteLine(resourceManager.GetString("ThankYou"));

    }
    static bool startGame()
    {
        Console.WriteLine(resourceManager.GetString("WelcomeMessage")); // "Welcome to the Maze Game!"
        Console.WriteLine(resourceManager.GetString("EnterMazeSize"));
        int size;

        while (!int.TryParse(Console.ReadLine(), out size) || size < 5)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMazeSize"));
        }

        // Create the maze
        MazeGeneration generatorMaze = new MazeGeneration(size);// Create the maze
        
        Token[] tokens = TokenFactory.GetAvailableTokens();

        Console.WriteLine(resourceManager.GetString("AvailableTokens"));
        for (int i = 0; i < tokens.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {tokens[i]}");
        }

        Console.WriteLine(resourceManager.GetString("Player1ChooseToken"));
        int choice1;
         
        while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || choice1 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice1--;  // Adjust for 0-based indexing

        var player1Position=GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player1 = new Player("Player 1", tokens[choice1], player1Position.x, player1Position.y, generatorMaze);
        generatorMaze.SetPlayer1Position(player1Position.x, player1Position.y);



        Console.WriteLine(resourceManager.GetString("Player2ChooseToken")); 
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice2--;

        var player2Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player2 = new Player("Player 2", tokens[choice2], player2Position.x, player2Position.y, generatorMaze);
        generatorMaze.SetPlayer2Position(player2Position.x, player2Position.y);

        string? playerChosenTokenTemplate = resourceManager.GetString("PlayerChoseToken");
if (!string.IsNullOrEmpty(playerChosenTokenTemplate))
{
    Console.WriteLine(string.Format(playerChosenTokenTemplate, "Player 1", tokens[choice1].Name));
    Console.WriteLine(string.Format(playerChosenTokenTemplate, "Player 2", tokens[choice2].Name));
}
else
{
    Console.WriteLine("Error: Resource string for 'PlayerChoseToken' not found.");
}


        players = new List<Player> { player1, player2 };
        generatorMaze.GenerateTeleportationPortal(); 



          while (true)
        {
            foreach(var player in players)
            {
                //AnsiConsole.Clear();
                System.Threading.Thread.Sleep(500); 

                generatorMaze.PrintMazeSpectre();

                string? playerTurnTemplate = resourceManager.GetString("PlayerTurn");
if (!string.IsNullOrEmpty(playerTurnTemplate))
{
    Console.WriteLine(string.Format(playerTurnTemplate, player.Name, player.Position));
}
else
{
    Console.WriteLine("Error: Resource string for 'PlayerTurn' not found.");
}

                if (player.SkipTurns > 0)
                {
                    Console.WriteLine(resourceManager.GetString("SkippingTurn"));
                    player.SkipTurns--;  // Decrease the skip count
                    continue; 
                }
                player.Token.ReduceCooldown();
                player.CheckCooldownAndRestoreSpeed();

                Console.WriteLine(resourceManager.GetString("UseAbilityPrompt")); // "Do you want to use your ability? (Y/N):"
                string? input = Console.ReadLine();

                while (input == null || input.ToUpper() != "Y" && input.ToUpper() != "N")
                {
                    Console.WriteLine(resourceManager.GetString("InvalidInput"));
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

                    //AnsiConsole.Clear();
                    System.Threading.Thread.Sleep(500); 
                    //generatorMaze.PrintMazeSpectre();

                
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
                string? victoryMessageTemplate = resourceManager.GetString("VictoryMessage");
if (!string.IsNullOrEmpty(victoryMessageTemplate))
{
    Console.WriteLine(string.Format(victoryMessageTemplate, winner));
}
else
{
    Console.WriteLine("Error: Resource string for 'VictoryMessage' not found.");
}
 // "{Player} has reached the exit and won the game!"
                Console.WriteLine(resourceManager.GetString("PlayAgain"));
 // "Do you want to play again? (Y/N):"

                 string? playAgainInput = Console.ReadLine()?.ToUpper();
while (playAgainInput != "Y" && playAgainInput != "N")
{
    Console.WriteLine(resourceManager.GetString("InvalidInput")); // "Invalid input. Please enter 'Y' for Yes or 'N' for No:"
    playAgainInput = Console.ReadLine()?.ToUpper(); // Keep reading until valid input
}

// Play again if 'Y', otherwise stop the game
return playAgainInput == "Y";

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
                
                Trap? trap = generatorMaze.IsTrapAtPosition(player.Position.x, player.Position.y);
                if (trap != null)// && player.Token.Name != "Elf")
                {
                    Console.WriteLine($"Trap {trap.Name} triggered at ({player.Position.x}, {player.Position.y}) while stepping back.");
                    trap.ApplyEffect(player);
                }
                return true;
                }
                /*
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
            */
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
        Trap? trap = generatorMaze.IsTrapAtPosition(finalX, finalY);
        if (trap != null)
        {
            Console.WriteLine($"Trap {trap.Name} found at ({finalX}, {finalY})");
                trap.ApplyEffect(player);  // Only call ApplyEffect if trap is not null
            }
            else{
                Console.WriteLine("No trap at the final position.");
            }
            generatorMaze.CheckTeleportation(player);
        // Update player position in the maze after the move
        if (player == players[0]) //DELETED FROM HERE
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

