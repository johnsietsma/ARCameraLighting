using UnityEngine;
using UnityEngine.Rendering;
using System;

public interface IARCameraBlit {
	void BlitCameraTexture( CommandBuffer commandBuffer, int destinationTextureID );
}
