using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelControl : MonoBehaviour
{
    public GameObject infoPanelControl;

    void Start()
    {
        infoPanelControl.SetActive(true);
    }

    public void closePanel()
    {
        infoPanelControl.SetActive(false);
    }
    
}
