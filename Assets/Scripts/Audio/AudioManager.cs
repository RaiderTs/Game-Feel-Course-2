using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0f, 2f)] [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private SoundsCollectionSO _soundsCollectionSO;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;

    private AudioSource _currentMusic;

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        FightMusic();
    }

    private void OnEnable()
    {
        Gun.OnShoot += Gun_OnShoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        PlayerController.OnJump += PlayerController_OnJump;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
        Health.OnDeath = HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent += DiscoBallMusic;
    }

    private void OnDisable()
    {
        Gun.OnShoot -= Gun_OnShoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        PlayerController.OnJump -= PlayerController_OnJump;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
        Health.OnDeath = HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent -= DiscoBallMusic;
    }

    #endregion

    #region Sound Methods

    private void PlayRandomSound(SoundSO[] sounds) // рандомный звук из коллекции
    {
        if (sounds != null && sounds.Length > 0)
        {
            int randomIndex = Random.Range(0, sounds.Length);
            SoundSO soundSO = sounds[randomIndex];
            SoundToPlay(soundSO);
        }
    }

    private void SoundToPlay(SoundSO soundSO)
    {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume * _masterVolume;
        bool loop = soundSO.Loop;
        AudioMixerGroup audioMixerGroup;

        pitch = RandomizePitch(soundSO, pitch);
        audioMixerGroup = DetermineAudioMixerGroup(soundSO);

        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO)
    {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;
                break;
            default:
                audioMixerGroup = null;
                break;
        }

        return audioMixerGroup;
    }

    private static float RandomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandomizePitch)
        {
            float randomPitchModifier =
                Random.Range(-soundSO.RandomPitchRangeModifier, soundSO.RandomPitchRangeModifier);
            pitch = soundSO.Pitch + randomPitchModifier;
        }

        return pitch;
    }

    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;

        audioSource.Play();

        if (!loop)
        {
            Destroy(soundObject, clip.length);
        }

        DetermineMusic(audioMixerGroup, audioSource);
    }

    private void DetermineMusic(AudioMixerGroup audioMixerGroup, AudioSource audioSource)
    {
        if (audioMixerGroup == _musicMixerGroup)
        {
            if (_currentMusic != null)
            {
                _currentMusic.Stop();
            }

            _currentMusic = audioSource;
        }
    }

    #endregion

    #region SFX

    private void Gun_OnShoot()
    {
        PlayRandomSound(_soundsCollectionSO.GunShoot);
    }

    private void PlayerController_OnJump()
    {
        PlayRandomSound(_soundsCollectionSO.Jump);
    }

    private void Health_OnDeath(Health health)
    {
        PlayRandomSound(_soundsCollectionSO.Splat);
    }

    private void Health_OnDeath()
    {
        PlayRandomSound(_soundsCollectionSO.Splat);
    }

    private void PlayerController_OnJetpack()
    {
        PlayRandomSound(_soundsCollectionSO.Jetpack);
    }

    public void Grenade_OnBeep()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeBeep);
    }

    public void Grenade_OnExplode()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeExplode);
    }

    private void Gun_OnGrenadeShoot()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeShoot);
    }

    public void Enemy_OnPlayerHit()
    {
        PlayRandomSound(_soundsCollectionSO.PlayerHit);
    }

    private void AudioManager_Megakill()
    {
        PlayRandomSound(_soundsCollectionSO.Megakill);
    }

    #endregion

    #region Music

    private void FightMusic()
    {
        PlayRandomSound(_soundsCollectionSO.FightMusic);
    }

    private void DiscoBallMusic()
    {
        PlayRandomSound(_soundsCollectionSO.DiscoParty);
        float soundLength =
            _soundsCollectionSO.DiscoParty[0].Clip.length; // Получаем длину первого аудиоклипа из коллекции DiscoParty
        Utils.RunAfterDelay(this, soundLength, FightMusic);
    }

    #endregion

    #region Custom SFX Logic

    private List<Health> _deathList = new List<Health>();

    private Coroutine _deathCoroutine;

    private void HandleDeath(Health health)
    {
        var isEnemy = health.GetComponent<Enemy>(); // Проверяем, является ли объект врагом
        if (isEnemy)
        {
            _deathList.Add(health);
        }

        if (_deathCoroutine == null)
        {
            _deathCoroutine = StartCoroutine(DeathWindowRoutine());
        }
    }

    private IEnumerator DeathWindowRoutine()
    {
        yield return null;

        int megaKillAmount = 3;

        if (_deathList.Count >= megaKillAmount)
        {
            AudioManager_Megakill();
        }

        Health_OnDeath();
        _deathList.Clear();
        _deathCoroutine = null;
    }

    #endregion
}