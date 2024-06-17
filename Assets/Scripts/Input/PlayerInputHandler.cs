using Game;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Active { get; private set; }

    public Vector2 Movement { get; set; }

    public UnityEvent OnSwitchFireMode;

    private void HandleMovement(Vector2 value) => Movement = value;
    private void HandleSwitchFireMode() => OnSwitchFireMode?.Invoke();

    private void OnEnable()
    {
        InputManager.OnMovementInput += HandleMovement;
        InputManager.OnSwitchFireModeInput += HandleSwitchFireMode;
    }

    private void OnDisable()
    {
        InputManager.OnMovementInput -= HandleMovement;
        InputManager.OnSwitchFireModeInput -= HandleSwitchFireMode;
    }
}
