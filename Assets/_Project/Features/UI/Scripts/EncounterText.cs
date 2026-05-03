using CherryFramework.DependencyManager;
using InsideVentura.World.v1;
using TMPro;

public class EncounterText : InjectMonoBehaviour
{
  private TextMeshProUGUI _text;

  private bool _lastIsEncounter;

  private void Awake()
  {
    _text = GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    bool isEncounter = EncounterStatus.Active;
    if (_lastIsEncounter != isEncounter)
    {
      _lastIsEncounter = isEncounter;
      _text.enabled = isEncounter;
    }
  }
}

