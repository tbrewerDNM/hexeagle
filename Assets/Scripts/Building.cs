using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private int _tier;
    public int tier
    {
        get
        {
            return _tier;
        }
        set
        {
            if (value - 1 < 0)
            {
                tierStars.sprite = null;
            }
            else
            {
                tierStars.sprite = AssetManager.assetManager.tierSprites[value - 1];
            }

            _tier = value;
        }
    }
    public ResourceType type;
    public string buildingName = "Default";
    public string buildingDesc = "Default";
    public SpriteRenderer tierStars;
    public int index;

    protected bool makesUnits;
    private int timer = 61;

    // Start is called before the first frame update
    void Start()
    {
        tier = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if (!CardGameManager.menuOpen && CardGameManager.localPlayer.player.id == index)
        {
            BuildingWindow.buildingWindow.Show(true);
            BuildingWindow.buildingWindow.Init(this);
        }
    }

    public void Upgrade()
    {
        if (tier < 3 && CardGameManager.localPlayer.Purchase(Shop.GetCost((int)type, tier)))
        {
            Debug.Log(CardGameManager.localPlayer + " purchased an upgrade!");
            tier += 1;
            CardGameManager.localPlayer.player.buildingTiers[(int)type] = tier;
            CardGameManager.UpdateGame();
        }
    }
}
