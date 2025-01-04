public class Token
{
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
            Console.WriteLine($"{Name}, tu habilidad está en enfriamiento");
            return;
        }
        
        AbilityAction(user, target);

        CurrentCooldown = CooldownTime;
        CooldownTime = BaseCooldown; // Reset cooldown
        Console.WriteLine($"{user.Name} used their ability. Cooldown reset to {CooldownTime}.");
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
    AbilityDescription = $"Mimics: {targetToken.AbilityDescription}";

    Console.WriteLine($"{user.Name}'s Elf has permanently mimicked {targetToken.Name}'s ability: {AbilityDescription}.");

    // Immediately execute the mimicked ability
    AbilityAction(user, target);
}
}

public static class TokenFactory
{
    public static Token[] GetAvailableTokens()
    {
        return new Token[]
        {
            new Token("Elf", "Permanently copies the ability of another token and uses it immediately", 3, 5, 
    (user, target) =>
    {
        Console.WriteLine($"{user.Name}'s Elf is mimicking {target.Name}'s {target.Token.Name} ability.");
        user.Token.MimicAbility(target.Token, user, target); // Mimic and execute the target's ability
    }),



            
            new Token("Wizard", "Reduce la velocidad de su enemigo", 4, 3,
                (user, target) =>
                {Console.WriteLine($"{user.Name}'s Wizard reduce la velocidad de {target.Name}.");
                target.Token.Speed = Math.Max(1, target.Token.Speed - 1); }),
            
            new Token("Fairy", "Cambia su posición con la de su enemigo", 7, 4,
                (user, target) =>
                {Console.WriteLine($"{user.Name}'s Fairy alterna su posición con {target.Name}.");
                Player.SwapPlayerPositions(user, target); }),// Call the method in Program.cs

            new Token("Siren🧜‍♀️", "Le quita un turno a su enemigo", 5, 3,
                (user, target) =>
                {Console.WriteLine($"{user.Name}'s Siren salta el turno de {target.Name}.");
                target.SkipTurns = 1; }),


            new Token("Abuela", "Aumenta su velocidad por 1", 2, 2,
                (user, target) =>
                {
                    Console.WriteLine($"{user.Name}'s Abuela su propia velocidad por 1.");
                    user.Token.Speed += 1;
                    user.Token.SetCooldown(1); // Ensure her ability has a cooldown. ver si funciona
                }),
            
            new Token("Dragon", "Bendición de la suerte: Activa un efecto al azar", 6, 4,
                (user, target) =>
                {
                    Random rand = new Random();
                    int effect = rand.Next(1, 4); // Randomly pick between 1, 2, or 3

                    switch (effect)
                    {
                        case 1: // Swap positions
                            Console.WriteLine($"{user.Name}'s Dragon swaps positions with {target.Name}.");
                            Player.SwapPlayerPositions(user, target);
                            break;

                        case 2: // Skip the target's turn
                            Console.WriteLine($"{user.Name}'s Unicorn skips {target.Name}'s turn.");
                            target.SkipTurns = 1;
                            break;

                        case 3: // Increase Unicorn's speed
                            Console.WriteLine($"{user.Name}'s Unicorn increases its own speed by 1.");
                            user.Token.Speed += 1;
                            break;
                    }
                })
        };
    }
}