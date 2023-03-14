using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Player refs")]
    public Color[] player_colors;
    public List<PlayerController> players = new List<PlayerController>();
    public Transform[] spawn_points;




    [Header("prefab refs")]

    public GameObject playerContainerPrefab;

    [Header("Level Vars")]
    public int startTime;
    public float curTime;
    public List<PlayerController> winningPlayers;


    [Header("Components")]
    private AudioSource audio;
    public AudioClip[] game_fx;
    public Transform playerContainerParent;
    public TextMeshProUGUI time;



    public static GameManager instance;



    private void Awake()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
        startTime = PlayerPrefs.GetInt("roundTimer", 100);
    }

    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        time.text = curTime.ToString();
        winningPlayers= new List<PlayerController>();
        
    }
    private void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        time.text = ((int)curTime).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (curTime <= 0)
        {
            winningPlayers.Clear();
            int highscre = 0;
            int index = 0;
            foreach (PlayerController player in players)
            {
                if (player.score > highscre)
                {
                    winningPlayers.Clear();
                    highscre = player.score;
                    index = players.IndexOf(player);
                    winningPlayers.Add(player);
                }
                else if (player.score == highscre)
                {
                    
                    winningPlayers.Add(player);
                    time.text = ((int)curTime).ToString();
                }
            }

            if (winningPlayers.Count > 1)
            {
                //tie
                //play over time sound
                audio.PlayOneShot(game_fx[1]);
                foreach (PlayerController player in players) { 
                if (!winningPlayers.Contains(player))
                    {
                        player.drop_out();
                        
                    }
                }
                curTime = 30;
            }
            else
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("WinScene");
            }



            


        }
    }


    public void onPlayerJoined(PlayerInput player)
    {
        //play sound
        audio.PlayOneShot(game_fx[0]);
        //set player color when joined
        player.GetComponentInChildren<SpriteRenderer>().color = player_colors[players.Count];
        // creat ui player container
        PlayerContainerUI containerUI = Instantiate(playerContainerPrefab, playerContainerParent).GetComponent<PlayerContainerUI>();
        player.GetComponent<PlayerController>().setUiContainer(containerUI);
        containerUI.initialize(player_colors[players.Count]);

        // added the player to the players list
        players.Add(player.GetComponent<PlayerController>());
        // choose Spawn point 
        player.transform.position = spawn_points[Random.Range(0,spawn_points.Length)].position;
    }




}
