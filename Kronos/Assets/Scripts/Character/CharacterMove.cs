using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : Character
{
    float sturnTime;
    bool canMove;

    private void Start()
    {
        sturnTime = 0f;
        canMove = true;
    }

    private void Update()
    {
        sturnTime -= Time.deltaTime;
        if (sturnTime > 0f) canMove = false;
        else                canMove = true;


    }

    void BattleHit(float HitPower)
        /*
         * ���� �� ���ݴ����� �� �����ϴ� �Լ�
         * �ǰ� �ִϸ��̼��� ����ϰ�, HitPower�� ����Ͽ� �����ð����� ������ �Ұ���������.
         */
    {

    }






}
