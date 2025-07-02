using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;

public class Gun : MonoBehaviour
{
    public static Action OnShoot; // делегат для выстрелов
    public static Action OnGrenadeShoot; // делегат для гранат

    [Header("Bullet")] [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = .5f; // время между выстрелами
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _muzzleFlashTime = 0.5f;


    [Header("Grenade")] [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _grenadeShootCD = .8f; // время между выстрелами


    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool; // пул для быстрой генерации пуль
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire"); // хэш для анимации

    private Vector2 _mousePos;
    private float _lastFireTime = 0f;
    private float _lastGrenadeTime = 0f;

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private CinemachineImpulseSource _impulseSource;
    private Animator _animator;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInput;
    }


    private void Start()
    {
        CreateBulletPool();
    }

    private void Update()
    {
        GatherInput(); // собираем ввод
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
        OnShoot += MuzzleFlash;
        OnGrenadeShoot += ShootGrenade;
        OnGrenadeShoot += FireAnimation;
        OnGrenadeShoot += ResetLastGrenadeShootTime;
    }

    private void OnDisable()
    {
        OnShoot -= ShootProjectile; // синтаксис отписки от события
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzleFlash;
        OnGrenadeShoot -= ShootGrenade;
        OnGrenadeShoot -= FireAnimation;
        OnGrenadeShoot -= ResetLastGrenadeShootTime;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput; // собираем ввод
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
            OnShoot?.Invoke(); // Вызываем событие. Вернее подписываемя на событие
        }

        if (_frameInput.Grenade && Time.time >= _lastGrenadeTime) // проверяем нажатие ПРАВОЙ кнопки мыши и время
        {
            OnGrenadeShoot?.Invoke();
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos); // Передаем позицию спавна и позицию мыши
    }

    private void ShootGrenade()
    {
        Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastGrenadeTime = Time.time;
    }

    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f); // 0 это номер Layer в табе Animator
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD; // обновляем время последнего выстрела
    }


    private void ResetLastGrenadeShootTime()
    {
        _lastGrenadeTime = Time.time + _grenadeShootCD; // обновляем время последнего броска гранаты
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

    private void MuzzleFlash()
    {
        if (_muzzleFlashRoutine != null)
        {
            StopCoroutine(_muzzleFlashRoutine);
        }

        _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }
}