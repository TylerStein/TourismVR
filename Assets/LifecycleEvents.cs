using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifecycleEvents : MonoBehaviour
{
    public List<UnityEvent> startEvents = new List<UnityEvent>();
    public List<UnityEvent> applicationQuitEvents = new List<UnityEvent>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (UnityEvent ev in startEvents) {
            ev.Invoke();
        }
    }

    private void OnApplicationQuit() {
        foreach (UnityEvent ev in applicationQuitEvents) {
            ev.Invoke();
        }
    }
}
