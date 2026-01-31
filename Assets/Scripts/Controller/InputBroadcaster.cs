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
    public static event Action AttackEvent;

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

    public void OnAttack(InputValue value)
    {
        if (value.isPressed) AttackEvent?.Invoke();
    }
}