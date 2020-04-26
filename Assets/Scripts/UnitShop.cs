using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitShop : MonoBehaviour
{
    public static UnitShop unitShop;
    public static int currentTier = 0;

    public Text[] tierLevels;
    public Text[] unitPowers;
    public Text[] unitAmounts;
    public Text[] infantryCosts;
    public Text[] archerCosts;
    public Text[] cavalryCosts;
    public Image[] unitImages;

    // Start is called before the first frame update
    void Start()
    {
        unitShop = this;
        UpdateText();
        Show(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseUnit(int unitType)
    {
        if (CardGameManager.localPlayer.unitBuildings[unitType].tier > currentTier 
            && CardGameManager.localPlayer.Purchase(Shop.GetUnitCost(unitType, currentTier)))
        {
            CardGameManager.localPlayer.unitMatrix[unitType][currentTier]++;
            CardGameManager.localPlayer.player.unitTiers.Add(unitType + "_" + currentTier);
            CardGameManager.UpdateUnitCount();
            CardGameManager.UpdateGame();
            UpdateText();
        }
    }

    public void SubTier()
    {
        currentTier = Mathf.Clamp(currentTier - 1, 0, 2);
        UpdateText();
    }

    public void AddTier()
    {
        currentTier = Mathf.Clamp(currentTier + 1, 0, 2);
        UpdateText();
    }

    public void UpdateText()
    {
        Shop shop = FindObjectOfType<Shop>();

        foreach (Text text in tierLevels)
        {
            text.text = "Tier " + (currentTier + 1);
        }

        foreach (Text text in unitPowers)
        {
            text.text = "Unit Power: " + (currentTier + 1);
        }

        for (int i = 0; i < 4; i++)
        {
            int tmp = Shop.GetUnitCost(0, currentTier)[i] * 10;
            infantryCosts[i].text = tmp.ToString();
            tmp = Shop.GetUnitCost(1, currentTier)[i] * 10;
            archerCosts[i].text = tmp.ToString();
            tmp = Shop.GetUnitCost(2, currentTier)[i] * 10;
            cavalryCosts[i].text = tmp.ToString();
        }

        for (int i = 0; i < 3; i++) {
            unitAmounts[i].text = "Count: " + CardGameManager.localPlayer.unitMatrix[i][currentTier].ToString();

            switch (currentTier)
            {
                case 0:
                    unitImages[i].color = new Color(1, 1, 1, 1);
                    break;
                case 1:
                    unitImages[i].color = new Color(1, 0, 0, 1);
                    break;
                case 2:
                    unitImages[i].color = new Color(0, 1, 1, 1);
                    break;
            }
        }
    }

    public void Show(bool show)
    {
        if (show && CardGameManager.menuOpen)
        {

        }
        else
        {
            CardGameManager.menuOpen = show;
            gameObject.SetActive(show);
        }
    }
}
