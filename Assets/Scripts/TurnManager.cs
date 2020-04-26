using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager turnManager;

    public int turn = 1;
    public int ready = 0;
    public PhaseType phase = PhaseType.TOWN;
    public Text turnText;
    public Text phaseText;
    public GameObject townUI;
    public GameObject overworldUI;
    public Button readyButton;

    // Start is called before the first frame update
    void Start() {
        turnManager = this;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ReadyUp()
    {
        if (ServerManager.serverManager.localGame)
        {
            NextPhase();
        }
        else
        {
            StartCoroutine(ServerManager.serverManager.SendToServer("ReadyUp SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + CardGameManager.localPlayer.player.id, null));
            ready = 1;
            readyButton.GetComponentInChildren<Text>().resizeTextForBestFit = true;
            readyButton.GetComponentInChildren<Text>().text = "Waiting for other players...";
            readyButton.interactable = false;
        }
    }

    public static void NextPhase() {
        turnManager.readyButton.GetComponentInChildren<Text>().resizeTextForBestFit = false;
        turnManager.readyButton.GetComponentInChildren<Text>().text = "End Turn";
        turnManager.readyButton.interactable = true;
        turnManager.ready = 0;
        switch (turnManager.phase) {
            case PhaseType.TOWN:
                foreach (PlayerHUD hud in CardGameManager.cardGameManager.playerHuds)
                {
                    hud.buttons[0].interactable = true;
                }
                turnManager.phaseText.text = "Overworld Phase";
                turnManager.townUI.SetActive(false);
                turnManager.overworldUI.SetActive(true);
                turnManager.phase = PhaseType.OVERWORLD;
                break;
            case PhaseType.OVERWORLD:
            default:
                turnManager.phaseText.text = "Town Phase";
                turnManager.townUI.SetActive(true);
                turnManager.overworldUI.SetActive(false);
                turnManager.phase = PhaseType.TOWN;
                NextTurn();
                if (ServerManager.serverManager.localGame)
                    AI();
                break;
        }
    }

    private static void AI()
    {
        foreach (GamePlayer gp in CardGameManager.cardGameManager.gamePlayers)
        {
            int unit;
            switch (gp.player.id)
            {
                // Lizard.
                case 3:
                    unit = 2;
                    if (gp.resourceBuildings[3].tier < 2)
                        if (gp.Purchase(Shop.GetCost(3, gp.resourceBuildings[3].tier)))
                            gp.resourceBuildings[3].tier++;
                    if (gp.unitBuildings[unit].tier < 2)
                        if (gp.Purchase(Shop.GetCost(unit, gp.unitBuildings[unit].tier)))
                            gp.unitBuildings[unit].tier++;
                    for (int i = 2; i >= 0; i--)
                    {
                        while (gp.Purchase(Shop.GetUnitCost(3, unit, i))) {
                            gp.unitMatrix[unit][i]++;
                        }
                    }
                    break;
                case 2:
                    unit = 0;
                    if (gp.resourceBuildings[3].tier < 2)
                        if (gp.Purchase(Shop.GetCost(3, gp.resourceBuildings[3].tier)))
                            gp.resourceBuildings[3].tier++;
                    if (gp.unitBuildings[unit].tier < 2)
                        if (gp.Purchase(Shop.GetCost(unit, gp.unitBuildings[unit].tier)))
                            gp.unitBuildings[unit].tier++;
                    for (int i = 2; i >= 0; i--)
                    {
                        while (gp.Purchase(Shop.GetUnitCost(2, unit, i)))
                        {
                            gp.unitMatrix[unit][i]++;
                        }
                    }
                    break;
                case 1:
                    unit = 1;
                    if (gp.resourceBuildings[3].tier < 2)
                        if (gp.Purchase(Shop.GetCost(3, gp.resourceBuildings[3].tier)))
                            gp.resourceBuildings[3].tier++;
                    if (gp.unitBuildings[unit].tier < 2)
                        if (gp.Purchase(Shop.GetCost(unit, gp.unitBuildings[unit].tier)))
                            gp.unitBuildings[unit].tier++;
                    for (int i = 2; i >= 0; i--)
                    {
                        while (gp.Purchase(Shop.GetUnitCost(1, unit, i)))
                        {
                            gp.unitMatrix[unit][i]++;
                        }
                    }
                    break;
                default:
                case 0:
                    break;
            }
        }
    }

    public static void NextTurn() {
        CardGameManager.localPlayer.UpdateResources();

        if (ServerManager.serverManager.localGame)
            foreach (GamePlayer gp in CardGameManager.cardGameManager.gamePlayers)
            {
                if (gp.player.id > 0)
                {
                    gp.UpdateResources();
                }
            }

        turnManager.turn++;
        turnManager.turnText.text = "Turn " + turnManager.turn;
    }
}
