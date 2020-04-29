using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public static BuildingWindow buildingWindow;

    public Text dispName;
    public Text dispDesc;
    public Image dispImage;
    public Text[] dispCost;
    public Button upgradeButton;

    private Building building;

    // Start is called before the first frame update
    void Awake()
    {
        buildingWindow = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Building selected)
    {
        building = selected;
        dispDesc.text = building.buildingDesc;
        dispImage.sprite = AssetManager.assetManager.buildingImages[(int)selected.type];
        UpdateCosts();
    }

    private void UpdateCosts()
    {
        dispName.text = building.buildingName + " Tier " + building.tier;
        upgradeButton.interactable = true;

        int[] cost = Shop.GetCost(building);

        if (cost[0] < 0)
        {
            cost = Shop.GetCost((int)building.type, 2);
            upgradeButton.interactable = false;
        }

        for (int i = 0; i < 4; i++)
        {
            dispCost[i].text = "" + (cost[i] * 10);
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

    public void Upgrade()
    {
        building.Upgrade();
        UpdateCosts();
    }
}
