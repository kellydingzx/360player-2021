using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class LoadHotspots : MonoBehaviour
{
    public StatusController statusController;
    public PanelControl inputPanelControl;

    public GameObject hotspot;
    public VideoPlayer videoPlayer;

    public string current_id;

    private Hashtable table;

    public bool ready_to_load;

    private bool hotspots_loaded;


    void Start()
    {
        table = new Hashtable();
    }

    void Update()
    {
        if (videoPlayer.length != 0 && ready_to_load && !hotspots_loaded)
        {
            load();
        }

        if (hotspots_loaded) {
            innerUpdate();
            checkHotspotValidity(); }
    }

    public void please_load() { ready_to_load = true; }

    private void innerUpdate()
    {
        //View hotspot on left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Trigger")
                {
                    current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    openWindow(current_id);
                }
            }
        }
    }

    public void openWindow(string id)
    {
        Hotspot current_hotspot = (Hotspot)table[id];
        inputPanelControl.loadHotspot(id, current_hotspot.getName(), current_hotspot.getText(), current_hotspot.getUrl_photo());
        goToHotspot(current_hotspot);
        videoPlayer.Pause();
    }

    public void goToHotspot(Hotspot hs)
    {
        double hs_start_time = hs.getStart();
        videoPlayer.Prepare();
        Debug.Log(videoPlayer.frameCount);
        Debug.Log(videoPlayer.frameRate);
        long location_frame = Convert.ToInt64(hs_start_time / (videoPlayer.frameCount / videoPlayer.frameRate) * videoPlayer.frameCount) + 5;
        videoPlayer.frame = location_frame;
        Debug.Log(location_frame);
        Camera.main.transform.LookAt(hs.getHotspot().transform);
    }

    public void checkHotspotValidity()
    {
        double current_time = videoPlayer.time;
        foreach (DictionaryEntry entry in table)
        {
            Hotspot h = (Hotspot)entry.Value;
            if (h.getStart() >= current_time || h.getEnd() <= current_time)
            {
                h.getHotspot().SetActive(false);
            }
            else
            {
                h.getHotspot().SetActive(true);
            }
        }
    }

    public Transform getTransformByID(string hotspot_id)
    {
        return ((Hotspot)table[hotspot_id]).getHotspot().transform;
    }

    public void removeAllHotspots()
    {
        hotspots_loaded = false;
        ready_to_load = false;
        List<String> needs_removed = new List<string>();
        foreach (DictionaryEntry entry in table)
        {
            Hotspot h = (Hotspot)table[entry.Key.ToString()];
            GameObject a = h.getHotspot();
            Destroy(a);
        }
        table.Clear();
    }


    public class Hotspot
    {
        GameObject hotspot;
        double start_time;
        double end_time;
        string name;
        string text;
        string url_photo;
        string url_video;

        public Hotspot(GameObject a, double start, double end, string name)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
            this.name = name;
        }

        public Hotspot(GameObject a, double start, double end, string name, string text, string url_photo, string url_video)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
            this.name = name;
            this.text = text;
            this.url_photo = url_photo;
            this.url_video = url_video;
        }

        public double getStart()
        {
            return start_time;
        }
        public double getEnd()
        {
            return end_time;
        }
        public string getName()
        {
            return name;
        }
        public string getText()
        {
            return text;
        }
        public string getUrl_photo()
        {
            return url_photo;
        }
        public string getUrl_video()
        {
            return url_video;
        }
        public GameObject getHotspot()
        {
            return hotspot;
        }
        public void SetEndTime(double endtime)
        {
            this.end_time = endtime;
        }

        public void SetMoreInfo(string name, string text, string url_photo, string url_video)
        {
            if (!name.Equals("Hotspot Name")) { this.name = name; }
            if (!text.Equals("Description")) { this.text = text; }
            if (!url_photo.Equals("Photo path here")) { this.url_photo = url_photo; }
            if (!url_video.Equals("Video path here.")) { this.url_video = url_video; }

        }
    }

    public class HotspotDatas
    {
        public string[] hotspotdatas;
    }
    public class HotspotData
    {
        public double start_time;
        public double end_time;
        public string name;
        public string text;
        public string url_photo;
        public string url_video;
        public Vector3 worldPosition;
        public Quaternion rot;
        public HotspotData(double _start_time, double _end_time, string _name, string _text, string _url_photo, string _url_video, GameObject _hotspot)
        {
            start_time = _start_time;
            end_time = _end_time;
            name = _name;
            text = _text;
            url_photo = _url_photo;
            url_video = _url_video;
            worldPosition = _hotspot.transform.position;
            rot = _hotspot.transform.rotation;
        }
    }
    public void save(Hashtable a)
    {
        Debug.Log("Saving");
        List<string> jsonlist = new List<string>();
        foreach (DictionaryEntry entry in a)
        {
            Hotspot h1 = (Hotspot)entry.Value;
            HotspotData h2 = new HotspotData(h1.getStart(), h1.getEnd(), h1.getName(), h1.getText(), h1.getUrl_photo(), h1.getUrl_video(), h1.getHotspot());
            string h3 = JsonUtility.ToJson(h2);
            jsonlist.Add(h3);
        }
        string[] jsons = new string[jsonlist.Count];
        jsons = jsonlist.ToArray();
        HotspotDatas hotspotdatas = new HotspotDatas() { hotspotdatas = jsons };
        string json = JsonUtility.ToJson(hotspotdatas);
        //Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/hotspots.json", json);
    }
    public void load()
    {
        table = new Hashtable();
        string json_path = System.IO.Path.Combine(statusController.getPath(), "hotspots.json");
        Debug.Log(json_path);
        if (File.Exists(json_path))
        {
            string hotspotjsons = File.ReadAllText(json_path);
            HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
            foreach (string json in hotspotdatas.hotspotdatas)
            {
                HotspotData h1 = JsonUtility.FromJson<HotspotData>(json);
                GameObject a = Instantiate(hotspot, h1.worldPosition, h1.rot);
                a.name = a.GetInstanceID().ToString();
                table.Add(a.GetInstanceID().ToString(), new Hotspot(a, h1.start_time, h1.end_time, h1.name, h1.text, h1.url_photo, h1.url_video));
            }
            hotspots_loaded = true;
        }
    }


    // reference: https://github.com/SrejonKhan/AnotherFileBrowser Accessed on: 23rd Feb 2021
    // <summary>
    /// FileDialog for picking a single file
    /// </summary>
    public string OpenFileBrowser(string typee)
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = typee + " files (*." + typee + ")|*." + typee;
        bp.filterIndex = 0;

        string res = "";

        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            res = result;
        });

        return res;
#endif
    }
}
