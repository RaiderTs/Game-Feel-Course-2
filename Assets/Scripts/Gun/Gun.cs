using System;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;

public class Gun : MonoBehaviour
{
    public static Action OnShoot; // делегат для выстрелов

    // public Transform BulletSpawnPoint => _bulletSpawnPoint;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = .5f; // время между выстрелами

    private ObjectPool<Bullet> _bulletPool; // пул для быстрой генерации пуль
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire"); // хэш для анимации
    
    private Vector2 _mousePos;
    private float _lastFireTime = 0f;


    private CinemachineImpulseSource _impulseSource;
    private Animator _animator;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
    }


    private void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        Shoot();
        RotateGun();
    }

    // подписка на делегат событий (обьявлен вверху класса)
    private void OnEnable()
    {
        OnShoot += ShootProjectile; // синтаксис подписки на событие
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
    }

    private void OnDisable()
    {
        OnShoot -= ShootProjectile; // синтаксис отписки от события
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }


    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(
            () => { return Instantiate(_bulletPrefab); }, // создаем пулю
            bullet => { bullet.gameObject.SetActive(true); }, // активируем пулю
            bullet => { bullet.gameObject.SetActive(false); }, // деактивируем пулю
            bullet => { Destroy(bullet); }, // уничтожаем пулю
            false,
            20, // максимальное количество пуль
            40 // максимальное количество активных пуль
        );
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime) // проверяем нажатие левой кнопки мыши и время
        {
            // ShootProjectile();
            // ResetLastFireTime();
            OnShoot?.Invoke(); // вызываем событие. Вернее подписываемя на событие
        }
    }

    private void ShootProjectile()
    {
        // Bullet newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos); // Передаем позицию спавна и позицию мыши
    }

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f); // 0 это номер Layer в табе Animator
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD; // обновляем время последнего выстрела
    }

    private void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input
            .mousePosition); // получаем координаты мыши в мировых координатах
        Vector2 direction =
            PlayerController.Instance.transform.InverseTransformPoint(_mousePos); // получаем вектор направления
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // вычисляем угол поворота
        transform.localRotation = Quaternion.Euler(0, 0, angle); // устанавливаем поворот
    }
}