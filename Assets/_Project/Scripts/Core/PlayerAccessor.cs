public class PlayerAccessor {
  public Player Player { get; private set; }

  public void RegisterPlayer(Player player) {
    Player = player;
  }

  public void UnregisterPlayer(Player player) {
    if (Player == player)
      Player = null;
  }
}
