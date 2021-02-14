using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatus
{
    static public float timeMax;
    static public float timeRemain;

    static public int HPMax;
    static public int HP;
    static public int StaminaMax;
    static public int Stamina;

    static public int attack;
    static public int shield;
    static public int attackSpeed;
    static public int moveSpeed;
    static public int criticalProb;
    static public int criticalDamage;
    static public float avoidanceRate;
    static public float coolTimeDecreaseRate;

    static public void init()
    {
        timeMax = 30;
        timeRemain = 30;

        HPMax = 100;
        HP = 100;
        StaminaMax = 100;
        Stamina = 100;
        attack = 10;
        shield = 0;
        attackSpeed = 100;
        moveSpeed = 100;
        criticalProb = 0;
        criticalDamage = 150;
        avoidanceRate = 0;
        coolTimeDecreaseRate = 0;
    }


}
