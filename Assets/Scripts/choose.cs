using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class choose : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// FileDialog for picking a single file
    /// </summary>
    public void OpenFileBrowser()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "mp4 files (*.mp4)|*.mp4";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            videoPlayer.url = result;
            Debug.Log(result);
        });
#endif
    }
}
