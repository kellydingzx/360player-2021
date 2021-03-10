using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact_Window : MonoBehaviour
{
    public GameObject contactWindow;
    // The Contact Window is always open at the start of the video
    void Start()
    {
        contactWindow.SetActive(true);
    }

    public void closeContactWindow()
    {
        contactWindow.SetActive(false);
    }

    public void openContactWindow()
    {
        contactWindow.SetActive(true);
    }
}
