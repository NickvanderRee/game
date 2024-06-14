using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceController : MonoBehaviour
{
    private Light lightSource;
    private bool isOn = false;

    void Start()
    {
        // Get the Light component attached to this GameObject
        lightSource = GetComponent<Light>();
        
        if (lightSource == null)
        {
            Debug.LogError("No Light component found on this GameObject.");
            return;
        }

        // Start with the light off
        lightSource.enabled = isOn;
    }

    void Update()
    {
        // Toggle the light on/off when the L key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            lightSource.enabled = isOn;
        }
    }
}