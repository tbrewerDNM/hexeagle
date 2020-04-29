using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string playerName;
    public int id = 0;
    private string hiddenId;
    public PlayerRace race = PlayerRace.HUMAN;
    public int[] buildingTiers { get; set; }
    public List<string> unitTiers = new List<string>();

    public Player(string paramstr) {
        buildingTiers = new int[7];
        string[] parameters = paramstr.Split('|');
        playerName = parameters[0].Split('-')[0];
        hiddenId = parameters[0].Split('-')[1];
        race = (PlayerRace)System.Int32.Parse(parameters[0].Split('-')[2]);

        string[] bts = parameters[1].Split('-');

        for (int i = 0; i < 7; i++)
        {
            buildingTiers[i] = System.Int32.Parse(bts[i]);
        }

        if (parameters.Length > 2)
        {
            string[] units = parameters[2].Split('-');
            unitTiers = new List<string>();

            for (int i = 0; i < units.Length; i++)
            {
                unitTiers.Add(units[i]);
            }
        }
    } 

    // serialize this object
    public override string ToString() {
        string output = playerName + "-" + hiddenId + "-" + (int)race + "|";

        for (int i = 0; i < 7; i++)
        {
            output += buildingTiers[i] + ((i >= 6) ? "" : "-");
        }

        if (unitTiers.Count > 0)
        {
            output += "|";

            foreach (string unit in unitTiers)
            {
                output += unit + "-";
            }

            output = output.Substring(0, output.Length - 1);
        }

        return output;
    }
}
