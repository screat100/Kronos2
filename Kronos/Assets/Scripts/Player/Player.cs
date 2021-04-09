using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Character
{
    public enum State
    {
        Die = -999, // ���
        Sturn = -99,
        Hit = -9,
        Idle = 0,
        WalkForwardBattle = 1, WalkLeftBattle = 2, WalkRightBattle = 3, WalkBackwardBattle = 4, 
        JumpUp=10, JumpDown=11, JumpEnd=12,
        Defend=20, // ���

        //�⺻����
        ValkyrieAtk01=10001, ValkyrieAtk02=10002, ValkyrieAtk03=10003,

        //ȸ�Ǳ�(Shift)
        RollForward =1001, RollLeft=1004, RollRight= 1009, RollBackward= 1006, RollForwardLeft= 1005, RollForwardRight= 1010, RollBackwardLeft= 1020, RollBackwardRight= 1025,
        
    }

    public State state;


    private void Start()
    {
        timeFlowOn = true;

        // ���̺� ���Ͽ��� ������ �ҷ���
        DataLoad();

        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        remainJumpCoolTime = 0f;
        remainShiftCoolTime = 0f;

        sturnTime = 0f;
        canMove = true;
        state = State.Idle;

        // �⺻����, ȸ�Ǳ�, ��ų���� ����
        basicAttack = BasicAttack.Valkyrie;
        shiftSkill = global::ShiftSkill.Rolling;
        weaponSkillSet = WeaponSkillSet.Valkyrie;

        // Status Initialization
        StatusInit();
        PlayingStatusInit_base();
        PlayingStatusInit_abs();


    }

    private void Update()
    {
        // managing status using time
        TimeFlow();
        DefendTimeFlow();
        remainJumpCoolTime -= Time.deltaTime;
        remainShiftCoolTime -= Time.deltaTime;

        if (!canMove)
            return;

        // Input
        KeyboardMove();
        Jump();
        Attack();
        Defend();
        ShiftSkill();
        QSkill();
        ESkill();

        animator.SetInteger("Input", (int)state);
        Debug.Log(state);
    }

}

/*
 * Player�� Status�� ��ġ���� ������ �����ϴ� �κ� ��ũ��Ʈ
 */
public partial class Player : Character
{

    [Header("PlayerStatus")]
    public float stamina;
    public float time;

    [Header("PlayerStatus-Playing")]
    public float time_p;
    public float stamina_p;

    [Header("Player Skills")]
    Skill m_QSkill;
    Skill m_ESkill;

    [Header("Player Equipment")]
    Equipment m_Weapon;
    Equipment m_Armor;
    Equipment m_Shield;
    

    [Header("SandGlass")]    // �𷡽ð� �ý���
    public int level;
    public int remainPoint;
    public float exp;
    public float expMax;

    
    [System.NonSerialized]
    public bool timeFlowOn; // �ð� �帧 on/off

    [System.NonSerialized]
    public float noDefendTime; // �и� ���� �Ǵ��� ���� 

    protected override void StatusInit()
    {
        time = 30;
        stamina = 100;

        HP = 100;
        attack = 10;
        shield = 0;
        attackSpeed = 100;
        moveSpeed = 100;
        criticalProb = 0;
        criticalDamage = 150;
        avoidanceRate = 0;
        coolTimeDecreaseRate = 0;

        exp = 0;
        expMax = 100;

    }

    protected override void PlayingStatusInit_abs()
    {
        time_p = time;
        stamina_p = stamina;
    }

    void TimeFlow()
    {
        if (timeFlowOn)
            time_p -= Time.deltaTime;
    }

    void DefendTimeFlow()
    {
        if (state != State.Defend)
        {
            noDefendTime += Time.deltaTime;

            if (noDefendTime >= 3 && stamina_p < stamina)
            {
                stamina_p += 33 * Time.deltaTime;

                if (stamina_p >= stamina)
                    stamina_p = stamina;
            }
        }
    }


    public void DataLoad()
        /*
         * ����� �����͸� �ҷ��´�.
         * - �����ϴ� �����ʹ� ���� ���� ������ ����
         */
    {

    }

    public void GainExp(float amount)
        /*
         * ���͸� ���� �� ����
         */
    {
        exp += amount;

        // ������ ����
        while (exp >= expMax)
        {
            exp -= expMax;
            level++;
            remainPoint++;
        }
    }

    public void DeleteExp()
        /*
         * ����� ����ġ ����
         */
    {
        exp = 0;
    }


}

