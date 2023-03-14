using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WinScreenScript : MonoBehaviour
{

    public Color[] player_colors;
    public TextMeshProUGUI winMessage;


    // Start is called before the first frame update
    void Start()
    {
        winMessage.color = player_colors[PlayerPrefs.GetInt("colorIndex",0)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
