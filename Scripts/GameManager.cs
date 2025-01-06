// GameManager.cs
public static class GameManager
{
    public static Player? Player1 { get; set; }
    public static Player? Player2 { get; set; }

    public static void InitializePlayers(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
}
