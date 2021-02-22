using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoplayer;
    public GameObject playButton;
    public GameObject pauseButton;

    public GameObject displayWindow;
    public GameObject videoURLdisplay;

    public GameObject changeVideoButton;
    public GameObject backVideoButton;
    

    //Variables for switching videos
    private GameObject[] objs;
    private List<GameObject> needs_back;
    private long location;
    private string url_old_video;
    private Vector3 camera_pos;

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

    public void ChangeVideo()
    {
        //Record the old video;
        videoplayer.Stop();
        location = videoplayer.frame;
        url_old_video = videoplayer.url;
        
        //Play second video
        PlaySecondVideo(videoURLdisplay.GetComponent<Text>().text);
    }

    public void PlaySecondVideo(string video_path)
    {
        videoplayer.url = video_path;
        videoplayer.Play();
        camera_pos = Camera.main.transform.position;
        //Deactive all buttons from the previous video
        objs = GameObject.FindGameObjectsWithTag("Trigger");
        needs_back = new List<GameObject>();
        foreach (GameObject button in objs)
        {
            if (button.activeSelf)
            {
                needs_back.Add(button);
            }
            button.SetActive(false);
        }
        displayWindow.SetActive(false);
        backVideoButton.SetActive(true);
    }

    public void BacktoPrevVideo()
    {
        videoplayer.Stop();
        videoplayer.url = url_old_video;
        videoplayer.frame = location;
        Camera.main.transform.rotation = Quaternion.Euler(camera_pos);
        foreach (GameObject button in needs_back)
        {
            button.SetActive(true);
        }
        videoplayer.Play();
        backVideoButton.SetActive(false);
    }
}
