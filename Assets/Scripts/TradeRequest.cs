using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRequest
{
    public int pid;
    // The resources this player is willing to give.
    public int[] outResources;
    // The resources this player is willing to take.
    public int[] inResources;

    // Local turn value, different from server.
    public int turn = -1;
    public int accept1 = 0;
    public int accept2 = 0;
    public bool isReset;

    public TradeRequest(int pid2)
    {
        pid = pid2;

        outResources = new int[4];
        inResources = new int[4];
    }

    public void Reset()
    {
        outResources = new int[4];
        inResources = new int[4];
        accept1 = 0;
        accept2 = 0;
        turn = -1;
        isReset = true;
    }

    public static void PrintMatrix(int[] m)
    {
        string output = "";
        for (int i = 0; i < 4; i++)
        {
            output += m[i];
        }
        MonoBehaviour.print("Matrix " + output);
    }

    public void UpdateTradeRequest(SortedDictionary<string,string> ps, int pid2)
    {
        // Get player from param string.
        pid = pid2;
        isReset = false;

        // Init matricies.
        outResources = new int[4];
        inResources = new int[4];

        string[] rm1 = ps["rm" + CardGameManager.localPlayer.player.id].Split('-');
        string[] rm2 = ps["rm" + pid2].Split('-');

        accept1 = System.Int32.Parse(ps["accept1"]);
        accept2 = System.Int32.Parse(ps["accept2"]);

        for (int i = 0; i < 4; i++)
        {
            outResources[i] = System.Int32.Parse(rm1[i]);
            inResources[i] = System.Int32.Parse(rm2[i]);
        }

        PrintMatrix(outResources);
        PrintMatrix(inResources);
    }

    public override string ToString()
    {
        string output = "turn=" + turn + "&pid1=" + CardGameManager.localPlayer.player.id + "&pid2=" + pid + "&rm1=";
        string rm2 = "&rm2=";

        for (int i = 0; i < 4; i++)
        {
            output += outResources[i] + "-";
            rm2 += inResources[i] + "-";
        }

        output = output.Substring(0, output.Length - 1);
        rm2 = rm2.Substring(0, rm2.Length - 1);

        output += rm2;

        return output;
    }
}
