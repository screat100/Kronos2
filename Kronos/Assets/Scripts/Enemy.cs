using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.NonSerialized]
    public bool SwordSlashHitCheck;
    [System.NonSerialized]
    public static bool SwordSlashAttack;


    public float MonsterMaxHP = 100;

    [System.NonSerialized]
    public float MonsterHP;

    GameObject player;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        MonsterHP = MonsterMaxHP; //초기 hp 설정
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider col)
    {
        receiveDamage(col); //데미지 적용하는 함수
    }

    void receiveDamage(Collider obj)
        /*
         * 몬스터가 받는 데미지 적용할 함수.
         */
    {
         //기본 공격 데미지(데미지, 부딪힌대상)
         SwordSlashDamage(20f,obj);

        //다른 공격도 여기다 넣음됨

    }
    void SwordSlashDamage(float damage,Collider obj)
        /*
         * 기본공격 데미지 적용
         */
    {
        SwordSlashAttack = player.GetComponent<PlayerMove>().SwordSlashAttack;
        if (SwordSlashAttack && SwordSlashHitCheck&& obj.gameObject.CompareTag("Sword"))
        {
            MonsterHP -= damage;
            SwordSlashHitCheck = false;
            Debug.Log("Damage :" + damage + "//현재 몬스터 hp :" + MonsterHP);
        }
    }

    public void SwordSlashHitCheck_true()
    {
        SwordSlashHitCheck = true;
    }
}
