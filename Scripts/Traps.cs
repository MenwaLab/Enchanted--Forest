using System.Resources;
public class Trap
{
    static ResourceManager resourceManager2 = new ResourceManager("Enchanted__Forest.Resources.Strings", typeof(Trap).Assembly);
    public string Name {get; set;} 
    public string Effect {get; set;} 
    public string Emoji {get; set;} 
    public bool Triggered {get; set;} 
    public int X {get; set;}  
    public int Y {get; set;}  

    public Trap(int x, int y, string name, string effect, string emoji)
    {
        X = x;
        Y = y;
        Name = name;
        Effect = effect;
        Emoji = emoji;
        Triggered = false; 
    }

    public void ApplyEffect(Player player)
    {
        if (!Triggered)
        {
            switch (Name)
            {
                case "T1": 
                    player.SkipTurns = 1; // Salta un turno 
                    string? trap1Triggered = resourceManager2.GetString("Trap1Triggered");
                    if (!string.IsNullOrEmpty(trap1Triggered))
                    {
                        Console.WriteLine(string.Format(trap1Triggered, player.Name));
                    }
                    break;
                case "T2": 
                    player.Position = (0, 0); // Envía al jugador al (0, 0)
                    string? trap2Triggered = resourceManager2.GetString("Trap2Triggered");
                    if (!string.IsNullOrEmpty(trap2Triggered))
                    {
                        Console.WriteLine(string.Format(trap2Triggered, player.Name));
                    }
                    break;
                case "T3": //Reduce la velocidad de la ficha
                    player.Token.Speed = Math.Max(1, player.Token.Speed - 1); //Asegura que la velocidad es al menos 1 para que no muera permanentemente
                    string? trap3Triggered = resourceManager2.GetString("Trap3Triggered");
                    if (!string.IsNullOrEmpty(trap3Triggered))
                    {
                        Console.WriteLine(string.Format(trap3Triggered, player.Name, player.Token.Speed));
                    }
                    break;
                case "T4":
                    player.Token.SetCooldown(player.Token.CurrentCooldown + 2); // Aumenta el tiempo de enfriamiento de la ficha
                    string? trap4Triggered = resourceManager2.GetString("Trap4Triggered");
                    if (!string.IsNullOrEmpty(trap4Triggered))
                    {
                        Console.WriteLine(string.Format(trap4Triggered, player.Name, player.Token.CurrentCooldown));
                    }
                    break;
            }
                    Triggered = true; // Entonces la trampa fue activada     
        }
    }
}