/*
* Player�� ������(�Է�), �ִϸ��̼�, ����Ʈ�� ���� �����ϴ� �κ� ��ũ��Ʈ
*/
public partial class Player : Character
{
    

#pragma warning disable CS0108 // ����� ��ӵ� ����� ����ϴ�. new Ű���尡 �����ϴ�.
    Rigidbody rigidbody;
    Animator animator;

    [Header("Effect")]
    [SerializeField]
    GameObject attack1Effect;

    [SerializeField]
    GameObject attack2Effect;

    [SerializeField]
    GameObject attack3Effect;

    [SerializeField]
    GameObject defendEffect;

    [SerializeField]
    GameObject paryingEffect;


    //�� ���õ� ����
    float parryingTime;



    [Header("Player Behavior Factor")]
    float jumpCoolTime = 0.5f;
    float shiftCoolTime = 1.5f;

    float remainJumpCoolTime = 0f;
    float remainShiftCoolTime = 0f;
    float remainQSkillCool = 0f;
    float remainESkillCool = 0f;


    //���� ����
    [System.NonSerialized]
    public bool SwordSlashAttack;


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            // ���� �ִϸ��̼�
            if (animator.GetInteger("Input") == (int)State.JumpUp || animator.GetInteger("Input") == (int)State.JumpDown)
            {
                state = State.JumpEnd;
            }
        }
    }

    public override void Die()
    {
        state = State.Die;
    }



    void KeyboardMove()
    /*
     * WASD Ű�� ĳ���� (������ǥ) ������
     * 1 = ����
     * 2 = ��, 3 = ��� �ȱ�
     * 4 = ����
     */
    {
        if ((state != State.Idle
            && ( state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle )
            && ( state != State.JumpUp && state != State.JumpDown && state != State.JumpEnd)
            || (state == State.Die)))
            return;


        // �¿� �̵��� ���� ����
        float forceRight = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            forceRight = -50000f;
            if ((int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle)
            {
                state = State.WalkLeftBattle;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forceRight = 50000f;
            if ((int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle)
            {
                state = State.WalkRightBattle;
            }
        }


        // ���� �̵��� ����...
        float forceForward = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            forceForward = 50000f;
            if ((int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle)
            {
                state = State.WalkForwardBattle;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forceForward = -50000f;
            if ((int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle)
            {
                state = State.WalkBackwardBattle;
            }
        }

        // �밢�� ��츦 ����Ͽ�, ��� �������� ������ �ӵ��� ������ �Ѵ�.
        if (forceForward != 0 && forceRight != 0)
        {
            forceForward = forceForward / Mathf.Sqrt(2);
            forceRight = forceRight / Mathf.Sqrt(2);
        }

        // �Է��� ���ٸ�, Idle ���·� ��ȯ
        else if (forceForward == 0 && forceRight == 0 && ((int)state >= (int)State.WalkForwardBattle && (int)state <= (int)State.WalkBackwardBattle))
        {
            state = State.Idle;
        }


        // �ڷ� ���� �� �̵��ӵ� 25% ����
        float backMovingCorrectionValue = 1.0f;
        if (forceForward < 0)
            backMovingCorrectionValue = 0.75f;
        
        // ������ ����
        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        playerForward = playerForward.normalized;

        Vector3 playerRight = gameObject.transform.right;
        playerRight.y = 0;
        playerRight = playerRight.normalized;

        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        rigidbody.velocity += playerForward * moveSpeed / 100 * forceForward / 10000 * backMovingCorrectionValue;
        rigidbody.velocity += playerRight * moveSpeed / 100 * forceRight / 10000;
    }

    void OnWalkEvent()
    /*
     * KeyboardMove �ִϸ��̼� �� ȣ���ϴ� �Լ�
     * - �ȱ� ���� ���
     */
    {

    }


    void Jump()
    /*
     * space Ű�� ����
     * 10 = ���� ���� -> ���� ��
     * 11 = ���� �ٿ�
     * 12 = ����
     */
    {
        if (state == State.Die)
            return;

        // �߶�
        if (rigidbody.velocity.y < -0.99f)
        {
            state = State.JumpDown;
        }

        if (state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle)
            && (state != State.JumpUp && state != State.JumpDown && state != State.JumpEnd))
            return;

        // ����
        if (Input.GetKeyDown(KeyCode.Space)
            && (state == State.Idle || state == State.WalkForwardBattle || state == State.WalkBackwardBattle || state==State.WalkRightBattle || state==State.WalkLeftBattle)
            && remainJumpCoolTime <= 0f)
        {
            rigidbody.AddForce(new Vector3(0, 1, 0) * 10000f);
            state = State.JumpUp;

            remainJumpCoolTime = jumpCoolTime;
        }

        if (animator.GetInteger("Input") == (int)State.JumpEnd
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd")
            )
        {
            state = State.Idle;
        }

    }

    int MoveWithAttack()
    /*
     * ���� �� ��/�� ����Ű�� �Է�, ���� �� �����̴� ���� ����
     */
    {
        // ���� �� �������� ����
        if (Input.GetKey(KeyCode.W))
            return 22500;

        else if (Input.GetKey(KeyCode.S))
            return 0;

        else
            return 7500;
    }

    void Attack()
    /*
     * ��Ŭ�� �Է��� ������ �⺻���� �ִϸ��̼� �� ������ ���
     */
    {
        if (
            (
            state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle)
            && (state != State.ValkyrieAtk01 && state!= State.ValkyrieAtk02 && state != State.ValkyrieAtk03)
            ) 
            || state == State.Die)

            return;

        //���ǹ� : �������̶��
        //if (animator.GetInteger("Input") >= 20 && animator.GetInteger("Input") <= 22 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        //{
        //    rigidbody.velocity *= 0.97f;
        //}


        if (Input.GetMouseButtonDown(0))
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);

            switch(basicAttack)
            {
                case BasicAttack.Valkyrie:
                    BasicAttack_Valkyrie();
                    break;
            }


            if ((int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle)
            {
                state = State.Attack;
            }

            else if (animator.GetInteger("Input") == 20
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
            {
                state = State.Attack;
            }

            else if (animator.GetInteger("Input") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
            {
                state = State.Attack;
            }

        }

        else if (animator.GetInteger("Input") == 20
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            state = State.Idle;
        }

        else if (animator.GetInteger("Input") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
        {
            state = State.Idle;
        }

        else if (animator.GetInteger("Input") == 22
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
        {
            state = State.Idle;
        }
    }

    void OnAttack1Event()
    /*
     * �⺻���� 1Ÿ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�
     * - �⺻���� �� ���Ĺ� �����ӿ� ���� ����
     * - ���� ���� ���
     * - ���� ����Ʈ �߻�
     */
    {
        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        int movePower = MoveWithAttack();
        rigidbody.AddForce(playerForward * movePower);


        GameObject attack1_effect = GameObject.Instantiate(attack1Effect);
        //attack1_effect.GetComponent<AttackEffect>().EffectInit(
        //    Team.Player, attack_p

        Vector3 effectPos = gameObject.transform.position;
        effectPos += 0.33f * gameObject.transform.up + 0.5f * gameObject.transform.right + playerForward * movePower / 22500;
        attack1_effect.transform.position = effectPos;

        attack1_effect.transform.rotation = gameObject.transform.rotation;
        attack1_effect.transform.Rotate(0, -100, 0);
        attack1_effect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(attack1_effect, 0.75f * attack1_effect.GetComponent<ParticleSystem>().main.duration);


    }

    void OnAttack2Event()
    /*
     * �⺻���� 2Ÿ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�
     * - �⺻���� �� ���Ĺ� �����ӿ� ���� ����
     * - ���� ���� ���
     * - ���� ����Ʈ �߻�
     */
    {
        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        int movePower = MoveWithAttack();
        rigidbody.AddForce(playerForward * movePower);

        GameObject attack2_effect = GameObject.Instantiate(attack2Effect);
        attack2_effect.transform.position = gameObject.transform.position + 0.66f * gameObject.transform.up + playerForward * movePower / 22500;
        attack2_effect.transform.rotation = gameObject.transform.rotation;
        attack2_effect.transform.Rotate(90, 0, 0);
        attack2_effect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(attack2_effect, 0.75f * attack2_effect.GetComponent<ParticleSystem>().main.duration);
    }

    void OnAttack3Event()
    /*
     * �⺻���� 3Ÿ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�
     * - �⺻���� �� ���Ĺ� �����ӿ� ���� ����
     * - ���� ���� ���
     * - ���� ����Ʈ �߻�
     */
    {

        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        int movePower = MoveWithAttack();
        rigidbody.AddForce(playerForward * movePower);

        GameObject attack3_effect = GameObject.Instantiate(attack3Effect);

        Vector3 effectPos = gameObject.transform.position;
        effectPos += 0.33f * gameObject.transform.right + 0.5f * gameObject.transform.up + playerForward * movePower / 22500;
        attack3_effect.transform.position = effectPos;

        attack3_effect.transform.rotation = gameObject.transform.rotation;
        attack3_effect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(attack3_effect, 0.75f * attack3_effect.GetComponent<ParticleSystem>().main.duration);
    }

    void Defend()
    /*
     * ��Ŭ������ ���
     * 30 : ���
     */
    {
        if ((state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle)
            && state != State.Defend) || state == State.Die)
            return;

        

        // ��Ŭ�� ����
        if (Input.GetMouseButtonDown(1)
            && (int)state >= (int)State.Idle && (int)state <= (int)State.WalkBackwardBattle
            && stamina_p > 10)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            
            state = State.Defend;
            noDefendTime = 0f;

            stamina_p -= 10;
            parryingTime = 0.5f;
        }

        // ��Ŭ�� ���� ����
        if ((state == State.Defend && !Input.GetMouseButton(1)) || stamina_p <= 0)
        {
            state = State.Idle;
            parryingTime = 0f;
            if (stamina_p <= 0)
                stamina_p = 0f;
        }

        // ��Ŭ�� ����
        else if (state == State.Defend)
        {
            parryingTime -= Time.deltaTime;
            stamina_p -= Time.deltaTime * 20;
            noDefendTime = 0f;
        }
    }



    void ShiftSkill()
    /*
     * ȸ�Ǳ�
     */
    {
        if (state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle))
            return;


        if (Input.GetKeyDown(KeyCode.LeftShift) && remainShiftCoolTime <= 0f)
        {
            switch(shiftSkill)
            {
                case global::ShiftSkill.Rolling:
                    ShiftSkill_Roll();
                    break;

                case global::ShiftSkill.Dash:
                    break;

                case global::ShiftSkill.Teleporting:
                    break;
            }
        }
    }




    void QSkill()
    /*
     * QŰ�� ��ų1 ���
     */
    {
        if (Input.GetKeyDown(KeyCode.Q) && remainQSkillCool <= 0)
        {



        }
    }

    void ESkill()
    /*
     * EŰ�� ��ų2 ���
     */
    {
        if (Input.GetKeyDown(KeyCode.E) && remainESkillCool <= 0)
        {



        }
    }



    public IEnumerator AttackRigidy(float time)
    /*
     * ���ݿ� ���� ������ ����
     * - time : �������� ������ �ð�
     */
    {
        animator.speed = 0.0f;
        Debug.Log("������");
        yield return new WaitForSeconds(time);
        animator.speed = 1.0f;
    }

    IEnumerator DamagedRigidy(float time)
    /*
     * �ǰݽ� ���� ����
     */
    {
        if (state == State.Die)
            yield break;

        // ���� ��� ����
        state = State.Hit;

        // ���� ���� �� ȸ��
        yield return new WaitForSeconds(time);

        if (state == State.Die)
            yield break;

        state = State.Idle;
    }

    public override void Damaged(float damageAmount, float stiffenTime, Vector3 hitPoint)
    /*
     * �ǰݿ� ���� ����/���/������ ����
     * - time : ���� ���� �ð�
     * - damage : �ǰ� ������
     */
    {
        // ������ �� �ǰ����� ����
        if (state == State.Die
            || invincibility)
            return;

        // �� ���� ����
        else if (state == State.Defend)
        {
            // �浹������ �÷��̾� ������ ����
            Vector3 hitPointVector = (hitPoint - gameObject.transform.position).normalized;

            // right�� �� ���� ������ ������ ����
            float angleBetHitpAndForward = Mathf.Acos(
                Vector3.Dot(hitPointVector, gameObject.transform.forward) / Vector3.Magnitude(hitPointVector) / Vector3.Magnitude(gameObject.transform.forward));
            angleBetHitpAndForward *= Mathf.Rad2Deg;


            // ��� ������ �� 120��
            if (angleBetHitpAndForward <= 60 && angleBetHitpAndForward >= -60)
            {
                // ��� ����
                rigidbody.AddForce(-gameObject.transform.forward * 10000f);

                // �и� ����
                if (parryingTime >= 0)
                {
                    // �и� ����Ʈ
                    GameObject _parryingEffect = GameObject.Instantiate(paryingEffect);
                    _parryingEffect.transform.parent = GameObject.Find("@Effect").transform;
                    _parryingEffect.transform.position = hitPoint;
                    _parryingEffect.transform.rotation = gameObject.transform.rotation;
                    Destroy(_parryingEffect, _parryingEffect.GetComponent<ParticleSystem>().main.duration);
                }

                else
                {
                    // ��� ����Ʈ
                    GameObject _defendEffect = GameObject.Instantiate(defendEffect);
                    _defendEffect.transform.parent = GameObject.Find("@Effect").transform;
                    _defendEffect.transform.position = hitPoint;
                    _defendEffect.transform.rotation = gameObject.transform.rotation;
                    Destroy(_defendEffect, _defendEffect.GetComponent<ParticleSystem>().main.duration);
                }

                return;
            }

            Debug.Log("Defend Failed");
        }


        // ȸ����
        int avoidance = Random.Range(0, 100);
        if (avoidance < avoidanceRate)
        {
            Debug.Log("ȸ��!");
            return;
        }

        // �����
        float shieldRate = 50f * Mathf.Log(shield_p + 10) - 50f;
        damageAmount *= (shieldRate / 100);

        // ü�� ���� ����
        HP_p -= (int)damageAmount;
        Debug.Log($"PlayerHP = {HP}");

        // ������� �˻�
        if (HP <= 0)
        {
            Debug.Log("You Die");
            state = State.Die;
            return;
        }

        // ����
        if(!superArmor)
        {
            sturnTime = stiffenTime;
            state = State.Hit;
            StartCoroutine(DamagedRigidy(stiffenTime));
        }

        // ���� ����



    }

}


public enum BasicAttack
{
    Valkyrie,
    Rapier,
    Infinity,
    MagicBall,
    Berserker,
}

public enum ShiftSkill
{
    Rolling,
    Dash,
    Teleporting,
}

public enum WeaponSkillSet
{
    Valkyrie,
    Rapier,
    Bastard,
    Katana,
    Staff,
    Gladius,
    ShortSword,
    LongSword,
}

public enum Parrying
{
    Basic,
}

/*
 * �⺻����, ȸ�Ǳ�, ��ų
 */
public partial class Player : Character
{
    public BasicAttack basicAttack;
    public WeaponSkillSet weaponSkillSet;
    public ShiftSkill shiftSkill;

    void BasicAttack_Valkyrie()
    {

    }

    void ShiftSkill_Roll()
    {
        Vector3 playerFront = gameObject.transform.forward;
        Vector3 playerRight = gameObject.transform.right;
        playerFront.y = 0;
        playerRight.y = 0;
        playerFront = playerFront.normalized;
        playerRight = playerRight.normalized;

        rigidbody.velocity = new Vector3(0, 0, 0);
        float frontForce = 0;
        float rightForce = 0;

        int rollDir = 0;

        // �Է¿� ���� ���� ���
        // �¿�
        if (Input.GetKey(KeyCode.A))
        {
            rollDir += 4;
            rightForce = -20000;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rollDir += 9;

            rightForce = 20000;
        }
        // ����
        if (Input.GetKey(KeyCode.S))
        {
            rollDir += 16;
            frontForce = -20000;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            rollDir += 1;
            frontForce = 20000;
        }

        if (frontForce != 0 && rightForce != 0)
        {
            frontForce /= Mathf.Sqrt(2);
            rightForce /= Mathf.Sqrt(2);
        }

        // �ִϸ��̼� �� ���� ����
        if (!(frontForce == 0 && rightForce == 0))
        {
            switch(rollDir)
            {
                case 1:
                    state = State.RollForward;
                    break;
                case 4:
                    state = State.RollLeft;
                    break;
                case 5:
                    state = State.RollForwardLeft;
                    break;
                case 9:
                    state = State.RollRight;
                    break;
                case 10:
                    state = State.RollForwardRight;
                    break;
                case 16:
                    state = State.RollBackward;
                    break;
                case 20:
                    state = State.RollBackwardLeft;
                    break;
                case 25:
                    state = State.RollBackwardRight;
                    break;
            }

            rigidbody.AddForce(playerFront * frontForce);
            rigidbody.AddForce(playerRight * rightForce);

            remainShiftCoolTime = shiftCoolTime;
        }
    }


    // ������ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�, ������ ���� ���
    void OnRoll()
    {

    }

    //�����Ⱑ ������ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�
    void RollEnd()
    {
        state = State.Idle;
    }


}