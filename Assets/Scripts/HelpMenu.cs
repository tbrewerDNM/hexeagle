using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour
{
    int index;
    public Image dispImage;
    public Sprite[] dispSprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextImage()
    {
        index++;
        if (index >= dispSprites.Length)
        {
            index = 0;
            Show(false);
        }
        else
        {
            dispImage.sprite = dispSprites[index];
        }
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
