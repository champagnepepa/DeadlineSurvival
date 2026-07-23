using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    private Light LightToFlicker;
    [SerializeField, Range(0f, 100000f)] private float minIntensity = 100000;
    [SerializeField, Range(0f, 100000f)] private float maxIntensity = 100000f;
    [SerializeField, Min(0f)] private float timeBetweenIntensity = 0.1f;

    private float currentTimer;

    private void Awake()
    {
        if (LightToFlicker == null)
        {
            LightToFlicker = GetComponent<Light>();
        }

        ValidateIntensityBounds();
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;
        if (!(currentTimer >= timeBetweenIntensity))
            return;
        LightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
        currentTimer = 0;
    }

    private void ValidateIntensityBounds()
    {
        if(!(minIntensity > maxIntensity))
        {
            return;
        }
        Debug.LogWarning("Min Intensity is greater then max Intensity, swapping values!");
        (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
    }
}
