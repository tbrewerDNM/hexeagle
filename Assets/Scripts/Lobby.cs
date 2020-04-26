using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby
{
    public static Player localPlayer;
    private List<Player> lobbyPlayers;
    private string sessionId;
    private bool inGame;

    public Lobby() {
        this.lobbyPlayers = new List<Player>();
        this.sessionId = GenerateRandomString(32);
        this.inGame = false;
    }

    public Lobby(string id) {
        this.lobbyPlayers = new List<Player>();
        this.sessionId = id;
    }

    public void UpdateLobbyPlayers(SortedDictionary<string, string> serial)
    {
        int size = System.Int32.Parse(serial["size"]);
        lobbyPlayers = new List<Player>();

        for (int i = 0; i < size; i++)
        {
            if (i == PlayerWrapper.player.id)
            {
                lobbyPlayers.Add(PlayerWrapper.player);
            }
            else
            {
                Player newPlayer = new Player(serial["player_" + i]);
                newPlayer.id = i;
                this.AddPlayer(newPlayer);
            }
        }
    }

    public static Lobby FromString(SortedDictionary<string, string> serial) {
        Lobby newLobby = new Lobby(serial["SESSIONID"]);

        newLobby.inGame = (serial["ingame"] == "1");

        newLobby.UpdateLobbyPlayers(serial);

        return newLobby;
    }

    private static string GenerateRandomString(int length)
    {
        string characters = "abcdefghijklmnopqrstuvqxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string rs = "";

        for (int i = 0; i < length; i++)
        {
            int rand = Random.Range(0, characters.Length);
            rs += characters[rand];
        }

        return rs;
    }

    public bool IsInGame()
    {
        return this.inGame;
    }

    public int FirstAvailable()
    {
        List<int> available = new List<int> { 0, 1, 2, 3 };

        foreach (Player player in lobbyPlayers)
        {
            available.Remove(player.id);
        }

        return available[0];
    }

    public void AddPlayer(Player player) {
        this.lobbyPlayers.Add(player);
    }

    public Player Get(int index) {
        return this.lobbyPlayers[index];
    }

    // check how many players are in the lobby
    public int Count() {
        return this.lobbyPlayers.Count;
    }

    // check if the lobby is full
    public bool isFull() {
        return this.Count() >= 4;
    }

    public string GetId()
    {
        return this.sessionId;
    }

    public string GetName()
    {
        return lobbyPlayers[0].playerName + "'s lobby";
    }

    // serialize this object
    override public string ToString() {
        string output = "SESSIONID=" + sessionId;

        for (int i = 0; i < lobbyPlayers.Count; i++) {
            output += "&player_" + i + "=" + lobbyPlayers[i].ToString();
        }

        return output;
    }
}
