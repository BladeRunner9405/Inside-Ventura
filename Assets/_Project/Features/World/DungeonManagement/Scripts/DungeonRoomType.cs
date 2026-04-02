using UnityEngine;

// Types of room in a dungeon. The only special type is Normal: this is a room where enemies waves get processed.
// In other room types, nothing happens when player enters.
public enum DungeonRoomType
{
  Spawn, // Room where the player should spawn.
  Normal, // Room with a fight.
  Boss, // Room with a boss.
  Reward, // Room with a reward.
  Shop, // Room with a shop.
}
