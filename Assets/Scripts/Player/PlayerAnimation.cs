using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // пишем логику чтобы не было эффекта если игрок подпрыгнул
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _poofDustVFX;
    [SerializeField] private float _tiltAngle = 20f;
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatTransform;
    [SerializeField] private float _cowboyHatTiltModifier = 2f;
    [SerializeField] private float _yLandVelocityCheck = -10f; // Velocity - вектор скорости обьекта

    private Vector2 _velocityBeforePhysicsUpdate;
    private Rigidbody2D _rigidBody; // Rigidbody - переводится как твердое тело
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>(); 
    }

    private void Update()
    {
        DetectMoveDust();
        ApplyTilt();
    }

    private void OnEnable()
    {
        PlayerController.OnJump += PlayPoofDustVFX;
    }

    private void OnDisable()
    {
        PlayerController.OnJump -= PlayPoofDustVFX;
    }

    private void FixedUpdate()
    {
        _velocityBeforePhysicsUpdate = _rigidBody.velocity;
        Debug.Log(_velocityBeforePhysicsUpdate);
    }   

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_velocityBeforePhysicsUpdate.y < _yLandVelocityCheck)
        {
            PlayPoofDustVFX();
            _impulseSource.GenerateImpulse();
        }
    }

    private void DetectMoveDust()
    {
        if (PlayerController.Instance.CheckGrounded())
        {
            if (!_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Play();
            }
        }
        else
        {
            if (_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Stop();
            }
        }
    }

    private void PlayPoofDustVFX()
    {
        _poofDustVFX.Play();
    }

    private void ApplyTilt() // применить наклон
    {
        float targetAngle;

        if (PlayerController.Instance.MoveInput.x < 0f)
        {
            targetAngle = _tiltAngle;
        }
        else if (PlayerController.Instance.MoveInput.x > 0f)
        {
            targetAngle = -_tiltAngle;
        }
        else
        {
            targetAngle = 0f;
        }

        // Получается текущий поворот спрайта игрока (_characterSpriteTransform.rotation).
        //     Создаётся новая целевая ориентация (targetCharacterRotation), где угол по оси Z устанавливается в targetAngle, а остальные оси (X и Y) остаются как были.
        //     Применяется этот новый поворот к спрайту игрока, чтобы визуально наклонить его влево, вправо или вернуть в исходное положение в зависимости от направления движения.

        // Player Sprite
        Quaternion currentCharacterRotation = _characterSpriteTransform.rotation;
        Quaternion targetCharacterRotation = Quaternion.Euler(currentCharacterRotation.eulerAngles.x,
            currentCharacterRotation.eulerAngles.y, targetAngle);

        _characterSpriteTransform.rotation = Quaternion.Lerp(currentCharacterRotation, targetCharacterRotation,
            _tiltSpeed * Time.deltaTime);

        // Cowboy hat sprite
        Quaternion currentHatRotation = _cowboyHatTransform.rotation;
        Quaternion targetHatRotation = Quaternion.Euler(currentHatRotation.eulerAngles.x,
            currentHatRotation.eulerAngles.y, -targetAngle / _cowboyHatTiltModifier);

        _cowboyHatTransform.rotation = Quaternion.Lerp(currentHatRotation, targetHatRotation,
            _tiltSpeed * _cowboyHatTiltModifier * Time.deltaTime);
    }
}