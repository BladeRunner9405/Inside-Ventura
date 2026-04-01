public class MoneyItem : Item {
  protected override void OnPickup() {
    if (!PlayerAccessor.Stats) return;

    // PlayerAccessor.Stats.Money += 1;
  }
}
