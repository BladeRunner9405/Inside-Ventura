using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyVisual", menuName = "Inside-Ventura/EnemyVisual")]
public class EnemyVisualData : ScriptableObject
{
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] attackSprites;
    public Sprite deathSprite;
    
    public float animationSpeed = 0.1f;
    [Tooltip("На каком кадре атаки спавнить хитбокс?")]
    public int attackDamageFrame = 2; 
}