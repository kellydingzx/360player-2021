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
    public StatusController statusController;
    public LoadHotspots controller;
    public VideoManager videoManager;

    private string root_folder;

    public void openProject()
    {
        string zip_path = browse_package();
        string project_name = Path.GetFileNameWithoutExtension(zip_path);
        root_folder = Directory.GetParent(zip_path).FullName;
        root_folder = Path.Combine(root_folder, project_name);
        extractFiles(zip_path);
        string project_path = Path.Combine(root_folder, project_name);
        statusController.setNameAndPath(project_name, project_path);
        videoManager.loadVideo(Path.Combine(project_path, "MainVideo.mp4"));
        controller.please_load();
    }

    public void extractFiles(string zip_path)
    {
        try
        {
            ZipFile.ExtractToDirectory(zip_path, root_folder);
        }
        catch (IOException ioException)
        {
            root_folder += "-2";
            extractFiles(zip_path);
        }
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

}
