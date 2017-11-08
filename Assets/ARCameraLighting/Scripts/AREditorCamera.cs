using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR

public class AREditorCamera : MonoBehaviour, IARCamera
{
	[Range(0,1)]
	public float pixelIntensity = 0.5f;

	public Texture defaultTexture;

	private Material blitMat;

	void Awake() 
	{
		// Load up the material that does the blit.
		blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
		Debug.Assert(blitMat);
	}

	public Camera Camera { get { return Camera.main; } }

	public float LightEstimation { get { return pixelIntensity; } }

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
        Debug.Assert(blitMat);
        commandBuffer.Blit(defaultTexture, destinationTextureID, blitMat);
	}
}

#endif