using GoogleARCore;
using GoogleAR.UnityNative;
using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_ANDROID

public class ARCoreCamera : MonoBehaviour, IARCamera {

	private Material blitMat;
	private EnvironmentalLightEx environmentalLight;

	void Awake() 
	{
		// Load up the material that does the blit.
		blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
		Debug.Assert(blitMat);

		environmentalLight = GetComponent<EnvironmentalLightEx>();
		Debug.Assert(environmentalLight);
	}

	private void Update()
	{
		// TODO: Required?
		blitMat.SetFloat("_ScreenOrientation", (float)Screen.orientation);
	}

	public Camera Camera { get { return ARResources.IsConnected ? Device.backgroundRenderer.camera : null; } }

	public float LightEstimation { get { return environmentalLight.colorScale; } }
	
	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		commandBuffer.Blit(ARResources.CameraTexture, destinationTextureID, blitMat);
	}
}

#endif