using System.Globalization;
using System.Resources;
class Program
{ 
    static List<Player> players = new List<Player>();
    static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings",  typeof(Program).Assembly);
    static void Main(string[] args)
    {
        MainMenu.ShowMenu();
    
        bool playAgain=startGame(); //true;
        while(playAgain)
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

        while (!int.TryParse(Console.ReadLine(), out size) || size < 7)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMazeSize"));
        }

        MazeGeneration generatorMaze = new MazeGeneration(size);
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
        choice1--;  

        var player1Position=GameManager.GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player1 = new Player("P1", tokens[choice1], player1Position.x, player1Position.y, generatorMaze);
        generatorMaze.SetPlayer1Position(player1Position.x, player1Position.y);

        Console.WriteLine(resourceManager.GetString("Player2ChooseToken")); 
        int choice2;

        while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || choice2 > tokens.Length)
        {
            Console.WriteLine(resourceManager.GetString("InvalidTokenChoice")); 
        }
        choice2--;

        var player2Position =GameManager.GetRandomValidPosition(generatorMaze, generatorMaze.exit);
        Player player2 = new Player("P 2", tokens[choice2], player2Position.x, player2Position.y, generatorMaze);
        generatorMaze.SetPlayer2Position(player2Position.x, player2Position.y);

        string? playerChosenTokenTemplate = resourceManager.GetString("PlayerChoseToken");
        if (!string.IsNullOrEmpty(playerChosenTokenTemplate))
        {
            Console.WriteLine(string.Format(playerChosenTokenTemplate, "P1", tokens[choice1].Name));
            Console.WriteLine(string.Format(playerChosenTokenTemplate, "P2", tokens[choice2].Name));
        }

        players = new List<Player> { player1, player2 };
        generatorMaze.GenerateTeleportationPortal(); 
        GameManager.InitializePlayers(player1, player2);

          while (true)
        {
            foreach(var player in players)
            {
                System.Threading.Thread.Sleep(500); 
                MazePrinter mazePrinter = new MazePrinter(generatorMaze);
                mazePrinter.PrintMazeSpectre();

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
                player.Token.ReduceCooldown();
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
                        Console.WriteLine(resourceManager.GetString("InvalidInput")); 
                        input = Console.ReadLine();
                    }
                }
                GameManager.HandleMovement(player, generatorMaze,input);

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
                    string? victoryMessageTemplate = resourceManager.GetString("VictoryMessage");
                    if (!string.IsNullOrEmpty(victoryMessageTemplate))
                    {
                        Console.WriteLine(string.Format(victoryMessageTemplate, winner));
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
                            Console.WriteLine(resourceManager.GetString("InvalidInput")); // "Invalid input. Please enter 1 for Yes or 0 for No."
                            playAgainInput = Console.ReadLine();
                        }
                    }

                }
            }
        }
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
    public static bool TryMovePlayer(Player player, int dx, int dy, int steps, MazeGeneration generatorMaze)
    {
        int startX = player.Position.x;
        int startY= player.Position.y;

        int maxSteps = player.Token.Speed;
        steps = Math.Min(steps, maxSteps);

        if (steps== 0)
        {
            Console.WriteLine(resourceManager.GetString("InvalidMove"));
            return false;
        }

        int totalStepsMoved = 0;

        for (int step = 1; step <= steps; step++) // Intentar mover casilla a casilla
        {
            int nextX = startX + dx * step;
            int nextY = startY + dy * step;

            if (!GameManager.IsValidMove(nextX, nextY, generatorMaze)) // Ver si la nueva posición esta dentro del tablero
            {
                if (step > 1) 
                {
                    player.Position = (startX + dx * (step - 1), startY + dy * (step - 1)); //Actualizar la posición del jugador a la última válida
                    UpdatePlayerPositionInMaze(player, generatorMaze);
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
        int finalX = startX + dx * totalStepsMoved; //Actualizar la posición del jugador despu+es del movimiento exitoso
        int finalY = startY + dy * totalStepsMoved;

        if (GameManager.IsValidMove(finalX, finalY, generatorMaze)) //Asegurar que la última posición está dentro del laberinto
        {
            player.Position = (finalX, finalY);
            
            UpdatePlayerPositionInMaze(player, generatorMaze);
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