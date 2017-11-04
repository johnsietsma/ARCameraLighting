using System;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_IOS && !UNITY_EDITOR

public class ARKitCameraBlit : MonoBehaviour, IARCameraBlit {

	public Material clearMaterial;

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		// Piggyback off the UnityARVideo material
		commandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, clearMaterial);
	}
}

#endif