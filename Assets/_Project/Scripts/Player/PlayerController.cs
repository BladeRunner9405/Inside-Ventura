using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_interactAction;

    public Entity player;
    public AimTarget playerAim;
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt; // в координатах мира, используя основную камеру

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_lookAction = InputSystem.actions.FindAction("Look");
        m_interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = Camera.main.ScreenToWorldPoint(m_lookAction.ReadValue<Vector2>());

        if (m_interactAction.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("Action: Interract");

        // ...
    }

    private void FixedUpdate()
    {
        Walking();
        Looking();
    }

    private void Walking()
    {
        player.moveTo(m_moveAmt);
    }

    private void Looking()
    {
        playerAim.aimAt(m_lookAmt);
    }
}
