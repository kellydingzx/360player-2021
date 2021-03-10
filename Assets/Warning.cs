using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public GameObject warningPanel;
    public GameObject warningMessage;

    void Start()
    {
        warningPanel.SetActive(false);
    }

    public void displayErrorMessage(string error_mess){
        warningMessage.GetComponent<Text>().text = error_mess;
        warningPanel.SetActive(true);
    }

    public void closeWarningPanel(){
        warningPanel.SetActive(false);
    }
}
