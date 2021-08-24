using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class EnemySelectButton : MonoBehaviour
{

    public GameObject EnemyPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleState>().InputEnemySelect(EnemyPrefab);
    }

    public void HideSelector()
    {
        if (EnemyPrefab.GetComponent<AxisState>().MaskActive == true)
        {
            EnemyPrefab.transform.Find("MaskArrow").gameObject.SetActive(false);
        }
        else if (EnemyPrefab.GetComponent<AxisState>().MaskActive == false)
        {
            EnemyPrefab.transform.Find("Arrow").gameObject.SetActive(false);
        }
    }
    public void ShowSelector()
    {
        if (EnemyPrefab.GetComponent<AxisState>().MaskActive == true)
        {
            EnemyPrefab.transform.Find("MaskArrow").gameObject.SetActive(true);
        }
        else if (EnemyPrefab.GetComponent<AxisState>().MaskActive == false)
        {
            EnemyPrefab.transform.Find("Arrow").gameObject.SetActive(true);
        }
    }

}
