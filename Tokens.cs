using System.Resources;

public class Token
{
    static ResourceManager resourceManager5 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    public string Name { get; set; }
    public string AbilityDescription { get; set; }
    public int Speed { get; set; } //velocidad actual
    public int BaseSpeed { get; set; }//velocidad original
    public int CooldownTime { get; set; }
    public int CurrentCooldown { get; private set; }
    public int BaseCooldown { get; set; } // Original cooldown duration
    

    private Action<Player, Player> AbilityAction;

    public Token(string name, string abilityDescription, int speed, int cooldownTime, Action<Player, Player> abilityAction)
    {
        Name = name;
        AbilityDescription = abilityDescription;
        Speed = speed;
        BaseSpeed = speed;
        CooldownTime = cooldownTime;
        BaseCooldown = cooldownTime; 
        AbilityAction = abilityAction;
        CurrentCooldown = 0; 
    }

    public void UseAbility(Player user, Player target)
    {
        if (CurrentCooldown > 0)
        {
            string? abilityInCooldown = resourceManager5.GetString("AbilityInCooldown");
        if (!string.IsNullOrEmpty(abilityInCooldown))
        {
            Console.WriteLine(string.Format(abilityInCooldown, Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'AbilityInCooldown' not found.");
        }
            return;
        }
        
        AbilityAction(user, target);

        CurrentCooldown = CooldownTime;
        CooldownTime = BaseCooldown; // Reset cooldown
        string? abilityUsedMessage = resourceManager5.GetString("AbilityUsed");
    if (!string.IsNullOrEmpty(abilityUsedMessage))
    {
        Console.WriteLine(string.Format(abilityUsedMessage, user.Name, CooldownTime));
    }
    else
    {
        Console.WriteLine("Error: Resource string for 'AbilityUsed' not found.");
    }
    }

    public void ReduceCooldown()
    {
        if (CurrentCooldown > 0)
        {
            CurrentCooldown--;
        }
    }
  //ver si es de utilidad pq era para puppy pero lo cambie por abuela
    public void SetCooldown(int turns)
    {
        CurrentCooldown = Math.Max(turns, 0); 
    }
    public override string ToString()
    {
        return $"{Name}: {AbilityDescription}, Velocidad: {Speed}, Tiempo de enfriamiento: {CooldownTime}";
    }
    public void MimicAbility(Token targetToken, Player user, Player target)
{
    // Copy the ability and description
    AbilityAction = targetToken.AbilityAction;
    //AbilityDescription = $"Mimics: {targetToken.AbilityDescription}";


    // Immediately execute the mimicked ability
    AbilityAction(user, target);
}
}

public static class TokenFactory
{
    public static Token[] GetAvailableTokens()
    {
        ResourceManager resourceManager6 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
        return new Token[]
        {
            new Token("Elf🧝", "Permanently copies the ability of another token and uses it immediately", 3, 5, 
            (user, target) =>
            {string? elfMimicAbility = resourceManager6.GetString("ElfMimicAbility");
        if (!string.IsNullOrEmpty(elfMimicAbility))
        {
            Console.WriteLine(string.Format(elfMimicAbility, user.Name, target.Name, target.Token.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'ElfMimicAbility' not found.");
        }
            user.Token.MimicAbility(target.Token, user, target); }),

            new Token("Wizard🧙", "Reduce la velocidad de su enemigo", 4, 3,
                (user, target) =>
                {string? wizardReduceSpeed = resourceManager6.GetString("WizardReduceSpeed");
        if (!string.IsNullOrEmpty(wizardReduceSpeed))
        {
            Console.WriteLine(string.Format(wizardReduceSpeed, user.Name, target.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'WizardReduceSpeed' not found.");
        }
                target.Token.Speed = Math.Max(1, target.Token.Speed - 1); }),
            
            new Token("Fairy🧚", "Cambia su posición con la de su enemigo", 7, 4,
                (user, target) =>
                {string? fairySwapPosition = resourceManager6.GetString("FairySwapPosition");
        if (!string.IsNullOrEmpty(fairySwapPosition))
        {
            Console.WriteLine(string.Format(fairySwapPosition, user.Name, target.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'FairySwapPosition' not found.");
        }
                Player.SwapPlayerPositions(user, target); }),// Call the method in Program.cs

            new Token("Siren🧜", "Le quita un turno a su enemigo", 5, 3,
                (user, target) =>
                {string? sirenSkipTurn = resourceManager6.GetString("SirenSkipTurn");
        if (!string.IsNullOrEmpty(sirenSkipTurn))
        {
            Console.WriteLine(string.Format(sirenSkipTurn, user.Name, target.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'SirenSkipTurn' not found.");
        }
                target.SkipTurns = 1; }),

            new Token("Abuela👵", "Aumenta su velocidad por 1", 2, 2,
                (user, target) =>
                {
                    string? abuelaIncreaseSpeed = resourceManager6.GetString("AbuelaIncreaseSpeed");
        if (!string.IsNullOrEmpty(abuelaIncreaseSpeed))
        {
            Console.WriteLine(string.Format(abuelaIncreaseSpeed, user.Name));
        }
        else
        {
            Console.WriteLine("Error: Resource string for 'AbuelaIncreaseSpeed' not found.");
        }
                    user.Token.Speed += 1;
                    user.Token.SetCooldown(1); // Ensure her ability has a cooldown. ver si funciona
                }),
            
            new Token("Unicorn🦄", "Bendición de la suerte: Activa un efecto al azar", 6, 4,
                (user, target) =>
                {
                    Random rand = new Random();
                    int effect = rand.Next(1, 4); // Randomly pick between 1, 2, or 3
                    string? unicornMessage = string.Empty;

                    switch (effect)
                    {
                        case 1: // Swap positionsunicornMessage = resourceManager.GetString("UnicornSwapPosition");
                if (!string.IsNullOrEmpty(unicornMessage))
                {
                    Console.WriteLine(string.Format(unicornMessage, user.Name, target.Name));
                }Console.WriteLine($"{user.Name}'s Dragon swaps positions with {target.Name}.");
                            Player.SwapPlayerPositions(user, target);
                            break;

                        case 2: // Skip the target's turn
                            unicornMessage = resourceManager6.GetString("UnicornSkipTurn");
                if (!string.IsNullOrEmpty(unicornMessage))
                {
                    Console.WriteLine(string.Format(unicornMessage, user.Name, target.Name));
                }
                            target.SkipTurns = 1;
                            break;

                        case 3: // Increase Unicorn's speed
                            unicornMessage = resourceManager6.GetString("UnicornIncreaseSpeed");
                if (!string.IsNullOrEmpty(unicornMessage))
                {
                    Console.WriteLine(string.Format(unicornMessage, user.Name));
                }
                            user.Token.Speed += 1;
                            break;
                    }
                })
        };
    }
}