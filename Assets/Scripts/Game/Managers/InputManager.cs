using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        public static event Action<Vector2> OnMovementInput;
        public static event Action OnSwitchFireModeInput;
        public static event Action OnPauseInput;

        public void OnMovement(InputAction.CallbackContext context)
        {
            OnMovementInput?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSwitchFireMode(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnSwitchFireModeInput?.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnPauseInput?.Invoke();
            }
        }
    }
}
