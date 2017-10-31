using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Captures the AR camera feed as a RenderTexture. 
 * Optional downsize and blue the texture.
 */
[RequireComponent(typeof(FastBlur))]
public class ARCoreCameraRenderTexture : MonoBehaviour
{
    // Optionally blur the render tex
    public bool shouldBlurRenderTexture;

    [Range(0, 3)]
    public int downsampleLevel = 1;

    // For debugging, use this texture in editor instead of the camera feed
    public Texture defaultTexture;

    public static int RenderTextureID = Shader.PropertyToID("_ARCameraRenderTexture");

    // Are we capturing the camera feed?
    public bool IsCapturing { get { return isCapturing; } }

    // The RenderTexture that is being rendered to
    //public Texture RenderTexture { get { return Shader.GetGlobalTexture(renderTextureId); } }

    private Material blitMat;
    private CommandBuffer m_blitCommandBuffer;
    private CommandBuffer m_releaseCommandBuffer;
    private FastBlur fastBlur;
    private bool isCapturing;

    IEnumerator Start()
    {
        // Load up the material that does the blit.

        blitMat = Resources.Load<Material>("Materials/ARCoreBlit");
        Debug.Assert(blitMat);

        // Wait for the AR session to start
        while (!ARResources.IsConnected)
        {
            yield return null;
        }

        SetupBackgroundBlit();
    }

    private void SetupBackgroundBlit()
    {
        // Get a reference to the AR camera's Texture 
        Texture cameraTexture = ARResources.CameraTexture ?? defaultTexture;
        Camera arCamera = ARResources.Camera;

        // Optional shrink the size of the texture we're working with.
        //   Provides a cheap blurring through bilinear filtering, also is more performant to work
        //   with a smaller texture
        int renderTextureWidth = Screen.width >> downsampleLevel;
        int renderTextureHeight = Screen.height >> downsampleLevel;

        // Clean up any previous command buffer and events hooks
        if (m_blitCommandBuffer != null)
        {
            ARResources.DeregisterChangeCallback(SetupBackgroundBlit);
            arCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);
            arCamera.RemoveCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);
        }

        // Create the blit command buffer
        m_blitCommandBuffer = new CommandBuffer();
        m_blitCommandBuffer.GetTemporaryRT(RenderTextureID, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
        m_blitCommandBuffer.name = "Get ARBackground";

        // Capture the camera's render texture
        m_blitCommandBuffer.Blit(cameraTexture, RenderTextureID, blitMat);

        if (shouldBlurRenderTexture)
        {
            // The optional blur step
            fastBlur = GetComponent<FastBlur>();
            Debug.Assert(fastBlur);
            fastBlur.CreateBlurCommandBuffer(m_blitCommandBuffer, RenderTextureID, renderTextureWidth, renderTextureHeight);
        }

        // Run the command buffer just before opaque rendering
        arCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);

        // Cleanup the temp render textures
        m_releaseCommandBuffer = new CommandBuffer();
        m_releaseCommandBuffer.name = "Release ARBackground";
        m_releaseCommandBuffer.ReleaseTemporaryRT(RenderTextureID);
        if( shouldBlurRenderTexture) {
            fastBlur.ReleaseBlurTexture(m_releaseCommandBuffer);
        }
        arCamera.AddCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);

        isCapturing = true;

        // Rebuild the command buffer if anything changes
        ARResources.RegisterChangeCallback(SetupBackgroundBlit);
    }

    private void Update()
    {
        // TODO: Required?
        blitMat.SetFloat("_ScreenOrientation", (float)Screen.orientation);
    }
}
