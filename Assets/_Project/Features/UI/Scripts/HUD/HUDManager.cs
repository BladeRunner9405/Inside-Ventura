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
  private float _lastMaxHealth = -1f;

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

  protected override void OnEnable() {
    base.OnEnable();

    _playerAccessor.OnPlayerRegistered += OnPlayerRegistered;
  }

  protected void OnDisable() {
    Bindings.ReleaseAllBindings();
    _playerAccessor.OnPlayerRegistered -= OnPlayerRegistered;
  }

  private void OnPlayerRegistered(Player player) {
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
  }

  private void UpdateMoneyUI(float money) {
    if (moneyText != null)
      moneyText.text = Mathf.FloorToInt(money).ToString();
  }

  private void UpdateHealthUI(float currentHealth) {
    var maxHealth = GetMaxHealth();
    UpdateHearts(currentHealth, maxHealth);
  }

  private float GetMaxHealth() {
    return _playerAccessor.GetStat(StatName.MaxHealth).Value;
  }

  private void UpdateHearts(float currentHealth, float maxHealth) {
    if (heartsContainer == null || heartPrefab == null) return;

    if (Math.Abs(_lastMaxHealth - maxHealth) > 0.01f) {
      _lastMaxHealth = maxHealth;
      RebuildHearts();
    }

    var totalHearts = heartsContainer.childCount;
    if (totalHearts == 0) return;

    var healthPerHeart = maxHealth / totalHearts;
    for (var i = 0; i < totalHearts; ++i) {
      var heart = heartsContainer.GetChild(i);
      var heartImage = heart.GetComponent<Image>();
      if (heartImage == null) continue;

      var heartStartHealth = i * healthPerHeart;
      var heartEndHealth = (i + 1) * healthPerHeart;
      var heartHealth = Mathf.Clamp(currentHealth - heartStartHealth, 0, healthPerHeart);

      if (heartHealth <= 0)
        SetHeartSprite(heartImage, HeartState.Empty);
      else if (heartHealth >= healthPerHeart)
        SetHeartSprite(heartImage, HeartState.Full);
      else
        SetHeartSprite(heartImage, HeartState.Half);
    }
  }

  private void RebuildHearts() {
    foreach (Transform child in heartsContainer)
      Destroy(child.gameObject);

    var heartCount = Mathf.CeilToInt(_lastMaxHealth);
    for (var i = 0; i < heartCount; ++i) Instantiate(heartPrefab, heartsContainer);
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
