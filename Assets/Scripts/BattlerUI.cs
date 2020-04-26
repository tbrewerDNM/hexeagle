using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerUI : MonoBehaviour
{
    public Image playerImage;
    public Text playerName;
    public Text playerPower;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(Player player, int power)
    {
        playerImage.sprite = AssetManager.assetManager.raceFaces[(int)player.race];
        playerName.text = player.playerName;
        playerPower.text = "Unit Power: " + power;
    }

    public void UpdatePower(int power)
    {
        playerPower.text = "Unit Power: " + power;
    }
}
