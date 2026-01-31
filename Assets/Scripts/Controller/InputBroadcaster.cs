using UnityEngine;
using UnityEngine.InputSystem;
using System; // Required for Events

[RequireComponent(typeof(PlayerInput))]
public class InputBroadcaster : MonoBehaviour
{
    // Static events allow any object in the scene to listen in
    public static event Action<Vector2> MoveEvent;
    public static event Action PossessEvent;
    public static event Action ExorciseEvent;
    // NEW: Fire event that tells listeners if the button is pressed (true) or released (false)
    public static event Action<bool> AttackEvent;

    // Called by PlayerInput component
    public void OnMove(InputValue value)
    {
        // Broadcast the vector to anyone listening
        MoveEvent?.Invoke(value.Get<Vector2>());
    }

    public void OnPossess(InputValue value)
    {
        if (value.isPressed) PossessEvent?.Invoke();
    }

    public void OnExorcise(InputValue value)
    {
        if (value.isPressed) ExorciseEvent?.Invoke();
    }

    // NEW: This function catches the Input System signal
    public void OnAttack(InputValue value)
    {
        // Broadcasts "true" when pressed, "false" when released
        AttackEvent?.Invoke(value.isPressed);
    }
}