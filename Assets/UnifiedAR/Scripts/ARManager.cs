using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour {

	public GameObject arKitPrefab;
	public GameObject arCorePrefab;
    public GameObject arEditorPrefab;

    public GameObject WorldRoot; // The object that will be moved onto the plane.

    private GameObject arGameObject;
    private Transform cameraTransform;

	void Awake () {
        GameObject arPrefab = null;
#if UNITY_IOS && !UNITY_EDITOR
		arPrefab = arKitPrefab;
#elif UNITY_ANDROID && !UNITY_EDITOR
		arPrefab = arCorePrefab;
#else
        arPrefab = arEditorPrefab;
#endif
        arGameObject = Instantiate(arPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

	    cameraTransform = Camera.main.transform;
	}

    private void PlaneHit(Transform hitTransform)
    {
        Debug.Log("Set facing");
        WorldRoot.transform.position = hitTransform.position;
        FaceToward.SetFacing(WorldRoot.transform, cameraTransform.position);
        WorldRoot.transform.SetParent(hitTransform);

        // Should look at the camera but still be flush with the plane.
        WorldRoot.transform.LookAt(cameraTransform);
        WorldRoot.transform.rotation = Quaternion.Euler(0.0f,
            WorldRoot.transform.rotation.eulerAngles.y, WorldRoot.transform.rotation.z);
    }
}
