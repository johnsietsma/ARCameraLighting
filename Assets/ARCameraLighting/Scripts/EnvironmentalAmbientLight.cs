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

    [Tooltip("The EnvironmentalLight to get the estimation from. Can be null if the EnvironmentalLight is attached to the same GameObject")]
    public EnvironmentalLightEx environmentalLight;

    private float lightEstimationSmoothVelocity; // For the smoothing

    void Start()
    {
        if (environmentalLight == null)
        {
            environmentalLight = GetComponent<EnvironmentalLightEx>();
        }
        Debug.Assert(environmentalLight, "An EnvironmentalLight is required");

    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.ambientIntensity = Mathf.SmoothDamp(RenderSettings.ambientIntensity, environmentalLight.colorScale * intensityMultiplier, ref lightEstimationSmoothVelocity, lightEstimationSmoothTime);

    }
}
