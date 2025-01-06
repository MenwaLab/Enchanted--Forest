using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Resources;


public class MazeGeneration
{
    static ResourceManager resourceManager4 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    
    private Cell[,] maze;
    private int size;
    private Random rand = new Random();
    private List<Trap> traps = new List<Trap>(); 
    public (int x, int y) exit;
    public (int x, int y)? cooldownReductionTile = null;
    public (int x, int y)? speedIncreaseTile = null; 
    private (int x, int y) player1Pos;
    private (int x, int y) player2Pos;
    public (int x, int y)? randomTeleportPortal = null; // Teleports to a random valid position

    public MazeGeneration(int size)
    { 
        this.size = size;
        this.maze = new Cell[size,size];

        for(int i=0; i< size;i++)
        {
            for(int j=0; j<size;j++)
            {
                maze[i,j]=new Cell(false); // Todo paredes
            }
        }
        
        GenerateTheMaze(0, 0);
        SetExit();
        GenerateTraps();
        GenerateBeneficialTiles();
    }
    public int Size => size;
    private void GenerateTheMaze(int x, int y) // recursive backtracking 
{
    var directions = new (int dx, int dy)[]
    {
        (1, 0), (-1, 0), (0, 1), (0, -1)
    };

    Shuffle(directions); //asegura aleatoriedad
    // Marcar la celda como pasillo
    maze[x, y].isOpen = true;

    foreach (var (dx, dy) in directions)
    {
        int nx = x + dx * 2; // Mover 2 celdas en esa direccion
        int ny = y + dy * 2; // saltar la pared

        if (nx >= 0 && nx < size && ny >= 0 && ny < size && !maze[nx, ny].isOpen)
        {
            //Quitar la pared entre las celdas 
            maze[x + dx, y + dy].isOpen = true; 
            GenerateTheMaze(nx, ny); // Recursivamente 
        }
    }

    //Asegurar por lo menos una celda abierta en la ultima fila
    if (!HasOpenCellInRow(size - 1))
    {
        int col = rand.Next(size);
        maze[size - 1, col].isOpen = true;
    }
}

private bool HasOpenCellInRow(int row)
{
    for (int col = 0; col < size; col++)
    {
        if (maze[row, col].isOpen)
            return true;
    }
    return false;
}

       
    private void Shuffle((int dx, int dy)[] directions) //Fisher-Yates shuffle,In-Place Shuffling, cada elemento es cambiado una vez
    {
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1); //cada elemento desde el inicio del array hasta i tiene la misma probabilidad de ser escogida
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }

    }
    // Method to set Player 1 position
    public void SetPlayer1Position(int x, int y)
    {
        player1Pos = (x, y);
    }

    // Method to set Player 2 position
    public void SetPlayer2Position(int x, int y)
    {
        player2Pos = (x, y);
    }

    // Method to access Player 1 position
    public (int x, int y) GetPlayer1Position()
    {
        return player1Pos;
    }

    // Method to access Player 2 position
    public (int x, int y) GetPlayer2Position()
    {
        return player2Pos;
    }

public void PrintMazeSpectre()
{
    var table = new Table();
    table.HideHeaders();
    table.Border(TableBorder.Rounded);
    table.BorderColor(Color.Green1); 

    // Define the number of columns based on the maze size
    for (int i = 0; i < size; i++)
    {
        table.AddColumn(""); // Add a column for each maze row
    }

    for (int i = 0; i < size; i++)
    {
        var cells = new List<IRenderable>();

        for (int j = 0; j < size; j++)
        {
            var cellContent = GetCellContent(i, j);
            //cells.Add(new Markup(cellContent)); // Add the cell content
            cells.Add(new Markup($"  {cellContent}  "));
        }

        // Add the row with the correct cells
        table.AddRow(cells.ToArray());
    }
    AnsiConsole.WriteLine();
    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();
}





private string ConvertToSpectreColor(Color color)
{
    return $"bg#{color.R:X2}{color.G:X2}{color.B:X2}";
}





