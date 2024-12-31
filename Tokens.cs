public class Token
{
    public string Name { get; set; }
    public string AbilityDescription { get; set; }
    public int Speed { get; set; }
    public int CooldownTime { get; set; }
    public int CurrentCooldown { get; private set; }

    private Action<Player, Player> AbilityAction;

    public Token(string name, string abilityDescription, int speed, int cooldownTime, Action<Player, Player> abilityAction)
    {
        Name = name;
        AbilityDescription = abilityDescription;
        Speed = speed;
        CooldownTime = cooldownTime;
        AbilityAction = abilityAction;
        CurrentCooldown = 0; // Start with no cooldown
    }

    public void UseAbility(Player user, Player target)
    {
        if (CurrentCooldown > 0)
        {
            Console.WriteLine($"{Name}'s ability is on cooldown for {CurrentCooldown} more turns.");
            return;
        }

        AbilityAction(user, target);
        CurrentCooldown = CooldownTime;
    }

    public void ReduceCooldown()
    {
        if (CurrentCooldown > 0)
        {
            CurrentCooldown--;
        }
    }
  // Add a method to directly set the cooldown (for abilities like Puppy)
    public void SetCooldown(int turns)
    {
        CurrentCooldown = Math.Max(turns, 0); // Ensure it's not set to a negative value
    }
    public override string ToString()
    {
        return $"{Name}: {AbilityDescription}, Speed: {Speed}, Cooldown: {CooldownTime}";
    }
}

    public static class TokenFactory
{
    public static Token[] GetAvailableTokens()
    {
        return new Token[]
        {
            new Token("Elf", "Traps don't affect it", 4, 4, 
                (user, target) => Console.WriteLine($"{user.Name}'s Elf is unaffected by traps.")),
            
            new Token("Wizard", "Reduces speed of another player for 1 turn", 2, 3,
                (user, target) =>
                {
                    Console.WriteLine($"{user.Name}'s Wizard reduces {target.Name}'s speed for 1 turn.");
                    target.Token.Speed = 1;
                }),
            
            new Token("Fairy", "Moves another player back 1 square", 4, 2,
                (user, target) =>
                {
                    Console.WriteLine($"{user.Name}'s Fairy moves {target.Name} back 1 square.");
                    target.Position = (target.Position.x - 1, target.Position.y);
                }),
            
            new Token("Puppy", "Skips another player's turn", 5, 3,
                (user, target) =>
                {
                    Console.WriteLine($"{user.Name}'s Puppy skips {target.Name}'s turn.");
                    target.Token.SetCooldown(1); // Apply the cooldown directly
                }),
            
            new Token("Unicorn", "Unaffected by other players' abilities", 3, 4,
                (user, target) => Console.WriteLine($"{user.Name}'s Unicorn is unaffected by abilities."))
        };
    }
}