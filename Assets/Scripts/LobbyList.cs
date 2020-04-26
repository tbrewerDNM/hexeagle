using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyList : MonoBehaviour
{
    public static LobbyList lobbyList;
    public LobbySelect[] lobbyNames;
    public Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        lobbyList = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
