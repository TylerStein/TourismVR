using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

[System.Serializable]
public class FloatUnityEvent : UnityEvent<float> { }

public class VideoMessageListener : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private IEnumerator timeSampleRoutine;
    public FloatUnityEvent timestampEvent;

    public float videoTime = 0f;

    private void Start() {
        if (!videoPlayer) throw new UnityException("VideoMessageListener requires a VideoPlayer reference");
    }

    public void OnFileEvent(string message) {
        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = message;
        videoPlayer.Play();

        if (timeSampleRoutine != null) {
            Debug.Log("timeSample routine stopped");
            StopCoroutine(timeSampleRoutine);
        }

        videoTime = 0f;
        timeSampleRoutine = timeSample(timestampEvent);
        StartCoroutine(timeSampleRoutine);
    }

    public void OnPauseEvent(string message) {
        if (videoPlayer.isPlaying) {
            videoPlayer.Pause();
        }
    }


    public void OnPlayEvent(string message) {
        if (videoPlayer.isPaused) {
            videoPlayer.Play();
        }
    }

    public void OnDisconnectEvent(string message) {
        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
        }
    }

    public void Update() {
        if (videoPlayer != null) {
            videoTime = (float)videoPlayer.time;
        }
    }

    public IEnumerator timeSample(FloatUnityEvent callback) {
        while (true) {
            yield return new WaitForSeconds(1f);
            Debug.Log($"timeSample {videoTime}");
            callback.Invoke(videoTime);
        }
    }
}
