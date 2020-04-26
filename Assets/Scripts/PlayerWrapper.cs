using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWrapper : MonoBehaviour
{
    public string playerName;
    public static Player player;

    public InputField input;

    // Start is called before the first frame update
    void Start()
    {
        player = new Player(playerName + "-0|1-1-1-1-0-0-0");
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeName()
    {
        player = new Player(input.text + "-0|1-1-1-1-0-0-0");
    }
}
