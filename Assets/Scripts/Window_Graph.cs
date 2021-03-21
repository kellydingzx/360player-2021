using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    public int xmin = 100;
    public int xmax = 1950;
    public int ymin = 50;
    public int ymax = 250;
    public RectTransform graphContainer;
    public LoadHotspots controller;
    public TimelineController timelineController;
    public BranchProject branchProject;
    public void Awake() {
        graphContainer = transform.Find("Graph_Container").GetComponent<RectTransform>();
    }
    public GameObject CreateCircle(Vector2 pos1, string name) {
        GameObject hi = new GameObject(name,typeof(Image));

        Sprite circle = Resources.Load<Sprite>("Circle");
        hi.GetComponent<Image>().sprite = circle;

        hi.AddComponent<Button>();
        hi.GetComponent<Button>().onClick.AddListener(delegate { openWindow1(hi.GetInstanceID().ToString()); });

        hi.transform.SetParent(graphContainer);
        RectTransform rectTransform = hi.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = pos1;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        return hi;
    }
    public void openWindow1(string node_id) {
        string path_needs_branched = timelineController.getPathByID(node_id);
        Vector3 needs_lookat = timelineController.getWorldPositionByID(node_id);
        double needs_jump_to = timelineController.getStartTimeByID(node_id);
        branchProject.branchOutbyNode(path_needs_branched, needs_lookat, needs_jump_to);
    }
    public void CreateDotConnection(Vector2 pos1 ,Vector2 pos2,string name) {
        GameObject hi = new GameObject(name,typeof(Image));
        hi.transform.SetParent(graphContainer, false);
        hi.GetComponent<Image>().color = new Color(1,1,1,.5f);
        RectTransform rectTransform = hi.GetComponent<RectTransform>();
        Vector2 dir = (pos2 - pos1).normalized;
        float dis = Vector2.Distance(pos1, pos2);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(dis, 3f);
        rectTransform.anchoredPosition = pos1 + dir * dis * .5f;
        float angle = (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    public float MainBranch(string name) {
        CreateCircle(new Vector2(xmin, (ymax+ymin)/2), name + "c");
        CreateDotConnection(new Vector2(xmin, (ymax+ymin)/2),new Vector2(xmax, (ymax+ymin)/2),name);
        return ((ymax+ymin)/2);
    }
    public void ClearGraph() {
        foreach (Transform child in graphContainer)
            if(child.name != "Background") {
                Destroy(child.gameObject);
            }
    }
}

