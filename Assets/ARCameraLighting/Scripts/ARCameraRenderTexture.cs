using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Captures the AR camera feed as a RenderTexture. 
 * Optional downsize and blue the texture.
 */
[RequireComponent(typeof(FastBlur), typeof(ARCamera))]
public class ARCameraRenderTexture : MonoBehaviour
{
    public RenderTexture targetRenderTexture;

    // Optionally blur the render tex
    public bool shouldBlurRenderTexture;

    // Are we capturing the camera feed?
    public bool IsCapturing { get { return isCapturing; } }

	private IARCamera cameraBlit;
    private int workingRenderTextureID = Shader.PropertyToID("_ARCameraRenderTexture");
    private CommandBuffer m_blitCommandBuffer;
    private CommandBuffer m_releaseCommandBuffer;
    private FastBlur fastBlur;
    private bool isCapturing;

    void Start()
    {
        Debug.Assert(targetRenderTexture, "Please assign a target RenderTexture");
        SetupBackgroundBlit();
    }

    private void SetupBackgroundBlit()
    {
        Debug.Assert(Camera.main != null, "You must have a Camera tagged as MainCamera in your scene.");
        Camera mainCamera = Camera.main;
        Debug.Log("Main Camera is on GameObject - " + mainCamera.gameObject.name);

        IARCamera arCamera = GetComponent<ARCamera>().Camera;
		Debug.Assert (arCamera!=null);

        int renderTextureWidth = targetRenderTexture.width;
        int renderTextureHeight = targetRenderTexture.height;

        // Clean up any previous command buffer and events hooks
        if (m_blitCommandBuffer != null)
        {
			mainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);
            mainCamera.RemoveCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);
        }

        // Create the blit command buffer
        m_blitCommandBuffer = new CommandBuffer();
        m_blitCommandBuffer.GetTemporaryRT(workingRenderTextureID, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
        m_blitCommandBuffer.name = "Get ARBackground";

		arCamera.BlitCameraTexture(m_blitCommandBuffer, workingRenderTextureID);

        if (shouldBlurRenderTexture)
        {
            // The optional blur step
            fastBlur = GetComponent<FastBlur>();
            Debug.Assert(fastBlur);
            fastBlur.CreateBlurCommandBuffer(m_blitCommandBuffer, workingRenderTextureID, renderTextureWidth, renderTextureHeight);
        }

        // Copy over to the target texture.
        m_blitCommandBuffer.Blit(workingRenderTextureID, targetRenderTexture);

        // Run the command buffer just before opaque rendering
        mainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);

        // Cleanup the temp render textures
        m_releaseCommandBuffer = new CommandBuffer();
        m_releaseCommandBuffer.name = "Release ARBackground";
        m_releaseCommandBuffer.ReleaseTemporaryRT(workingRenderTextureID);
        mainCamera.AddCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);

        isCapturing = true;
    }


}
