using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Action OnExplode; // делегат
    public Action OnBeep;

    [SerializeField] private GameObject _explodeVFX; // объект взрыва
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _torqueAmount = 2f;
    [SerializeField] private float _explosionRadius = 3.5f; // радиус взрыва
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _lightBlinkTime = 0.15f; // время мигания
    [SerializeField] private int _totalBlinks = 3; // количество миганий
    [SerializeField] private int _explodeTime = 3; // время взрыва

    private int _currentBlinks;
    private Rigidbody2D _rigidBody; // получаем доступ к компоненту Rigidbody2D

    private CinemachineImpulseSource
        _impulseSource; // получаем доступ к компоненту CinemachineImpulseSource для создания импульса

    private void OnEnable()
    {
        OnExplode += Explosion;
        OnExplode += GrenadeScreenShake;
        OnExplode += DamageNearBy;
        OnBeep += BlinkLight;
    }

    private void OnDisable()
    {
        OnExplode -= Explosion;
        OnExplode -= GrenadeScreenShake;
        OnExplode -= DamageNearBy;
        OnBeep -= BlinkLight;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        LaunchGrenade();
        StartCoroutine(CountdownExplodeRoutine());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            OnExplode?.Invoke();
        }
    }

    private void LaunchGrenade() // метод для запуска гранаты
    {
        Vector2 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition); // получаем позицию мыши в мировых координатах
        Vector2 directionToMouse =
            (mousePos - (Vector2)transform.position).normalized; // вычисляем направление движения гранаты
        _rigidBody.AddForce(directionToMouse * _launchForce,
            ForceMode2D.Impulse); // добавляем импульс в направлении мыши
        _rigidBody.AddTorque(_torqueAmount, ForceMode2D.Impulse); // добавляем импульс вращения
    }

    public void Explosion() // метод для взрыва гранаты
    {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void GrenadeScreenShake() // метод для создания импульса
    {
        _impulseSource.GenerateImpulse();
    }

    private void DamageNearBy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemyLayerMask);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>(); // получаем доступ к компоненту Health
            health?.TakeDamage(_damageAmount); // наносим урон
        }
    }

    private IEnumerator CountdownExplodeRoutine()
    {
        while (_currentBlinks < _totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / _totalBlinks); // ждем время взрыва
            OnBeep?.Invoke(); // вызываем делегат
            yield return new WaitForSeconds(_lightBlinkTime); // ждем время взрыва
            _grenadeLight.SetActive(false);
        }

        OnExplode?.Invoke(); // вызываем делегат
    }

    private void BlinkLight()
    {
        _grenadeLight.SetActive(true);
        _currentBlinks++;
    }
}