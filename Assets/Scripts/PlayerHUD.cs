using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static bool requestSent = false;

    public Text playerName;
    public Image playerBg;
    public Image playerImage;
    public int index;
    // Index 0 is battle, index 1 is trade.
    public Button[] buttons;

    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.turnManager.phase == PhaseType.OVERWORLD && index != CardGameManager.localPlayer.player.id)
        {
            buttons[1].interactable = true;
        }
        else
        {
            buttons[0].interactable = false;
            buttons[1].interactable = false;
        }
    }

    public void Init(Player p) {
        player = p;
        playerName.text = p.playerName;
        playerImage.sprite = AssetManager.assetManager.raceFaces[(int)p.race];
        playerBg.sprite = AssetManager.assetManager.colorBgs[p.id];
    }

    public void OpenTrade()
    {
        CardGameManager.cardGameManager.tradeMenu.Show(CardGameManager.cardGameManager.gamePlayers[index]);
    }

    public void StartBattle()
    {
        foreach (PlayerHUD hud in CardGameManager.cardGameManager.playerHuds)
        {
            hud.buttons[0].interactable = false;
        }

        if (!ServerManager.serverManager.localGame)
        {
            requestSent = true;
            StartCoroutine(ServerManager.serverManager.SendToServer("StartBattle SESSIONID=" + LobbyManager.lobby.GetId()
                                                                                             + "&atk=" + CardGameManager.localPlayer.player.id
                                                                                             + "&def=" + index
                                                                                             + "&time=" + System.DateTime.UtcNow.AddSeconds(30).ToString().Replace(' ', '-'), null));
        }
        else
        {
            BattleManager.battleManager.InitBattle(CardGameManager.localPlayer, CardGameManager.cardGameManager.gamePlayers[index]);
            BattleManager.StartBattle();
            CardGameManager.inCombat = true;
        }
    }
}
