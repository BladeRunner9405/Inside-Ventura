using System;
using System.Collections.Generic;
using System.Linq;

public enum StatOperationType { Add, Multiply }

public class StatModifier 
{
    public readonly StatOperationType Type;
    public readonly float Value;
    public readonly object Source; // Кто добавил этот модификатор (обычно это Effect)

    public StatModifier(StatOperationType type, float value, object source) 
    {
        Type = type;
        Value = value;
        Source = source;
    }
}

[Serializable]
public class ModifiableStat : Stat 
{
    private List<StatModifier> modifiers = new();

    public ModifiableStat() { }
    public ModifiableStat(float baseValue) { Value = baseValue; }

    public float ModifiedValue 
    {
        get {
            var addSum = modifiers.Where(m => m.Type == StatOperationType.Add).Sum(m => m.Value);
            var multiplyFactor = modifiers.Where(m => m.Type == StatOperationType.Multiply)
                                          .Aggregate(1f, (current, m) => current * m.Value);
            return (Value + addSum) * multiplyFactor;
        }
    }

    public void AddModifier(StatModifier modifier) 
    {
        modifiers.Add(modifier);
    }

    // НОВЫЙ МЕТОД: удаляем все модификаторы от конкретного источника
    public void RemoveModifiersFromSource(object source) 
    {
        modifiers.RemoveAll(m => m.Source == source);
    }
}