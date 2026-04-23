using Edgar.Unity;
using InsideVentura.World.v1;
using UnityEngine;


namespace InsideVentura.World {
  public class DungeonDoor : InteractableObject {
    [Header("Sprites (0: North, 1: South, 2: East, 3: West)")] [SerializeField]
    private Sprite[] closedDoors = new Sprite[4];

    [SerializeField] private Sprite[] openedDoors = new Sprite[4];

    [Space] [SerializeField] private SpriteRenderer spriteRenderer;

    private DoorInstanceGrid2D _doorInstance;
    private DoorDirection _direction;
    private bool _locked = false;

    public void Init(DoorInstanceGrid2D doorInstance) {
      _doorInstance = doorInstance;
      var vecDir = doorInstance.FacingDirection;
      if (vecDir == Vector2Int.up) {
        _direction = DoorDirection.North;
      }
      else if (vecDir == Vector2Int.down) {
        _direction = DoorDirection.South;
      }
      else if (vecDir == Vector2Int.left) {
        _direction = DoorDirection.West;
      }
      else if (vecDir == Vector2Int.right) {
        _direction = DoorDirection.East;
      }
    }

    public void SetClosed() {
      _locked = true;
      if (closedDoors.Length > (int)_direction) {
        Debug.Log(_direction);
        spriteRenderer.sprite = closedDoors[(int)_direction];
      }
    }

    public void SetOpen() {
      _locked = false;
      if (openedDoors.Length > (int)_direction) {
        spriteRenderer.sprite = openedDoors[(int)_direction];
      }
    }

    public override void OnInteract() {
      if (!_locked) {
        // Find the position to teleport the player, hide this door and show the other.
        var player = PlayerAccessor.Transform;

        Vector2Int dir = _doorInstance.FacingDirection * 2;
        player.position = transform.position + new Vector3(dir.x, dir.y, 0);

        var otherTemplate = _doorInstance.ConnectedRoomInstance.RoomTemplateInstance;
        otherTemplate.gameObject.SetActive(true);
        otherTemplate.GetComponent<RoomManagerBase>().OnRoomEnter(player.gameObject);

        var thisRoomManager = transform.parent.GetComponent<RoomManagerBase>();
        thisRoomManager.OnRoomLeave(player.gameObject);
        thisRoomManager.gameObject.SetActive(false);
      }
      else {
        Debug.Log("<color=red>Дверь заперта!</color> Нужно победить всех врагов.");
        // Здесь можно запустить звук "дверь закрыта" или анимацию тряски
      }
    }
  }
}
