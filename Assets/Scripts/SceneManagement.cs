using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement sceneManager;

    public GameObject closeMenuPrefab;

    private GameObject closeMenu;

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (closeMenu == null)
            {
                OpenExitMenu();
            }
            else
            {
                CloseExitMenu();
            }
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenExitMenu()
    {
        closeMenu = Instantiate(closeMenuPrefab, FindObjectOfType<Canvas>().transform);
    }

    public void CloseExitMenu()
    {
        Destroy(closeMenu);
        closeMenu = null;
    }
}
