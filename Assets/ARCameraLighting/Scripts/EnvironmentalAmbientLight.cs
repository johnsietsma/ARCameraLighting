using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Change the ambient light intensity based on the camera pixel intensity
[RequireComponent(typeof(EnvironmentalLightEx))]
public class EnvironmentalAmbientLight : MonoBehaviour
{
	[Tooltip("The time taken to change to a new light estimate (in seconds)")]
    public float lightEstimationSmoothTime = 1;

	[Tooltip("How much to multiply the environmental light estimate by")]
    public float intensityMultiplier = 3;

	public ARCamera arCamera;

    private float lightEstimationSmoothVelocity; // For the smoothing

    void Start()
    {
		if (arCamera == null)
        {
			arCamera = GetComponent<ARCamera>();
        }
		Debug.Assert(arCamera, "An ARCamera is required");

    }

    void Update()
    {
		RenderSettings.ambientIntensity = Mathf.SmoothDamp(RenderSettings.ambientIntensity, arCamera.Camera.LightEstimation * intensityMultiplier, ref lightEstimationSmoothVelocity, lightEstimationSmoothTime);
    }
}
