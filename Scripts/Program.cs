using System.Resources;
using Spectre.Console;
class Program
{ 
    static List<Player> players = new List<Player>();
    static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings",  typeof(Program).Assembly);
    static void Main(string[] args)
    {
        MainMenu.ShowMenu();
    
        bool playAgain=startGame(); 

        while(playAgain)
        {
            playAgain=startGame();
        }
        Console.WriteLine(resourceManager.GetString("ThankYou"));
    }
    static bool startGame()
    {
        Console.WriteLine("");
        AnsiConsole.MarkupLine($"[chartreuse4]{resourceManager.GetString("WelcomeMessage")}[/]");
        Console.WriteLine("");
        Console.WriteLine(resourceManager.GetString("EnterMazeSize"));
        int size;

        while (!int.TryParse(Console.ReadLine(),out size) || size < 7)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMazeSize"));
        }
        size = Math.Clamp(size, 7, 15); 

        MazeCreation generatorMaze = new MazeCreation(size);

        Token[] tokens = TokenCreation.GetTokens();

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
        choice1--;  

        var player1Position=GameManager.GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player1 = new Player("Player 1", tokens[choice1], player1Position.x, player1Position.y, generatorMaze);
        generatorMaze.SetPlayer1Position(player1Position.x, player1Position.y);

        Console.WriteLine(resourceManager.GetString("Player2ChooseToken")); 
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice2--;

        var player2Position =GameManager.GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player2 = new Player("Player 2", tokens[choice2], player2Position.x, player2Position.y, generatorMaze);
        generatorMaze.SetPlayer2Position(player2Position.x, player2Position.y);

        string? playerChosenTokenTemplate = resourceManager.GetString("PlayerChoseToken");
        if (!string.IsNullOrEmpty(playerChosenTokenTemplate))
        {
            Console.WriteLine(string.Format(playerChosenTokenTemplate, "Player 1", tokens[choice1].Name));
            Console.WriteLine(string.Format(playerChosenTokenTemplate, "Player 2", tokens[choice2].Name));
        }

        players = new List<Player> {player1, player2};
        generatorMaze.GenerateTeleportationPortal(); 
        GameManager.InitializePlayers(player1, player2);

