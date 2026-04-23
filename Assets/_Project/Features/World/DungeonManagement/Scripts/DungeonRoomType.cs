using UnityEngine;

namespace InsideVentura.World {
// Types of room in a dungeon. The only special type is Normal: this is a room where enemies waves get processed.
// In other room types, nothing happens when player enters.
  public enum DungeonRoomType {
    Safe, // Safe room with no enemies
    Spawn, // Room where the player should spawn.
    NormalRank1,
    NormalRank2,
    NormalRank3,
    MiniBoss,
    Boss, // Room with a boss.
    Reward, // Room with a reward.
    Shop // Room with a shop.
  }
}