private string GetCellContent(int i, int j)
{
    if (i == exit.x && j == exit.y)
    {
        return "[bold white]⭐[/]"; // Exit cell
    }
    if (player1Pos.x == i && player1Pos.y == j && player2Pos.x == i && player2Pos.y == j)
    {
        return "[bold blue]🤼[/]"; // Both players in same cell
    }
  if (player1Pos.x == i && player1Pos.y == j)
{
    if (GameManager.Player1 != null)
    {
        return $"[bold blue]{GetTokenEmoji(GameManager.Player1.Token)}[/]"; // Player 1 with token emoji
    }
    return "[bold blue]🤷‍♂️[/]"; // Default for Player 1 if null
}

    

    // Check if Player 2 is in the current cell
    if (player2Pos.x == i && player2Pos.y == j)
    {
        // Check if Player2 is not null before accessing Token
        if (GameManager.Player2 != null)
        {
            return $"[bold yellow]{GetTokenEmoji(GameManager.Player2.Token)}[/]"; // Player 2 with token emoji
        }
        return "[bold yellow]🤷‍♀️[/]"; // Default for Player2 if null
    }
    
    Trap? trap = IsTrapAtPosition(i, j);
    if (trap != null)
    {
        return $"[bold red]{trap.Emoji}[/]";  // Trap
    }
    if (IsBeneficialTile(i, j, out string tileType))
    {
        if (tileType == "Cooldown Reduction")
        {
            return "[bold cyan]🫅[/]";  // Cooldown reduction tile
        }
        else if (tileType == "Speed Increase")
        {
            return "[bold red]👸[/]";  // Speed increase tile
        }
    }         

    return maze[i, j].isOpen 
        ? "[green].[/]" // Open path with green background
        : "[black]🌲[/]"; // Wall with black background
}
private string GetTokenEmoji(Token token)
{
    // Return the corresponding emoji for the player's token
    switch (token.Name)
    {
        case "Elf🧝":
            return "🧝";
        case "Fairy🧚":
            return "🧚";
        case "Abuela👵":
            return "👵";
        case "Unicorn🦄":
            return "🦄";
        case "Siren🧜":
            return "🧜";
        case "Wizard🧙":
            return "🧙";
        default:
            return "❓"; // Fallback emoji if token not recognized
    }
}

    private Color GetCellColor(int i, int j)
{
    if (i == exit.x && j == exit.y)
    {
        return Color.White;
    }
    if (player1Pos.x == i && player1Pos.y == j || player2Pos.x == i && player2Pos.y == j)
    {
        return Color.Blue;
    }
    Trap? trap = IsTrapAtPosition(i, j);
    if (trap != null)
    {
        return Color.Red;
    }
    return maze[i, j].isOpen ? Color.Green : Color.Black;
}






    public class Cell
    {
        public bool isOpen;
        public Cell(bool shouldBeOpen)
        {
            isOpen=shouldBeOpen;
        }
    }

private void GenerateTraps()
{
    int trapCount = 0; 
    int totalTraps = 4; 

    string[] trapEffects = {
        "Pierdes 1 turno",          // Effect for T1
        "Regresa a la casilla inicial",    // Effect for T2
        "Reduce tu velocidad" ,       // Effect for T3
        "Aumento tu tiempo de enfriamiento"
        
    };
    string[] trapEmojis = {
        "🔥", // Snake emoji for T1
        "🕳️", // Fire emoji for T2
        "🪵", // Trunk emoji for T3
        "🦇"  // 
    };

    //Todas las posiciones validas para trampas
    List<(int x, int y)> validPositions = new List<(int x, int y)>();
    List<Trap> trapsList = new List<Trap>();

    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            //Una posicion valida es un pasillo
            if (maze[i, j].isOpen&& !(i == 0 && j == 0)&& !(i == exit.x && j == exit.y))
            {
                validPositions.Add((i, j));
            }
        }
    }

    // Convertir a un array para el shuffling
    var validPositionsArr = validPositions.ToArray();

    Shuffle(validPositionsArr);

    // Poner trampa en la primera posicion valida en totalTraps
    foreach (var (x, y) in validPositionsArr)
    {
        if (trapCount < totalTraps)
        {
            string trapName = "T" + (trapCount + 1); // Nombre "T1", "T2", "T3"
            string effect = trapEffects[trapCount]; // Tomar su efecto
            string emoji = trapEmojis[trapCount];

            trapsList.Add(new Trap(x, y, trapName, effect,emoji));
            trapCount++;
        }

        if (trapCount == totalTraps)
            break;
    }
    traps = trapsList;  // Asignar la lista de trampas al objeto MazeGeneration 
}

public Trap? IsTrapAtPosition(int i, int j)
{
    foreach (var trap in traps)
    {
        if (trap.X == i && trap.Y == j)
        {
            
            return trap; 
        }
    }
    return null; 
}

private void SetExit()
{
    int exitRow = size - 1;

    // Ver si hay una casilla abierta alcanzable en la ultima fila
    for (int col = 0; col < size; col++)
    {
        if (maze[exitRow, col].isOpen && IsExitReachable(exitRow, col))
        {
            exit = (exitRow, col);
            return;
        }
    }

    // Fallback:abrir una casilla aleatoria en la ultima fila y asegurar que sea alcanzable
    for (int col = 0; col < size; col++)
    {
        if (maze[exitRow, col].isOpen || !maze[exitRow, col].isOpen)
        {
            maze[exitRow, col].isOpen = true; 
            if (IsExitReachable(exitRow, col))
            {
                exit = (exitRow, col);
                return;
            }

            //si no es alcanzable, cerrarla 
            maze[exitRow, col].isOpen = false;
        }
    }

    //como ultima instancia abrir una celda aleatoria como salida
    int randomCol = rand.Next(size);
    maze[exitRow, randomCol].isOpen = true;
    exit = (exitRow, randomCol);
}

