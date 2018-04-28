using GoogleARCore;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_ANDROID

public class ARCoreCamera : MonoBehaviour, IARCamera
{
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
	    const string topLeftRightVar = "_UvTopLeftRight";
	    const string bottomLeftRightVar = "_UvBottomLeftRight";

	    var uvQuad = Frame.CameraImage.DisplayUvCoords;
	    blitMat.SetVector(topLeftRightVar,
	        new Vector4(uvQuad.TopLeft.x, uvQuad.TopLeft.y, uvQuad.TopRight.x, uvQuad.TopRight.y));
	    blitMat.SetVector(bottomLeftRightVar,
	        new Vector4(uvQuad.BottomLeft.x, uvQuad.BottomLeft.y, uvQuad.BottomRight.x, uvQuad.BottomRight.y));
	}

	public float LightEstimation { get { return environmentalLight.colorScale; } }
	
	public void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID )
	{
		commandBuffer.Blit(Frame.CameraImage.Texture, destinationTextureID, blitMat);
	}
}

#endif