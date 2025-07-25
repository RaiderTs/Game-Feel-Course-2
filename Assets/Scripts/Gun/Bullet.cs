using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletVFX;
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
        Instantiate(_bulletVFX, transform.position, Quaternion.identity);

        IHitable iHitable = other.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();


        IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(_fireDirection, _damageAmount, _knockbackTrust);

        _gun.ReleaseBulletFromPool(this); // освобождаем пулю из пуула
    }
}