using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour
{

    public static ServerManager serverManager;

    public int status = 0;
    public int ping = 0;
    public string form;
    public int lobbyIndex = 0;
    public bool localGame = false;
    public Button[] buttons;

    private bool pinging = false;
    private float pingCounter;

    // Start is called before the first frame update
    void Start()
    {
        serverManager = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (pinging)
        {
            pingCounter++;
        }
    }

    private void TurnOffButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    IEnumerator GetText() {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:5000/save");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    public void StartLocalGame()
    {
        localGame = true;
        CreateLobby();
    }

    public void CreateLobby() {
        TurnOffButtons();
        LobbyManager.lobby = new Lobby();
        LobbyManager.lobby.AddPlayer(PlayerWrapper.player);
        StartCoroutine(SendToServer("CreateLobby " + LobbyManager.lobby.ToString(), CreateLobbyCallback));
    }

    private void CreateLobbyCallback() {
        switch (status) {
            /* success case */
            case -1:
                localGame = true;
                SceneManager.LoadScene("Lobby");
                break;
            case 0:
                SceneManager.LoadScene("Lobby");
                break;
        }
    }

    public void GetLobby() {
        StartCoroutine(SendToServer("GetLobby index=" + lobbyIndex, GetLobbyCallback));
    }

    private void GetLobbyCallback() {
        if (form == "")
            return;

        switch (status) {
            /* success case */
            case 0:
                string[] lobbySplit = form.Split(' ');

                for (int i = 0; i < lobbySplit.Length; i++)
                {
                    Lobby newLobby = Lobby.FromString(ps_to_map(lobbySplit[i]));
                    print(newLobby);
                    LobbyList.lobbyList.lobbyNames[i].lobby = newLobby;
                    LobbyList.lobbyList.lobbyNames[i].UpdateText();
                }

                for (int i = 0; i < LobbyList.lobbyList.lobbyNames.Length; i++) {
                    if (i > lobbySplit.Length)
                    {
                        LobbyList.lobbyList.lobbyNames[i].gameObject.SetActive(false);
                    }
                }

                break;
            /* fail case */
            case -1:
                break;
        }
    }

    // paramstring to map
    public static SortedDictionary<String, String> ps_to_map(string ps) {
        SortedDictionary<String, String> sd = new SortedDictionary<string, string>();

        print(ps);

        if (ps == "")
            return null;

        string[] psSplit = ps.Split('&');

        foreach (string p in psSplit) {
            string[] tmp = p.Split('=');

            try
            {
                sd[tmp[0]] = tmp[1];
            }
            catch (Exception e)
            {
                sd.Add(tmp[0], tmp[1]);
            }
        }

        return sd;
    }

    public static void PingCallback()
    {
        serverManager.pinging = false;
        // pingCounter is in frames (1/ 60), need ms (1 / 1000).
        serverManager.ping = Mathf.CeilToInt((serverManager.pingCounter * 60) / 1000);
        serverManager.pingCounter = 0;
        CardGameManager.cardGameManager.pingText.text = serverManager.ping + " ms";
    }

    public static void Ping()
    {
        if (!serverManager.pinging)
        {
            print("pinging server");
            serverManager.pinging = true;
        }
    }

    public IEnumerator SendToServer(string data, System.Action callback) {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        print(data);

        formData.Add(new MultipartFormDataSection("arg " + data));

        UnityWebRequest www;

        if (localGame)
            www = UnityWebRequest.Post("http://127.0.0.1:5000/save", formData);
        else
            www = UnityWebRequest.Post("https://hexeagle-server.herokuapp.com/save", formData);
            //www = UnityWebRequest.Post("http://127.0.0.1:5000/save", formData);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            print(data);
            Debug.Log(www.error);
            status = -1;
            if (callback != null)
                callback();
        }
        else
        {
            form = www.downloadHandler.text;
            status = 0;
            if (callback != null)
                callback();
        }
    }

    public void NextLobby(int value)
    {
        lobbyIndex = Mathf.Clamp(lobbyIndex, 0, value);
    }
}
