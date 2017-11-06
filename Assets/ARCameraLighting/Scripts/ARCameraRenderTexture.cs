using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Captures the AR camera feed as a RenderTexture. 
 * Optional downsize and blue the texture.
 */
[RequireComponent(typeof(FastBlur))]
public class ARCameraRenderTexture : MonoBehaviour
{
    public RenderTexture targetRenderTexture;

    // Optionally blur the render tex
    public bool shouldBlurRenderTexture;

    // For debugging, use this texture in editor instead of the camera feed
    public Texture defaultTexture;

    // Are we capturing the camera feed?
    public bool IsCapturing { get { return isCapturing; } }

	private IARCameraBlit cameraBlit;
    private int workingRenderTextureID = Shader.PropertyToID("_ARCameraRenderTexture");
    private CommandBuffer m_blitCommandBuffer;
    private CommandBuffer m_releaseCommandBuffer;
    private FastBlur fastBlur;
    private Material blendMaterial = null;
    private bool isCapturing;

    IEnumerator Start()
    {
        Debug.Assert(targetRenderTexture, "Please assign a target RenderTexture");

        blendMaterial = Resources.Load<Material>("Materials/Blend");
        Debug.Assert(blendMaterial);

        // Wait for the AR session to start
        while (!ARResources.IsConnected)
        {
            yield return null;
        }

        SetupBackgroundBlit();
    }

    private void SetupBackgroundBlit()
    {
		IARCameraBlit cameraBlit;
#if UNITY_ANDROID && !UNITY_EDITOR
		cameraBlit = gameObject.AddComponent<ARCoreCameraBlit>();
#elif UNITY_IOS && !UNITY_EDITOR
		ARKitCameraBlit arkitCameraBlit = gameObject.AddComponent<ARKitCameraBlit>();
		var arVideo = Camera.main.GetComponent<UnityEngine.XR.iOS.UnityARVideo>();
		Debug.Assert(arVideo);
		arkitCameraBlit.clearMaterial = arVideo.m_ClearMaterial;
		cameraBlit = arkitCameraBlit;
#else
		AREditorCameraBlit editorCameraBlit = gameObject.AddComponent<AREditorCameraBlit>();
		editorCameraBlit.defaultTexture = defaultTexture;
		cameraBlit = editorCameraBlit;
#endif

        Camera arCamera = ARResources.Camera;

        int renderTextureWidth = targetRenderTexture.width;
        int renderTextureHeight = targetRenderTexture.height;

        // Clean up any previous command buffer and events hooks
        if (m_blitCommandBuffer != null)
        {
            ARResources.DeregisterChangeCallback(SetupBackgroundBlit);
            arCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);
            arCamera.RemoveCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);
        }

        // Create the blit command buffer
        m_blitCommandBuffer = new CommandBuffer();
        m_blitCommandBuffer.GetTemporaryRT(workingRenderTextureID, renderTextureWidth, renderTextureHeight, 0, FilterMode.Bilinear);
        m_blitCommandBuffer.name = "Get ARBackground";

		cameraBlit.BlitCameraTexture(m_blitCommandBuffer, workingRenderTextureID);

        if (shouldBlurRenderTexture)
        {
            // The optional blur step
            fastBlur = GetComponent<FastBlur>();
            Debug.Assert(fastBlur);
            fastBlur.CreateBlurCommandBuffer(m_blitCommandBuffer, workingRenderTextureID, renderTextureWidth, renderTextureHeight);
        }

        // Copy over to the target texture. Use a blend texture to stop light flickering.
        m_blitCommandBuffer.Blit(workingRenderTextureID, targetRenderTexture, blendMaterial);

        // Run the command buffer just before opaque rendering
        arCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);

        // Cleanup the temp render textures
        m_releaseCommandBuffer = new CommandBuffer();
        m_releaseCommandBuffer.name = "Release ARBackground";
        m_releaseCommandBuffer.ReleaseTemporaryRT(workingRenderTextureID);
        arCamera.AddCommandBuffer(CameraEvent.AfterSkybox, m_releaseCommandBuffer);

        isCapturing = true;

        // Rebuild the command buffer if anything changes
        ARResources.RegisterChangeCallback(SetupBackgroundBlit);
    }


}
