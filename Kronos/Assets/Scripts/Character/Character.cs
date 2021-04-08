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


    private void Start()
    {
        sturnTime = 0f;
        canMove = true;

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

    public abstract void Damaged(float damageNumber, float stiffenTime, Vector3 hitPoint);


    public abstract void Die();

}
