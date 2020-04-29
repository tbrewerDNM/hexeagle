using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    public Player player;
    public int index;
    public Text playerName;
    public Dropdown dropdown;

    // Needed for other players where dropdown disabled.
    public Image raceBg;
    public Image raceImage;
    public Text raceText;

    // Start is called before the first frame update
    void Start()
    {
        if (index == PlayerWrapper.player.id)
            dropdown.onValueChanged.AddListener(delegate { UpdateRace(); });
        else
            dropdown.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        playerName.text = player.playerName;
        if (player.id != PlayerWrapper.player.id)
        {
            raceImage.sprite = AssetManager.assetManager.raceFaces[(int)player.race];
            raceText.text = player.race.ToString();
        }
    }

    public void UpdateRace()
    {
        LobbyManager.lobby.Get(PlayerWrapper.player.id).race = (PlayerRace)dropdown.value;
        StartCoroutine(ServerManager.serverManager.SendToServer("UpdatePlayer SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + player.id
                                                                                                          + "&player="
                                                                                                          + player.ToString(), null));
    }
}
