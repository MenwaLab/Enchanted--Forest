using Spectre.Console;
using System.Globalization;
using System.Resources;

public static class MainMenu
{
    private static ResourceManager resourceManager = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(MainMenu).Assembly);

    public static void ShowMenu()
    {
        while (true) //Mantiene el menu hasta que seleccione Salir
        {
            var title = new FigletText("Enchanted Forest")
                .Centered()
                .Color(Color.Green);

            //Renderizar titulo
            AnsiConsole.Write(title);

            
            string playOption = resourceManager.GetString("Play") ?? "Play";
            string tokensOption = resourceManager.GetString("Tokens") ?? "Tokens";
            string changeLanguageOption = resourceManager.GetString("ChangeLanguage") ?? "Change Language";
            string exitOption = resourceManager.GetString("Exit") ?? "Exit";

            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .AddChoices(playOption, tokensOption, changeLanguageOption, exitOption)
            );

            if (selectedOption == playOption)
            {
                AnsiConsole.MarkupLine("[green]" + (resourceManager.GetString("StartingGame") ?? "Starting game...") + "[/]");
                return;
            }
            else if (selectedOption == tokensOption)
            {
                ShowTokens();
            }
            else if (selectedOption == changeLanguageOption)
            {
                ChangeLanguage();
            }
            else if (selectedOption == exitOption)
            {
                AnsiConsole.MarkupLine("[red]" + (resourceManager.GetString("ExitingGame") ?? "Exiting game...") + "[/]");
                Environment.Exit(0);
            }
        }
    }

    private static void ChangeLanguage()
    {
        string language = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices("Español", "English")
                .HighlightStyle(new Style(foreground: Color.Green))
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

        AnsiConsole.MarkupLine($"[green]{resourceManager.GetString("LanguageChanged") ?? "Language changed"}: {language}[/]");
    }

    private static void ShowTokens()
    {
        var tokens = new List<(string Name, string Description, string Abilities, int Speed, int Cooldown)>
        {
            ("Elf🧝", resourceManager.GetString("ElfDescription") ?? "No description available.", resourceManager.GetString("ElfAbilities") ?? "No abilities available.", 3, 5),
            ("Wizard🧙", resourceManager.GetString("WizardDescription") ?? "No description available.", resourceManager.GetString("WizardAbilities") ?? "No abilities available.",4, 3),
            ("Fairy🧚", resourceManager.GetString("FairyDescription") ?? "No description available.", resourceManager.GetString("FairyAbilities") ?? "No abilities available.",7, 4),
            ("Siren🧜", resourceManager.GetString("SirenDescription") ?? "No description available.", resourceManager.GetString("SirenAbilities") ?? "No abilities available.",5, 3),
            ("Grandma👵", resourceManager.GetString("GrandmaDescription") ?? "No description available.", resourceManager.GetString("GrandmaAbilities") ?? "No abilities available.",2, 2),
            ("Unicorn🦄", resourceManager.GetString("UnicornDescription") ?? "No description available.", resourceManager.GetString("UnicornAbilities") ?? "No abilities available.",6, 4)
        };

        string backToMenuOption = resourceManager.GetString("BackToMenu") ?? "Back to Menu";
        string speedLabel = resourceManager.GetString("Speed") ?? "Speed";
        string cooldownLabel = resourceManager.GetString("CooldownTime") ?? "Cooldown Time";
        string abilitiesLabel = resourceManager.GetString("Abilities") ?? "Abilities";


        while (true)
        {
            var selectedToken = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[lightgreen][underline][bold]{resourceManager.GetString("TokenDescriptionsTitle") ?? "Token Descriptions"}[/][/][/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .AddChoices(tokens.Select(t => t.Name).Append(backToMenuOption).ToArray())
            );

            if (selectedToken == backToMenuOption)
            {
                return;
            }

            var token = tokens.FirstOrDefault(t => t.Name == selectedToken);

            if (token != default)
            {
                AnsiConsole.MarkupLine($"[green]{token.Name}[/]");
                AnsiConsole.MarkupLine($"[darkgreen]{resourceManager.GetString("Description") ?? "Description"}:[/] {token.Description}");
                AnsiConsole.MarkupLine($"[springgreen4]{speedLabel}:[/] {token.Speed}");
                AnsiConsole.MarkupLine($"[darkgreen]{cooldownLabel}:[/] {token.Cooldown}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{resourceManager.GetString("TokenDescriptionNotFound") ?? "Description not found"}[/]");
            }

            AnsiConsole.MarkupLine("[]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }
    }

