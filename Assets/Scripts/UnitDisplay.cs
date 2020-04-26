using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDisplay : MonoBehaviour
{
    private List<GameObject> units;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void UpdateDisplay()
    {
        int count = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string unitType = null;

                switch (i)
                {
                    case 0:
                        unitType = "infantry";
                        break;
                    case 1:
                        unitType = "archer";
                        break;
                    case 2:
                        unitType = "cavalry";
                        break;
                }

                for (int k = 0; k < CardGameManager.cardGameManager.gamePlayers[index].unitMatrix[i][j]; k++)
                {
                    transform.GetChild(count).gameObject.SetActive(true);
                    transform.GetChild(count).GetComponent<Animator>().Play(unitType + "_idle_" + j);
                    count++;
                }
            }
        }

        for (int i = count; i < 30; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
