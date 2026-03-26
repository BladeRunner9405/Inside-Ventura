using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
[RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
public class CameraShaker : MonoBehaviour
{
    [SerializeField] private float intensity = 3f; // Это размах движений камеры

    [SerializeField] private float frequency = 2f; // Это скорость этих движений. Как много колебаний (рывков туда-сюда) камера совершает за одну секунду.
    [SerializeField] private float duration = 0.2f;

    public static CameraShaker Instance { get; private set; }

    private CinemachineBasicMultiChannelPerlin _perlinNoise;

    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        _perlinNoise = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float damageAmount)
    {
        _perlinNoise.AmplitudeGain = intensity;
        
        _perlinNoise.FrequencyGain = frequency; 
        
        _startingIntensity = intensity;
        _shakeTimerTotal = duration;
        _shakeTimer = duration;
    }

    private void Update()
    {
        // Если таймер запущен, плавно уменьшаем тряску
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            
            // Mathf.Lerp плавно опускает амплитуду от стартовой до нуля
            float progress = 1f - (_shakeTimer / _shakeTimerTotal);
            _perlinNoise.AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, progress);
            
            // Когда время вышло, зануляем частоту тоже
            if (_shakeTimer <= 0)
            {
                _perlinNoise.FrequencyGain = 0f;
            }
        }
    }
}