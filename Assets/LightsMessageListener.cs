using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsMessageListener : MonoBehaviour
{
    public new Light light;

    private void Start() {
        if (!light) throw new UnityException("LightMessageListener requires a light reference");
    }

    public void ReceiveMessage(string message) {
        if (message == "enable") {
            EnableLight();
        } else if (message == "disable") {
            DisableLight();
        } else {
            Debug.LogWarning($"Unhandled message: {message}", this);
        }
    }

    public void ToggleLight() {
        light.enabled = !light.enabled;
    }

    public void EnableLight() {
        light.enabled = true;
    }

    public void DisableLight() {
        light.enabled = false;
    }
}
