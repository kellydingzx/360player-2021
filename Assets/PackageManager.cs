using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class PackageManager : MonoBehaviour
{
    //Panels 
    public GameObject packageControlPanel;
    public GameObject packageNameText;
    public GameObject packagePathText;
    public GameObject packageNameInputPanel;
    public GameObject packageNameInputField;

    //Controllers
    public GameObject mainController;
    public GameObject warningController;
    public GameObject videoManager;

    //Public Status Variable
    public bool path_ready;
    public bool mainVideo_ready;

    private string root_folder;
    private string project_name;
    private string package_path;

    void Start(){
        path_ready = false;
        root_folder = "";
        packageNameInputPanel.SetActive(false);
        packageControlPanel.SetActive(false);
    }

    public void start_newProject(){
         root_folder = browse_folder();
         packageNameInputPanel.SetActive(true); 
         packageControlPanel.SetActive(false);
    }

    public void start_existProject(){
        package_path = browse_folder();
        project_name = Path.GetFileName(package_path);
        Debug.Log(project_name);
        string main_video_path = System.IO.Path.Combine(package_path,"MainVideo.mp4");
        mainVideo_ready = File.Exists(@main_video_path);
        Debug.Log(main_video_path);
        if(!mainVideo_ready){
            warningController.GetComponent<Warning>().displayErrorMessage("Please add a main video.");
        }else{
            videoManager.GetComponent<Video_Manager>().addUrlToplayer(main_video_path);
        }
        path_ready = true;
        displayNameAndPath();
    }


    public string browse_folder(){
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
        bp.filterIndex = 0;

        string selected_path = "";
        new FileBrowser().OpenFolderBrowser(bp, path =>
        {
            selected_path = path;
        });

        return selected_path;
    }

    public void save_name(){
        project_name = packageNameInputField.GetComponent<InputField>().text;
        Debug.Log(project_name);
        packageNameInputPanel.SetActive(false);
        create_folder();
        path_ready = true;
        displayNameAndPath();
    }

    public void create_folder(){
        package_path = System.IO.Path.Combine(root_folder, project_name);
        Debug.Log(package_path);
        System.IO.Directory.CreateDirectory(package_path);
        create_SubElems(package_path);
    }

    public void create_SubElems(string folder_url){
        string videos_folder_path = System.IO.Path.Combine(folder_url, "Videos");
        if(!Directory.Exists(@videos_folder_path)){
            System.IO.Directory.CreateDirectory(videos_folder_path);
        }
        string pics_folder_path = System.IO.Path.Combine(folder_url, "Pictures");
        if(!Directory.Exists(@pics_folder_path)){
            System.IO.Directory.CreateDirectory(pics_folder_path);
        }
        string json_path = System.IO.Path.Combine(folder_url, "hotspots.json");
        if(!File.Exists(@json_path)){           
            File.Create(json_path).Dispose();
            mainController.GetComponent<Controller>().save(new Hashtable(), folder_url);
        }
    }

    public void displayNameAndPath(){
        packageNameText.GetComponent<Text>().text = project_name;
        packagePathText.GetComponent<Text>().text = package_path;
    }

    public void open_package_control_panel(){
        packageControlPanel.SetActive(true);
    }

    public string addMainVideo(string url){
        string new_path = System.IO.Path.Combine(package_path, "MainVideo.mp4");
        System.IO.File.Copy(url, new_path, true);
        packageControlPanel.SetActive(false);
        mainVideo_ready = true;
        return new_path;
    }

    public string copyFile(string original_url, string new_name, string folder_name){
        string file_folder_path = System.IO.Path.Combine(package_path, folder_name);
        string new_file_name = new_name+System.IO.Path.GetExtension(original_url);
        string new_file_url = System.IO.Path.Combine(file_folder_path, new_file_name);
        System.IO.File.Copy(original_url, new_file_url, true);
        Debug.Log(System.IO.Path.Combine(folder_name, new_file_name));
        return System.IO.Path.Combine(folder_name, new_file_name);
    }

}
