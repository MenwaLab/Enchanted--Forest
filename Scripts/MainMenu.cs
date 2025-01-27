using Spectre.Console;
using System.Globalization;
using System.Resources;

public static class MainMenu
{
    private static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(MainMenu).Assembly);
    

    public static void ShowMenu()
    {
        while (true) // Mantener el menú hasta que se seleccione "Salir"
        {
            // Título del juego
            var title = new FigletText("Enchanted Forest")
                .Centered()
                .Color(Color.Green);

            // Renderizar título
            AnsiConsole.Write(title);

            string? playOption = resourceManager.GetString("Play");
            string? changeLanguageOption = resourceManager.GetString("ChangeLanguage");
            string? exitOption = resourceManager.GetString("Exit");

            if (string.IsNullOrEmpty(playOption) || string.IsNullOrEmpty(changeLanguageOption) || string.IsNullOrEmpty(exitOption))
            {
                AnsiConsole.MarkupLine("[red]Error: Resource strings not found. Please check your resource file.[/]");
                return;
            }
            
            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { playOption, changeLanguageOption, exitOption })
            );
            
            if (selectedOption == playOption)
            {
                AnsiConsole.MarkupLine("[green]" + resourceManager.GetString("StartingGame") + "[/]");
                return; // Salir del menú para continuar el juego
            }
            else if (selectedOption == changeLanguageOption)
            {
                ChangeLanguage();
            }
            else if (selectedOption == exitOption)
            {
                AnsiConsole.MarkupLine("[red]" + resourceManager.GetString("ExitingGame") + "[/]");
                Environment.Exit(0);
            }
        }
    }

     private static void ChangeLanguage()
    {
        // Menú para seleccionar idioma
        string language = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(new[] { "Español", "English" })
        );

        if (language == "Español")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        }
        else if (language == "English")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        AnsiConsole.MarkupLine($"[blue]{resourceManager.GetString("LanguageChanged")}: {language}[/]");
    }
}
