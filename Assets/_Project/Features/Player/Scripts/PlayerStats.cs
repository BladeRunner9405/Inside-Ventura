using UnityEngine;
using CherryFramework.BaseClasses;
using CherryFramework.DependencyManager;
using CherryFramework.DataModels;
using GeneratedDataModels; // <-- Подключаем тот самый сгенерированный namespace!

public class PlayerStats : BehaviourBase 
{
    [Inject] private ModelService _modelService;
    
    private PlayerDataModel _model; 

    protected override void OnEnable() 
    {
        base.OnEnable(); 
        _model = _modelService.GetOrCreateSingletonModel<PlayerDataModel>();
    }

    public float Mana 
    {
        get => _model.mana;
        set => _model.mana = value;
    }
}