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

	public Camera Camera { get { return Camera.main; } }

	public float LightEstimation { get { return pixelIntensity; } }

	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
        if( blitMat==null ) LoadBlitMat();
        commandBuffer.Blit(defaultTexture, destinationTextureID, blitMat);
	}

    private void LoadBlitMat()
    {
        // Load up the material that does the blit.
        blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
        Debug.Assert(blitMat);
    }
}

#endif