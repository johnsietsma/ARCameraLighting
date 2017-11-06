using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR

public class AREditorCameraBlit : MonoBehaviour, IARCameraBlit 
{
	public Texture defaultTexture;

	private Material blitMat;

	void Awake() 
	{
		// Load up the material that does the blit.
		blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
		Debug.Assert(blitMat);
	}

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
        Debug.Assert(blitMat);
        commandBuffer.Blit(defaultTexture, destinationTextureID, blitMat);
	}
}

#endif