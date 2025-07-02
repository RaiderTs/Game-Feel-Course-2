using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{
    public static Action OnDiscoBallHitEvent;

    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = 0.2f;
    [SerializeField] private Light2D _globalLight;

    private float _defaultGlobalLightIntensity;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotlights;

    private void Awake()
    {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }

    private void Start()
    {
        _allSpotlights = FindObjectsOfType<ColorSpotlight>();
    }

    private void OnEnable()
    {
        OnDiscoBallHitEvent += DimTheLights;
    }

    private void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLights;
    }

    public void DiscoBallParty()
    {
        if (_discoCoroutine != null)
        {
            return;
        }

        OnDiscoBallHitEvent.Invoke();
    }


    private void DimTheLights()
    {
        foreach (ColorSpotlight spotlight in _allSpotlights)
        {
            StartCoroutine(spotlight.SpotLightDiscoParty(_discoBallPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine()
    {
        _globalLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
}