using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAllies : MonoBehaviour {

    public string name;
    public float cur_manpower;
    public float max_manpower;
    public int attack;
    public int defence;

    public UnitType FriendlyType;
    public List<BaseActions> actions = new List<BaseActions>();
}
public enum UnitType
{
    LIGHT,
    HEAVY,
    ARTILLERY
}
