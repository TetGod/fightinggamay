using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public GameObject panel;
    



    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onOptionClick()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void onCloseClick() { 
        panel.SetActive(false); 
    }

    public void onLevelSelect(int level)
    {
        SceneManager.LoadScene(level);
    }

}
