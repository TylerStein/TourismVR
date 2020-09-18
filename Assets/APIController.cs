using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;


[System.Serializable]
public class StringUnityEvent : UnityEvent<string> { }

[System.Serializable]
public class AuthPlayerResponse
{
    public string token;

    public static AuthPlayerResponse FromJson(string json) {
        return JsonUtility.FromJson<AuthPlayerResponse>(json);
    }
}

public class APIController : MonoBehaviour
{
    public string key = "xxx";

    public string playerAuthUrl = "http://localhost:3000/v1/auth/player";

    public StringUnityEvent recieveTokenEvent = new StringUnityEvent();

    public void Authenticate() {
        StartCoroutine(authenticate(key, onAuthenticated));
    }

    private void onAuthenticated(UnityWebRequest result) {
        if (result.isNetworkError || result.isHttpError) {
            Debug.LogError("Error authenticating API access: " + result.error, this);
            return;
        }

        string responseText = result.downloadHandler.text;
        AuthPlayerResponse authResponse = AuthPlayerResponse.FromJson(responseText);

        recieveTokenEvent.Invoke(authResponse.token);
    }

    private IEnumerator authenticate(string clientKey, UnityAction<UnityWebRequest> onComplete) {
        UnityWebRequest authRequest = CreateJsonPostRequest(playerAuthUrl, $"{{\"key\":\"{clientKey}\"}}");
        yield return authRequest.SendWebRequest();
        onComplete.Invoke(authRequest);
    }

    private UnityWebRequest CreateJsonPostRequest(string url, string bodyJsonString) {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw) as UploadHandler;
        request.downloadHandler = new DownloadHandlerBuffer() as DownloadHandler;
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }
}