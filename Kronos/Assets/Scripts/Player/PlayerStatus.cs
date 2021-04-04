using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerStatus : Status
{
    public float timeMax;
    public float timeRemain;


    // �𷡽ð� �ý���
    public int level;
    public int remainPoint;
    public float exp;
    public float expMax;


    // �ð� �帧 on
    bool timeFlowOn;

    public bool isDefending; // ��� ��������
    public float noDefendTime; // �и� ���� �Ǵ��� ���� 


    private void Awake()
    {
        init();
        timeFlowOn = true;
        isDefending = false;

        // ���̺� ���Ͽ��� ������ �ҷ���
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
         * ����� �����͸� �ҷ��´�.
         * - �����ϴ� �����ʹ� ���� ���� ������ ����
         */
    {

    }

    public void GainExp(float amount)
        /*
         * ���͸� ���� �� ����
         */
    {
        exp += amount;

        // ������ ����
        while (exp >= expMax)
        {
            exp -= expMax;
            level++;
            remainPoint++;
        }
    }

    public void DeleteExp()
        /*
         * ����� ����ġ ����
         */
    {
        exp = 0;
    }


}
