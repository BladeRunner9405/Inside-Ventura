using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// Теперь Artifact - это строго неизменяемая база данных (Static Data)
public abstract class Artifact : ScriptableObject 
{
    [SerializeField] public string artifactName;
    [SerializeField] protected int slotsCount = 3;
    
    // Базовые мысли для старта игры (настраиваются в инспекторе)
    [SerializeField] private Thought[] initialThoughts;

    public int SlotsCount => slotsCount;
    public Thought[] InitialThoughts => initialThoughts;

    // Метод убран, так как Artifact больше не отслеживает состояние
    // public virtual Stat GetStat(StatName statName) { return null; }
}