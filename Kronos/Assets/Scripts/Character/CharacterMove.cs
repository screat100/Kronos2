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
         * 전투 중 공격당했을 때 실행하는 함수
         * 피격 애니메이션을 재생하고, HitPower에 비례하여 일정시간동안 조작이 불가능해진다.
         */
    {

    }






}
