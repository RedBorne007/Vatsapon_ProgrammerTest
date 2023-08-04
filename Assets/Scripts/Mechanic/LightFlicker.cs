using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Minimum amount of light's intensity")]
    [SerializeField] private float minIntensity = 2f;
    [Tooltip("Maximum amount of light's intensity")]
    [SerializeField] private float maxIntensity = 4f;
    [Tooltip("Minimum duration to wait before start flickering again")]
    [SerializeField] private float waitMin = 3f;
    [Tooltip("Maximum duration to wait before start flickering again")]
    [SerializeField] private float waitMax = 5f;
    [Tooltip("Minimum duration to between each flickering")]
    [SerializeField] private float waitInBetweenMin = 0.1f;
    [Tooltip("Maximum duration to between each flickering")]
    [SerializeField] private float waitInBetweenMax = 0.5f;
    [Tooltip("Minimum amount of flickering")]
    [SerializeField] private int minFlick = 2;
    [Tooltip("Maximum amount of flickering")]
    [SerializeField] private int maxFlick = 5;

    [Header("References")]
    [Tooltip("Renderer of light source")]
    [SerializeField] private MeshRenderer lightRenderer;
    [Tooltip("Default material when light goes dark")]
    [SerializeField] private Material defaultMaterial;
    [Tooltip("Light material in normal state")]
    [SerializeField] private Material lightMaterial;

    private float currentWaitDuration;
    private float currentWaitInBetweenDuration;
    private float targetWaitInBetween;
    private int flickerTime;
    private float targetFlick;

    private Light lights;

    private void Start()
    {
        lights = GetComponent<Light>();
        currentWaitDuration = Random.Range(waitMin, waitMax);
    }

    private void Update()
    {
        lightRenderer.material = lights.intensity < 0.25f ? defaultMaterial : lightMaterial;

        if (currentWaitDuration > 0f)
        {
            currentWaitDuration -= Time.deltaTime;

            if (currentWaitDuration <= 0f)
            {
                targetWaitInBetween = Random.Range(waitInBetweenMin, waitInBetweenMax);
                targetFlick = Random.Range(minFlick, maxFlick);

                currentWaitInBetweenDuration = targetWaitInBetween;
            }

            return;
        }

        if (currentWaitInBetweenDuration > 0f)
        {
            currentWaitInBetweenDuration -= Time.deltaTime;
            return;
        }

        lights.intensity = Random.Range(minIntensity, maxIntensity);
        flickerTime++;
        
        // If it reach target flick time, reset.
        if (flickerTime == targetFlick)
        {
            lights.intensity = Random.value > 0.5f ? minIntensity : maxIntensity;

            flickerTime = 0;
            currentWaitDuration = Random.Range(waitMin, waitMax);
            return;
        }

        currentWaitInBetweenDuration = targetWaitInBetween;
    }
}
