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
  }

  public float CurrentHealth
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
  }
}
