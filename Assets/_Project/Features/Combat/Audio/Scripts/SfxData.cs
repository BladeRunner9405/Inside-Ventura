using UnityEngine;

[CreateAssetMenu(fileName = "NewSfxData", menuName = "Audio/Sfx Data")]
public class SfxData : ScriptableObject
{
    [Header("Combat")]
    public AudioClip attack;
    public AudioClip takeDamage;
    public AudioClip death;

    [Header("Movement")]
    public AudioClip dash;

    [Header("Random Pitch")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
}