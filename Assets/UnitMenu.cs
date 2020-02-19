using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenu : MonoBehaviour
{

    public RectTransform menu;
    public Slider slider;

    private float end;

    // Start is called before the first frame update
    void Start() {
        // 140 is the height of each child
        float y = menu.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        end = -(y / 2) + (menu.childCount - 3) * y;
    }

    // Update is called once per frame
    void Update() {
        RectTransform rt = menu.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, slider.value * end, rt.localPosition.z);
    }
}
