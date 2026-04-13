using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtItemTooltip : InjectMonoBehaviour
{
  [SerializeField] private Image tooltipBackground;
  [SerializeField] private TooltipText tooltip;

  [SerializeField] private float heightOffset = 2.5f;

  private RectTransform _rectTransform;
  private Camera _mainCamera;
  private Vector3 _currentWorldPosition;
  private bool _isVisible;

  protected override void OnEnable()
  {
    base.OnEnable();

    _rectTransform = GetComponent<RectTransform>();
    _mainCamera = Camera.main;

    Hide();
  }

  public void Show(Thought thought, Vector3 worldPosition)
  {
    if (thought == null || tooltip == null) return;

    _isVisible = true;
    _currentWorldPosition = worldPosition;

    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);

    string tooltipTextStr = ThoughtTooltip.BuildTooltipText(thought);
    tooltip.SetText(tooltipTextStr);

    UpdatePosition();
  }

  public void Hide()
  {
    _isVisible = false;

    if (tooltipBackground != null)
      tooltipBackground.color = Color.clear;

    tooltip?.Clear();
  }

  private void LateUpdate()
  {
    if (_isVisible)
      UpdatePosition();
  }

  private void UpdatePosition()
  {
    if (_mainCamera == null) return;

    Vector3 worldPos = _currentWorldPosition + Vector3.up * heightOffset;
    Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

    if (_rectTransform != null)
      _rectTransform.position = screenPos;
  }
}
