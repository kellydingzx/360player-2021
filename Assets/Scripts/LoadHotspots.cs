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
    public GameObject hotspot;
    public GameObject displayPanel;
    public GameObject displayName;
    public GameObject displayText;
    public GameObject displayVideoURL;
    public RawImage displayPhoto;
    public VideoPlayer videoPlayer;

    private Hashtable table;
    private string json_path;
    private Boolean loaded;

    void Start()
    {
        table = new Hashtable();
        loaded = false;
        displayPanel.SetActive(false);
    }

    private void Update()
    {
        if (json_path != null && !loaded)
        {
            table = load();
            loaded = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.gameObject.tag == "Trigger")
                {
                    string current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    Hotspot h = (Hotspot)table[current_id];
                    print(h.getName());
                    addToWindow(h);
                    videoPlayer.Pause();
                    displayPanel.SetActive(true);
                }
            }
        }

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

    private void addToWindow(Hotspot h)
    {
        displayName.GetComponent<Text>().text= h.getName();
        displayText.GetComponent<Text>().text= h.getText();
        //Add photo to window
        if (h.getUrl_photo() != null)
        {
            WWW www = new WWW("file:///" + h.getUrl_photo());
            displayPhoto.texture = www.texture;
        }
        //Add video url
        if (h.getUrl_video() != null) {displayVideoURL.GetComponent<Text>().text = h.getUrl_video(); }
        
    }

    public void closeWindow()
    {
        displayPanel.SetActive(false);
        videoPlayer.Play();
    }


    class Hotspot
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
        string text;
        string url_photo;
        string url_video;
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
    public Hashtable load()
    {
        Hashtable r = new Hashtable();
        string hotspotjsons = File.ReadAllText(json_path);
        //File.ReadAllText(Application.dataPath + "/hotspots.json");
        HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
        foreach (string json in hotspotdatas.hotspotdatas)
        {
            HotspotData h1 = JsonUtility.FromJson<HotspotData>(json);
            GameObject a = Instantiate(hotspot, h1.worldPosition, Quaternion.identity);
            r.Add(a.GetInstanceID().ToString(), new Hotspot(a, h1.start_time, h1.end_time, h1.name));
        }
        return r;
    }

    public void loadjson()
    {
        json_path = OpenFileBrowser("json");
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
