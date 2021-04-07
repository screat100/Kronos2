using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffect;

    // Effect Status
    Team m_team;
    float damageAmount;
    float criticalProb;
    float criticalDamage;
    bool isShortHit; //��Ÿ������, �ٴ���Ʈ������
    float hitInterval; //�ٴ���Ʈ���� ��� �� �ʿ� �� ���� Ÿ���ϴ���
    float stiffenTime; //�����ð�

    Vector3 pushDir;
    float pushPower;

    float hitTime;

    public void EffectInit(int team_id, float damageAmount, float criticalProb, float criticalDamage, bool ShortHit, float hitInterval
        ,Vector3 pushDir, float pushPower)
    {
        m_team = (Team)team_id;
        isShortHit = ShortHit;
        this.damageAmount = damageAmount;
        this.criticalProb = criticalProb;
        this.criticalDamage = criticalDamage;
        this.hitInterval = hitInterval;
        this.pushDir = pushDir;
    }

    private void Start()
    {
        hitTime = hitInterval;
    }

    private void Update()
    {
        hitTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��Ÿ�� ��ų�� ���
        if(isShortHit)
        {
            OnHit(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // �ٴ���Ʈ�� ��ų�� ���
        if (!isShortHit && hitTime <= 0)
        {
            hitTime = hitInterval;
            OnHit(other);
        }
    }

    void OnHit(Collider other)
    {
        bool hit = false;

        float FinalDamage = FinalDamageCalculate();
        Vector3 hitPoint = other.ClosestPoint(gameObject.transform.position);

        // Player --(hit)--> Enemy
        if (m_team == Team.Player && other.tag == "enemy")
        {
            hit = true;
            other.GetComponent<Enemy>().ApplyDamage(FinalDamage);
            other.GetComponent<Rigidbody>().AddForce(pushDir * pushPower);
        }

        // Enemy --(hit)--> Player
        else if (m_team == Team.Enemy && other.tag == "player")
        {
            hit = true;
            other.GetComponent<Player>().Damaged(FinalDamage, stiffenTime, hitPoint);
            other.GetComponent<Rigidbody>().AddForce(pushDir * pushPower);
        }

        // if this effect object was made by trap
        else if (m_team == Team.Trap)
        {
            other.GetComponent<Rigidbody>().AddForce(pushDir * pushPower);
        }

        // hit on breakable object
        if (other.tag == "breakable")
        {
            hit = true;
        }

        // hit effect 
        if (hit)
        {
            HitEffect(hitPoint);
        }
    }


    float FinalDamageCalculate()
    {
        float criticalRand = Random.Range(0f, 1f);

        if (criticalRand <= this.criticalProb)
            return damageAmount * criticalDamage;
        
        else
            return damageAmount;
    }


    public void HitEffect(Vector3 collisionPos)
    /*
     * ���� ���� �� ����Ʈ�� �߻���Ű�� �Լ�
     * Enemy.cs �� "OnTriggerEnter"���� ����
     */
    {
        if(hitEffect)
        {
            GameObject hitEffect = GameObject.Instantiate(this.hitEffect);
            hitEffect.transform.position = collisionPos;
            hitEffect.transform.rotation = gameObject.transform.rotation;
            hitEffect.transform.parent = GameObject.Find("@Effect").transform;

            Destroy(hitEffect, hitEffect.GetComponent<ParticleSystem>().main.duration);
        }

        else
        {
            Debug.Log("Hit-effect No references");
        }
    }
}
