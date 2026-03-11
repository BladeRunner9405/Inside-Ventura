using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  [SerializeField] private Player player;

  [SerializeField] private PlayerEquipment playerEquipment;

  [SerializeField] private AimTarget playerAim;
  private InputAction m_abilityAction;

  private InputAction m_attackAction;
  private InputAction m_interactAction;
  private InputAction m_lookAction;
  private Vector2 m_lookAmt; // в координатах мира, используя основную камеру

  private InputAction m_moveAction;
  private Vector2 m_moveAmt;

  private void Awake() {
    m_moveAction = InputSystem.actions.FindAction("Move");
    m_lookAction = InputSystem.actions.FindAction("Look");
    m_interactAction = InputSystem.actions.FindAction("Interact");

    m_attackAction = InputSystem.actions.FindAction("Attack");
    m_abilityAction = InputSystem.actions.FindAction("UseAbility");
  }

  private void Update() {
    m_moveAmt = m_moveAction.ReadValue<Vector2>();
    m_lookAmt = Camera.main.ScreenToWorldPoint(m_lookAction.ReadValue<Vector2>());

    if (m_interactAction.WasPressedThisFrame()) Interact();

    if (m_attackAction.WasPressedThisFrame()) Attack();

    if (m_abilityAction.WasPressedThisFrame()) UseAbility();
  }

  private void FixedUpdate() {
    Walking();
    Looking();
  }

  private void Interact() {
    player.TryToInteract();
  }

  private void Attack() {
    if (playerEquipment) {
      var direction = (m_lookAmt - (Vector2)player.transform.position).normalized;
      playerEquipment.TryToAttack(direction);
    }
  }

  private void UseAbility() {
    if (playerEquipment) {
      var direction = (m_lookAmt - (Vector2)player.transform.position).normalized;
      playerEquipment.TryToUseAbility(direction);
    }
  }

  private void Walking() {
    player.Move(m_moveAmt);
  }

  private void Looking() {
    playerAim.aimAt(m_lookAmt);
  }
}
