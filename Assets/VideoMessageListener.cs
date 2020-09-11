using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoMessageListener : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start() {
        if (!videoPlayer) throw new UnityException("VideoMessageListener requires a VideoPlayer reference");
    }

    public void ReceiveMessage(string message) {
        if (videoPlayer.isPlaying) {
            videoPlayer.Stop();
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = message;
        videoPlayer.Play();
    }
}
