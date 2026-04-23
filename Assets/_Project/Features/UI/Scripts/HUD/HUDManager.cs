using System;
using System.Collections;
using CherryFramework.BaseClasses;
using CherryFramework.DataModels;
using CherryFramework.DependencyManager;
using GeneratedDataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : BehaviourBase {
  [Header("UI Elements")]
  [SerializeField] private TextMeshProUGUI moneyText;

  [SerializeField] private Transform heartsContainer;
  [SerializeField] private Image cooldownFillImage;
  [SerializeField] private GameObject heartPrefab;

  [Header("Cooldown Indicator")]
  [SerializeField] private Color cooldownActiveColor = Color.red;
  [SerializeField] private Color cooldownReadyColor = Color.green;
  [SerializeField] private float readyFlashDuration = 0.3f;

  private Accessor<float> _healthAccessor;
  private float _lastMaxHealth;

  [Inject] private ModelService _modelService;
  private Accessor<float> _moneyAccessor;
  [Inject] private PlayerAccessor _playerAccessor;

  private PlayerDataModel _playerDataModel;
  private Coroutine _readyFlashCoroutine;
  private bool _wasCooldownReady = true;

  private bool _playerReady = false;

  private void Update() {
    if (_playerReady) UpdateCooldownUI();
  }

  private void Start() {
    _playerAccessor.OnPlayerRegistered += OnPlayerRegistered;
    _playerAccessor.OnPlayerUnregistered += OnPlayerUnregistered;
  }

  protected override void OnDestroy() {
    Bindings.ReleaseAllBindings();
    _playerAccessor.OnPlayerRegistered -= OnPlayerRegistered;
    _playerAccessor.OnPlayerUnregistered -= OnPlayerUnregistered;

    base.OnDestroy();
  }

  private void OnPlayerRegistered() {
    _playerReady = true;

    _playerDataModel = _modelService.GetOrCreateSingletonModel<PlayerDataModel>();

    _moneyAccessor = _playerDataModel.moneyAccessor;
    if (_moneyAccessor != null) {
      Bindings.CreateBinding(_moneyAccessor, UpdateMoneyUI);
      UpdateMoneyUI(_moneyAccessor.Value);
    }

    _healthAccessor = _playerDataModel.currentHealthAccessor;
    if (_healthAccessor != null) {
      Bindings.CreateBinding(_healthAccessor, UpdateHealthUI);
      UpdateHealthUI(_healthAccessor.Value);
    }

    _playerAccessor.OnStatModified += OnPlayerStatModified;
  }

  private void OnPlayerUnregistered() {
    _playerAccessor.OnStatModified -= OnPlayerStatModified;
  }

  private void OnPlayerStatModified(StatName statName)
  {
    switch (statName)
    {
      case StatName.MaxHealth:
        UpdateHealthUI(_healthAccessor.Value);
        break;
    }
  }

  private void UpdateMoneyUI(float money) {
    if (moneyText != null)
      moneyText.text = Mathf.FloorToInt(money).ToString();
  }

  private float GetMaxHealth() {
    return _playerAccessor.GetStatValue(StatName.MaxHealth);
  }

  private void UpdateHealthUI(float currentHealth) {
    UpdateHearts(currentHealth, GetMaxHealth());
  }

  private void UpdateHearts(float currentHealth, float maxHealth) {
    if (heartsContainer == null || heartPrefab == null) return;

    RebuildHearts(maxHealth);

    for (var i = 0; i < maxHealth; ++i) {
      var heart = heartsContainer.GetChild(i);
      var heartImage = heart.GetComponent<Image>();
      if (heartImage == null) continue;

      var heartHealth = Mathf.Clamp(currentHealth - i, 0f, 1f);

      if (heartHealth <= 0)
        SetHeartSprite(heartImage, HeartState.Empty);
      else if (heartHealth >= 1)
        SetHeartSprite(heartImage, HeartState.Full);
      else
        SetHeartSprite(heartImage, HeartState.Half);
    }
  }

  private void RebuildHearts(float maxHealth) {
    var lastHeartsCount = Mathf.CeilToInt(_lastMaxHealth);
    var heartsCount = Mathf.CeilToInt(maxHealth);

    if (lastHeartsCount == heartsCount) return;

    if (lastHeartsCount > heartsCount) {
      Debug.Log(1);
      for (var i = 0; i < lastHeartsCount - heartsCount; ++i) {
        var heart = heartsContainer.GetChild(heartsCount + i);
        heart.gameObject.SetActive(false);
      }
    }
    else if (lastHeartsCount < heartsCount) {
      for (var i = 0; i < heartsCount - lastHeartsCount; ++i) {
        if (heartsContainer.childCount - lastHeartsCount - i > 0) {
          var heart = heartsContainer.GetChild(lastHeartsCount + i);
          heart.gameObject.SetActive(true);
        }
        else {
          Instantiate(heartPrefab, heartsContainer);
        }
      }
    }

    _lastMaxHealth = maxHealth;
  }

  private void SetHeartSprite(Image image, HeartState state) {
    var heartView = image.GetComponent<HeartView>();
    if (heartView != null)
      heartView.SetState(state);
    else
      Debug.LogWarning("HeartView component missing on heart prefab");
  }

  private void UpdateCooldownUI() {
    if (cooldownFillImage == null) return;

    var accessoryInstance = _playerAccessor.Equipment?.Accessory;
    if (accessoryInstance == null) {
      cooldownFillImage.fillAmount = 1f;
      cooldownFillImage.color = cooldownReadyColor;
      return;
    }

    var cooldown = accessoryInstance.Cooldown?.ModifiedValue ?? 5f;
    float lastUse = accessoryInstance.LastUseTime;

    var remaining = Mathf.Max(0, lastUse + cooldown - Time.time);
    var isReady = remaining <= 0;

    var fill = isReady ? 1f : 1f - remaining / cooldown;
    cooldownFillImage.fillAmount = fill;

    if (isReady && !_wasCooldownReady) {
      cooldownFillImage.color = cooldownReadyColor;
      if (_readyFlashCoroutine != null) StopCoroutine(_readyFlashCoroutine);
      _readyFlashCoroutine = StartCoroutine(ReadyFlash());
    }
    else if (!isReady) {
      cooldownFillImage.color = cooldownActiveColor;
    }

    _wasCooldownReady = isReady;
  }

  private IEnumerator ReadyFlash() {
    var originalColor = cooldownFillImage.color;
    cooldownFillImage.color = Color.white;
    yield return new WaitForSeconds(readyFlashDuration / 2f);
    cooldownFillImage.color = cooldownReadyColor;
    yield return new WaitForSeconds(readyFlashDuration / 2f);
    cooldownFillImage.color = cooldownReadyColor;
    _readyFlashCoroutine = null;
  }
}
