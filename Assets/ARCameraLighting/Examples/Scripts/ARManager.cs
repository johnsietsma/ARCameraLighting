using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour {

	public GameObject arKitGameObject;
	public GameObject arCoreGameObject;
    public GameObject arEditorGameObject;

	private bool arKitActive = false;
	private bool arCoreActive = false;
    private bool arEditorActive = false;

	void Awake () {
#if UNITY_IOS && !UNITY_EDITOR
		arKitActive = true;
#elif UNITY_ANDROID && !UNITY_EDITOR
		arCoreActive = true;
#else
        arEditorActive = true;
#endif
		arKitGameObject.SetActive (arKitActive);
		arCoreGameObject.SetActive(arCoreActive);
        arEditorGameObject.SetActive (arEditorActive);
	}
}