          while (true)
        {
            foreach(var player in players)
            {
                Thread.Sleep(500); //asi los jugadores pueden ajustarse al cambio en la consola
                MazePrinter mazePrinter = new MazePrinter(generatorMaze);
                mazePrinter.PrintMazeWithSpectre();

                string? playerTurnTemplate = resourceManager.GetString("PlayerTurn");
                if (!string.IsNullOrEmpty(playerTurnTemplate))
                {
                    Console.WriteLine(string.Format(playerTurnTemplate, player.Name, player.Position));
                }

                if (player.SkipTurns > 0)
                {
                    Console.WriteLine(resourceManager.GetString("SkippingTurn"));
                    player.SkipTurns--;  
                    continue; 
                }
                player.Token.DecreaseCooldown();
                player.CheckCooldownAndRestoreSpeed();

                Console.WriteLine("");
                Console.WriteLine(resourceManager.GetString("UseAbilityPrompt"));
                
                string? input = Console.ReadLine();
                while (true)
                {
                    if (input == "1")
                    {
                        player.HasUsedAbility = true;
                        Player target = player == players[0] ? players[1] : players[0];
                        player.Token.Ability(player, target);
                        break;
                    }
                    else if (input == "0")
                    {
                        player.HasUsedAbility = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine(resourceManager.GetString("InvalidInput")); 
                        input = Console.ReadLine();
                    }
                }
                GameManager.HandlesMovement(player, generatorMaze,input);

                player.Token.Speed = player.Token.Speed > player.Token.BaseSpeed
                    ? player.Token.BaseSpeed
                    : player.Token.Speed;

                string BenefitialTileType;
                if (generatorMaze.IsBeneficialTile(player.Position.x,player.Position.y, out BenefitialTileType)) 
                {
                    switch (BenefitialTileType)
                    {
                        case "Cooldown Reduction":
                            if (player.Token.CooldownTime > 0)
                            {
                                player.Token.CooldownTime = Math.Max(0, player.Token.CooldownTime - 2); //no permite tiempo de enfriamiento negativo
                                string? cooldownReducedTemplate = resourceManager.GetString("CooldownReduced");
                                if (!string.IsNullOrEmpty(cooldownReducedTemplate))
                                {
                                    Console.WriteLine(string.Format(cooldownReducedTemplate, player.Name, player.Token.CooldownTime));
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
                            break;
                    }
                }

                string? winner =GameManager.Win(player, generatorMaze.exit);
                if (winner != null)
                {
                    string? victoryMessage = resourceManager.GetString("VictoryMessage");
                    var starburst = new FigletText("✨")
                        .Centered()
                        .Color(Color.Yellow);

                    AnsiConsole.Write(starburst);
                    AnsiConsole.MarkupLine("[yellow bold]✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨[/]");
                    AnsiConsole.MarkupLine("[yellow bold]✨✨✨✨✨   🎉🎉🎉🎉🎉🎉  ✨✨✨✨✨[/]");
                    AnsiConsole.MarkupLine("[yellow bold]✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨✨[/]");

                    if (!string.IsNullOrEmpty(victoryMessage))
                    {
                        Console.WriteLine(string.Format(victoryMessage, winner));
                    }
                
                    Console.WriteLine(resourceManager.GetString("PlayAgain"));
                    string? playAgainInput = Console.ReadLine();

                    while (true)
                    {
                        if (playAgainInput == "1")
                        {
                            return true; 
                        }
                        else if (playAgainInput == "0")
                        {
                            return false; 
                        }
                        else
                        {
                            Console.WriteLine(resourceManager.GetString("InvalidInput"));
                            playAgainInput = Console.ReadLine();
                        }
                    }

                }
            }
        }
    }
    private static void UpdatePosition(Player player, MazeCreation generatorMaze)
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
    public static bool TryMove(Player player, int dx, int dy, int move, MazeCreation generatorMaze)
    {
        int startX = player.Position.x;
        int startY= player.Position.y;
        int maxMove = player.Token.Speed;
        move = Math.Min(move, maxMove);

        if (move == 0)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMove"));
            return false;
        }

        int totalStepsMoved = 0;

        for (int steps = 1;steps <=move;steps++) // Intentar mover casilla a casilla
        {
            int nextX = startX + dx *steps;
            int nextY = startY + dy *steps;

            if (!GameManager.IsValidMove(nextX, nextY, generatorMaze)) // Ver si la nueva posición está dentro del tablero
            {
                if (steps > 1) 
                {
                    player.Position = (startX + dx * (steps - 1), startY + dy * (steps - 1)); //Actualizar la posición del jugador a la última válida
                    UpdatePosition(player, generatorMaze);
                    generatorMaze.CheckTeleportation(player);
                    
                    Trap? trap = generatorMaze.IsTrapAtPosition(player.Position.x, player.Position.y);
                    if (trap !=null)
                    {
                        trap.ApplyEffect(player);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            totalStepsMoved++; // Exitosamente movió una casilla
        }
        int finalX = startX + dx * totalStepsMoved; //Actualizar la posición del jugador después del movimiento exitoso
        int finalY = startY + dy * totalStepsMoved;

        if (GameManager.IsValidMove(finalX, finalY, generatorMaze)) //Asegurar que la última posición está dentro del laberinto
        {
            player.Position = (finalX, finalY);
            
            UpdatePosition(player, generatorMaze);
            Trap? trap = generatorMaze.IsTrapAtPosition(finalX, finalY);
            if (trap != null)
            {
                trap.ApplyEffect(player);  
            }
        
            generatorMaze.CheckTeleportation(player);

            if (player == players[0]) 
            {
                generatorMaze.SetPlayer1Position(player.Position.x, player.Position.y); 
            }
            else
            {
                generatorMaze.SetPlayer2Position(player.Position.x, player.Position.y); 
            }
            return true;
        }
        else
        {
            return false; 
        }
    }
}