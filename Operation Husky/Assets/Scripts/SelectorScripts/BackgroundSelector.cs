using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSelector : MonoBehaviour
{

    private BattleState BSM;
    public GameObject Beach;
    public GameObject Grassland;
    //public GameObject Artillery;

    private int BackgroundChoice;

    void Start()
    {
        Beach.SetActive(false);
        Grassland.SetActive(false);

        BSM = GameObject.Find("BattleManager").GetComponent<BattleState>();

        CreateBackground();
    }

    public void CreateBackground()
    {
        BackgroundChoice = Random.Range(0, 2);

        if (BackgroundChoice == 0)
        {
            Beach.SetActive(true);
        }
        else if (BackgroundChoice == 1)
        {
            Grassland.SetActive(true);
        }
        /*else if (UnitChoice == 2)
        {
            Heavy.SetActive(true);
        }*/
    }
}
