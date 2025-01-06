
    
using System;
using System.Formats.Asn1;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;

class Program
{ 
    static List<Player> players = new List<Player>();

    static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings",  typeof(Program).Assembly);
    
    static void Main(string[] args)
    {
        Console.WriteLine("Select the language you will play in / Seleccione el idioma con el que jugará:");
        Console.WriteLine("1. English");
        Console.WriteLine("2. Español");

        string? languageChoice = Console.ReadLine();
    
        while (true)
        {
            if (languageChoice == "1")
            {
                Console.WriteLine($"Language set to: {CultureInfo.CurrentCulture.Name}");
                CultureInfo.CurrentCulture = new CultureInfo("en");
                break;  
            }
            if (languageChoice=="2")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-Es");
                CultureInfo.CurrentCulture = new CultureInfo("es");
                break;  
            }
            else
            {
                Console.WriteLine("Invalid input / Entrada no válida. '1' for English o '2' para Español.");
                languageChoice = Console.ReadLine();
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
        Console.WriteLine("");
        Console.WriteLine(resourceManager.GetString("WelcomeMessage"));
        Console.WriteLine("");
        Console.WriteLine(resourceManager.GetString("EnterMazeSize"));
        int size;

        while (!int.TryParse(Console.ReadLine(), out size) || size < 5)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMazeSize"));
        }

        // Create the maze
        MazeGeneration generatorMaze = new MazeGeneration(size);// Create the maze
        
        Token[] tokens = TokenFactory.GetAvailableTokens();

        Console.WriteLine("");
        Console.WriteLine(resourceManager.GetString("AvailableTokens"));
        Console.WriteLine("");

        for (int i = 0; i < tokens.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {tokens[i]}");
        }

        Console.WriteLine("");
        Console.WriteLine(resourceManager.GetString("Player1ChooseToken"));
        int choice1;
         
        while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || choice1 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice1--;  // Adjust for 0-based indexing

        var player1Position=GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        
        Player player1 = new Player("P 1", tokens[choice1], player1Position.x, player1Position.y, generatorMaze);
        generatorMaze.SetPlayer1Position(player1Position.x, player1Position.y);

        Console.WriteLine(resourceManager.GetString("Player2ChooseToken")); 
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice2--;

        var player2Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player2 = new Player("P 2", tokens[choice2], player2Position.x, player2Position.y, generatorMaze);
        generatorMaze.SetPlayer2Position(player2Position.x, player2Position.y);

        string? playerChosenTokenTemplate = resourceManager.GetString("PlayerChoseToken");
if (!string.IsNullOrEmpty(playerChosenTokenTemplate))
{
    Console.WriteLine(string.Format(playerChosenTokenTemplate, "P 1", tokens[choice1].Name));
    Console.WriteLine(string.Format(playerChosenTokenTemplate, "P 2", tokens[choice2].Name));
}
else
{
    Console.WriteLine("Error: Resource string for 'PlayerChoseToken' not found.");
}


        players = new List<Player> { player1, player2 };
        generatorMaze.GenerateTeleportationPortal(); 
        // After creating player1 and player2
GameManager.InitializePlayers(player1, player2);




          while (true)
        {
            foreach(var player in players)
            {
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

                Console.WriteLine();
                //
                Console.WriteLine(resourceManager.GetString("UseAbilityPrompt")); // "Do you want to use your ability? (Y/N):"
                string? input = Console.ReadLine();

                while (true)
                {
    if (input == "1")
    {
                    player.HasUsedAbility = true;
        Player target = player == players[0] ? players[1] : players[0];
        player.Token.UseAbility(player, target);
        break;
    }
    else if (input == "0")
    {
        player.HasUsedAbility = false;
        break;
    }
    else
    {
        Console.WriteLine(resourceManager.GetString("InvalidInput")); // "Invalid input. Please enter 1 for Yes or 0 for No."
        input = Console.ReadLine();
    }
                //
                //Console.WriteLine(resourceManager.GetString("UseAbilityPrompt")); // "Do you want to use your ability? (1 for Yes, 0 for No):"





        
}

                   
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
        string? cooldownReducedTemplate = resourceManager.GetString("CooldownReduced");
if (!string.IsNullOrEmpty(cooldownReducedTemplate))
{
    Console.WriteLine(string.Format(cooldownReducedTemplate, player.Name, player.Token.CooldownTime));
}
else
{
    Console.WriteLine("Error: Resource string for 'CooldownReduced' not found.");
}


    }
    break;


        case "Speed Increase":
            player.Token.Speed += 1;
            string? speedIncreasedTemplate = resourceManager.GetString("SpeedIncreased");
if (!string.IsNullOrEmpty(speedIncreasedTemplate))
{
    Console.WriteLine(string.Format(speedIncreasedTemplate, player.Name));
}
else
{
    Console.WriteLine("Error: Resource string for 'SpeedIncreased' not found.");
}

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

                 string? playAgainInput = Console.ReadLine();

while (true)
{
    if (playAgainInput == "1")
    {
        return true; // Play again
    }
    else if (playAgainInput == "0")
    {
        return false; // Don't play again
    }
    else
    {
        Console.WriteLine(resourceManager.GetString("InvalidInput")); // "Invalid input. Please enter 1 for Yes or 0 for No."
        playAgainInput = Console.ReadLine();
    }
}

            }
        }
    }
}

