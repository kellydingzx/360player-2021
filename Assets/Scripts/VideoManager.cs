using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoplayer;
    public GameObject playButton;
    public GameObject pauseButton;

    void Start()
    {
        pauseButton.SetActive(false);
    }

    public void Play()
    {
        if (videoplayer.isPlaying)
        {
            videoplayer.Pause();
            pauseButton.SetActive(false);
            playButton.SetActive(true);
        }
        else
        {
            videoplayer.Play();
            pauseButton.SetActive(true);
            playButton.SetActive(false);
        }
    }
}
