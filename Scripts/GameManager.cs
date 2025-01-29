using System.Resources;
public static class GameManager
{
    static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings",  typeof(Program).Assembly);
    public static Player? Player1 {get; set;}
    public static Player? Player2 {get; set;}

    public static void InitializePlayers(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
    public static void HandlesMovement(Player player, MazeCreation generatorMaze, string input)
    {
        while (true) // Seguir intentando hasta que el jugador se mueva o cambie de dirección
        {
            
            Console.WriteLine(resourceManager.GetString("ArrowKeyPrompt"));
            ConsoleKeyInfo key = Console.ReadKey(true); 

            int dx= 0, dy= 0;
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

            if (Program.TryMove(player, dx, dy, player.Token.Speed, generatorMaze))
            {
                
                break; // Se movió exitosamente, sal del ciclo
            }  
                
            else
                Console.WriteLine(resourceManager.GetString("InvalidMove"));
        }
    }
    public static bool IsValidMove(int newX, int newY, MazeCreation generatorMaze)
    {
        if (newX< 0 || newY< 0 || newX >= generatorMaze.Size || newY >= generatorMaze.Size)
        {   
            return false;

        }
            
        if (generatorMaze.IsWall(newX, newY))
        {
            return false; 
        }

        Trap? trapAtPosition = generatorMaze.IsTrapAtPosition(newX, newY);
        if (trapAtPosition != null)
        {
            if (trapAtPosition.Triggered)
            {
                Console.WriteLine(resourceManager.GetString("TrapAlreadyTriggered"));
                return true; // Permitir movimiento porque la trampa ya fue activada
            }
            else
            {
                
                return true; // Permitir movimiento auque la trampa no esté activada
            }
        }
            
        return true;
    }
    public static string? Win(Player player, (int x, int y) exit)
    {
        if (player.Position.x == exit.x && player.Position.y == exit.y)
        {
            return player.Name; 
        }
        return null; //Todavía no hay un ganador
    }
     public static (int x, int y) GetRandomValidPosition(MazeCreation maze, (int x, int y) exit)
    {
        Random random = new Random();
        int size = maze.Size;

        while (true)
        {
            int x = random.Next(0, size);
            int y = random.Next(0, size);

            if (!maze.IsWall(x, y) && maze.IsTrapAtPosition(x, y) == null && (x, y) != exit) // Asegurar que no sea una pared, haya una trampa o la salida
            {
                return (x, y);
            }
        }
    }
    public static void CheckTheTeleportation(Player player, (int x, int y) randomTeleportPortal, MazeCreation generatorMaze)
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
            player.Position = GetRandomValidPosition(generatorMaze, generatorMaze.exit); 
        }
    }
}