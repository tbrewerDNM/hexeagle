using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager cardGameManager;
    public static GamePlayer localPlayer { get; set; }
    public static bool inCountdown = false;
    public static bool inCombat = false;
    public static bool menuOpen = false;

    public GamePlayer[] gamePlayers;
    public UnitDisplay[] unitDisplays;
    public PlayerHUD[] playerHuds;
    public Text[] resourceText;
    public TradeWindow tradeMenu;
    public GameObject generalUI;
    public Text countdownText;
    public Text pingText;

    private DateTime startTime;
    private int counter = 0;

    // Start is called before the first frame update
    void Awake() {
        cardGameManager = this;

        FindObjectOfType<MusicPlayer>().GetComponent<AudioSource>().volume = 0.1f;
        FindObjectOfType<MusicPlayer>().Play(0);

        for (int i = 0; i < LobbyManager.lobby.Count(); i++) {

            // Set local player to the player this runs on.
            if (i == PlayerWrapper.player.id)
            {
                localPlayer = gamePlayers[i];
            }

            // Initalize in-game players with the actual players.
            gamePlayers[i].player = LobbyManager.lobby.Get(i);
            gamePlayers[i].currentRequest = new TradeRequest(gamePlayers[i].player.id);

            // Init player huds.
            playerHuds[i].gameObject.SetActive(true);
            playerHuds[i].Init(gamePlayers[i].player);

            if (localPlayer != null && i == localPlayer.player.id)
            {
                foreach (Button button in playerHuds[i].buttons)
                {
                    button.interactable = false;
                }
            }

            // Hide the trade menu.
            tradeMenu.Show(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        pingText.text = ServerManager.serverManager.ping + " ms";

        // poll every 3 seconds
        if (counter >= 180)
        {
            counter = -1;
            UpdateGame();
        }

        if (counter >= 0 && !ServerManager.serverManager.localGame)
        {
            counter++;
        }
        else
        {
            UpdatePlayers();
        }

        // Cannot trade in bot games.
        if (ServerManager.serverManager.localGame)
        {
            foreach (PlayerHUD hud in  playerHuds)
            {
                hud.buttons[1].interactable = false;
            }
        }

        DateTime currentTime = DateTime.UtcNow;
        int countdown = 100;

        if (startTime != null && inCountdown && !inCombat)
        {
            countdown = Mathf.FloorToInt(((startTime.Ticks - currentTime.Ticks) / 6000000));
            countdownText.text = "Combat starting in: " + countdown;

            foreach (PlayerHUD hud in playerHuds)
            {
                hud.buttons[0].interactable = false;
            }

            if (countdown <= 0)
            {
                BattleManager.StartBattle();
                inCombat = true;
                inCountdown = false;
            }
        }
    }

    // check if game should start
    public static void UpdateGame()
    {
        cardGameManager.StartCoroutine(ServerManager.serverManager.SendToServer("UpdatePlayer SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + localPlayer.player.id 
                                                                                                          + "&player=" 
                                                                                                          + localPlayer.player.ToString(), null));
        cardGameManager.StartCoroutine(ServerManager.serverManager.SendToServer("PollTrade SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + localPlayer.player.id, cardGameManager.UpdateTradeCallback));

        if (!inCountdown && !inCombat)
            cardGameManager.StartCoroutine(ServerManager.serverManager.SendToServer("PollBattle SESSIONID=" + LobbyManager.lobby.GetId(), cardGameManager.UpdateBattleCallback));
        ServerManager.Ping();
        cardGameManager.StartCoroutine(ServerManager.serverManager.SendToServer("PollGame SESSIONID=" + LobbyManager.lobby.GetId(), cardGameManager.UpdateGameCallback));
    }

    protected void UpdateGameCallback()
    {
        switch (ServerManager.serverManager.status)
        {
            /* success case */
            case 0:
                print(ServerManager.serverManager.form);
                if (ServerManager.serverManager.form != "")
                {
                    ServerManager.PingCallback();
                    SortedDictionary<string, string> ps = ServerManager.ps_to_map(ServerManager.serverManager.form);
                    LobbyManager.lobby.UpdateLobbyPlayers(ps);
                    UpdatePlayers();

                    string[] rm = ps["ready"].Split('-');

                    if (System.Int32.Parse(rm[localPlayer.player.id]) != TurnManager.turnManager.ready)
                    {
                        if (TurnManager.turnManager.ready == 1 && System.Int32.Parse(rm[localPlayer.player.id]) == 0)
                        {
                            TurnManager.turnManager.ready = 0;
                            TurnManager.turnManager.readyButton.interactable = true;
                        }
                        else
                        {
                            TurnManager.turnManager.ReadyUp();
                        }
                    }

                    if (System.Int32.Parse(ps["phase"]) != (int)TurnManager.turnManager.phase)
                    {
                        TurnManager.NextPhase();
                    }
                }

                counter = 0;
                break;
            /* fail case */
            case -1:
                break;
        }
    }

    protected void UpdateBattleCallback()
    {
        switch (ServerManager.serverManager.status)
        {
            /* success case */
            case 0:
                print(ServerManager.serverManager.form);
                SortedDictionary<string, string> ps;

                if (ServerManager.serverManager.form != "" && !inCountdown)
                {
                    PlayerHUD.requestSent = false;
                    ps = ServerManager.ps_to_map(ServerManager.serverManager.form);

                    int id1 = System.Int32.Parse(ps["atk"]);
                    int id2 = System.Int32.Parse(ps["def"]);

                    GamePlayer gp1 = (id1 == localPlayer.player.id) ? localPlayer : gamePlayers[id1];
                    GamePlayer gp2 = (id2 == localPlayer.player.id) ? localPlayer : gamePlayers[id2];

                    BattleManager.battleManager.InitBattle(gp1, gp2);

                    print(ps["time"]);

                    startTime = DateTime.Parse(ps["time"].Replace('-', ' '));

                    print(startTime);

                    inCountdown = true;
                }
                else if (ServerManager.serverManager.form == "" && PlayerHUD.requestSent)
                {
                    playerHuds[localPlayer.player.id].StartBattle();
                }

                break;
            /* fail case */
            case -1:
                break;
        }
    }

    private void Swap(in int value1, in int value2, out int val1, out int val2)
    {
        int tmp = value1;
        val1 = value2;
        val2 = tmp;
    }

    protected void UpdateTradeCallback()
    {
        switch (ServerManager.serverManager.status)
        {
            /* success case */
            case 0:
                SortedDictionary<string, string> ps;
                print("Trade form " + ServerManager.serverManager.form);

                if (ServerManager.serverManager.form != "")
                {
                    string[] psSplit = ServerManager.serverManager.form.Split('|');

                    foreach (string psString in psSplit)
                    {
                        ps = ServerManager.ps_to_map(psString);

                        // id1 should always be the local players id, id2 is the other players id.
                        int id1 = System.Int32.Parse(ps["pid1"]);
                        int id2 = System.Int32.Parse(ps["pid2"]);
                        int accept1 = System.Int32.Parse(ps["accept1"]);
                        int accept2 = System.Int32.Parse(ps["accept2"]);

                        if (id2 == localPlayer.player.id)
                        {
                            Swap(id1, id2, out id1, out id2);
                            Swap(accept1, accept2, out accept1, out accept2);
                        }

                        int turn = System.Int32.Parse(ps["turn"]);

                        // Poll once if turn is different.
                        if (turn != gamePlayers[id2].currentRequest.turn && id1 != id2)
                        {
                            gamePlayers[id2].currentRequest.turn = turn;
                            gamePlayers[id2].currentRequest.UpdateTradeRequest(ps, id2);

                            print(gamePlayers[id2].currentRequest);

                            tradeMenu.Show(gamePlayers[id2]);
                        }

                        print(accept1 + accept2);

                        print(gamePlayers[id2].currentRequest.isReset);

                        if (accept1 > 0 || accept1 > 0)
                        {
                            tradeMenu.tradeUis[0].EnableAll(false);
                            tradeMenu.tradeUis[1].EnableAll(false);
                        }

                        // Cannot change values if either party has accepted the deal.
                        if ((accept1 == 1 || accept2 == 1) && !gamePlayers[id2].currentRequest.empty())
                        {
                            gamePlayers[id2].currentRequest.Reset();
                            gamePlayers[id2].currentRequest.isReset = false;
                            tradeMenu.Init();
                            tradeMenu.Show(false);
                            tradeMenu.tradeUis[0].EnableAll(true);
                            tradeMenu.tradeUis[1].EnableAll(true);
                            StartCoroutine(ServerManager.serverManager.SendToServer("ResetTrade SESSIONID=" + LobbyManager.lobby.GetId() + "&pid1=" + id1 + "&pid2=" + id2, null));
                            return;
                        }

                        // If both parties accept, give each their respected resources.
                        // Then reset request.
                        if (accept1 + accept2 == 4)
                        {
                            print("Both parties have accepted the trade request!");
                            print(gamePlayers[id2].currentRequest);

                            if (!gamePlayers[id2].currentRequest.empty())
                            {
                                localPlayer.Obtain(gamePlayers[id2].currentRequest.outResources);
                                gamePlayers[id2].currentRequest.Reset();
                                tradeMenu.Init();
                                tradeMenu.Show(false);
                                tradeMenu.tradeUis[0].EnableAll(true);
                                tradeMenu.tradeUis[1].EnableAll(true);
                                StartCoroutine(ServerManager.serverManager.SendToServer("ResetTrade SESSIONID=" + LobbyManager.lobby.GetId() + "&pid1=" + id1 + "&pid2=" + id2, null));
                            }
                            gamePlayers[id2].currentRequest.isReset = false;
                        }
                        else
                        {
                            print("found the cancer");
                            // Poll only if it is the other player's turn.
                            if ((id1 == localPlayer.player.id && turn == id2) || (id2 == localPlayer.player.id && turn == id1))
                            {
                                //gamePlayers[id1].currentRequest.UpdateTradeRequest(ps, id1);
                                gamePlayers[id2].currentRequest.UpdateTradeRequest(ps, id2);
                                tradeMenu.Init();
                            }
                        }
                    }
                }
                
                break;
            /* fail case */
            case -1:
                break;
        }
    }

    public void UpdatePlayers()
    {
        for (int i = 0; i < LobbyManager.lobby.Count(); i++)
        {
            gamePlayers[i].player = LobbyManager.lobby.Get(i);
            if (i == localPlayer.player.id)
                continue;

            if (!ServerManager.serverManager.localGame)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j < 4)
                    {
                        gamePlayers[i].resourceBuildings[j].tier = gamePlayers[i].player.buildingTiers[j];
                    }
                    else
                    {
                        gamePlayers[i].unitBuildings[j - 4].tier = gamePlayers[i].player.buildingTiers[j - 4];
                    }
                }

                gamePlayers[i].unitMatrix = new int[3][];

                for (int j = 0; j < 3; j++)
                {
                    gamePlayers[i].unitMatrix[j] = new int[3];
                }

                foreach (string unit in gamePlayers[i].player.unitTiers)
                {
                    // First index is unit type, second is tier.
                    string[] split = unit.Split('_');

                    gamePlayers[i].unitMatrix[System.Int32.Parse(split[0])][System.Int32.Parse(split[0])]++;
                }
            }

            unitDisplays[i].UpdateDisplay();
        }
    }

    // Update resource count in UI
    public static void UpdateResourceCount() {
        for (int i = 0; i < cardGameManager.resourceText.Length; i++) {
            int rTenFold = localPlayer.GetResourceCount(i) * 10; // multiply by 10 to appear greater
            cardGameManager.resourceText[i].text = rTenFold.ToString();
        }
    }

    public static void UpdateUnitCount()
    {
        cardGameManager.unitDisplays[localPlayer.player.id].UpdateDisplay();
    }

    public static void Win(GamePlayer winner)
    {
        cardGameManager.countdownText.text = winner.player.playerName + " won!";
        cardGameManager.Invoke("ReturnToMain", 5f);
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
