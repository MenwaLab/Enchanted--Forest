using System.Resources;

public class Trap
{
    static ResourceManager resourceManager2 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    public int X { get; set; }  
    public int Y { get; set; }  
    public bool Triggered { get; set; } 
    public string Name { get; set; } 
    public string Effect { get; set; } 
    public string Emoji { get; set; } 

    public Trap(int x, int y, string name, string effect, string emoji)
    {
        X = x;
        Y = y;
        Triggered = false;
        Name = name;
        Effect = effect;
        Emoji = emoji;
    }

    public void ApplyEffect(Player player)
    {
        if (!Triggered)
        {
            //Console.WriteLine($"{Name} activada! {Effect}");
            switch (Name)
            {
                case "T1": 
                    // Lose 1 turn for T1
                    player.SkipTurns = 1;
                    string? trap1Activated = resourceManager2.GetString("Trap1Activated");
            if (!string.IsNullOrEmpty(trap1Activated))
            {
                Console.WriteLine(string.Format(trap1Activated, player.Name));
            }
            else
            {
                Console.WriteLine("Error: Resource string for 'Trap1Activated' not found.");
            }
                    break;
                case "T2":
                // Send the player back to the origin (0, 0)
                    player.Position = (0, 0);
                    string? trap2Activated = resourceManager2.GetString("Trap2Activated");
                    if (!string.IsNullOrEmpty(trap2Activated))
                    {
                        Console.WriteLine(string.Format(trap2Activated, player.Name));
                    }
                    else
                    {
                        Console.WriteLine("Error: Resource string for 'Trap2Activated' not found.");
                    }
                    break;

                case "T3":
                    //Reduce speed of your token 
                    player.Token.Speed = Math.Max(1, player.Token.Speed - 1); // Reduce speed but ensure it's at least 1
                    string? trap3Activated = resourceManager2.GetString("Trap3Activated");
                    if (!string.IsNullOrEmpty(trap3Activated))
                    {
                        Console.WriteLine(string.Format(trap3Activated, player.Name, player.Token.Speed));
                    }
                    else
                    {
                        Console.WriteLine("Error: Resource string for 'Trap3Activated' not found.");
                    }
                    break;
                case "T4":
                    player.Token.SetCooldown(player.Token.CurrentCooldown + 2); // Increment current cooldown
                    string? trap4Activated = resourceManager2.GetString("Trap4Activated");
                    if (!string.IsNullOrEmpty(trap4Activated))
                    {
                        Console.WriteLine(string.Format(trap4Activated, player.Name, player.Token.CurrentCooldown));
                    }
                    else
                    {
                        Console.WriteLine("Error: Resource string for 'Trap4Activated' not found.");
                    }
                    break;

            }
                    Triggered = true; // Mark the trap as triggered       
        }

        //else{Console.WriteLine(string.Format(resourceManager2.GetString("TrapAlreadyActivated"), Name));}
    }
}
