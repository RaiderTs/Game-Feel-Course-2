using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private PlayerInputActions _playerInputActions;
    private InputAction _move, _jump, _jetpack, _granade;


    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
        _jetpack = _playerInputActions.Player.Jetpack;
        _granade = _playerInputActions.Player.Grenade;
    }

    private void OnEnable() // запускается при активации скрипта
    {
        _playerInputActions.Enable();
    }

    private void OnDisable() // запускается при деактивации скрипта
    {
        _playerInputActions.Disable();
    }

    private void Update()
    {
        FrameInput = GatherInput();
    }

    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            Jump = _jump.WasPressedThisFrame(),
            Jetpack = _jetpack.WasPressedThisFrame(),
            Grenade = _granade.WasPressedThisFrame()
        };
    }
}


public struct FrameInput
{
    public Vector2 Move;
    public bool Jump;
    public bool Jetpack;
    public bool Grenade;
}