using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonRoom : RoomBase {
  public DungeonRoomType type;

  /// <summary>
  ///     Room templates assigned to the room.
  /// </summary>
  [FormerlySerializedAs("IndividualRoomTemplates")]
  public List<GameObject> individualRoomTemplates = new List<GameObject>();

  /// <summary>
  ///     Assigned room template sets.
  /// </summary>
  [FormerlySerializedAs("RoomTemplateSets")]
  public List<RoomTemplatesSet> roomTemplateSets = new List<RoomTemplatesSet>();

  public override List<GameObject> GetRoomTemplates() {
    return individualRoomTemplates
      .Union(roomTemplateSets
        .Where(x => x != null)
        .SelectMany(x => x.RoomTemplates)
      )
      .Distinct()
      .ToList();
  }

  public override string GetDisplayName() {
    // Use the type of the room as its display name.
    return type.ToString();
  }

  public override RoomEditorStyle GetEditorStyle(bool isFocused) {
    var style = base.GetEditorStyle(isFocused);

    var backgroundColor = style.BackgroundColor;

    // Use different colors for different types of rooms
    switch (type) {
      case DungeonRoomType.Spawn:
        backgroundColor = new Color(38 / 256f, 115 / 256f, 38 / 256f);
        break;

      case DungeonRoomType.Boss:
        backgroundColor = new Color(128 / 256f, 0 / 256f, 0 / 256f);
        break;

      case DungeonRoomType.Shop:
        backgroundColor = new Color(102 / 256f, 0 / 256f, 204 / 256f);
        break;

      case DungeonRoomType.Reward:
        backgroundColor = new Color(102 / 256f, 0 / 256f, 204 / 256f);
        break;
    }

    style.BackgroundColor = backgroundColor;

    // Darken the color when focused
    if (isFocused) {
      style.BackgroundColor = Color.Lerp(style.BackgroundColor, Color.black, 0.7f);
    }

    return style;
  }
}
