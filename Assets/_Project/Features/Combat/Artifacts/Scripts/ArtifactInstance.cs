using System;
using UnityEngine;

public abstract class ArtifactInstance
{
    public readonly Artifact BaseData;
    public readonly PlayerAccessor PlayerAccessor; // <-- Добавили доступ к игроку
    public Thought[] EquippedThoughts { get; private set; }

    public event Action<int, Thought> OnThoughtEquipped;
    public event Action<int> OnThoughtUnequipped;

    // Передаем PlayerAccessor при создании
    protected ArtifactInstance(Artifact baseData, PlayerAccessor playerAccessor)
    {
        BaseData = baseData;
        PlayerAccessor = playerAccessor;
        EquippedThoughts = new Thought[baseData.SlotsCount];

        if (baseData.InitialThoughts != null)
        {
            for (int i = 0; i < Mathf.Min(baseData.InitialThoughts.Length, baseData.SlotsCount); i++)
            {
                if (baseData.InitialThoughts[i] != null)
                {
                    EquipThought(baseData.InitialThoughts[i], i);
                }
            }
        }
    }

    public void EquipThought(Thought thought, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= EquippedThoughts.Length) return;
        if (thought == null || EquippedThoughts[slotIndex] == thought) return;

        // Проверка типа: подходит ли мысль этому артефакту?
        if (!thought.HasRightType(BaseData)) return;

        UnequipThought(slotIndex); // Снимаем старую мысль

        EquippedThoughts[slotIndex] = thought;
        
        // Передаем текущий ИНСТАНС мысли, чтобы она могла применить свои эффекты
        thought.OnEquip(this); 

        OnThoughtEquipped?.Invoke(slotIndex, thought);
    }

    public void UnequipThought(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= EquippedThoughts.Length || EquippedThoughts[slotIndex] == null) return;

        var thought = EquippedThoughts[slotIndex];
        
        thought.OnUnequip(this);

        EquippedThoughts[slotIndex] = null;
        OnThoughtUnequipped?.Invoke(slotIndex);
    }

    // Вспомогательный метод для получения статов (по аналогии с твоим старым GetStat)
    public virtual Stat GetStat(StatName statName)
    {
        return null; // Переопределяется в наследниках (WeaponInstance и т.д.)
    }
}