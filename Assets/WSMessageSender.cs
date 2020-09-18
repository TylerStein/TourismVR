using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMessageSender : MonoBehaviour
{
    public WSController wsController;

    public void SendTimestampEvent(float timestamp) {
        WSMessage message = new WSMessage("scrub", Mathf.Round(timestamp).ToString());
        wsController.SendMessage(message);
    }
}
