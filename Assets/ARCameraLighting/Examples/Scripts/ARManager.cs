using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour {

	public GameObject arKitGameObject;
	public GameObject arCoreGameObject;

	private bool arKitActive = false;
	private bool arCoreActive = false;

	void Awake () {
		#if UNITY_IOS
		arKitActive = true;
		arCoreActive = false;
		#elif UNITY_ANDROID
		arKitActive = false;
		arCoreActive = true;
		#endif

		arKitGameObject.SetActive (arKitActive);
		arCoreGameObject.SetActive (arCoreActive);
	}
}
