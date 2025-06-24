using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd; // паттерн обсервер


    [SerializeField] private float _knockbackTime = 0.2f; // время удара

    private Vector3 _hitDirection; // направление удара
    private float _knockbackTrust; // дистанция удара

    private Rigidbody2D _rigidbody; // компонент Rigidbody2D

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>(); // получаем компонент Rigidbody2D
    }

    private void OnEnable()
    {
        OnKnockbackStart += ApplyKnockbackForce; // применяем удар
        OnKnockbackEnd += StopKnockRoutine;
    }

    private void OnDisable()
    {
        OnKnockbackStart -= ApplyKnockbackForce; // применяем удар
        OnKnockbackEnd -= StopKnockRoutine;
    }

    public void GetKnockedBack(Vector3 hitDirection, float knockbackTrust) // получаем удар
    {
        Debug.Log("knocked back");
        _hitDirection = hitDirection;
        _knockbackTrust = knockbackTrust;

        OnKnockbackStart?.Invoke();
    }

    private void ApplyKnockbackForce() // применяем удар
    {
        Vector3 difference =
            (transform.position - _hitDirection).normalized * _knockbackTrust *
            _rigidbody.mass; // вычисляем разницу между позицией и направлением удара
        _rigidbody.AddForce(difference, ForceMode2D.Impulse); // применяем силу удара
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine() // рутину удара
    {
        yield return new WaitForSeconds(_knockbackTime); // ждем время удара
        OnKnockbackEnd?.Invoke(); // вызываем событие завершения удара
    }

    private void StopKnockRoutine() // завершение выполнения рутины удара
    {
        _rigidbody.velocity = Vector2.zero; // Останавливаем движение. Velocity - скорость
    }
}