using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Loads and updates the skybox material
public class ARSkybox : MonoBehaviour
{
    private static readonly int WORLD_TO_CAMERA_MATRIX_PROP_ID = Shader.PropertyToID("_WorldToCameraMatrix");

    private Material skyboxMaterial;
    private Material oldSkyboxMaterial;
    private Camera arCamera;

    void Start()
    {
        Debug.Assert(Camera.main != null, "You must have a Camera tagged as MainCamera in your scene.");
        arCamera = Camera.main;
    }

    void OnEnable()
    {
        skyboxMaterial = Resources.Load<Material>("Materials/ARSkybox");
        Debug.Assert(skyboxMaterial);

        oldSkyboxMaterial = RenderSettings.skybox;
        RenderSettings.skybox = skyboxMaterial;
    }

    void OnDisable()
    {
        RenderSettings.skybox = oldSkyboxMaterial;
    }

    void Update()
    {
        Debug.Assert(skyboxMaterial);

        // The skybox material requires the camera matrix for correct environment orientation
		skyboxMaterial.SetMatrix(WORLD_TO_CAMERA_MATRIX_PROP_ID, arCamera.worldToCameraMatrix);
    }
}
