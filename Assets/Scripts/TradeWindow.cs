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
            // Set next turn to the next player.
            tradeRequest.turn = currentTrader.id;
            StartCoroutine(ServerManager.serverManager.SendToServer("TradeRequest SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + CardGameManager.localPlayer.player.id + "&" + tradeRequest.ToString(), null));
            Show(CardGameManager.cardGameManager.gamePlayers[currentTrader.id]);
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

                // If the other party has already accepted, give the resources locally.
                if (tradeRequest.accept1 + tradeRequest.accept2 == 4)
                {
                    print("obtained acc1");
                    TradeRequest.PrintMatrix(tradeRequest.inResources);
                    TradeRequest.PrintMatrix(tradeRequest.outResources);
                    TradeRequest.PrintMatrix(CardGameManager.localPlayer.GetResourceMatrix());
                    CardGameManager.localPlayer.Obtain(tradeRequest.outResources);
                    TradeRequest.PrintMatrix(CardGameManager.localPlayer.GetResourceMatrix());
                    tradeRequest.Reset();
                }

                // Send response to server.
                StartCoroutine(ServerManager.serverManager.SendToServer("RespondRequest SESSIONID=" + LobbyManager.lobby.GetId() + "&" + tradeRequest.ToString() + "&id=" + CardGameManager.localPlayer.player.id + "&response=2", null));
                
                // Update the UI.
                Show(CardGameManager.cardGameManager.gamePlayers[tradeRequest.pid]);
            }
        }
    }
}