public static void HandleMovement(Player player, MazeGeneration generatorMaze, string input)
{
    while (true) // Retry until the player successfully moves or changes direction
    {
        
        Console.WriteLine(resourceManager.GetString("ArrowKeyPrompt"));
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
                Console.WriteLine(resourceManager.GetString("InvalidArrowKey"));
                continue;
        }

        // Attempt to move the player
        if (TryMovePlayer(player, dx, dy, player.Token.Speed, generatorMaze))
        {
            
            break;
        }  
             // Move successful; exit the loop
        else
            Console.WriteLine(resourceManager.GetString("InvalidMove"));
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
        Console.WriteLine(resourceManager.GetString("InvalidMove"));
        return false;
    }

    int totalStepsMoved = 0;

    // Try moving step by step
    for (int step = 1; step <= steps; step++)
    {
        int nextX = startX + dx * step;
        int nextY = startY + dy * step;

        

        // Check if the new position is within bounds and not a wall
        if (!IsValidMove(nextX, nextY, generatorMaze))
        {
            

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
                    
                    trap.ApplyEffect(player);
                }
                return true;
                }
                
            else
            {
                // If the first step is blocked, stop the movement entirely
                //Console.WriteLine("No valid moves in this direction. You are stuck!");// TURN TO RESX
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
        
        UpdatePlayerPositionInMaze(player, generatorMaze);
        Trap? trap = generatorMaze.IsTrapAtPosition(finalX, finalY);
        if (trap != null)
        {
            //Console.WriteLine($"Trap {trap.Name} found at ({finalX}, {finalY})");// TURN TO RESX
                trap.ApplyEffect(player);  // Only call ApplyEffect if trap is not null
            }
    
            generatorMaze.CheckTeleportation(player);
        // Update player position in the maze after the move
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
        //Console.WriteLine($"Blocked by a wall at ({finalX}, {finalY}). Cannot move there.");// TURN TO RESX
        return false; // If final position is invalid, return false
    }
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
     Trap? trapAtPosition = generatorMaze.IsTrapAtPosition(newX, newY);
    if (trapAtPosition != null)
    {
        // If the trap has been triggered, it's still considered open for movement
        if (trapAtPosition.Triggered)
        {
            Console.WriteLine(resourceManager.GetString("TrapAlreadyTriggered"));
            return true; // Allow movement even though the trap is triggered
        }
        else
        {
            
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
    if (player.Position == randomTeleportPortal)
    {
        string? teleportRandomTemplate = resourceManager.GetString("TeleportRandom");
if (!string.IsNullOrEmpty(teleportRandomTemplate))
{
    Console.WriteLine(string.Format(teleportRandomTemplate, player.Name));
}
else
{
    Console.WriteLine("Error: Resource string for 'TeleportRandom' not found.");
}

        player.Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit); // Teleport to a random valid position
    }
}




}

