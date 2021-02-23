using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class PlayerStatus
{
    static public float timeMax;
    static public float timeRemain;

    static public int HPMax;
    static public int HP;
    static public float StaminaMax;
    static public float Stamina;

    static public int attack;
    static public int shield;
    static public int attackSpeed;
    static public int moveSpeed;
    static public int criticalProb;
    static public int criticalDamage;
    static public float avoidanceRate;
    static public float coolTimeDecreaseRate;

    // 모래시계 시스템
    static public int level;
    static public int remainPoint;
    static public float exp;
    static public float expMax;


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

        exp = 0;
        expMax = 100;
    }

    static public void Load()
        /*
         * 저장된 데이터를 불러온다.
         * - 저장하는 데이터는 찍은 스탯 정보와 레벨
         */
    {

    }

    static public void GainExp(float amount)
        /*
         * 몬스터를 잡을 때 적용
         */
    {
        exp += amount;

        // 레벨업 판정
        while (exp >= expMax)
        {
            exp -= expMax;
            level++;
            remainPoint++;
        }
    }

    static public void DeleteExp()
        /*
         * 사망시 경험치 삭제
         */
    {
        exp = 0;
    }


}
