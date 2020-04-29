using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeWindow : MonoBehaviour
{
    private TradeRequest tradeRequest;
    private Player currentTrader;
    public TradeUI[] tradeUis;
    // 0 - Trade, 1 - Accept, 2 - Decline
    public Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Show(GamePlayer player)
    {
        tradeRequest = player.currentRequest;
        print(tradeRequest);
        gameObject.SetActive(true);
        currentTrader = player.player;
        Init();

        // Disable buttons with it is the other persons turn.
        if (tradeRequest.turn != CardGameManager.localPlayer.player.id && tradeRequest.turn != -1)
        {
            buttons[0].GetComponentInChildren<Text>().text = "Waiting on " + player.player.playerName + "...";
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
        }
        else
        {
            buttons[0].GetComponentInChildren<Text>().text = "Trade";
            foreach (Button button in buttons)
            {
                button.interactable = true;
            }
        }

        if (tradeRequest.empty())
        {
            buttons[1].interactable = false;
            buttons[2].interactable = false;
        }
    }

    public void Init()
    {
        tradeUis[0].UpdateUI(CardGameManager.localPlayer.player, tradeRequest.accept1);
        tradeUis[0].UpdateUI(tradeRequest.inResources);
        tradeUis[1].UpdateUI(currentTrader, tradeRequest.accept2);
        tradeUis[1].UpdateUI(tradeRequest.outResources);
    }

    public void SendTrade()
    {
        if (CardGameManager.localPlayer.CanAfford(tradeRequest.inResources))
        {
            // First reset any trade that was there.
            StartCoroutine(ServerManager.serverManager.SendToServer("ResetTrade SESSIONID=" + LobbyManager.lobby.GetId() + "&pid1=" + CardGameManager.localPlayer.player.id + "&pid2=" + currentTrader.id, null));

            // Set next turn to the next player.
            tradeRequest.turn = currentTrader.id;
            StartCoroutine(ServerManager.serverManager.SendToServer("TradeRequest SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + CardGameManager.localPlayer.player.id + "&" + tradeRequest.ToString(), SendTradeCallback));
            Show(CardGameManager.cardGameManager.gamePlayers[currentTrader.id]);
        }
    }

    private void SendTradeCallback()
    {
        switch (ServerManager.serverManager.status)
        {
            /* success case */
            case 0:
                break;
            /* fail case */
            case -1:
                // Resend on fail.
                SendTrade();
                break;
        }
    }

    public void UpdateTraderValue(int index)
    {
        UpdateValue(index, false);
    }

    public void UpdateLocalValue(int index)
    {
        UpdateValue(index, true);
    }

    public void UpdateValue(int index, bool local)
    {
        if (local)
        {
            tradeRequest.inResources[index] = tradeUis[0].dropdowns[index].value;
        }
        else
        {
            tradeRequest.outResources[index] = tradeUis[1].dropdowns[index].value;
        }
        print(tradeRequest);
    }

    public void Accept()
    {
        // Only accept if this is responder, or other party has responded.
        if (CardGameManager.localPlayer.player.id == tradeRequest.turn)
        {
            if (CardGameManager.localPlayer.CanAfford(tradeRequest.outResources))
            {
                tradeRequest.accept1 = 2;
                tradeRequest.turn = currentTrader.id;
                print("Player has accepted the request!");
                print(tradeRequest);

                // Take the resources they wish to trade.
                CardGameManager.localPlayer.resourceBuffer = tradeRequest.inResources;
                CardGameManager.localPlayer.Purchase(tradeRequest.inResources);

                tradeUis[0].EnableAll(false);
                tradeUis[1].EnableAll(false);

                // If the other party has already accepted, give the resources locally.
                if (tradeRequest.accept1 + tradeRequest.accept2 == 4)
                {
                    print("obtained acc1");
                    CardGameManager.localPlayer.Obtain(tradeRequest.outResources);
                    tradeRequest.Reset();
                    tradeUis[0].EnableAll(true);
                    tradeUis[1].EnableAll(true);
                }

                // Send response to server.
                StartCoroutine(ServerManager.serverManager.SendToServer("RespondRequest SESSIONID=" + LobbyManager.lobby.GetId() + "&" + tradeRequest.ToString() + "&id=" + CardGameManager.localPlayer.player.id + "&response=2", null));

                // Update the UI.
                Show(CardGameManager.cardGameManager.gamePlayers[tradeRequest.pid]);
            }
            else
            {
                print("Cant afford");
            }
        }
        else
        {
            print("Not your turn!");
        }
    }

    public void Decline()
    {
        print("Decline");
        if (CardGameManager.localPlayer.player.id == tradeRequest.turn)
        {
            print("yes");
            tradeRequest.accept1 = 1;
            tradeRequest.turn = currentTrader.id;
            // Send response to server.
            StartCoroutine(ServerManager.serverManager.SendToServer("RespondRequest SESSIONID=" + LobbyManager.lobby.GetId() + "&" + tradeRequest.ToString() + "&id=" + CardGameManager.localPlayer.player.id + "&response=1", null));
            tradeRequest.Reset();
            tradeUis[0].EnableAll(true);
            tradeUis[1].EnableAll(true);
            Show(CardGameManager.cardGameManager.gamePlayers[tradeRequest.pid]);
        }
    }
}
