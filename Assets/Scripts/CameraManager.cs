using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager cameraManager;

    public Camera[] cameras;
    public Camera battleCamera;
    public Light[] lights;

    // Start is called before the first frame update
    void Start()
    {
        cameraManager = this;
        SetActive(PlayerWrapper.player.id);
    }

    public void SetActive(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == index)
            {
                cameras[i].gameObject.SetActive(true);
                lights[i].gameObject.SetActive(true);
            }
            else
            {
                cameras[i].gameObject.SetActive(false);
                lights[i].gameObject.SetActive(false);
            }
        }
    }

    public static void HideAll()
    {
        foreach (Camera camera in cameraManager.cameras)
        {
            camera.gameObject.SetActive(false);
        }
    }

    public static void ShowBattleCamera(bool show)
    {
        BattleManager.Show(show);
        cameraManager.battleCamera.gameObject.SetActive(show);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
