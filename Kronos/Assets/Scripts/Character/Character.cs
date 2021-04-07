using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Team
{
    Player,
    Enemy,
    Trap,
}

public abstract class Character : MonoBehaviour
{

    [Header("Status")]
    public Team team;
    public int HP;
    public int attack;
    public int shield;
    public int attackSpeed;
    public int moveSpeed;
    public int criticalProb;
    public int criticalDamage;
    public float avoidanceRate;
    public float coolTimeDecreaseRate;

    [Header("Status-Playing")]
    public int HP_p;
    public int attack_p;
    public int shield_p;
    public int attackSpeed_p;
    public int moveSpeed_p;
    public int criticalProb_p;
    public int criticalDamage_p;
    public float avoidanceRate_p;
    public float coolTimeDecreaseRate_p;

    [Header("Special State")]
    public bool superArmor;
    public bool invincibility;

    [Header("Behavior Factor")]
    protected float sturnTime;
    protected bool canMove;
    protected float speedRate = 1.0f;
    protected float maxSpeed = 5f;

    public enum State
    {
        Die, // ���
        Idle, 
        Walk,
        Jump,
        Fall, // y�� �ӵ��� ����(=��������)�� ����
        Roll, // ������(�÷��̾� ����)
        Attack, // ����
        Defend, // ���
        Hit, // �ǰ�
    }

    public State state;

    private void Start()
    {
        sturnTime = 0f;
        canMove = true;
        state = State.Idle;

        // Status Initialization
        StatusInit();
        PlayingStatusInit_base();
        PlayingStatusInit_abs();
    }


    void Update()
    {
        sturnTime -= Time.deltaTime;
        if (sturnTime > 0f) canMove = false;
        else canMove = true;
    }

    protected abstract void StatusInit();
    protected void PlayingStatusInit_base()
    {
        HP_p = HP;
        attack_p = attack;
        shield_p = shield;
        attackSpeed_p = attackSpeed;
        moveSpeed_p = moveSpeed;
        criticalProb_p = criticalProb;
        criticalDamage_p = criticalDamage;
        avoidanceRate_p = avoidanceRate;
        coolTimeDecreaseRate_p = coolTimeDecreaseRate;

        superArmor = false;
        invincibility = false;
    }
    protected abstract void PlayingStatusInit_abs();

    public virtual void Damaged(float damageNumber, float stiffenTime, Vector3 hitPoint)
    {
        if (state == State.Die || invincibility)
            return;

        // ����� ������ ����� ���� ���ط��� hp�� ����
        float shieldRate = 50 * Mathf.Log(shield_p + 10) - 50;
        HP_p -= (int)(damageNumber * shieldRate / 100);

        // ������� �˻�
        if(HP_p <= 0)
        {
            HP_p = 0;
            Die();
            return;
        }

        // �����ð�
        if(!superArmor)
        {
            sturnTime = stiffenTime;
            state = State.Hit;
        }

    }

    public void Die()
    {
        state = State.Die;
    }


}
