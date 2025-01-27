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
            switch (Name)
            {
                case "T1": // Sakta un turno a quien lo activò 
                    player.SkipTurns = 1;
                    string? trap1Activated = resourceManager2.GetString("Trap1Activated");
                    if (!string.IsNullOrEmpty(trap1Activated))
                    {
                        Console.WriteLine(string.Format(trap1Activated, player.Name));
                    }
                    break;
                case "T2": // Manda al jugador al (0, 0)
                    player.Position = (0, 0);
                    string? trap2Activated = resourceManager2.GetString("Trap2Activated");
                    if (!string.IsNullOrEmpty(trap2Activated))
                    {
                        Console.WriteLine(string.Format(trap2Activated, player.Name));
                    }
                    break;
                case "T3": //Reduce la velocidad de la ficha
                    player.Token.Speed = Math.Max(1, player.Token.Speed - 1); //Asegura que la velocidad es al menos 1 para que no muera
                    string? trap3Activated = resourceManager2.GetString("Trap3Activated");
                    if (!string.IsNullOrEmpty(trap3Activated))
                    {
                        Console.WriteLine(string.Format(trap3Activated, player.Name, player.Token.Speed));
                    }
                    break;
                case "T4":
                    player.Token.SetCooldown(player.Token.CurrentCooldown + 2); // Increment current cooldown
                    string? trap4Activated = resourceManager2.GetString("Trap4Activated");
                    if (!string.IsNullOrEmpty(trap4Activated))
                    {
                        Console.WriteLine(string.Format(trap4Activated, player.Name, player.Token.CurrentCooldown));
                    }
                    break;
            }
                    Triggered = true; // Mark the trap as triggered       
        }
    }
}