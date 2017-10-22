using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour {

    public GameObject environmentProbes;
    public GameObject lightProbes;


    private bool areEnvironmentProbesActive;
    private bool areLightProbesActive;

    void Start()
    {
        areEnvironmentProbesActive = environmentProbes.activeInHierarchy;
        areLightProbesActive = lightProbes.activeInHierarchy;
    }

    public void ToggleReflectionProbes()
    {
        areEnvironmentProbesActive = !areEnvironmentProbesActive;
        environmentProbes.SetActive(areEnvironmentProbesActive);
    }

    public void ToggleLightProbes()
    {
        areLightProbesActive = !areLightProbesActive;
        lightProbes.SetActive(areLightProbesActive);
    }
}
