using UnityEngine;

public class ARCamera : MonoBehaviour {

	public IARCamera Camera { get; private set; }

	// For debugging, use this texture in editor instead of the camera feed
	public Texture defaultTexture;

	void Awake()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Camera = gameObject.AddComponent<ARCoreCamera>();
#elif UNITY_IOS && !UNITY_EDITOR
		ARKitCamera arkitCamera = gameObject.AddComponent<ARKitCamera>();
		var arVideo = UnityEngine.Camera.main.GetComponent<UnityEngine.XR.iOS.UnityARVideo>();
		Debug.Assert(arVideo);
		arkitCamera.clearMaterial = arVideo.m_ClearMaterial;
		Camera = arkitCamera;
#else
AREditorCamera editorCamera = gameObject.AddComponent<AREditorCamera> ();
		editorCamera.defaultTexture = defaultTexture;
		Camera = editorCamera;
#endif
		Debug.Assert(Camera!=null);
	}
}
