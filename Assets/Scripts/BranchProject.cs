using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class BranchProject : MonoBehaviour
{
    public PanelControl inputpanelcontrol;

    public LoadHotspots controller;
    public StatusController statusController;
    public FileManagement packageManager;
    public VideoManager videoManager;
    public Warning warningController;
    public TimelineController timelineController;

    public GameObject backButton;

    //Private states before branch
    private int floor_count;
    private long prev_video_frame;
    private Transform prev_tramsform;

    void Start()
    {
        backButton.SetActive(false);
    }

    public void goToBranch()
    {
        string hotspot_name = inputpanelcontrol.getName();
        floor_count++;
        //Store status data before branch
        prev_video_frame = videoManager.getFrame();
        prev_tramsform = controller.getTransformByID(inputpanelcontrol.getCurrentID());
        videoManager.removeVideo();
        controller.removeAllHotspots();
        inputpanelcontrol.closeWindow();
        //Branch and load data
        statusController.branch_out(hotspot_name);
        string current_path = statusController.getPath();
        string mainVideo_path = Path.Combine(current_path, "MainVideo.mp4");
        if (File.Exists(mainVideo_path)){
            videoManager.loadVideo(mainVideo_path);
        }else
        {
            warningController.displayErrorMessage("Please add a Main Video.");
        }
        controller.please_load();
        backButton.SetActive(true);
    }

    public void branchOutbyNode(string relative_path, Vector3 location, double start_time)
    {
        
        string intendedPath = timelineController.getRoot() + relative_path;
        Debug.Log(intendedPath);
        if(!intendedPath.Equals(statusController.getPath()))
        {
            videoManager.removeVideo();
            controller.removeAllHotspots();
            statusController.branch_into(relative_path);
            Debug.Log(relative_path);
            string mainVideo_path = Path.Combine(statusController.getPath(), "MainVideo.mp4");
            videoManager.loadVideo(mainVideo_path);
            controller.please_load();
        }
        Camera.main.transform.LookAt(location);
        videoManager.goToPosinTime(start_time);
    }

    public void backPreviousBranch()
    {
        controller.removeAllHotspots();
        floor_count--;
        statusController.branch_back();
        backtoPrevVideo(statusController.mainVideoPath(), prev_video_frame, prev_tramsform);
        if (floor_count == 0) { backButton.SetActive(false); }
    }

    private void backtoPrevVideo(string url_video, long prev_frame, Transform prev_hotspot)
    {
        videoManager.loadVideoAtFrame(prev_frame, url_video);
        Camera.main.transform.LookAt(prev_hotspot);
        controller.please_load();
    }
}