using UnityEngine;
using UnityEngine.Rendering;
using System;

public interface IARCamera {
	float LightEstimation { get; }
	void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID );
}
