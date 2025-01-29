using System.Resources;

public class Token
{
    static ResourceManager resourceManager5 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    public string Name {get; set;}
    public string DescriptionOfAbility {get; set;}
    public int BaseSpeed {get; set;}//velocidad original de la ficha
    public int Speed {get; set;} //velocidad actual
    public int BaseCooldown {get; set;} // Tiempo de enfriamiento original 
    public int CooldownTime {get; set;}
    public int CurrentCooldown {get; private set;}
    
    private Action<Player, Player> AbilityAction;

    public Token(string name, string abilityDescription, int speed, int cooldownTime, Action<Player, Player> abilityAction)
    {
        Name = name;
        DescriptionOfAbility = abilityDescription;
        AbilityAction = abilityAction;
        BaseSpeed = speed;
        Speed = speed;
        BaseCooldown = cooldownTime; 
        CooldownTime = cooldownTime;
        CurrentCooldown = 0; 
    }

    public void Ability(Player user, Player target)
    {
        if (CurrentCooldown > 0) //no puede usar habilidad
        {
            string? abilityInCooldown = resourceManager5.GetString("AbilityInCooldown");
            if (!string.IsNullOrEmpty(abilityInCooldown))
            {
                Console.WriteLine(string.Format(abilityInCooldown, Name));
            }
            return;
        }
        
        AbilityAction(user, target);
        CurrentCooldown = CooldownTime;
        CooldownTime = BaseCooldown; 

        string?abilityUsed = resourceManager5.GetString("AbilityUsed");
        if (!string.IsNullOrEmpty(abilityUsed))
        {
            Console.WriteLine(string.Format(abilityUsed, user.Name, CooldownTime));
        }
    }

    public void DecreaseCooldown()
    {
        if (CurrentCooldown > 0)
        {
            CurrentCooldown--;
        }
    }
  
    public void SetCooldown(int turns)
    {
        CurrentCooldown = Math.Max(turns, 0); //tiempo de enfriamiento no pude ser negativo
    }
    public override string ToString()
    {
        return $"{Name}: {DescriptionOfAbility}, Velocidad /Speed: {Speed}, Tiempo de enfriamiento /Cooldown: {CooldownTime}"; //para mostrar a los jugadores en el menu
    }
    public void CopyAbility(Token targetToken, Player user, Player target)
    {
        AbilityAction = targetToken.AbilityAction; // Copia la habilidad
        AbilityAction(user, target); //ejecuta esa habilidad
    }
}

public static class TokenCreation
{
    public static Token[] GetTokens()
    {
        ResourceManager resourceManager6 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);

        return
        [
            new Token("Elf🧝", resourceManager6.GetString("ElfAbilities") ?? "Default description for Elf", 3, 5, 
            (user, target) =>{
                
            string? elfMimicAbility = resourceManager6.GetString("ElfMimicAbility");
            if (!string.IsNullOrEmpty(elfMimicAbility))
            {
                Console.WriteLine(string.Format(elfMimicAbility, user.Name, target.Name, target.Token.Name));
            }
            user.Token.CopyAbility(target.Token, user, target); }),

            new Token("Wizard🧙", resourceManager6.GetString("WizardAbilities") ?? "Default description for Wizard", 4, 3,
                (user, target) =>{
                    
                string? wizardReduceSpeed = resourceManager6.GetString("WizardReduceSpeed");
                if (!string.IsNullOrEmpty(wizardReduceSpeed))
                {
                    Console.WriteLine(string.Format(wizardReduceSpeed, user.Name, target.Name));
                }
                target.Token.Speed = Math.Max(1, target.Token.Speed - 1); }),
            
            new Token("Fairy🧚", resourceManager6.GetString("FairyAbilities") ?? "Default description for Fairy", 7, 4,
                (user, target) =>{
                    
                string? fairySwapPosition = resourceManager6.GetString("FairySwapPosition");
                if (!string.IsNullOrEmpty(fairySwapPosition))
                {
                    Console.WriteLine(string.Format(fairySwapPosition, user.Name, target.Name));
                }
                Player.SwapPositions(user, target); }),

            new Token("Siren🧜", resourceManager6.GetString("SirenAbilities") ?? "Default description for Siren", 5, 3,
                (user, target) =>{
                string? sirenSkipTurn = resourceManager6.GetString("SirenSkipTurn");
                if (!string.IsNullOrEmpty(sirenSkipTurn))
                {
                    Console.WriteLine(string.Format(sirenSkipTurn, user.Name, target.Name));
                }
                target.SkipTurns = 1; }),

            new Token("Abuela👵", resourceManager6.GetString("GrandmaAbilities") ?? "Default description for Grandma", 2, 2, 
                (user, target) =>{
                string? abuelaIncreaseSpeed = resourceManager6.GetString("AbuelaIncreaseSpeed");
                if (!string.IsNullOrEmpty(abuelaIncreaseSpeed))
                {
                    Console.WriteLine(string.Format(abuelaIncreaseSpeed, user.Name));
                }
                user.Token.Speed += 1;
                user.Token.SetCooldown(1);}),
            
            new Token("Unicorn🦄", resourceManager6.GetString("UnicornAbilities") ?? "Default description for Unicorn", 6, 4,
                (user, target) =>{
                Random rand = new Random();
                int effect = rand.Next(1, 4); // Aleatoriamente escoje entre 1 a 3
                string? unicornMessage = string.Empty;

                    switch (effect)
                    {
                        case 1: //Hace swap a las posiciones
                            unicornMessage = resourceManager6.GetString("UnicornSwapPosition");
                            if (!string.IsNullOrEmpty(unicornMessage))
                            {
                                Console.WriteLine(string.Format(unicornMessage, user.Name, target.Name));
                            }
                            Player.SwapPositions(user, target);
                            break;

                        case 2: //Salta el turno del otro jugador
                            unicornMessage = resourceManager6.GetString("UnicornSkipTurn");
                            if (!string.IsNullOrEmpty(unicornMessage))
                            {
                                Console.WriteLine(string.Format(unicornMessage, user.Name, target.Name));
                            }
                            target.SkipTurns = 1;
                            break;

                        case 3: //aumenta su velocidad
                            unicornMessage = resourceManager6.GetString("UnicornIncreaseSpeed");
                            if (!string.IsNullOrEmpty(unicornMessage))
                            {
                                Console.WriteLine(string.Format(unicornMessage, user.Name));
                            }
                            user.Token.Speed += 1;
                            break;
                    }
                })
        ];
    }
}