using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitSelect : MonoBehaviour
{

    private BattleState BSM;
    public GameObject Light;
    public GameObject Heavy;
    public GameObject Artillery;

    private int UnitChoice;

    void Start()
    {
        Artillery.SetActive(false);
        Light.SetActive(false);
        Heavy.SetActive(false);

        BSM = GameObject.Find("BattleManager").GetComponent<BattleState>();

        CreateEnemyUnit();
    }

    public void CreateEnemyUnit ()
    {
        UnitChoice = Random.Range(0, 3);

        if (UnitChoice == 0)
        {
            Light.SetActive(true);
            //BSM.AxisInBattle.Add(Light);
        }
        else if (UnitChoice == 1)
        {
            Artillery.SetActive(true);
            //BSM.AxisInBattle.Add(Artillery);
        }
        else if (UnitChoice == 2)
        {
            Heavy.SetActive(true);
            //BSM.AxisInBattle.Add(Heavy);
        }
    }
}
