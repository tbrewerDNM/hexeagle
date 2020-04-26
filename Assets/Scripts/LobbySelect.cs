using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbySelect : MonoBehaviour
{
    public Lobby lobby = null;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        gameObject.SetActive(true);
        text.text = (lobby != null) ? lobby.GetName() : "";
    }

    private void AddPlayer()
    {
        StartCoroutine(ServerManager.serverManager.SendToServer("AddPlayer SESSIONID=" + LobbyManager.lobby.GetId() + "&player=" + PlayerWrapper.player.ToString(), AddPlayerCallback));
    }

    private void AddPlayerCallback()
    {

    }

    public void Click() {
        PlayerWrapper.player.id = lobby.FirstAvailable();
        lobby.AddPlayer(PlayerWrapper.player);
        LobbyManager.lobby = lobby;
        AddPlayer();
        SceneManager.LoadScene("Lobby");
    }

    public static void AddBot(Player player)
    {
        Lobby localLobby = LobbyManager.lobby;
        player.id = localLobby.FirstAvailable();
        localLobby.AddPlayer(player);
    }
}
