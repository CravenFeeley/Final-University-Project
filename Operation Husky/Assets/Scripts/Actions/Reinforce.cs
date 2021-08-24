using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforce : BaseActions {

    float curMan;
    float maxMan;
    float manGain;

    private void Start()
    {
        curMan = GetComponent<AlliesState>().Allies.cur_manpower;
        maxMan = GetComponent<AlliesState>().Allies.max_manpower;
        
    }

    public Reinforce()
    {

        attackName = "Reinforce";
        manpowerGain = curMan + maxMan - curMan;


    }

}
