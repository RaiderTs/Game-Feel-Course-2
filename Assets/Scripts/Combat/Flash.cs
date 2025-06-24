using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _whiteFlashMaterial;
    [SerializeField] private float _flashTime = 0.1f;

    private SpriteRenderer[] _spriteRenderer;
    private ColorChanger _colorChanger;

    private void Awake()

    {
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>(); // берем все спрайты
        _colorChanger = GetComponent<ColorChanger>(); // берем компонент ColorChanger
    }

    public void StartFlash()
    {
        StartCoroutine(FlashRoutine()); // запускаем рутину
    }

    private IEnumerator FlashRoutine()
    {
        foreach (var sr in _spriteRenderer)
        {
            sr.material = _whiteFlashMaterial;
            if (_colorChanger)
            {
                _colorChanger.SetColor(Color.white);
            }
        }

        yield return new WaitForSeconds(_flashTime);
        SetDefaultMaterial();
    }

    private void SetDefaultMaterial()
    {
        foreach (var sr in _spriteRenderer)
        {
            sr.material = _defaultMaterial;

            if (_colorChanger)
            {
                _colorChanger.SetColor(_colorChanger.DefaultColor);
            }
        }
    }
}