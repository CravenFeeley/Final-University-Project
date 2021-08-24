using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass : MonoBehaviour
{


    public string name;

    public int manpower;

    public int attack;
    public int defence;

    public enum Type
    {
        LIGHT,
        HEAVY,
        ARTILLERY
    }

    public Type UnitType;
}
