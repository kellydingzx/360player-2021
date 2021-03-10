using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Camera cam;
    public GameObject controller;

    public GameObject displayWindow;
    public GameObject videoURLdisplay;
    public GameObject backVideoButton;

    public bool playingMain;
    //Variables for switching videos
    private GameObject current_hotspot;
    private GameObject[] objs;
    private List<GameObject> needs_back;
    private long location;
    private string url_old_video;
    private Vector3 camera_pos;

    void Start()
    {
        backVideoButton.SetActive(false);
        playingMain = true;
    }

    public void ChangeVideo()
    {
        //Record the old video;
        location = videoPlayer.frame;
        url_old_video = videoPlayer.url;
        videoPlayer.Stop();
        //Play second video
        PlaySecondVideo(videoURLdisplay.GetComponent<Text>().text);
        playingMain = false;
    }

    public void PlaySecondVideo(string video_path)
    {
        videoPlayer.url = video_path;
        videoPlayer.Play();
        //Deactive all buttons from the previous video
        objs = GameObject.FindGameObjectsWithTag("Trigger");
        needs_back = new List<GameObject>();
        foreach (GameObject button in objs)
        {
            if (button.activeSelf)
            {
                needs_back.Add(button);
                Debug.Log(button.GetInstanceID().ToString());
                if (button.GetInstanceID().ToString().Equals(
                    controller.GetComponent<LoadHotspots>().current_id))
                {
                    current_hotspot = button;
                }
            }
            button.SetActive(false);
        }
        if(current_hotspot != null)
        {
            camera_pos = current_hotspot.transform.position;
        }
        if(camera_pos == null)
        {
            camera_pos = new Vector3(0, 0, 0);
        }
        displayWindow.SetActive(false);
        backVideoButton.SetActive(true);
    }

    public void BacktoPrevVideo()
    {
        videoPlayer.url = url_old_video;
        videoPlayer.Prepare();
        videoPlayer.Play();
        videoPlayer.frame = location;
        cam.transform.LookAt(camera_pos);
        foreach (GameObject button in needs_back)
        {
            button.SetActive(true);
        }
        videoPlayer.Play();
        backVideoButton.SetActive(false);
    }
}
