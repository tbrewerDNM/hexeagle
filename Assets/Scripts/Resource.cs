using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public int workers = 0;
    public int wood = 0;
    public Image progressBar;
    public Text woodCount;
    public Text workerCount;

    private float baseWidth;
    private Vector3 basePos;
    private float percent = 1;

    // Start is called before the first frame update
    void Start() {
        this.basePos = progressBar.rectTransform.localPosition;
        this.baseWidth = progressBar.rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update() {
        if (percent <= 0) {
            percent = 1;
            wood++;
            woodCount.text = "Wood: " + wood;
        } else {
            percent -= 0.01f * workers;
        }
        changeBar(percent);
    }

    public void increaseWorkers() {
        this.workers++;
        workerCount.text = "Woodcutters: " + workers;
    }

    public void showMenu(GameObject menu) {
        menu.SetActive(true);
    }

    public void hideMenu(GameObject menu) {
        menu.SetActive(false);
    }

    private void changeBar(float percent) {
        float newWidth = baseWidth * percent;
        progressBar.rectTransform.localPosition = new Vector3(basePos.x - (baseWidth - newWidth) / 2, basePos.y, basePos.z);
        progressBar.rectTransform.sizeDelta = new Vector2(newWidth, progressBar.rectTransform.sizeDelta.y);
    }
}
