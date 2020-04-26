using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager;

    public BattlerUI[] battlerUis;
    public UnitBattler[] battlers;
    public GameObject battleUi;
    public Text battleMessage;

    private GamePlayer attacker;
    private GamePlayer defender;
    private Queue<Unit> attackerQueue;
    private Queue<Unit> defenderQueue;
    private int attackerPower = 0;
    private int defenderPower = 0;
    private int attackerKills = 0;
    private int defenderKills = 0;
    private Unit currentAttacker;
    private Unit currentDefender;

    private int counter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        battleManager = this;
        Show(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CardGameManager.inCombat)
        {
            if (counter >= 240)
            {
                StartCoroutine(NextRound());
                counter = 0;
            }
            else
            {
                counter++;
            }
        }
    }

    public static void Show(bool show)
    {
        CardGameManager.cardGameManager.generalUI.SetActive(!show);
        battleManager.gameObject.SetActive(show);
        battleManager.battleUi.gameObject.SetActive(show);
    }

    public static void StartBattle()
    {
        Show(true);
        battleManager.StartCoroutine(ServerManager.serverManager.SendToServer("CombatBegan SESSIONID=" + LobbyManager.lobby.GetId(), null));
        CameraManager.HideAll();
        CameraManager.ShowBattleCamera(true);
        battleManager.NextRound();
    }

    private IEnumerator EndBattle(int flag)
    {
        switch (flag)
        {
            case 3:
                // Tie.
                battleMessage.text = defender.player.playerName + " won " + defenderKills + " Gold!";
                BattleCleanup();
                yield return new WaitForSeconds(2);
                Show(false);
                break;
            case 2:
                // Defender wins.
                if (CardGameManager.localPlayer.player.id == defender.player.id)
                {
                    CardGameManager.localPlayer.AddGold(defenderKills);
                }
                battleMessage.text = defender.player.playerName + " won " + defenderKills + " Gold!";
                BattleCleanup();
                yield return new WaitForSeconds(2);
                Show(false);
                break;
            case 1:
                // Attacker wins.
                if (CardGameManager.localPlayer.player.id == attacker.player.id)
                {
                    CardGameManager.localPlayer.AddGold(attackerKills);
                }
                battleMessage.text = attacker.player.playerName + " won " + attackerKills + " Gold!";
                BattleCleanup();
                yield return new WaitForSeconds(2);
                Show(false);
                break;
            default:
            case 0:
                yield return new WaitForSeconds(0);
                break;
        }
    }

    private void BattleCleanup()
    {
        UpdateUnits();
        CardGameManager.inCombat = false;
        CardGameManager.cardGameManager.countdownText.text = "";
        CameraManager.ShowBattleCamera(false);
        CameraManager.cameraManager.SetActive(CardGameManager.localPlayer.player.id);
    }

    public void InitBattle(GamePlayer player1, GamePlayer player2)
    {
        battleManager.attacker = player1;
        battleManager.defender = player2;

        List<Unit> attackerUnits = new List<Unit>();
        List<Unit> defenderUnits = new List<Unit>();
        attackerQueue = new Queue<Unit>();
        defenderQueue = new Queue<Unit>();
        attackerPower = 0;
        defenderPower = 0;
        attackerKills = 0;
        defenderKills = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < player1.unitMatrix[i][j]; k++)
                {
                    attackerUnits.Add(new Unit(i, j));
                    attackerPower += j + 1;
                }
                for (int k = 0; k < player2.unitMatrix[i][j]; k++)
                {
                    defenderUnits.Add(new Unit(i, j));
                    defenderPower += j + 1;
                }
            }
        }

        battlerUis[0].Init(player1.player, attackerPower);
        battlerUis[1].Init(player2.player, defenderPower);

        // Randomize each one.
        attackerUnits.Sort(delegate (Unit u1, Unit u2) { return u1.id.CompareTo(u2.id); });
        defenderUnits.Sort(delegate (Unit u1, Unit u2) { return u1.id.CompareTo(u2.id); });

        // Enqueue each in their respected queues.
        foreach (Unit unit in attackerUnits)
        {
            attackerQueue.Enqueue(unit);
        }

        foreach (Unit unit in defenderUnits)
        {
            defenderQueue.Enqueue(unit);
        }
    }

    // Return outcome of the attack, including winner.
    // Value can be:
    // 0, this loses without a trade
    // 1, this draws with a trade
    // 2, this wins with a trade
    // 3, this loses with a trade
    // 4, this wins without a trade
    public IEnumerator NextRound()
    {
        if (currentAttacker == null)
        {
            currentAttacker = attackerQueue.Dequeue();
            battlers[0].gameObject.SetActive(true);
            battlers[0].GetComponent<Animator>().Play(currentAttacker.ToIdle());
        }

        if (currentDefender == null)
        {
            currentDefender = defenderQueue.Dequeue();
            battlers[1].gameObject.SetActive(true);
            battlers[1].GetComponent<Animator>().Play(currentDefender.ToIdle());
        }

        battleMessage.text = currentAttacker.GetName() + " versus " + currentDefender.GetName();

        yield return new WaitForSeconds(2f);

        switch (currentAttacker.Attack(currentDefender))
        {
            case 0:
                battlers[1].GetComponent<Animator>().Play(currentDefender.ToString());
                battlers[0].gameObject.SetActive(false);
                defenderKills++;
                attackerPower -= currentAttacker.tier;
                currentAttacker = null;
                battleMessage.text = currentDefender.GetName() + " wins!";
                break;
            case 1:
                print(currentDefender.ToString());
                battlers[1].GetComponent<Animator>().Play(currentDefender.ToString());
                battlers[0].GetComponent<Animator>().Play(currentAttacker.ToString());

                currentAttacker.tier--;
                attackerPower--;

                battleMessage.text = "It's a draw!";

                if (currentAttacker.tier < 0)
                {
                    battlers[0].gameObject.SetActive(false);
                    defenderKills++;
                    currentAttacker = null;
                }
                else
                {
                    battlers[0].GetComponent<Animator>().Play(currentAttacker.ToIdle());
                }

                currentDefender.tier--;
                defenderPower--;

                if (currentDefender.tier < 0)
                {
                    battlers[1].gameObject.SetActive(false);
                    attackerKills++;
                    currentDefender = null;
                }
                else
                {
                    battlers[1].GetComponent<Animator>().Play(currentDefender.ToIdle());
                }

                break;
            case 2:
                battlers[1].GetComponent<Animator>().Play(currentDefender.ToString());
                battlers[0].GetComponent<Animator>().Play(currentAttacker.ToString());

                currentAttacker.tier--;
                attackerPower--;

                if (currentAttacker.tier < 0)
                {
                    battlers[0].gameObject.SetActive(false);
                    defenderKills++;
                    currentAttacker = null;
                }
                else
                {
                    battlers[0].GetComponent<Animator>().Play(currentAttacker.ToIdle());
                }

                battlers[1].gameObject.SetActive(false);
                defenderPower -= currentDefender.tier;
                attackerKills++;
                currentDefender = null;

                battleMessage.text = currentAttacker.GetName() + " barely wins!";

                break;
            case 3:
                battlers[1].GetComponent<Animator>().Play(currentDefender.ToString());
                battlers[0].GetComponent<Animator>().Play(currentAttacker.ToString());

                attackerPower -= currentAttacker.tier;
                battlers[0].gameObject.SetActive(false);
                defenderKills++;
                currentAttacker = null;

                currentDefender.tier--;
                defenderPower--;

                if (currentDefender.tier < 0)
                {
                    battlers[1].gameObject.SetActive(false);
                    attackerKills++;
                    currentDefender = null;
                }
                else
                {
                    battlers[1].GetComponent<Animator>().Play(currentDefender.ToIdle());
                }

                battleMessage.text = currentDefender.GetName() + " barely wins!";

                break;
            case 4:
                battlers[0].GetComponent<Animator>().Play(attacker.ToString());
                battlers[1].gameObject.SetActive(false);
                attackerKills++;
                defenderPower -= currentDefender.tier;
                currentDefender = null;
                battleMessage.text = currentAttacker.GetName() + " wins!";
                break;
        }

        yield return new WaitForSeconds(2f);

        if (attackerQueue.Count == 0 && defenderQueue.Count == 0)
        {
            StartCoroutine(EndBattle(3));
        }
        else if (attackerQueue.Count == 0)
        {
            StartCoroutine(EndBattle(2));
        }
        else if (defenderQueue.Count == 0)
        {
            StartCoroutine(EndBattle(1));
        }
        else
        {
            StartCoroutine(EndBattle(0));
        }
    }

    private void UpdateUnits()
    {
        int[][] newUM = new int[][] { new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 } };
        List<string> units = new List<string>();
        int id = CardGameManager.localPlayer.player.id;

        if (attacker.player.id == id || defender.player.id == id)
        {
            Queue<Unit> unitQueue = new Queue<Unit>();

            if (attacker.player.id == id && attackerQueue.Count > 0)
            {
                unitQueue = attackerQueue;
                unitQueue.Enqueue(currentAttacker);
            }
            else if (defender.player.id == id && defenderQueue.Count > 0)
            {
                unitQueue = defenderQueue;
                unitQueue.Enqueue(currentDefender);
            }

            print(unitQueue.Count);

            while (unitQueue.Count != 0)
            {
                print("hello");
                Unit unit = unitQueue.Dequeue();
                newUM[unit.type][unit.tier]++;
                units.Add(unit.type + "_" + unit.tier); 
            }

            CardGameManager.localPlayer.unitMatrix = newUM;
            CardGameManager.localPlayer.player.unitTiers = units;
            CardGameManager.cardGameManager.unitDisplays[CardGameManager.localPlayer.player.id].UpdateDisplay();
            StartCoroutine(ServerManager.serverManager.SendToServer("UpdatePlayer SESSIONID=" + LobbyManager.lobby.GetId() + "&id=" + CardGameManager.localPlayer.player.id
                                                                                                  + "&player="
                                                                                                  + CardGameManager.localPlayer.player.ToString(), null));
        }

    }
}
