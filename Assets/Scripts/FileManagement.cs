using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class FileManagement : MonoBehaviour
{
    //Panels 
    public GameObject packageNameText;
    public GameObject packagePathText;

    //Controllers
    public GameObject mainController;

    //Public Status Variable
    public bool path_ready;
    public string project_path;

    private string root_folder;
    private string project_name;
    

    void Start()
    {
        path_ready = false;
        root_folder = "";
    }


    public void openProject()
    {
        string zip_path = browse_package();
        project_name = Path.GetFileNameWithoutExtension(zip_path);
        root_folder = Directory.GetParent(zip_path).FullName;
        ZipFile.ExtractToDirectory(zip_path, root_folder);
        project_path = System.IO.Path.Combine(root_folder, project_name);
        Debug.Log(project_path);
        Debug.Log(project_name);
        path_ready = true;
        displayNameAndPath();
    }


    public string browse_package()
    {
        var bp = new BrowserProperties();
        bp.filter = "zip files (*.zip)|*.zip";
        bp.filterIndex = 0;

        string path = "";
        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            path = result;
        });

        return path;
    }

    public void displayNameAndPath()
    {
        packageNameText.GetComponent<Text>().text = project_name;
        packagePathText.GetComponent<Text>().text = project_path;
    }

}
