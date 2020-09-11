using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WSMessageListener : MonoBehaviour
{
    public string matchEventType;
    public StringUnityEvent messageEvent;

    public void onMessage(string eventType, string messageData) {
        if (matchEventType == eventType) {
            messageEvent.Invoke(messageData);
        }
    }
}
