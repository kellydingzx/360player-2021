using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    private int xmin = 100;
    private int xmax = 1950;
    private int ymin = 50;
    private int ymax = 250;
    private RectTransform graphContainer;
    private void Awake() {
        graphContainer = transform.Find("Graph_Container").GetComponent<RectTransform>();
        float mainBY = MainBranch();
        float BY1 = NewBranch(mainBY, xmin + 100, new Vector2(xmax, (ymax+ymin)/2 + 50));
        float BY2 = NewBranch(BY1, xmin + 300, new Vector2(xmax, (ymax+ymin)/2 + 25));
        float BY3 = NewBranch(mainBY, xmin + 200, new Vector2(xmax, (ymax+ymin)/2 - 25));
    }
    private GameObject CreateCircle(Vector2 anchoredPosition, string name) {
        GameObject hi = new GameObject(name,typeof(Image));
        hi.transform.SetParent(graphContainer);
        Sprite circle = Resources.Load<Sprite>("Circle");
        hi.GetComponent<Image>().sprite = circle;
        RectTransform rectTransform = hi.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        return hi;
    }
    private void CreateDotConnection(Vector2 pos1 ,Vector2 pos2,string name) {
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
    private void BranchLine(Vector2 pos1 ,Vector2 pos2) {
        Vector2 pos3 = new Vector2(pos1.x + (pos2.x - pos1.x) * .05f,pos2.y);
        CreateDotConnection(pos1, pos3, "l1");
        CreateDotConnection(pos3, pos2, "l2");
    }
    private float MainBranch() {
        CreateCircle(new Vector2(xmin, (ymax+ymin)/2),"c1");
        CreateCircle(new Vector2(xmax, (ymax+ymin)/2),"c2");
        CreateDotConnection(new Vector2(xmin, (ymax+ymin)/2),new Vector2(xmax, (ymax+ymin)/2),"l1");
        return ((ymax+ymin)/2);
    }
    private float NewBranch(float stemY, float xpos, Vector2 to_add) {
        Vector2 stem = new Vector2(xpos, stemY);
        CreateCircle(to_add, "c1");
        BranchLine(stem,to_add);
        return to_add.y;
    }
}