public bool IsExitReachable(int exitRow, int exitCol) //Algortimo de Lee
{
        bool[,] visited=new bool[size,size];

        //marcar la celda inicial como abierta
        visited[0,0]=true;
        int[] dr={-1,1,0,0,};
        int[] dc={0,0,1,-1};

        bool change;
        do{
            change=false;
            for(int r=0;r<size;r++){
                for(int c=0;c<size;c++){
                    //saltar las celdas sin marcar
                    if(!visited[r,c]) continue;
    
//chequear celdas vecinas a [r,c]
                    for(int d=0;d<dr.Length;d++){
                        int vr=r+dr[d];
                        int vc=c+dc[d];
//determinar si el vecino es valido y no ha sido chequeado
                        if(ValidPosition(size,vr,vc) && !visited[vr,vc] && maze[vr,vc].isOpen){
                            visited[vr,vc]=true;
                            change=true;
                            if(vr==exitRow&&vc==exitCol){
                                return true;
                            }
                        }
                    }
                }
            }
        }
        while(change);
        return false; //salida no es alcanzable
    }

    public static bool ValidPosition(int size,int row,int col){
        return row >= 0 && row < size && col >= 0 && col < size;
    }
    
public bool IsWall(int row, int col)
{
    // Check for out-of-bounds
    if (row < 0 || row >= maze.GetLength(0) || col < 0 || col >= maze.GetLength(1))
    {
        string? outOfBoundsMessage = resourceManager4.GetString("OutOfBoundsMessage");
        if (!string.IsNullOrEmpty(outOfBoundsMessage))
        {
            Console.WriteLine(string.Format(outOfBoundsMessage, row, col));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'OutOfBoundsMessage' not found.");
        }
        return true; // Treat out-of-bounds as walls
    }

    // Check if the cell is a wall
    bool isWall = !maze[row, col].isOpen; // Correct indexing: [row][col]
    return isWall;
}
private void GenerateBeneficialTiles()
    {
        List<(int x, int y)> validPositions = new List<(int x, int y)>();

        // Find valid positions for beneficial tiles
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (maze[i, j].isOpen && !(i == exit.x && j == exit.y) && IsTrapAtPosition(i, j) == null)
                {
                    validPositions.Add((i, j));
                }
            }
        }

        if (validPositions.Count >= 2)
        {
            var validPositionsArr = validPositions.ToArray();
            Shuffle(validPositionsArr);

            // Assign beneficial tiles
            cooldownReductionTile = validPositions[0];
            speedIncreaseTile = validPositions[1];
        }
    }
    public bool IsBeneficialTile(int x, int y, out string tileType)
    {
        if (cooldownReductionTile.HasValue && cooldownReductionTile.Value == (x, y))
        {
            tileType = "Cooldown Reduction";
            return true;
        }
        else if (speedIncreaseTile.HasValue && speedIncreaseTile.Value == (x, y))
        {
            tileType = "Speed Increase";
            return true;
        }

        tileType = string.Empty;
        return false;
    }
public void GenerateTeleportationPortal()
{
    List<(int x, int y)> validPositions = new List<(int x, int y)>();

    // Find valid positions for teleportation (valid open cells)
    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            // Check if the position is valid for teleportation
            if (IsValidPositionForPortal(i, j))
            {
                validPositions.Add((i, j));  // Valid position, add to the list
            }
        }
    }

    // Check if there are valid positions to place the portal
    if (validPositions.Count > 0)
    {
        var validPositionsArr = validPositions.ToArray();
        Shuffle(validPositionsArr);  // Shuffle to randomize portal positions

        // Assign the random teleport portal
        randomTeleportPortal = validPositionsArr[0];  // Portal to a random valid position
    }
}


// Helper method to check if a position is valid for teleportation
private bool IsValidPositionForPortal(int x, int y)
{
    // Check if the position is open, not a wall, not the exit, not a trap, and not a beneficial tile
    if (maze[x, y].isOpen &&
        !(x == exit.x && y == exit.y)  &&  // Not start position
        IsTrapAtPosition(x, y) == null &&  // No trap
        !IsBeneficialTile(x, y, out _))  // Not a beneficial tile
    {
        return true;  // Position is valid for portal
    }
    
    return false;  // Position is invalid for portal
}




// This method checks if a player has stepped on a teleportation portal
public void CheckTeleportation(Player player)
{
    // Check if the player stepped on the back-to-start portal
    if (player.Position == randomTeleportPortal)
    {
        string? teleportMessage = resourceManager4.GetString("TeleportRandom");
if (!string.IsNullOrEmpty(teleportMessage))
{
    Console.WriteLine(string.Format(teleportMessage, player.Name));
}
else
{
    Console.WriteLine("Error: Resource string for 'TeleportRandom' not found.");
}

        player.Position = GetRandomValidPosition();  // Teleport to a random valid position
    }
}


public (int x, int y) GetRandomValidPosition()
{
    List<(int x, int y)> validPositions = new List<(int x, int y)>();

    // Find all valid open positions
    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            if (maze[i, j].isOpen && !(i == exit.x && j == exit.y))
            {
                validPositions.Add((i, j));
            }
        }
    }

    // Select a random position from the list of valid positions
    if (validPositions.Count > 0)
    {
        Random rand = new Random();
        int index = rand.Next(validPositions.Count);
        return validPositions[index];
    }

    // Fallback if no valid positions
    return (0, 0);  // Return a default value if no valid positions are found
}
}