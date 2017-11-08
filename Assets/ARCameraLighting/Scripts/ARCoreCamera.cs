using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_ANDROID

public class ARCoreCamera : MonoBehaviour, IARCameraBlit {

	private Material blitMat;
private EnvironmentalLightEX environmentalLight;

	void Awake() 
	{
		// Load up the material that does the blit.
		blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
		Debug.Assert(blitMat);

		environmentalLight = GetComponent<EnvironmentalLight>();
		Debug.Assert(environmentalLight);
	}

	private void Update()
	{
		// TODO: Required?
		blitMat.SetFloat("_ScreenOrientation", (float)Screen.orientation);
	}

	Camera Camera { get { Device.backgroundRenderer.camera; } }

	public float LightEstimation { get { return environmentalLight.colorScale; } }
	
	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		commandBuffer.Blit(ARResources.CameraTexture, destinationTextureID, blitMat);
	}
}

#endif