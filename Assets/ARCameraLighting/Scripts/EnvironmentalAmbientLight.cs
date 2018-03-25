using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change the ambient light intensity based on the camera pixel intensity
[RequireComponent(typeof(EnvironmentalLightEx))]
[RequireComponent(typeof(ARCamera))]
public class EnvironmentalAmbientLight : MonoBehaviour
{
	[Tooltip("The time taken to change to a new light estimate (in seconds)")]
    public float lightEstimationSmoothTime = 1;

	[Tooltip("How much to multiply the environmental light estimate by")]
    public float intensityMultiplier = 3;

	private ARCamera arCamera;

    private float lightEstimationSmoothVelocity; // For the smoothing

    void Start()
    {
		arCamera = GetComponent<ARCamera>();
    }

    void Update()
    {
		RenderSettings.ambientIntensity = Mathf.SmoothDamp(RenderSettings.ambientIntensity, arCamera.Camera.LightEstimation * intensityMultiplier, ref lightEstimationSmoothVelocity, lightEstimationSmoothTime);
    }
}
