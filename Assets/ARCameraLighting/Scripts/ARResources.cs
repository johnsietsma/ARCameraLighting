using GoogleARCore;
using GoogleAR.UnityNative;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Provide plaform specific resources like cameras and textures.
 * Excapsulate any platform specific code.
 */
public static class ARResources
{
	#if UNITY_ANDROID && !UNITY_EDITOR
	// ---- ARCore ----

    // Is the AR session connected?
    public static bool IsConnected
    {
        get { return Session.ConnectionState == SessionConnectionState.Connected; }
    }

   public static Texture CameraTexture
    {
        get
        {
            var backgroundRenderer = Device.backgroundRenderer;

            // There are two ways to get the background texture, try both
            if (backgroundRenderer.backgroundTexture != null)
            {
                return backgroundRenderer.backgroundTexture;
            }
            else if (backgroundRenderer.backgroundMaterial != null)
            {
                return backgroundRenderer.backgroundMaterial.GetTexture("_MainTex");
            }
            else {
                Debug.LogError("No background texture");
                return null;
            }
        }
    }

    public static void RegisterChangeCallback( Action callback )
    {
        Device.backgroundRenderer.backgroundRendererChanged += callback;
    }

    public static void DeregisterChangeCallback( Action callback )
    {
        Device.backgroundRenderer.backgroundRendererChanged -= callback;
    }


#elif UNITY_IOS && !UNITY_EDITOR
	// ---- ARKit ----

	// Is the AR session connected?
	public static bool IsConnected {
		get { return true; }
	}

	public static void RegisterChangeCallback (Action callback)
	{
	}

	public static void DeregisterChangeCallback (Action callback)
	{
	}

	#else
    // ---- Editor (for debugging) ----

    public static bool IsConnected { get { return true; } }

    public static Texture CameraTexture
    {
        get { return null; }
    }

    public static void RegisterChangeCallback(Action callback)
    {
        // no-op
    }

    public static void DeregisterChangeCallback(Action callback)
    {
        // no-op
    }

#endif
}
