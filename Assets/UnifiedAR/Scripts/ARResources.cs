using GoogleARCore;
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
        get { return Frame.CameraImage.Texture; }
    }


#elif UNITY_IOS && !UNITY_EDITOR
	// ---- ARKit ----

	// Is the AR session connected?
	public static bool IsConnected {
		get { return true; }
	}

#else
    // ---- Editor (for debugging) ----

    public static bool IsConnected { get { return true; } }

    public static Texture CameraTexture
    {
        get { return null; }
    }

#endif
}
