using InsideVentura.World;
using TMPro;
using UnityEngine;

public class EncounterText : MonoBehaviour
{
  [SerializeField]
  private EncounterManager encounterManager;

  private TextMeshProUGUI _text;

  private bool _lastActiveState;

  private void Awake()
  {
    _text = GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    if (encounterManager == null || _text == null) return;

    bool isActive = encounterManager.IsEncounterActive;
    if (_lastActiveState != isActive)
    {
      _lastActiveState = isActive;
      _text.enabled = isActive;
    }
  }
}

