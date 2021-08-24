﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class HandleTurn
{

    public string Attacker;//name of attacker
    public string Type;
    public GameObject AttackersGameObject;
    public GameObject AttackersTarget;

    //which attack is performed
    public BaseActions chosenAction;

}

