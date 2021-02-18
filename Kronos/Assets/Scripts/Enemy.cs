using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int MonsterMaxHP = 100;
    public int MonsterHP;
    public float MonsterShield;



    void Start()
    {
        MonsterHP = MonsterMaxHP; //초기 hp 설정
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "PlayerEffect")
        {
            float damage = other.GetComponent<PlayerDamage>().CalculatedDamage();
            ApplyDamage(damage);
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerMove>().AttackRigidy(0.05f));
        }
    }


    public void ApplyDamage(float damage)
        /*
         * 몬스터가 받는 데미지 적용할 함수.
         */
    {
        float guardRate = 50f * Mathf.Log(MonsterShield + 10) - 50f;
        damage *= (guardRate / 100);

        // 최종 피해량을 체력에 적용
        MonsterHP -= (int)damage;
        Debug.Log($"Monster was hitted : Damage = { (int)damage }");
        Debug.Log($"Left Monster HP = { MonsterHP }");

        // 사망 판정 검사
        if (MonsterHP <= 0) { MonsterDie(); }
        
    }

    void MonsterDie()
        /*
         * 사망판정
         * - 애니메이션 재생
         * - 사운드 재생
         * - 콜라이전 및 리지드바디 비활성화
         * - 일정시간 후 시체 삭제
         * - 기타...?
         */
    {

        Destroy(gameObject, 2f);
    }




}
