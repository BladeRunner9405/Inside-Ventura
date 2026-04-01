using System;
using UnityEngine;

public class AccessoryInstance : ArtifactInstance
{
    public readonly Accessory AccessoryData;
    public ModifiableStat Cooldown { get; private set; }
    private float _lastUseTime;

    // Событие для PushAwayEnemiesEffect
    public event Action<AccessoryInstance, Vector2> OnAbilityUsed; 

    public AccessoryInstance(Accessory baseData, PlayerAccessor playerAccessor) : base(baseData, playerAccessor)
    {
        AccessoryData = baseData;
        Cooldown = new ModifiableStat(baseData.baseCooldown);
        _lastUseTime = -Cooldown.ModifiedValue; 
    }

    public bool CanUse() => Time.time >= _lastUseTime + Cooldown.ModifiedValue;

    public void TryUseAbility(Vector2 direction)
    {
        if (!CanUse()) return;

        // Передаем выполнение в SO
        AccessoryData.ExecuteAbility(this, direction);
        
        // Уведомляем мысли (эффекты) о том, что аксессуар использован
        OnAbilityUsed?.Invoke(this, direction); 
        
        _lastUseTime = Time.time;
    }
}