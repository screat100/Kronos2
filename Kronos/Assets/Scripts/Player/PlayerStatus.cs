using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerStatus : Status
{
    public float timeMax;
    public float timeRemain;


    // 모래시계 시스템
    public int level;
    public int remainPoint;
    public float exp;
    public float expMax;


    // 시간 흐름 on
    bool timeFlowOn;

    public bool isDefending; // 방어 상태인지
    public float noDefendTime; // 패링 관련 판단을 위한 


    private void Awake()
    {
        init();
        timeFlowOn = true;
        isDefending = false;

        // 세이브 파일에서 데이터 불러옴
        Load();
    }

    private void Update()
    {
        if (timeFlowOn)
            timeRemain -= Time.deltaTime;

        if (!isDefending)
        {
            noDefendTime += Time.deltaTime;

            if (noDefendTime >= 3)
            {
                Stamina += 10 * Time.deltaTime;
            }
        }
    }

    public void init()
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

    public void Load()
        /*
         * 저장된 데이터를 불러온다.
         * - 저장하는 데이터는 찍은 스탯 정보와 레벨
         */
    {

    }

    public void GainExp(float amount)
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

    public void DeleteExp()
        /*
         * 사망시 경험치 삭제
         */
    {
        exp = 0;
    }


}
