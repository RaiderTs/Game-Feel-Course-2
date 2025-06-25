using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnDeath;

    [SerializeField] private GameObject _splatterPrefab;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private int _startingHealth = 3;


    private int _currentHealth;

    private void Start()
    {
        ResetHealth();
    }

    private void OnEnable()
    {
        OnDeath += SpawnDeathVFX;
        OnDeath += SpawnDeathSplatterPrefab;
    }

    private void OnDisable()
    {
        OnDeath -= SpawnDeathVFX;
        OnDeath -= SpawnDeathSplatterPrefab;
    }


    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private void SpawnDeathSplatterPrefab()
    {
        GameObject newSplatterPrefab = Instantiate(_splatterPrefab, transform.position, transform.rotation);
        SpriteRenderer deathSplatterSpriteRenderer = newSplatterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = GetComponent<ColorChanger>(); // получаем компонент ColorChanger
        Color currentColor = colorChanger.DefaultColor; // получаем цвет из компонента ColorChanger

        deathSplatterSpriteRenderer.color = currentColor;
    }

    private void SpawnDeathVFX()
    {
        GameObject deathVFX = Instantiate(_deathVFX, transform.position, transform.rotation);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;

        ColorChanger colorChanger = GetComponent<ColorChanger>(); // получаем компонент ColorChanger
        Color currentColor = colorChanger.DefaultColor; // получаем цвет из компонента ColorChanger
        ps.startColor = currentColor;
    }
}