using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager assetManager;
    public Sprite[] tierSprites;
    public Sprite[] raceFaces;
    public Sprite[] tradeImages;
    public Sprite[] buildingImages;
    public Sprite[] unitImages;
    public Sprite[] colorBgs;

    // Start is called before the first frame update
    void Awake()
    {
        assetManager = this;
        DontDestroyOnLoad(assetManager.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
