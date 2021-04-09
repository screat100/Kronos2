using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Character
{
    public enum State
    {
        Die = -999, // 사망
        Sturn = -99,
        Hit = -9,
        Idle = 0,
        WalkForwardBattle = 1, WalkLeftBattle = 2, WalkRightBattle = 3, WalkBackwardBattle = 4, 
        JumpUp=10, JumpDown=11, JumpEnd=12,
        Defend=20, // 방어

        //기본공격
        ValkyrieAtk01=10001, ValkyrieAtk02=10002, ValkyrieAtk03=10003,

        //회피기(Shift)
        RollForward =1001, RollLeft=1004, RollRight= 1009, RollBackward= 1006, RollForwardLeft= 1005, RollForwardRight= 1010, RollBackwardLeft= 1020, RollBackwardRight= 1025,
        
    }

    public State state;


    private void Start()
    {
        timeFlowOn = true;

        // 세이브 파일에서 데이터 불러옴
        DataLoad();

        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        remainJumpCoolTime = 0f;
        remainShiftCoolTime = 0f;

        sturnTime = 0f;
        canMove = true;
        state = State.Idle;

        // 기본공격, 회피기, 스킬구성 설정
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
 * Player의 Status등 수치적인 값들을 제어하는 부분 스크립트
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
    

    [Header("SandGlass")]    // 모래시계 시스템
    public int level;
    public int remainPoint;
    public float exp;
    public float expMax;

    
    [System.NonSerialized]
    public bool timeFlowOn; // 시간 흐름 on/off

    [System.NonSerialized]
    public float noDefendTime; // 패링 관련 판단을 위한 

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
         * 저장된 데이터를 불러온다.
         * - 저장하는 데이터는 찍은 스탯 정보와 레벨
         */
    {

    }

    public void GainExp(float amount)
        /*
         * 몬스터를 잡을 때 적용
         */
    {
        exp += amount;

        // 레벨업 판정
        while (exp >= expMax)
        {
            exp -= expMax;
            level++;
            remainPoint++;
        }
    }

    public void DeleteExp()
        /*
         * 사망시 경험치 삭제
         */
    {
        exp = 0;
    }


}

/*
* Player의 움직임(입력), 애니메이션, 이펙트에 대해 제어하는 부분 스크립트
*/
public partial class Player : Character
{
    

#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
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


    //방어에 관련된 변수
    float parryingTime;



    [Header("Player Behavior Factor")]
    float jumpCoolTime = 0.5f;
    float shiftCoolTime = 1.5f;

    float remainJumpCoolTime = 0f;
    float remainShiftCoolTime = 0f;
    float remainQSkillCool = 0f;
    float remainESkillCool = 0f;


    //공격 관련
    [System.NonSerialized]
    public bool SwordSlashAttack;


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            // 착지 애니메이션
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
     * WASD 키로 캐릭터 (수평좌표) 움직임
     * 1 = 전진
     * 2 = 좌, 3 = 우로 걷기
     * 4 = 후진
     */
    {
        if ((state != State.Idle
            && ( state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle )
            && ( state != State.JumpUp && state != State.JumpDown && state != State.JumpEnd)
            || (state == State.Die)))
            return;


        // 좌우 이동에 관한 변수
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


        // 전후 이동에 관한...
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

        // 대각선 경우를 고려하여, 모든 방향으로 동일한 속도를 내도록 한다.
        if (forceForward != 0 && forceRight != 0)
        {
            forceForward = forceForward / Mathf.Sqrt(2);
            forceRight = forceRight / Mathf.Sqrt(2);
        }

        // 입력이 없다면, Idle 상태로 전환
        else if (forceForward == 0 && forceRight == 0 && ((int)state >= (int)State.WalkForwardBattle && (int)state <= (int)State.WalkBackwardBattle))
        {
            state = State.Idle;
        }


        // 뒤로 걷을 때 이동속도 25% 감소
        float backMovingCorrectionValue = 1.0f;
        if (forceForward < 0)
            backMovingCorrectionValue = 0.75f;
        
        // 물리에 적용
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
     * KeyboardMove 애니메이션 중 호출하는 함수
     * - 걷기 사운드 재생
     */
    {

    }


    void Jump()
    /*
     * space 키로 점프
     * 10 = 점프 시작 -> 점프 업
     * 11 = 점프 다운
     * 12 = 착지
     */
    {
        if (state == State.Die)
            return;

        // 추락
        if (rigidbody.velocity.y < -0.99f)
        {
            state = State.JumpDown;
        }

        if (state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle)
            && (state != State.JumpUp && state != State.JumpDown && state != State.JumpEnd))
            return;

        // 점프
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
     * 공격 중 앞/뒤 방향키를 입력, 공격 중 움직이는 힘을 조절
     */
    {
        // 공격 중 움직임을 더함
        if (Input.GetKey(KeyCode.W))
            return 22500;

        else if (Input.GetKey(KeyCode.S))
            return 0;

        else
            return 7500;
    }

    void Attack()
    /*
     * 좌클릭 입력을 감지해 기본공격 애니메이션 및 움직임 재생
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

        //조건문 : 공격중이라면
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
     * 기본공격 1타 애니메이션에서 호출하는 함수
     * - 기본공격 중 전후방 움직임에 대한 제어
     * - 공격 사운드 재생
     * - 공격 이펙트 발생
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
     * 기본공격 2타 애니메이션에서 호출하는 함수
     * - 기본공격 중 전후방 움직임에 대한 제어
     * - 공격 사운드 재생
     * - 공격 이펙트 발생
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
     * 기본공격 3타 애니메이션에서 호출하는 함수
     * - 기본공격 중 전후방 움직임에 대한 제어
     * - 공격 사운드 재생
     * - 공격 이펙트 발생
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
     * 우클릭으로 방어
     * 30 : 방어
     */
    {
        if ((state != State.Idle
            && (state != State.WalkForwardBattle && state != State.WalkBackwardBattle && state != State.WalkLeftBattle && state != State.WalkRightBattle)
            && state != State.Defend) || state == State.Die)
            return;

        

        // 우클릭 시작
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

        // 우클릭 유지 종료
        if ((state == State.Defend && !Input.GetMouseButton(1)) || stamina_p <= 0)
        {
            state = State.Idle;
            parryingTime = 0f;
            if (stamina_p <= 0)
                stamina_p = 0f;
        }

        // 우클릭 유지
        else if (state == State.Defend)
        {
            parryingTime -= Time.deltaTime;
            stamina_p -= Time.deltaTime * 20;
            noDefendTime = 0f;
        }
    }



    void ShiftSkill()
    /*
     * 회피기
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
     * Q키로 스킬1 사용
     */
    {
        if (Input.GetKeyDown(KeyCode.Q) && remainQSkillCool <= 0)
        {



        }
    }

    void ESkill()
    /*
     * E키로 스킬2 사용
     */
    {
        if (Input.GetKeyDown(KeyCode.E) && remainESkillCool <= 0)
        {



        }
    }



    public IEnumerator AttackRigidy(float time)
    /*
     * 공격에 대한 역경직 적용
     * - time : 역경직을 적용할 시간
     */
    {
        animator.speed = 0.0f;
        Debug.Log("역경직");
        yield return new WaitForSeconds(time);
        animator.speed = 1.0f;
    }

    IEnumerator DamagedRigidy(float time)
    /*
     * 피격시 경직 적용
     */
    {
        if (state == State.Die)
            yield break;

        // 경직 모션 적용
        state = State.Hit;

        // 경직 적용 및 회복
        yield return new WaitForSeconds(time);

        if (state == State.Die)
            yield break;

        state = State.Idle;
    }

    public override void Damaged(float damageAmount, float stiffenTime, Vector3 hitPoint)
    /*
     * 피격에 대한 경직/모션/데미지 적용
     * - time : 경직 적용 시간
     * - damage : 피격 데미지
     */
    {
        // 구르기 중 피격하지 않음
        if (state == State.Die
            || invincibility)
            return;

        // 방어에 대한 적용
        else if (state == State.Defend)
        {
            // 충돌지점과 플레이어 사이의 벡터
            Vector3 hitPointVector = (hitPoint - gameObject.transform.position).normalized;

            // right와 두 벡터 사이의 각도를 구함
            float angleBetHitpAndForward = Mathf.Acos(
                Vector3.Dot(hitPointVector, gameObject.transform.forward) / Vector3.Magnitude(hitPointVector) / Vector3.Magnitude(gameObject.transform.forward));
            angleBetHitpAndForward *= Mathf.Rad2Deg;


            // 방어 범위는 총 120도
            if (angleBetHitpAndForward <= 60 && angleBetHitpAndForward >= -60)
            {
                // 방어 성공
                rigidbody.AddForce(-gameObject.transform.forward * 10000f);

                // 패링 성공
                if (parryingTime >= 0)
                {
                    // 패링 이펙트
                    GameObject _parryingEffect = GameObject.Instantiate(paryingEffect);
                    _parryingEffect.transform.parent = GameObject.Find("@Effect").transform;
                    _parryingEffect.transform.position = hitPoint;
                    _parryingEffect.transform.rotation = gameObject.transform.rotation;
                    Destroy(_parryingEffect, _parryingEffect.GetComponent<ParticleSystem>().main.duration);
                }

                else
                {
                    // 방어 이펙트
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


        // 회피율
        int avoidance = Random.Range(0, 100);
        if (avoidance < avoidanceRate)
        {
            Debug.Log("회피!");
            return;
        }

        // 방어율
        float shieldRate = 50f * Mathf.Log(shield_p + 10) - 50f;
        damageAmount *= (shieldRate / 100);

        // 체력 감소 적용
        HP_p -= (int)damageAmount;
        Debug.Log($"PlayerHP = {HP}");

        // 사망판정 검사
        if (HP <= 0)
        {
            Debug.Log("You Die");
            state = State.Die;
            return;
        }

        // 경직
        if(!superArmor)
        {
            sturnTime = stiffenTime;
            state = State.Hit;
            StartCoroutine(DamagedRigidy(stiffenTime));
        }

        // 경직 적용



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
 * 기본공격, 회피기, 스킬
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

        // 입력에 따른 방향 계산
        // 좌우
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
        // 전후
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

        // 애니메이션 및 물리 적용
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


    // 구르기 애니메이션에서 호출하는 함수, 구르기 사운드 재생
    void OnRoll()
    {

    }

    //구르기가 끝나면 애니메이션에서 호출하는 함수
    void RollEnd()
    {
        state = State.Idle;
    }


}