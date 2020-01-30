using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    Vector3 targetPosition;
    // Start is called before the first frame update
    void Start() {
        targetPosition = this.transform.position;
    }

    // Update is called once per frame
    /*void Update() {
        float ms = 0.05f;
        Vector3 thisPos = this.transform.localPosition;

        if (Input.GetKey("up")) {
            this.transform.localPosition = new Vector3(thisPos.x,thisPos.y + ms,thisPos.z);
            print("up");
        } else if (Input.GetKey("down")) {
            print("down");
            this.transform.localPosition = new Vector3(thisPos.x, thisPos.y - ms, thisPos.z);
        } else if (Input.GetKey("right")) {
            print("right");
            this.transform.localPosition = new Vector3(thisPos.x + ms, thisPos.y, thisPos.z);
        } else if (Input.GetKey("left")) {
            print("left");
            this.transform.localPosition = new Vector3(thisPos.x - ms, thisPos.y, thisPos.z);
        }
    }*/

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            print(targetPosition);
        }

        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
    }
}
