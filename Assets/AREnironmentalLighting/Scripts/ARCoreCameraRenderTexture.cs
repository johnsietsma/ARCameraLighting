using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using GoogleARCore;
using GoogleAR.UnityNative;


/**
 * When using ARCore, this will blit the front camera texture to a RenderTexture.
 */
[RequireComponent(typeof(FastBlur))]
public class ARCoreCameraRenderTexture : MonoBehaviour
{
    // The Material to blit with, use this to blur, color correct, etc
    public Material blitMat;

    // Optional blur the render tex
    public bool shouldBlurRenderTexture;

    // The RenderTexture to blit to
    [HideInInspector]
    public RenderTexture renderTex;


#if UNITY_EDITOR
    // For debugging, use this texture in editor instead of the camera feed
    public Texture defaultTexture;
#endif

    // Are we capturing the camera feed?
    public bool IsReady { get { return isReady; } }

    // Is the AR session connected?
    private bool IsConnected
    {
        get { return Session.ConnectionState == SessionConnectionState.Connected; }
    }

    // The Unity Camera being used to render the physical camera
    public Camera ARCamera { get { return arCamera; } }

    // The RenderTexture that is being rendered to
    public RenderTexture RenderTexture
    {
        get { return shouldBlurRenderTexture ? blurOptimized.blurredTexture : renderTex;  }
    }


    private Camera arCamera;
    private CommandBuffer m_blitCommandBuffer;
    private FastBlur blurOptimized;
    private bool isReady;

    private Texture externalTex;

    void Awake()
    {
        // Render to a buffer the same size as the screen.
        // Posible enhancement is to optionally downscale the camera
        // The blur step will downscale, we may as well do it now.
        renderTex = new RenderTexture(Screen.width, Screen.height, 0);
    }

    IEnumerator Start()
    {
        yield return null;

#if UNITY_ANDROID && !UNITY_EDITOR
        // Wait for the AR session to start
        while (!IsConnected)
        {
            yield return null;
        }
#endif

        SetupBackgroundBlit();
    }

    private void SetupBackgroundBlit()
    {
        // A reference to the textuer we're going to blit to
        Texture backgroundTexture = null;


#if UNITY_ANDROID && !UNITY_EDITOR
        var backgroundRenderer = Device.backgroundRenderer;

        arCamera = backgroundRenderer.camera;

        // There are two ways to get the background texture, try both
        if (backgroundRenderer.backgroundTexture != null)
        {
            backgroundTexture = backgroundRenderer.backgroundTexture;
        }
        else if(backgroundRenderer.backgroundMaterial!=null)
        {
            backgroundTexture = backgroundRenderer.backgroundMaterial.GetTexture("_MainTex");
        }
#endif

#if UNITY_EDITOR
        // Use a text texture for testing in the the editor
        backgroundTexture = defaultTexture;
        // Assume the main camera is the AR camera, may not be true!
        arCamera = Camera.main;
#endif

        // Clean up any previous command buffer
        if (m_blitCommandBuffer != null)
        {
            arCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);
        }

        // Create the blit command buffer
        m_blitCommandBuffer = new CommandBuffer();
        m_blitCommandBuffer.name = "GetARBackground";
        m_blitCommandBuffer.Blit(backgroundTexture, renderTex, blitMat);
        if (shouldBlurRenderTexture)
        {
            //`The optional blur step
            blurOptimized = GetComponent<FastBlur>();
            blurOptimized.BlurCommandBuffer(m_blitCommandBuffer, renderTex);
            Shader.SetGlobalTexture("_ProjectionTexture", blurOptimized.blurredTexture);
        }
        else
        {
            Shader.SetGlobalTexture("_ProjectionTexture", renderTex);
        }

        arCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_blitCommandBuffer);
        isReady = true;

#if !UNITY_EDITOR && !UNITY_EDITOR
        // Rebuild the command buffer if anything changes
        Device.backgroundRenderer.backgroundRendererChanged += SetupBackgroundBlit;
#endif
    }

    private void Update()
    {
        if (!IsConnected || arCamera == null) return;

        // TODO: Required?
        blitMat.SetFloat("_ScreenOrientation", (float)Screen.orientation);
    }
}
