using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWrapper : MonoBehaviour
{
    public string playerName;
    public string playerId;
    public static Player player;

    public InputField input;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PLAYER_NAME"))
        {
            playerName = PlayerPrefs.GetString("PLAYER_NAME");
            input.text = playerName;
        }

        if (PlayerPrefs.HasKey("PLAYER_ID"))
        {
            playerId = PlayerPrefs.GetString("PLAYER_ID");
        }
        else
        {
            playerId = Lobby.GenerateRandomString(16);
            PlayerPrefs.SetString("PLAYER_ID", playerId);
        }

        player = new Player(playerName + "-" + playerId + "-0|1-1-1-1-0-0-0");
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeName()
    {
        player = new Player(input.text + "-" + playerId + "-0|1-1-1-1-0-0-0");
        PlayerPrefs.SetString("PLAYER_NAME", input.text);
    }

    public void ClearCache()
    {
        PlayerPrefs.DeleteAll();
    }
}
