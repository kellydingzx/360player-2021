using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    //Panels
    public VideoPlayer videoPlayer;
    public GameObject window;
    //Input fields
    public GameObject nameinputField;
    public GameObject textInputField;
    //Buttons
    public GameObject hotspot;
    public GameObject back_button;
    
    //displays
    public GameObject id_display;
    public GameObject start_time;
    public GameObject photoUrlDisplay;
    public GameObject videoUrlDisplay;
    public GameObject packagePathDisplay;
    
    //Controllers
    public GameObject packageManager;
    public GameObject fileManager;

    private Hashtable all_hotspots;
    public string current_id;

    //Status variables
    private bool loaded;
    private string packagePath;
   
    void Start()
    {
        loaded = false;
        window.SetActive(false);
        back_button.SetActive(false);
    }

    void Update(){
        if(packageManager.GetComponent<PackageManager>().path_ready && !loaded){   
            packagePath = packagePathDisplay.GetComponent<Text>().text;
            all_hotspots = load();
            loaded = true;
        }
        if(packageManager.GetComponent<PackageManager>().mainVideo_ready){
            inner_update();
        }
    }

    void inner_update()
    {
        //Add Hotspot on right click
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,20));
            double start_time = videoPlayer.time;
            GameObject a = Instantiate(hotspot, worldPosition, transform.rotation);
            a.name = a.GetInstanceID().ToString();
            all_hotspots.Add(a.name, new Hotspot(a, start_time,videoPlayer.length));
            a.SetActive(true);
        }

        //View hotspot on left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit)){
                if(hit.transform.gameObject.tag == "Trigger")
                {
                    window.SetActive(true);
                    current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    Debug.Log(current_id);
                    id_display.GetComponent<Text>().text = current_id;
                    Hotspot h = (Hotspot)all_hotspots[current_id];
                    Debug.Log(h.getName());
                    loadHotspot(h);
                    videoPlayer.Pause();
                }
            }
        }

        //check the validity of hotspot in terms of time
        double current_time = videoPlayer.time;
        foreach (DictionaryEntry entry in all_hotspots)
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

    public void loadHotspot(Hotspot hs){
        start_time.GetComponent<Text>().text = hs.getStart().ToString();
        nameinputField.GetComponent<InputField>().text = hs.getName();
        Debug.Log(hs.getName());
        textInputField.GetComponent<InputField>().text = hs.getText();
        //display Photo
        if (hs.getUrl_photo() != null)
        {
            photoUrlDisplay.GetComponent<Text>().text = hs.getUrl_photo();
            string new_photo_url = System.IO.Path.Combine(packagePath, hs.getUrl_photo());
            fileManager.GetComponent<FileManager>().loadVideoOntoPanel(new_photo_url);
            Debug.Log(hs.getUrl_photo());
        }
        //display Video Url
        if(hs.getUrl_video() != null)
        {
            videoUrlDisplay.GetComponent<Text>().text = hs.getUrl_video();
            Debug.Log("video");
            Debug.Log(hs.getUrl_video());
        }
    }

    public void saveJson()
    {
        save(all_hotspots, packagePath);
    }

    public void closeWindow()
    {
        window.SetActive(false);
        videoPlayer.Play();
    }

    public void SetEndtime()
    {
        Hotspot a = (Hotspot)all_hotspots[id_display.GetComponent<Text>().text];
        a.SetEndTime(videoPlayer.time);
    }

    public void delete_hotspot()
    {
        string id = id_display.GetComponent<Text>().text;
        Hotspot h = (Hotspot)all_hotspots[id];
        GameObject a = h.getHotspot();
        all_hotspots.Remove(id);
        Destroy(a);
        window.SetActive(false);
        videoPlayer.Play();
    }

    public void ClickSave()
    {
        string name = nameinputField.GetComponent<InputField>().text;
        string text = textInputField.GetComponent<InputField>().text;
        string url_photo = photoUrlDisplay.GetComponent<Text>().text;
        string url_video = videoUrlDisplay.GetComponent<Text>().text;

        if (!url_photo.Equals("Photo path here.") && Path.IsPathRooted(@url_photo))
        { 
            url_photo = packageManager.GetComponent<PackageManager>().copyFile(url_photo, name, "Pictures");
        }

        if (!url_video.Equals("Video path here.") && Path.IsPathRooted(@url_video)) { 
            url_video = packageManager.GetComponent<PackageManager>().copyFile(url_video, name, "Videos");
        }

        Hotspot h = (Hotspot)all_hotspots[current_id];
        Debug.Log("video stored" + url_video);
        h.SetMoreInfo(name, text, url_photo, url_video);
        all_hotspots[current_id] = h;

        nameinputField.GetComponent<InputField>().text = "";
        textInputField.GetComponent<InputField>().text = "";
        photoUrlDisplay.GetComponent<Text>().text = "Photo path here.";
        videoUrlDisplay.GetComponent<Text>().text = "Video path here.";

        closeWindow();
    }

    void OnApplicationQuit()
    {
        if(packagePath != null){
            saveJson();
        }
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

        public Hotspot(GameObject a, double start, double end)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
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
            if(!name.Equals("Hotspot Name")) { this.name = name;}
            if(!text.Equals("Description")) { this.text = text; }
            if (!url_photo.Equals("Photo path here.")){ 
                this.url_photo = url_photo;
            }
            if (!url_video.Equals("Video path here.")) { 
                this.url_video = url_video;
            }
                
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
        public HotspotData(double _start_time, double _end_time, string _name, string _text, string _url_photo, string _url_video,GameObject _hotspot)
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
    public void save(Hashtable a, string json_folder)
    {
        Debug.Log("Saving");
        List<string> jsonlist = new List<string>();
        foreach (DictionaryEntry entry in a)
        {
            Hotspot h1 = (Hotspot)entry.Value;
            HotspotData h2 = new HotspotData(h1.getStart(),h1.getEnd(),h1.getName(),h1.getText(),h1.getUrl_photo(),h1.getUrl_video(),h1.getHotspot());
            string h3 = JsonUtility.ToJson(h2);
            jsonlist.Add(h3);
        }
        string[] jsons = new string[jsonlist.Count];
        jsons = jsonlist.ToArray();
        HotspotDatas hotspotdatas = new HotspotDatas() { hotspotdatas = jsons };
        string json = JsonUtility.ToJson(hotspotdatas);
        //Debug.Log(json);
        string json_path = System.IO.Path.Combine(json_folder, "hotspots.json");
        File.WriteAllText(json_path, json);
    }
    public Hashtable load()
    {
        Hashtable r = new Hashtable();
        string json_path = System.IO.Path.Combine(packagePath, "hotspots.json");
        string hotspotjsons = File.ReadAllText(json_path);
        HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
        foreach (string json in hotspotdatas.hotspotdatas)
        {
            HotspotData h1 = JsonUtility.FromJson<HotspotData>(json);
            GameObject a = Instantiate(hotspot, h1.worldPosition, h1.rot);
            r.Add(a.GetInstanceID().ToString(), new Hotspot(a, h1.start_time, h1.end_time, h1.name, h1.text, h1.url_photo, h1.url_video));
        }
        Debug.Log("addded");
        return r;
    }

    
}