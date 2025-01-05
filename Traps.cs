public class Trap
{
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
            Console.WriteLine($"{Name} activada! {Effect}");
            switch (Name)
            {
                case "T1": 
                    // Lose 1 turn for T1
                    player.SkipTurns = 1;
                    Console.WriteLine($"Trampa 1 ha sido activada! {player.Name} ha perdido un turno.");
                    break;
                case "T2":
                // Send the player back to the origin (0, 0)
                    player.Position = (0, 0);
                    Console.WriteLine($"Trampa 2 ha sido activada! {player.Name} ha sido enviado al origen de coordenadas (0, 0).");
                    break;

                case "T3":
                    //Reduce speed of your token 
                    player.Token.Speed = Math.Max(1, player.Token.Speed - 1); // Reduce speed but ensure it's at least 1
                    Console.WriteLine($"Trampa 3 ha sido activada!.{player.Name} tu velocidad ha sido reducida a {player.Token.Speed}.");
                    break;
                case "T4":
                    player.Token.SetCooldown(player.Token.CurrentCooldown + 2); // Increment current cooldown
                    Console.WriteLine($"Trampa 4 ha sido activada! {player.Name} ahora tiene {player.Token.CurrentCooldown} turnos de enfriamiento.");
                    break;

            }
                    Triggered = true; // Mark the trap as triggered       
        }
        else
        {
        Console.WriteLine($"Trampa {Name} ya fue activada.");
        }
    }
}
