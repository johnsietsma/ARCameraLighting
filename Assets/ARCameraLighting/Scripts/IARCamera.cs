using UnityEngine;
using UnityEngine.Rendering;
using System;

public interface IARCamera {
	Camera Camera { get; }
	float LightEstimation { get; }
	void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID );
}
