using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    private int[] resourceMatrix = new int[4];
    public int[][] unitMatrix = new int[3][];
    public ResourceBuilding[] resourceBuildings;
    public UnitBuilding[] unitBuildings;
    public Player player;
    public TradeRequest currentRequest;

    // Resources held in a buffer when making a trade.
    public int[] resourceBuffer = new int[4];

    // Start is called before the first frame update
    void Awake()
    {
        int defaultValue = 3;

        for (int i = 0; i < 4; i++)
        {
            resourceMatrix[i] = defaultValue;
        }

        for (int i = 0; i < 3; i++)
        {
            unitMatrix[i] = new int[] { 0, 0, 0};
        }

        UpdateResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetResourceCount(int rt)
    {
        return resourceMatrix[rt];
    }

    public bool CanAfford(int[] cost)
    {
        for (int i = 0; i < 4; i++)
        {
            if (resourceMatrix[i] < cost[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool Purchase(int[] cost)
    {
        // Double check for robustness.
        if (CanAfford(cost))
        {
            for (int i = 0; i < 4; i++) {
                resourceMatrix[i] -= cost[i];
            }

            // Update UI.
            CardGameManager.UpdateResourceCount();

            // Purchase successful.
            return true;
        }
        else
        {
            // Purchase unsuccessful.
            print("Purchase unsucessful");
            return false;
        }
    }

    public void Obtain(int[] cost)
    {
        for (int i = 0; i < 4; i++)
        {
            resourceMatrix[i] += cost[i];
        }

        // Update UI.
        CardGameManager.UpdateResourceCount();
    }

    public void UpdateResources() {
        foreach (ResourceBuilding rb in resourceBuildings)
        {
            resourceMatrix[(int)rb.type] += rb.tier;
        }

        if (!ServerManager.serverManager.localGame || player.id == 0)
            CardGameManager.UpdateResourceCount();

        if (resourceMatrix[3] >= 30)
        {
            CardGameManager.Win(this);
        }
    }

    public void AddGold(int value)
    {
        resourceMatrix[3] += value;
    }

    public int[] GetResourceMatrix()
    {
        return resourceMatrix;
    }
}
