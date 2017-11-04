using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Renders the AR camera's RenderTexture as a skybox
// This provides amnbient lighting(with light probes) and reflections (with reflection probes)
[RequireComponent(typeof(ARCameraRenderTexture))]
public class ARSkybox : MonoBehaviour
{
    // Shader property names
    private static readonly int WORLD_TO_CAMERA_MATRIX_PROP_ID = Shader.PropertyToID("_WorldToCameraMatrix");

    private Material skyboxMaterial;
    private ARCameraRenderTexture arRenderTexture;

    IEnumerator Start()
    {
        skyboxMaterial = Resources.Load<Material>("Materials/ARSkybox");
        Debug.Assert(skyboxMaterial);

        arRenderTexture = GetComponent<ARCameraRenderTexture>();
        while (!arRenderTexture.IsCapturing)
        {
            yield return null;
        }

        RenderSettings.skybox = skyboxMaterial;
    }

    void OnEnable()
    {
        RenderSettings.skybox = skyboxMaterial;
    }

    void OnDisable()
    {
        RenderSettings.skybox = null;
    }

    void Update()
    {
		if( !arRenderTexture.IsCapturing ) return;
        Debug.Assert(skyboxMaterial);

        // The skybox material requires the camera matrix for correct environment orientation
        skyboxMaterial.SetMatrix(WORLD_TO_CAMERA_MATRIX_PROP_ID, ARResources.Camera.worldToCameraMatrix);
    }
}
