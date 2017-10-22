using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggle between to anim states
public class AnimatedToggle : MonoBehaviour
{

    public Animator animator;
    public string toggleOnTrigger;
    public string toggleOffTrigger;

    private bool isOn = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        Debug.Assert(animator);
    }

    public void Toggle()
    {
        if (isOn) { ToggleOff(); }
        else { ToggleOn(); }
    }

    public void ToggleOn()
    {
        isOn = true;
        animator.SetTrigger(toggleOnTrigger);
    }

    public void ToggleOff()
    {
        isOn = false;
        animator.SetTrigger(toggleOffTrigger);
    }
}
