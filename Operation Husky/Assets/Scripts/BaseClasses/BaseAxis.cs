using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAxis : MonoBehaviour {

    public string name;
    public float cur_manpower;
    public float max_manpower;
    public int attack;
    public int defence;

    public UnitType EnemyType;
    public List<BaseActions> actions = new List<BaseActions>();
}
