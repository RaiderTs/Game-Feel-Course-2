using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jumpStrength = 7f;

    // private PlayerInput _playerInput;
    // private FrameInput _frameInput;

    private Rigidbody2D _rigidBody;
    // private Movement _movement;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _rigidBody = GetComponent<Rigidbody2D>();
        // _playerInput = GetComponent<PlayerInput>(); // получаем компонент PlayerInput
        // _movement = GetComponent<Movement>(); // получаем компонент Movement
    }

    private void Update()
    {
        GatherInput();
        Jump();
        HandleSpriteFlip();
        Movement();
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }


    private bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0f, _groundLayer); // проверяем на земле
        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    private void GatherInput()
    {
        // _frameInput = _playerInput.FrameInput;
    }

    private void Movement()
    {
        // _movement.SetCurrentDirection(_frameInput.Move.x); // устанавливаем направление движения
    }

    private void Jump()
    {
        // if (!_frameInput.Jump)
        // {
        //     return;
        // }

        if (CheckGrounded())
        {
            _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        }
    }

    private void HandleSpriteFlip()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
}