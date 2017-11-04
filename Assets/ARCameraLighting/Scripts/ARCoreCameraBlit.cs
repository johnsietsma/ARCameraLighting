using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_ANDROID && !UNITY_EDITOR

public class ARCoreCameraBlit : MonoBehaviour, IARCameraBlit {

	private Material blitMat;

	void Start() 
	{
		// Load up the material that does the blit.
		blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
		Debug.Assert(blitMat);
	}

	private void Update()
	{
		// TODO: Required?
		blitMat.SetFloat("_ScreenOrientation", (float)Screen.orientation);
	}

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		commandBuffer.Blit(ARResources.CameraTexture, destinationTextureID, blitMat);
	}
}

#endif