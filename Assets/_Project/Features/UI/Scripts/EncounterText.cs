using CherryFramework.DependencyManager;
using InsideVentura.World;
using TMPro;
using UnityEngine;

public class EncounterText : InjectMonoBehaviour
{
  [Inject]
  private DungeonManager _dungeonManager;

  private TextMeshProUGUI _text;

  private bool _lastIsEncounter;

  private void Awake()
  {
    _text = GetComponent<TextMeshProUGUI>();
  }

  private void Update()
  {
    if (_dungeonManager == null || _text == null) return;

    var currentRoomType = _dungeonManager.CurrentRoom.GetRoom().type;

    bool isEncounter = currentRoomType != DungeonRoomType.Safe && currentRoomType != DungeonRoomType.Spawn &&
                    currentRoomType != DungeonRoomType.Reward && currentRoomType != DungeonRoomType.Shop;
    if (_lastIsEncounter != isEncounter)
    {
      _lastIsEncounter = isEncounter;
      _text.enabled = isEncounter;
    }
  }
}

