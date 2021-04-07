using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [SerializeField]
    public float damageRate;

    [SerializeField]
    GameObject hitEffect;

    Player m_Player;


    private void Start()
    {
        m_Player = gameObject.GetComponent<Player>();
    }

    public float CalculatedDamage()
        /*
         * ���ݷ� * ���ذ����� * ���ط�(%) * ũ��Ƽ�����ط� * ��Ÿ ����������
         * ���� '��Ÿ ������ ����' �ݿ� �ȵ�
         */
    {
        float damage = m_Player.attack * (damageRate / 100f);

        int critical = Random.Range(0, 100);
        // ũ��Ƽ�� �߻�
        if(critical < m_Player.criticalProb)
        {
            damage *= (m_Player.criticalDamage / 100);
            Debug.Log("Critical Hit!");
        }

        return damage;
    }


    public void HitEffect(Vector3 collisionPos) 
        /*
         * ���� ���� �� ����Ʈ�� �߻���Ű�� �Լ�
         * Enemy.cs �� "OnTriggerEnter"���� ����
         */
    {
        GameObject _hitSwordEffect = GameObject.Instantiate(hitEffect);
        _hitSwordEffect.transform.position = collisionPos;
        _hitSwordEffect.transform.rotation = gameObject.transform.rotation;
        _hitSwordEffect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(_hitSwordEffect, _hitSwordEffect.GetComponent<ParticleSystem>().main.duration);
    }



}
