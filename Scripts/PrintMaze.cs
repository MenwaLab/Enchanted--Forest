using Spectre.Console;
using Spectre.Console.Rendering;

public class MazePrinter
{
    private MazeCreation maze;

    public MazePrinter(MazeCreation maze)
    {
        this.maze = maze;
    }
    public void PrintMazeWithSpectre()
    {
        var table = new Table();
        table.HideHeaders();
        table.Border(TableBorder.Rounded);
        table.BorderColor(Color.Green1); 

        for (int i = 0; i < maze.Size; i++)
        {
            table.AddColumn(""); //una columna por cada fila del tablero
        }

        for (int i = 0; i < maze.Size; i++) 
        {
            var cells = new List<IRenderable>();

            for (int j = 0; j < maze.Size; j++)
            {
                var cellContent = GetInteriorOfCell(i, j);
                cells.Add(new Markup($"  {cellContent}  "));
            }
            table.AddRow(cells.ToArray());
        }
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private string GetInteriorOfCell(int i, int j) //Toma el contenido de la celda, ej paredes, pasillo, fichas, salida, etc
    {
        var exit = maze.exit;
        var player1Pos = maze.GetPlayer1Position();
        var player2Pos = maze.GetPlayer2Position();

        if ((i,j) == maze.exit) 
        {
            return "[bold white]⭐[/]"; 
        }
        if (player1Pos==(i,j) && player2Pos==(i,j))
        {
            return "[bold blue]🤼[/]"; // Cuando las dos fichas están en la misma casilla
        }
        if (player1Pos==(i,j))
        {
            if (GameManager.Player1 != null)
            {
                return $"[bold blue]{GetTokenEmoji(GameManager.Player1.Token)}[/]"; 
            }
            return "[bold blue]🤷‍♂️[/]"; // Default si es nulo
        }

        if (player2Pos==(i,j)) 
        {
            if (GameManager.Player2 != null)
            {
                return $"[bold yellow]{GetTokenEmoji(GameManager.Player2.Token)}[/]"; 
            }
            return "[bold yellow]🤷‍♀️[/]"; 
        }
        
        Trap? trap = maze.IsTrapAtPosition(i, j);
        if (trap != null)
        {
            return $"[bold red]{trap.Emoji}[/]";  
        }
        if (maze.IsBeneficialTile(i, j, out string tileType))
        {
            if (tileType == "Cooldown Reduction")
            {
                return "[bold cyan]🫅[/]";  
            }
            else if (tileType == "Speed Increase")
            {
                return "[bold red]👸[/]";  
            }
        }         

        return maze.IsWall(i, j)
            ? "[black]🌲[/]" // Pared
            : "[green].[/]"; // camino
    }

    private string GetTokenEmoji(Token token)
    {
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
                return "❓";  //default en caso de que sea nulo
        }
    }
}