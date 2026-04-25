using System;
using UnityEngine;
using YG;

public class LanguageManager : MonoBehaviour
{
    public static event Action<string> OnLanguageChanged;

    public static string CurrentLanguage { get; private set; } = "ru";

    void Start()
    {
        YG2.onCorrectLang += OnLanguageDetected;
        YG2.onSwitchLang += OnLanguageSwitched;
    }

    private void OnLanguageDetected(string detectedLang)
    {
        string finalLang = GetSupportedLanguage(detectedLang);
        SetLanguage(finalLang);
    }

    private void OnLanguageSwitched(string switchedLang)
    {
        SetLanguage(switchedLang);
    }

    private string GetSupportedLanguage(string detectedLang)
    {
        detectedLang = detectedLang.ToLower();

        return "ru";

        // Логика для нескольких языков на будущее
        // if (detectedLang == "ru" || detectedLang == "en")
        // {
        //     return detectedLang;
        // }
        // else
        // {
        //     Debug.LogWarning($"Unsupported language '{detectedLang}'. Falling back to Russian.");
        //     return "ru";
        // }
    }

    private void SetLanguage(string newLang)
    {
        if (CurrentLanguage == newLang)
            return;

        CurrentLanguage = newLang;
        Debug.Log($"[LanguageManager] Language set to: {CurrentLanguage}");

        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void SwitchLanguageManually(string newLang)
    {
        if (newLang == "ru" || newLang == "en")
        {
            YG2.SwitchLanguage(newLang);
        }
        else
        {
            Debug.LogWarning($"[LanguageManager] Cannot switch to unsupported language: {newLang}");
        }
    }
}
