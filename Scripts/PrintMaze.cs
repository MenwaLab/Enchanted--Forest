using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Generic;

public class MazePrinter
{
    private MazeCreation maze;

    public MazePrinter(MazeCreation maze)
    {
        this.maze = maze;
    }
    public void PrintMazeSpectre()
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
                var cellContent = GetCellContent(i, j);
                cells.Add(new Markup($"  {cellContent}  "));
            }
            table.AddRow(cells.ToArray());
        }
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private string GetCellContent(int i, int j)
    {
        var exit = maze.exit;
        var player1Pos = maze.GetPlayer1Position();
        var player2Pos = maze.GetPlayer2Position();

        if (i == maze.exit.x && j == maze.exit.y)
        {
            return "[bold white]⭐[/]"; 
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

        if (player2Pos.x == i && player2Pos.y == j)
        {
            if (GameManager.Player2 != null)
            {
                return $"[bold yellow]{GetTokenEmoji(GameManager.Player2.Token)}[/]"; // Player 2 with token emoji
            }
            return "[bold yellow]🤷‍♀️[/]"; // Default for Player2 if null
        }
        
        Trap? trap = maze.IsTrapAtPosition(i, j);
        if (trap != null)
        {
            return $"[bold red]{trap.Emoji}[/]";  // Trap
        }
        if (maze.IsBeneficialTile(i, j, out string tileType))
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
                return "❓"; 
        }
    }

    private Color GetCellColor(int i, int j)
    {
        var exit = maze.exit;
        var player1Pos = maze.GetPlayer1Position();
        var player2Pos = maze.GetPlayer2Position();

        if (i == exit.x && j == exit.y)
        {
            return Color.White;
        }
        if (player1Pos.x == i && player1Pos.y == j || player2Pos.x == i && player2Pos.y == j)
        {
            return Color.Blue;
        }

        Trap? trap = maze.IsTrapAtPosition(i, j);
        if (trap != null)
        {
            return Color.Red;
        }
        return maze.IsWall(i, j) ? Color.Black : Color.Green;
    }
    private string ConvertToSpectreColor(Color color)
    {
        return $"bg#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}