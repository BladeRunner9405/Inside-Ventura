using CherryFramework.BaseClasses;
using CherryFramework.DataModels;
using CherryFramework.DependencyManager;
using GeneratedDataModels;
using UnityEngine;

public class PlayerStats : BehaviourBase
{
  [Inject]
  private ModelService _modelService;
  private PlayerDataModel _model;

  protected override void OnEnable()
  {
    base.OnEnable();
    _model = _modelService.GetOrCreateSingletonModel<PlayerDataModel>();

    CurrentHealth = new DataModelStat(
      () => _model.currentHealth,
      value => _model.currentHealth = value,
      _model.currentHealth
    );
    Mana = new DataModelStat(
      () => _model.mana,
      value => _model.mana = value,
      _model.mana
    );
    Money = new DataModelStat(
      () => _model.money,
      value => _model.money = value,
      _model.money
    );
  }

  public DataModelStat CurrentHealth { get; set; }
  public DataModelStat Mana { get; set; } // Idea Points, тратятся активацией активируемых мыслей
  public DataModelStat Money { get; set; } // Замыслы, игровая валюта


  /*public float CurrentHealth
  {
    get => _model.currentHealth;
    set => _model.currentHealth = value;
  }

  public float Mana
  {
    get => _model.mana;
    set => _model.mana = value;
  }

  public float Money
  {
    get => _model.money;
    set => _model.money = value;
  }*/

  public Stat GetStat(StatName statName)
  {
    switch (statName)
    {
      case StatName.Health: return CurrentHealth;
      case StatName.Mana: return Mana;
      default: return null;
    }
  }
}
