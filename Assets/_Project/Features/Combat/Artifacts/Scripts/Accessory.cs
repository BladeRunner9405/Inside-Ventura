using UnityEngine;

public abstract class Accessory : Artifact 
{
    [SerializeField] public float baseCooldown = 5f;
    
    // Передаем инстанс и направление, чтобы SO знал, откуда кастовать
    public abstract void ExecuteAbility(AccessoryInstance instance, Vector2 direction);
}