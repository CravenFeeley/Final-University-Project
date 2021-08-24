using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseActions : MonoBehaviour 
{
    public string attackName;

    public float attackDamage; //base attack 10 + 2 x attack
    public float attackCost;
    public float manpowerGain;
    public int reinforcesLeft;
    public bool isDefending;
}
