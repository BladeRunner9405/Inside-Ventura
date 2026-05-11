using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Важно для List

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    // Одиночный sfxSource мы убрали из Инспектора!

    [Header("Background Music")]
    [SerializeField] private AudioClip startingMusic;
    [SerializeField] private AudioClip pickupSound;

    [Header("SFX Settings")]
    [SerializeField] private int sfxChannels = 5; // Сколько звуков могут иметь независимый Pitch
    private List<AudioSource> _sfxSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXPool(); // Создаем наши каналы
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (startingMusic != null)
            PlayMusic(startingMusic, 0);
    }

    // --- Инициализация пула ---
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxChannels; i++)
        {
            // Скрипт сам вешает на себя 5 невидимых AudioSource
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.spatialBlend = 0f; // 2D звук
            _sfxSources.Add(newSource);
        }
    }

    // --- Музыка ---
    public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (musicSource.clip == clip) return;
        StartCoroutine(FadeMusic(clip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip nextClip, float duration)
    {
        if (musicSource.clip != null)
        {
            float startVol = musicSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0, t / duration);
                yield return null;
            }
        }

        musicSource.clip = nextClip;
        musicSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
    }

    // --- SFX (Обновлено) ---
    public void PlaySFX(AudioClip clip, float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        if (clip == null) return;

        // Находим свободный канал
        AudioSource availableSource = GetAvailableSFXSource();

        availableSource.pitch = Random.Range(minPitch, maxPitch);
        availableSource.PlayOneShot(clip);
    }

    private AudioSource GetAvailableSFXSource()
    {
        // Ищем канал, который сейчас ничего не играет
        foreach (var source in _sfxSources)
        {
            if (!source.isPlaying) return source;
        }

        // Если прямо сейчас орут 5 звуков одновременно (все каналы заняты),
        // просто берем первый попавшийся. PlayOneShot наслоит звук сверху, ничего не сломается.
        return _sfxSources[0];
    }

    public void PlaySFXpickup(float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        AudioSource availableSource = GetAvailableSFXSource();

        availableSource.pitch = Random.Range(minPitch, maxPitch);
        availableSource.PlayOneShot(pickupSound);
    }
}