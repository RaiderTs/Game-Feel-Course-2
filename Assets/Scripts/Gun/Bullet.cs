using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockbackTrust = 20f;

    private Vector2 _fireDirection; // вектор направления
    private Rigidbody2D _rigidBody;
    private Gun _gun;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _fireDirection * _moveSpeed;
    }

    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mousePos)
    {
        _gun = gun;
        transform.position = bulletSpawnPos; // задаем позицию объекта
        _fireDirection = (mousePos - bulletSpawnPos).normalized; // получаем вектор направления
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.gameObject.GetComponent<Health>();
        health?.TakeDamage(_damageAmount);

        Knockback knockback = other.gameObject.GetComponent<Knockback>();
        knockback?.GetKnockedBack(PlayerController.Instance.transform.position, _knockbackTrust);

        Flash flash = other.gameObject.GetComponent<Flash>();
        flash?.StartFlash();

        _gun.ReleaseBulletFromPool(this); // освобождаем пулю из пуула
    }
}