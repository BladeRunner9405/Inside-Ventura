using System;

public class DataModelStat : Stat
{
  private readonly Func<float> _getter;
  private readonly Action<float> _setter;

  public DataModelStat(Func<float> getter, Action<float> setter, float initialValue = 0)
  {
    _getter = getter;
    _setter = setter;
    Value = initialValue;
  }

  public override float Value
  {
    get => _getter();
    set => _setter(value);
  }
}
