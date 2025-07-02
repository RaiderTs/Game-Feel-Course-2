using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float _parallaxOffset = -0.1f;

    private Vector2 _startPos;
    private Camera _mainCamera;

    private Vector2 _travel =>
        (Vector2)_mainCamera.transform.position -
        _startPos; // Свойство, вычисляющее смещение камеры относительно стартовой позиции объекта

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _startPos = transform.position; // Сохраняем стартовую позицию объекта для расчёта параллакса
    }

    private void FixedUpdate()
    {
        Vector2
            newPosition =
                _startPos + new Vector2(_travel.x * _parallaxOffset,
                    0f); // Вычисляем новую позицию объекта с учётом параллакса по оси X, оставляя Y без изменений
        transform.position = new Vector2(newPosition.x, transform.position.y);
    }
}