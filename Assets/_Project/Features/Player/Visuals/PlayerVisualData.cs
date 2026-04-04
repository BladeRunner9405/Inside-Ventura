using UnityEngine;

namespace InsideVentura.Player
{
    [CreateAssetMenu(fileName = "NewPlayerVisual", menuName = "InsideVentura/Player/VisualData")]
    public class PlayerVisualData : ScriptableObject
    {
        [Header("Movement")]
        public Sprite[] idleSprites;
        public Sprite[] walkSprites;
        public Sprite[] dashSprites; // Кадры для переката/рывка
        
        [Header("State")]
        public Sprite deathSprite;

        [Header("Settings")]
        public float animationSpeed = 0.1f;
    }
}