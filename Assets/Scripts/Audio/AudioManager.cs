using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundSO _gunShoot;
    [SerializeField] private SoundSO _jump;

    private void OnEnable()
    {
        Gun.OnShoot += Gun_OnShoot;
        PlayerController.OnJump += PlayerController_OnJump;
    }

    private void OnDisable()
    {
        Gun.OnShoot -= Gun_OnShoot;
        PlayerController.OnJump += PlayerController_OnJump;
    }

    private void PlaySound(SoundSO soundSO)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = soundSO.Clip;
        audioSource.Play();
    }

    private void Gun_OnShoot()
    {
        PlaySound(_gunShoot);
    }

    private void PlayerController_OnJump()
    {
        PlaySound(_jump);
    }
}