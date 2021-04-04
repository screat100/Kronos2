using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [SerializeField]
    public float damageRate;

    [SerializeField]
    GameObject hitEffect;

    PlayerStatus m_PlayerStatus;


    private void Start()
    {
        m_PlayerStatus = gameObject.GetComponent<PlayerStatus>();
    }

    public float CalculatedDamage()
        /*
         * 공격력 * 피해감소율 * 피해량(%) * 크리티컬피해량 * 기타 데미지증가
         * 현재 '기타 데미지 증가' 반영 안됨
         */
    {
        float damage = m_PlayerStatus.attack * (damageRate / 100f);

        int critical = Random.Range(0, 100);
        // 크리티컬 발생
        if(critical < m_PlayerStatus.criticalProb)
        {
            damage *= (m_PlayerStatus.criticalDamage / 100);
            Debug.Log("Critical Hit!");
        }

        return damage;
    }


    public void HitEffect(Vector3 collisionPos) 
        /*
         * 공격 적중 시 이펙트를 발생시키는 함수
         * Enemy.cs 의 "OnTriggerEnter"에서 실행
         */
    {
        GameObject _hitSwordEffect = GameObject.Instantiate(hitEffect);
        _hitSwordEffect.transform.position = collisionPos;
        _hitSwordEffect.transform.rotation = gameObject.transform.rotation;
        _hitSwordEffect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(_hitSwordEffect, _hitSwordEffect.GetComponent<ParticleSystem>().main.duration);
    }



}
