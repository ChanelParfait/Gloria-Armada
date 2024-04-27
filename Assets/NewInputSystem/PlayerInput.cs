using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerActions actions;

    static PlayerInput() {
        actions = new PlayerActions();
        actions.Gameplay.Enable();
    }
    public static Vector2 Move() {
        return actions.Gameplay.Move.ReadValue<Vector2>();
    }
}
