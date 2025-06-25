using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // пишем логику чтобы не было эффекта если игрок подпрыгнул
    [SerializeField] private ParticleSystem _moveDustVFX;

    private void Update()
    {
        DetectMoveDust();
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
}