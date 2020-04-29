using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Dropdown[] dropdowns;
    public Image traderImage;
    public Image acceptImage;
    public Text traderName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(Player player, int accept)
    {
        traderImage.sprite = AssetManager.assetManager.raceFaces[(int)player.race];
        traderName.text = player.playerName;
        acceptImage.sprite = AssetManager.assetManager.tradeImages[accept];
    }

    public void UpdateUI(int[] resources)
    {
        for (int i = 0; i < 4; i++)
        {
            dropdowns[i].value = resources[i];
        }
    }

    public void EnableAll(bool enable=true)
    {
        foreach (Dropdown dropdown in dropdowns)
        {
            dropdown.interactable = enable;
        }
    }
}
