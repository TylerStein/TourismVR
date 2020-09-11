using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

using NativeWebSocket;

public class WSController : MonoBehaviour
{
    public string address = "ws://localhost:8080";

    public WSMessage lastMessage;
    public List<WSMessageListener> messageListeners;

    private WebSocket webSocket;
    private Dictionary<string, string> headers;

    public string authorizationToken;

    public async void Connect(string token) {
        authorizationToken = token;
        headers = new Dictionary<string, string>();
        headers["authorization"] = token;
        webSocket = new WebSocket(address, headers);

        webSocket.OnOpen += ws_onOpen;
        webSocket.OnError += ws_onError;
        webSocket.OnMessage += ws_onMessage;
        webSocket.OnClose += ws_onClose;

        await webSocket.Connect();
    }

    public async void SendMessage(WSMessage message) {
        byte[] data = message.ToBytes();
        await webSocket.Send(data);
    }

    private void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket != null) webSocket.DispatchMessageQueue();
#endif
    }

    private void ws_onOpen() {
        Debug.Log("ws_onOpen()");
    }

    private void ws_onClose(WebSocketCloseCode code) {
        Debug.Log($"ws_onClose({code.ToString()})");
    }

    private void ws_onMessage(byte[] data) {
        try {
            lastMessage = WSMessage.FromBytes(ref data);
        } catch (Exception exception) {
            Debug.LogError(exception, this);
            return;
        }

        Debug.Log($"ws_onMessage({lastMessage.ToString()})");
        foreach (WSMessageListener listener in messageListeners) {
            listener.onMessage(lastMessage.Event, lastMessage.Data);
        }
    }

    private void ws_onError(string errorMsg) {
        Debug.Log($"ws_onError({errorMsg})");
    }

    private async void OnApplicationQuit() {
        await webSocket.Close();
    }
}

[Serializable]
public class WSMessage
{
    [SerializeField] public string Event;
    [SerializeField] public string Data;

    public WSMessage(string Event, string Data) {
        this.Event = Event;
        this.Data = Data;
    }

    public static WSMessage FromBytes(ref byte[] data) {
        string message = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        return FromJson(message);
    }

    public static WSMessage FromJson(string message) {
        string regexString = "\"(\\w+)\"\\s*:\\s*\"(.+?)(?<!\\\\)\"";
        Regex rgx = new Regex(regexString);
        Dictionary<string, string> values = new Dictionary<string, string>();

        foreach (Match match in rgx.Matches(message)) {
            if (match.Groups.Count >= 3) {
                values.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        if (values.ContainsKey("event") && values.ContainsKey("data")) {
            return new WSMessage(values["event"], values["data"]);
        }

        throw new Exception($"Failed to parse json message: {message}");
    }

    public byte[] ToBytes() {
        string json = JsonUtility.ToJson(this);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public override string ToString() {
        return $"WSMessage(Event: {Event.ToString()}, Data: {Data.ToString()})";
    }
}