namespace InsideVentura.World
{
  public class MoneyItem : Item
  {
    protected override void OnPickup()
    {
      PlayerAccessor.Stats.Money.Change(StatOperationType.Add, 1);
    }
  }
}
