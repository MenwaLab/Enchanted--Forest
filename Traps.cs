public class Trap
{
    public int X { get; set; }  // X coordinate in the maze
    public int Y { get; set; }  // Y coordinate in the maze
    public bool Triggered { get; set; } // Whether the trap has been triggered
    public string Name { get; set; } // Name for the trap (e.g., "T1", "T2", "T3")
    public string Effect { get; set; } // Description of the trap's effect

    // Constructor to set the trap's position and effects
    public Trap(int x, int y, string name, string effect)
    {
        X = x;
        Y = y;
        Triggered = false;
        Name = name;
        Effect = effect;
    }

    // Method to apply the effect of the trap to a player
    public void ApplyEffect(Player player)
    {
        if (!Triggered)
        {
            Console.WriteLine($"{Name} activada! {Effect}");
            // Apply effect based on the trap type
            switch (Name)
            {
                case "T1": 
                    // Lose 1 turn for T1
                    player.SkipTurns = 1;
                    Console.WriteLine($"{player.Name} has lost a turn (T1). They will skip their next turn.");
                    break;
                case "T2":
                // Send the player back to the origin (0, 0)
                    player.Position = (0, 0);
                    Console.WriteLine($"{player.Name} has been sent back to the origin (0, 0)!Players position is now {player.Position}");
                    break;

                case "T3":
                    //Reduce speed of your token 
                    player.Token.Speed = Math.Max(1, player.Token.Speed - 1); // Reduce speed but ensure it's at least 1
                    Console.WriteLine($"{player.Name}'s speed has been reduced to {player.Token.Speed} (T3).");
                    break;
                case "T4":
                    player.Token.CooldownTime+=1;
                    Console.WriteLine("Trap 4 triggered! Aumento tu tiempo de enfriamiento. Lo siento :("); 
                    break;
            }
                    Triggered = true; // Mark the trap as triggered       
        }
        else
    {
        Console.WriteLine($"Trap {Name} has already been triggered and cannot trigger again.");
    }
    }
}
