using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Image bg;
    public Vector3 finalPos;
    public Vector3 finalScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bg.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(bg.GetComponent<RectTransform>().localPosition, finalPos, 1f);
        bg.GetComponent<RectTransform>().localScale = Vector3.MoveTowards(bg.GetComponent<RectTransform>().localScale, finalScale, 0.0036f);
    }
}
