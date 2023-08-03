using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Tooltip("Minimum amount of flickering")]
    [SerializeField] private int flickeringMin = 2;
    [Tooltip("Maximum amount of flickering")]
    [SerializeField] private int flickeringMax = 4;
    [Tooltip("Minimum duration to wait before start flickering again")]
    [SerializeField] private float waitMin = 3f;
    [Tooltip("Minimum duration to between each flickering")]
    [SerializeField] private float waitInBetweenMin = 0.5f;
    [Tooltip("Minimum target amunt of click.")]
    [SerializeField] private int minTargetFlick = 2;

    private float currentWaitDuration;
    private float currentWaitInBetweenDuration;
    private int flickerTime;
    private float targetIntensity;

    private Light lights;

    private void Start()
    {
        lights = GetComponent<Light>();
        currentWaitDuration = Random.Range(flickeringMin, flickeringMin + 5f);
    }

    private void Update()
    {
        if (currentWaitDuration > 0f)
        {
            currentWaitDuration -= Time.deltaTime;

            if (currentWaitDuration <= 0f)
            {
                currentWaitInBetweenDuration = Random.Range(waitInBetweenMin, waitInBetweenMin + 0.25f);
            }
            return;
        }

        if (currentWaitInBetweenDuration > 0f)
        {
            currentWaitInBetweenDuration -= Time.deltaTime;
            return;
        }

        targetIntensity = Random.Range(flickeringMin, flickeringMax);
        flickerTime++;

        if (flickerTime == minTargetFlick)
        {
            flickerTime = 0;
        }
    }
}
