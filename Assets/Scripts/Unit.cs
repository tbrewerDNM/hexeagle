using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    private static int[][] typeEffectiveness = new int[][] { new int[] { 1,0,2 }, new int[] { 2,1,0 }, new int[] { 1,2,1 } };

    public int tier;
    public int type;
    public int id;

    public Unit(int typ, int level)
    {
        type = typ;
        tier = level;
        id = Random.Range(0, 1000);
    }

    // Return outcome of the attack, including winner.
    // Value can be:
    // 0, this loses without a trade
    // 1, this draws with a trade
    // 2, this wins with a trade
    // 3, this loses with a trade
    // 4, this wins without a trade
    public int Attack(Unit foe)
    {
        // 0 means this loses type matchup, 1 means they are same type, 2 means this wins type matchup
        int typeEffective = typeEffectiveness[type][foe.type];

        switch (typeEffective)
        {
            case 0:
                if (tier - foe.tier == 2)
                    // Trade if this has a two tier diff.
                    return 1;
                else if (tier - foe.tier == 1)
                    // Lost trade at second tier.
                    return 3;
                else
                    return 0;
            case 1:
                // Always leads to neutral trade.
                return 1;
            case 2:
                if (foe.tier - tier == 2)
                    // Trade if this has a two tier diff.
                    return 1;
                else if (foe.tier - tier == 1)
                    // Lost trade at second tier.
                    return 2;
                else
                    return 4;
            default:
                return 1;
        }
    }

    public override string ToString()
    {
        /*string output = "";

        switch (type)
        {
            case 0:
                output = "infantry_attack_" + tier;
                break;
            case 1:
                output = "archer_attack_" + tier;
                break;
            case 2:
                output = "cavalry_attack_" + tier;
                break;
        }*/

        return ToIdle();
    }

    public string ToIdle()
    {
        string output = "";

        switch (type)
        {
            case 0:
                output = "infantry_idle_" + tier;
                break;
            case 1:
                output = "archer_idle_" + tier;
                break;
            case 2:
                output = "cavalry_idle_" + tier;
                break;
        }

        return output;
    }

    public string GetName()
    {
        string output = "";

        switch (type)
        {
            default:
            case 0:
                output = "Infantry Tier " + (tier + 1);
                break;
            case 1:
                output = "Archer Tier " + (tier + 1);
                break;
            case 2:
                output = "Cavalry Tier " + (tier + 1);
                break;
        }

        return output;
    }
}
