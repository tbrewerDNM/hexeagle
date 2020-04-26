using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop shop;

    // Follows the format iron|wood|food|gold. 
    // 3 Tiers each.
    public string[] mineCosts;
    public string[] woodsCosts;
    public string[] farmCosts;
    public string[] hallCosts;
    public string[] barracksCosts;
    public string[] archeryCosts;
    public string[] stablesCosts;

    // Follows the format iron|wood|food|gold-iron|wood|food|gold...
    // 3 Tiers each.
    public string[] infantryCosts;
    public string[] archerCosts;
    public string[] cavalryCosts;

    private static int[][][] upgradeCosts = null;
    private static int[][][][] unitCosts = null;

    // Start is called before the first frame update
    void Awake()
    {
        shop = this;

        // Init upgrade cost table.
        if (upgradeCosts == null)
        {
            // Seven Buildings.
            upgradeCosts = new int[7][][];

            for (int i = 0; i < 7; i++)
            {
                // 3 Tiers each.
                upgradeCosts[i] = new int[3][];

                string[] resourceBuilding = null;

                switch (i) {
                    case 0:
                        resourceBuilding = mineCosts;
                        break;
                    case 1:
                        resourceBuilding = woodsCosts;
                        break;
                    case 2:
                        resourceBuilding = farmCosts;
                        break;
                    case 3:
                        resourceBuilding = hallCosts;
                        break;
                    case 4:
                        resourceBuilding = barracksCosts;
                        break;
                    case 5:
                        resourceBuilding = archeryCosts;
                        break;
                    case 6:
                        resourceBuilding = stablesCosts;
                        break;
                }

                for (int j = 0; j < 3; j++)
                {
                    upgradeCosts[i][j] = CostToIntArr(resourceBuilding[j]);
                }
            }
        }

        InitUnitCost();
    }

    // Convert cost string to int array.
    public static int[] CostToIntArr(string cs)
    {
        int[] output = new int[4];
        string[] split = cs.Split('|');

        for (int i = 0; i < split.Length; i++) {
            output[i] = System.Int32.Parse(split[i]);
        }

        return output;
    }

    public void InitUnitCost()
    {
        // Four races.
        unitCosts = new int[4][][][];

        for (int k = 0; k < 4; k++)
        {
            unitCosts[k] = new int[3][][];
            // 3 unit types.
            for (int i = 0; i < 3; i++)
            {
                string[] unitCost = null;
                unitCosts[k][i] = new int[3][];

                switch (i)
                {
                    case 0:
                        unitCost = infantryCosts;
                        break;
                    case 1:
                        unitCost = archerCosts;
                        break;
                    case 2:
                        unitCost = cavalryCosts;
                        break;
                }

                // 3 Tiers each.
                for (int j = 0; j < 3; j++)
                {
                    // Array of costs for each race.
                    string[] costs = unitCost[j].Split('-');

                    unitCosts[k][i][j] = CostToIntArr(costs[k]);
                }
            }
        }
    }

    // Get the cost of a specific upgrade.
    public static int[] GetCost(int type, int tier)
    {
        return upgradeCosts[type][tier];
    }

    public static int[] GetUnitCost(int type, int tier)
    {
        return unitCosts[(int)CardGameManager.localPlayer.player.race][type][tier];
    }

    public static int[] GetUnitCost(int race, int type, int tier)
    {
        print(race);
        print(type);
        print(tier);
        return unitCosts[race][type][tier];
    }

    public static int[] GetCost(Building building)
    {
        if (building.tier >= 3)
            return new int[] { -1, -1, -1, -1 };
        else
            return upgradeCosts[(int)building.type][building.tier];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
