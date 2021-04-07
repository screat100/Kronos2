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
            // 이펙트에 달려있는 스크립트
            PlayerEffect _playerEffect = other.GetComponent<PlayerEffect>();

            // 데미지 계산 및 적용
            float damage = _playerEffect.CalculatedDamage();
            ApplyDamage(damage);

            // 플레이어에 역경직 적용
            StartCoroutine(GameObject.Find("Player").GetComponent<Player>().AttackRigidy(0.05f));

            if (MonsterHP > 0)
            {
                // 피격 모션
                gameObject.GetComponent<Animator>().Play("GetHit", -1, 0);
                //gameObject.GetComponent<GreenSlime>()._enemyState = GreenSlime.EnemyState.GetHit; // 에네미 상태변경
                // 피격 이펙트
                _playerEffect.HitEffect(other.ClosestPoint(gameObject.transform.position));
            }
            

            // 피격 사운드
            // 뭐가있지?...
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
