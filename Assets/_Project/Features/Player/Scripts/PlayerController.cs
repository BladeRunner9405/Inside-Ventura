using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player player;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_interactAction;
    private InputAction m_attackAction;
    private InputAction m_abilityAction;

    private Vector2 m_moveAmt;
    private Vector2 m_lookWorldPos;

    private void Awake()
    {
        // Кэшируем действия из InputSystem
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_lookAction = InputSystem.actions.FindAction("Look");
        m_interactAction = InputSystem.actions.FindAction("Interact");
        m_attackAction = InputSystem.actions.FindAction("Attack");
        m_abilityAction = InputSystem.actions.FindAction("UseAbility");
    }

    private void Update()
    {
        if (player == null || player.IsDead) return;

        // Сбор данных ввода
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        
        // Перевод экранных координат мыши в мировые
        Vector2 screenPos = m_lookAction.ReadValue<Vector2>();
        m_lookWorldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // Обработка нажатий
        if (m_interactAction.WasPressedThisFrame())
            player.TryToInteract();

        if (m_attackAction.WasPressedThisFrame())
            ExecuteAttack();

        if (m_abilityAction.WasPressedThisFrame())
            ExecuteAbility();
    }

    private void FixedUpdate()
    {
        if (player == null || player.IsDead) return;

        // Движение
        if (!player.IsDashing)
        {
            player.Move(m_moveAmt);
        }

        // Прицеливание (визуальный поворот оружия/персонажа)
        player.LookAt(m_lookWorldPos);
    }

    private void ExecuteAttack()
    {
        Vector2 direction = GetDirectionToMouse();
        player.Attack(direction);
    }

    private void ExecuteAbility()
    {
        // Приоритет направления: если движемся — используем вектор движения, иначе — вектор к мыши
        Vector2 direction = (m_moveAmt != Vector2.zero) 
            ? m_moveAmt.normalized 
            : GetDirectionToMouse();

        player.UseAbility(direction);
    }

    private Vector2 GetDirectionToMouse()
    {
        Vector2 direction = m_lookWorldPos - (Vector2)player.transform.position;
        return direction == Vector2.zero ? Vector2.right : direction.normalized;
    }
}