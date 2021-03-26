using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float speedRate = 1.0f;

    private Rigidbody rigidbody;
    float maxSpeed = 5f;

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
    float jumpCool;

    public enum PlayerState
    {
        Die,
        Idle,
        Walk,
        Jump,
        Fall,
        Roll,
        Attack,
        Defend,
        Hit,
    }

    public PlayerState _playerState;

    // 쿨타임 관련
    float rollCoolTime = 1.5f;
    float remainRollCoolTime = 0f;
    float remainQSkillCool = 0f;
    float remainESkillCool = 0f;


    //공격 관련
    [System.NonSerialized]
    public bool SwordSlashAttack;
    Animator animator;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        _playerState = PlayerState.Idle;
        jumpCool = 0f;
    }

    void Update()
    {
        KeyboardMove();
        Jump();
        Attack();
        Defend();
        Roll();
        QSkill();
        ESkill();

        PlayerMoveBase();

        jumpCool -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            // 착지 애니메이션
            if (animator.GetInteger("Input") == 10 || animator.GetInteger("Input") == 11)
            {
                animator.SetInteger("Input", 12);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playerState == PlayerState.Die)
            return;

        // 이펙트에 이 함수와 같은 기능을 달도록 옮겨주세요,,,
        if(other.transform.tag == "EnemyEffect")
        {
            // 이펙트에 달려있는 스크립트
            EnemyEffect _enemyEffect = other.GetComponent<EnemyEffect>();

            // 이펙트의 좌표
            Vector3 hitPoint = other.transform.position;

            // 데미지 계산 및 적용
            Damaged(0.25f, hitPoint, _enemyEffect); //경직시간은 임의로 설정하였음... 추후 수정 바람

        }

        // 트랩 폭발에 맞으면 밀려남,,, 이것도 이펙트로 옮겨주세요
        else if (other.transform.name == "TrapExplosion")
        {
            rigidbody.AddForce(
            other.GetComponent<TrapExplosion>().pushDir * other.GetComponentInParent<TrapExplosion>().pushPower);
        }
    }



    void PlayerMoveBase()
    {
        // 입력 없으면 속도 감소
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.95f,
            rigidbody.velocity.y,
            rigidbody.velocity.z * 0.95f);
    }


    void KeyboardMove()
        /*
         * WASD 키로 캐릭터 (수평좌표) 움직임
         * 1 = 전진
         * 2 = 좌, 3 = 우로 걷기
         * 4 = 후진
         */
    {
        if ((_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Jump
            && _playerState != PlayerState.Fall)
            ||(_playerState==PlayerState.Die))
            return;


        // 전후 이동에 관한...
        float forceForward = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            forceForward = 50000f;
            if(animator.GetInteger("Input")<=5)
                animator.SetInteger("Input", 1);
        }
        else if (Input.GetKey(KeyCode.S)) 
        { 
            forceForward = -50000f;
            if (animator.GetInteger("Input") <= 5)
                animator.SetInteger("Input", 4);
        }


        // 좌우 이동에 관한 변수
        float forceRight = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            forceRight = -50000f;
            if (animator.GetInteger("Input") <= 5)
                animator.SetInteger("Input", 2);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forceRight = 50000f;
            if (animator.GetInteger("Input") <= 5)
                animator.SetInteger("Input", 3);
        }

        // 대각선 경우를 고려하여, 모든 방향으로 동일한 속도를 내도록 한다.
        if(forceForward != 0 && forceRight != 0)
        {
            forceForward = forceForward / Mathf.Sqrt(2);
            forceRight = forceRight / Mathf.Sqrt(2);
        }

        else if (forceForward == 0 && forceRight == 0 && animator.GetInteger("Input") <= 4)
        {
            _playerState = PlayerState.Idle;
            animator.SetInteger("Input", 0);
        }

        float backMovingCorrectionValue = 1.0f;

        // 뒤로 걷을 때 이동속도 25% 감소
        if(forceForward < 0)
        {
            backMovingCorrectionValue = 0.75f;
        }


        // 최대속도 제한 & 물리에 적용
        // : 최대속도를 넘지 않을 경우에만 addForce 적용
        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        playerForward = playerForward.normalized;

        Vector3 playerRight = gameObject.transform.right;
        playerRight.y = 0;
        playerRight = playerRight.normalized;

        float horizontalSpeed = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude;
        if(horizontalSpeed < maxSpeed * backMovingCorrectionValue)
        {
            rigidbody.AddForce(playerForward * speedRate * forceForward * backMovingCorrectionValue * Time.deltaTime);
            rigidbody.AddForce(playerRight * speedRate * forceRight * Time.deltaTime);
        }

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
        if (_playerState == PlayerState.Die)
            return;

        // 추락
        if (rigidbody.velocity.y < -0.99f)
        {
            animator.SetInteger("Input", 11);
            _playerState = PlayerState.Fall;
        }

        if (_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Jump 
            && _playerState != PlayerState.Fall)
            return;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) 
            && (_playerState == PlayerState.Idle || _playerState == PlayerState.Walk)
            && jumpCool <= 0f)
        {
            rigidbody.AddForce(new Vector3(0, 1, 0) * 5000f);
            animator.SetInteger("Input", 10);
            _playerState = PlayerState.Jump;

            jumpCool = 0.5f;
        }

        if(animator.GetInteger("Input") == 12
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd")
            )
        {
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
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
         * Input 20~22 : 각각 기본공격 1타~3타
         */
    {

        if ((_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Attack) || _playerState==PlayerState.Die)
            return;


        if (animator.GetInteger("Input") >=20 && animator.GetInteger("Input") <= 22 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            rigidbody.velocity *= 0.97f;
        }


        if(Input.GetMouseButtonDown(0))
        {
            rigidbody.velocity = new Vector3(0, 0, 0);

            if (animator.GetInteger("Input") <= 5)
            {
                animator.SetInteger("Input", 20);
                _playerState = PlayerState.Attack;
            }

            else if (animator.GetInteger("Input") == 20
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
            {
                animator.SetInteger("Input", 21);
                _playerState = PlayerState.Attack;
            }

            else if (animator.GetInteger("Input") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
            {
                animator.SetInteger("Input", 22);
                _playerState = PlayerState.Attack;
            }

        }

        else if (animator.GetInteger("Input") == 20 
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
        }

        else if (animator.GetInteger("Input") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
        {
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
        }

        else if (animator.GetInteger("Input") == 22
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
        {
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
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

        Vector3 effectPos = gameObject.transform.position;
        effectPos += 0.33f * gameObject.transform.up + 0.5f * gameObject.transform.right + playerForward * movePower / 22500;
        attack1_effect.transform.position = effectPos;

        attack1_effect.transform.rotation = gameObject.transform.rotation;
        attack1_effect.transform.Rotate(0, -100, 0);
        attack1_effect.transform.parent = GameObject.Find("@Effect").transform;

        Destroy(attack1_effect, 0.75f*attack1_effect.GetComponent<ParticleSystem>().main.duration);


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
        attack2_effect.transform.position = gameObject.transform.position + 0.66f*gameObject.transform.up + playerForward * movePower / 22500;
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
        effectPos += 0.33f*gameObject.transform.right + 0.5f*gameObject.transform.up + playerForward * movePower / 22500;
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
        if ((_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Defend) || _playerState == PlayerState.Die)
            return;

        // 우클릭 시작
        if (Input.GetMouseButtonDown(1)
            && animator.GetInteger("Input") >= 0 && animator.GetInteger("Input") <=5 
            && PlayerStatus.Stamina > 10)
        {
            animator.SetInteger("Input", 30);
            _playerState = PlayerState.Defend;
            gameObject.GetComponent<PlayerStatusManager>().isDefending = true;
            gameObject.GetComponent<PlayerStatusManager>().noDefendTime = 0f;

            PlayerStatus.Stamina -= 10;
            parryingTime = 0.5f;
        }
        
        // 우클릭 유지 종료
        if((animator.GetInteger("Input") == 30 && !Input.GetMouseButton(1)) || PlayerStatus.Stamina <= 0)
        {
            gameObject.GetComponent<PlayerStatusManager>().isDefending = false;
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
            animator.Play("Idle_Battle");
            parryingTime = 0f;
            if(PlayerStatus.Stamina <= 0)
                PlayerStatus.Stamina = 0f;
        }

        // 우클릭 유지
        else if (animator.GetInteger("Input") == 30)
        {
            parryingTime -= Time.deltaTime;
            PlayerStatus.Stamina -= Time.deltaTime * 20;
        }
    }



    void Roll()
        /*
         * Shift로 구르기
         * 40 이상 70 미만
         * (+1) : 앞으로, (+4) 왼쪽으로, (+9) 오른쪽으로, (+16) 뒤로
         * 대각선은 두 숫자의 합
         */
    {
        remainRollCoolTime -= Time.deltaTime;

        if (_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Roll)
            return;


        if (Input.GetKeyDown(KeyCode.LeftShift) && remainRollCoolTime <= 0f)
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
            if(Input.GetKey(KeyCode.A))
            {
                rollDir += 4;
                rightForce = -30000;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rollDir += 9;

                rightForce = 30000;
            }
            // 전후
            if (Input.GetKey(KeyCode.S))
            {
                rollDir += 16;
                frontForce = -30000;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                rollDir += 1;
                frontForce = 30000;
            }

            if(frontForce != 0 && rightForce != 0)
            {
                frontForce /= Mathf.Sqrt(2);
                rightForce /= Mathf.Sqrt(2);
            }

            // 애니메이션 및 물리 적용
            if (!(frontForce == 0 && rightForce == 0))
            {
                _playerState = PlayerState.Roll;

                animator.SetInteger("Input", 40 + rollDir);
                rigidbody.AddForce(playerFront * frontForce);
                rigidbody.AddForce(playerRight * rightForce);

                remainRollCoolTime = rollCoolTime;
            }

        }


    }

    void OnRoll()
    {

    }

    void RollEnd()
        /*
         * 구르기가 끝나면 애니메이션에서 호출하는 함수
         */
    {
        _playerState = PlayerState.Idle;
        animator.SetInteger("Input", 0);
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
        if(_playerState == PlayerState.Die)
            yield break;
        
        // 경직 모션 적용
        _playerState = PlayerState.Hit;
        animator.SetInteger("Input", 100);

        // 경직 적용 및 회복
        yield return new WaitForSeconds(time);

        if (_playerState == PlayerState.Die)
            yield break;

        _playerState = PlayerState.Idle;
        animator.SetInteger("Input", 0);
    }

    public void Damaged(float time, Vector3 hitPoint, EnemyEffect _enemyEffect)
        /*
         * 피격에 대한 경직/모션/데미지 적용
         * - time : 경직 적용 시간
         * - damage : 피격 데미지
         */
    {

        // 구르기 중 피격하지 않음
        if (_playerState == PlayerState.Roll
            || _playerState == PlayerState.Die)
            return;

        // 방어에 대한 적용
        else if (_playerState == PlayerState.Defend)
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
                if(parryingTime >= 0)
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
        if (avoidance < PlayerStatus.avoidanceRate)
        {
            Debug.Log("회피!");
            return;
        }

        // 방어율
        float damage = _enemyEffect.CalculatedDamage();
        float guardRate = 50f * Mathf.Log(PlayerStatus.shield + 10) - 50f;
        damage *= (guardRate / 100);

        // 경직 적용
        StartCoroutine(DamagedRigidy(time));

       
        // 체력 감소 적용
        PlayerStatus.HP -= (int)damage;
        Debug.Log($"PlayerHP = {PlayerStatus.HP}");

        // 사망판정 검사
        if(PlayerStatus.HP <= 0)
        {
            Debug.Log("You Die");
            _playerState = PlayerState.Die;
            animator.SetInteger("Input", -999);
        }
    }

}
