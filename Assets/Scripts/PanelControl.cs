using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class PanelControl : MonoBehaviour
{
    //Controllers 
    public StatusController statusController;
    public FileManagement packageManager;
    public LoadHotspots controller;
    public VideoPlayer videoPlayer;

    //Panel 
    public GameObject inputPanel;

    //Displays
    public GameObject id_display;
    public GameObject photoUrlDisplay;
    public RawImage image;
    public GameObject displayName;
    public GameObject displayText;

    void Start()
    {
        inputPanel.SetActive(false);
    }

    public string getCurrentID() { return id_display.GetComponent<Text>().text; }

    public string getName() { return displayName.GetComponent<Text>().text; }


    public void closeWindow() //Used by x button
    {
        inputPanel.SetActive(false);
        videoPlayer.Play();
    }


    public void loadHotspot(string id, string hot_name, string hot_text, string hot_url_photo)
    {
        id_display.GetComponent<Text>().text = id;
        displayName.GetComponent<Text>().text = hot_name;
        displayText.GetComponent<Text>().text = hot_text;
        photoUrlDisplay.GetComponent<Text>().text = hot_url_photo;
        //display Photo
        if (hot_url_photo != "" && hot_url_photo != null)
        {   
            string new_photo_url = System.IO.Path.Combine(statusController.getPath(), hot_url_photo);
            loadVideoOntoPanel(new_photo_url);
        }
        inputPanel.SetActive(true);
    }

    private void loadVideoOntoPanel(string videoURL)
    {
        WWW www = new WWW("file:///" + videoURL);
        image.texture = www.texture;
    }
}
