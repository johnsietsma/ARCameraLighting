using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.iOS;

#if UNITY_IOS

public class ARKitCamera : MonoBehaviour, IARCamera
{
	public Material clearMaterial;

	public float LightEstimation { get; private set; }

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		// Piggyback off the UnityARVideo material
		commandBuffer.Blit(null, destinationTextureID, clearMaterial);
	}

	void Awake() 
	{
		UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateLightEstimation;
	}

	void UpdateLightEstimation(UnityARCamera camera)
	{
        LightEstimation = camera.lightData.arLightEstimate.ambientIntensity / 1000.0f;
	}
}

#endif