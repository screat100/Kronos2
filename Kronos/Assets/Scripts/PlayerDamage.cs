using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField]
    public float damageRate;


    public float CalculatedDamage()
        /*
         * 공격력 * 피해감소율 * 피해량(%) * 크리티컬피해량 * 기타 데미지증가
         * 현재 '기타 데미지 증가' 반영 안됨
         */
    {
        float damage = PlayerStatus.attack * (damageRate / 100f);

        int critical = Random.Range(0, 100);
        // 크리티컬 발생
        if(critical < PlayerStatus.criticalProb)
        {
            damage *= (PlayerStatus.criticalDamage / 100);
            Debug.Log("Critical Hit!");
        }

        return damage;
    }


}
