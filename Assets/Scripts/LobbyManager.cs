using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager lobbyManager;
    public static Lobby lobby { get; set; }

    public LobbyPlayer[] lobbyPlayers;
    public Text countdown;
    public Button startButton;

    private int i = 0;
    private bool showCountdown = false;
    private DateTime startTime;

    void Awake()
    {
        if (PlayerWrapper.player.id == 0)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start() {
        lobbyManager = this;

        if (ServerManager.serverManager.localGame)
        {
            LobbySelect.AddBot(new Player("Elf" + "-1|1-1-1-1-0-0-0"));
            LobbySelect.AddBot(new Player("Dwarf" + "-2|1-1-1-1-0-0-0"));
            LobbySelect.AddBot(new Player("Lizard" + "-3|1-1-1-1-0-0-0"));
        }

        UpdatePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime currentTime = DateTime.UtcNow;
        int cd = 100;

        if (showCountdown) {
            cd = Mathf.FloorToInt(((startTime.Ticks - currentTime.Ticks) / 6000000));
            countdown.text = "Game starting in " + cd;

            if (cd <= 0)
                SceneManager.LoadScene("CardGame");
        }

        // poll every 3 seconds
        if (i >= 180) {
            i = -1;
            UpdateLobby();
        }

        if (i >= 0 && !ServerManager.serverManager.localGame) {
            i++;
        }
    }

    // check if game should start
    protected void UpdateLobby() {
        StartCoroutine(ServerManager.serverManager.SendToServer("PollStart SESSIONID=" + LobbyManager.lobby.GetId(), UpdateLobbyCallback));
    }

    protected void UpdateLobbyCallback() {
        switch (ServerManager.serverManager.status) {
            /* success case */
            case 0:
                SortedDictionary<string, string> sd = ServerManager.ps_to_map(ServerManager.serverManager.form);
                lobby = Lobby.FromString(sd);
                print(lobby);
                print(ServerManager.serverManager.form);
                UpdatePlayers();

                if (sd["starttime"] != "")
                {
                    startTime = DateTime.Parse(sd["starttime"].Replace('-', ' '));
                    showCountdown = true;
                }

                i = 0;
                break;
            /* fail case */
            case -1:
                break;
        }
    }

    public void StartLobby() {
        startButton.interactable = false;
        StartCoroutine(ServerManager.serverManager.SendToServer("StartLobby SESSIONID=" + LobbyManager.lobby.GetId() + "&time=" + System.DateTime.UtcNow.AddSeconds(10).ToString().Replace(' ', '-'), StartLobbyCallback));
    }

    private void StartLobbyCallback() {
        switch (ServerManager.serverManager.status) {
            /* success case */
            case 0:
                break;
            /* fail case */
            case -1:
                SceneManager.LoadScene("CardGame");
                break;
        }
    }

    public void UpdatePlayers() {
        for (int i = 0; i < 4; i++) {
            if (i < LobbyManager.lobby.Count())
            {
                lobbyPlayers[i].gameObject.SetActive(true);
                lobbyPlayers[i].player = LobbyManager.lobby.Get(i);
                lobbyPlayers[i].UpdateText();
                lobbyPlayers[i].raceImage.sprite = AssetManager.assetManager.raceFaces[(int)lobbyPlayers[i].player.race];
                lobbyPlayers[i].raceText.text = lobbyPlayers[i].player.race.ToString();
            }
            else
            {
                lobbyPlayers[i].gameObject.SetActive(false);
            }
        }
    }

    public void LeaveLobby()
    {
        // If this is host, terminate lobby.
        // Else remove player from lobby.
        if (PlayerWrapper.player.id == 0)
        {
            StartCoroutine(ServerManager.serverManager.SendToServer("EndLobby SESSIONID=" + LobbyManager.lobby.GetId(), LeaveLobbyCallback));
        }
        else
        {
            StartCoroutine(ServerManager.serverManager.SendToServer("RemovePlayer SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + PlayerWrapper.player.id, LeaveLobbyCallback));
        }
    }

    private void LeaveLobbyCallback()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
