using UnityEngine;

public class ARCamera : MonoBehaviour {

	public IARCamera Camera {
        get
        {
            if (arCamera == null) SetARCamera();
            return arCamera;
        }
    }

    // For debugging, use this texture in editor instead of the camera feed
    public Texture defaultTexture;

    private IARCamera arCamera;

    private void SetARCamera()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		arCamera = gameObject.AddComponent<ARCoreCamera>();
#elif UNITY_IOS && !UNITY_EDITOR
		ARKitCamera arkitCamera = gameObject.AddComponent<ARKitCamera>();
		var arVideo = UnityEngine.Camera.main.GetComponent<UnityEngine.XR.iOS.UnityARVideo>();
		Debug.Assert(arVideo);
		arkitCamera.clearMaterial = arVideo.m_ClearMaterial;
		arCamera = arkitCamera;
#else
        AREditorCamera editorCamera = gameObject.AddComponent<AREditorCamera> ();
		editorCamera.defaultTexture = defaultTexture;
		arCamera = editorCamera;
#endif
        Debug.Assert(arCamera!=null);
	}
}
