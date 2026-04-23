namespace InsideVentura.World
{
  public class HealthItem : Item
  {
    protected override void OnPickup()
    {
      PlayerAccessor.Stats.CurrentHealth.Change(StatOperationType.Add, 1);
    }
  }
}
