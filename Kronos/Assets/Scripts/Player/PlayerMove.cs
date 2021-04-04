using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : CharacterMove
{
    float speedRate = 1.0f;

    private Rigidbody rigidbody;
    float maxSpeed = 5f;
    PlayerStatus m_PlayerStatus;
    Animator animator;

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

    // ��Ÿ�� ����
    float rollCoolTime = 1.5f;
    float remainRollCoolTime = 0f;
    float remainQSkillCool = 0f;
    float remainESkillCool = 0f;


    //���� ����
    [System.NonSerialized]
    public bool SwordSlashAttack;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        m_PlayerStatus = gameObject.GetComponent<PlayerStatus>();

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
            // ���� �ִϸ��̼�
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

        // ����Ʈ�� �� �Լ��� ���� ����� �޵��� �Ű��ּ���,,,
        if(other.transform.tag == "EnemyEffect")
        {
            // ����Ʈ�� �޷��ִ� ��ũ��Ʈ
            EnemyEffect _enemyEffect = other.GetComponent<EnemyEffect>();

            // ����Ʈ�� ��ǥ
            Vector3 hitPoint = other.transform.position;

            // ������ ��� �� ����
            Damaged(0.25f, hitPoint, _enemyEffect); //�����ð��� ���Ƿ� �����Ͽ���... ���� ���� �ٶ�

        }

        // Ʈ�� ���߿� ������ �з���,,, �̰͵� ����Ʈ�� �Ű��ּ���
        else if (other.transform.name == "TrapExplosion")
        {
            rigidbody.AddForce(
            other.GetComponent<TrapExplosion>().pushDir * other.GetComponentInParent<TrapExplosion>().pushPower);
        }
    }



    void PlayerMoveBase()
    {
        // �Է� ������ �ӵ� ����
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.95f,
            rigidbody.velocity.y,
            rigidbody.velocity.z * 0.95f);
    }


    void KeyboardMove()
        /*
         * WASD Ű�� ĳ���� (������ǥ) ������
         * 1 = ����
         * 2 = ��, 3 = ��� �ȱ�
         * 4 = ����
         */
    {
        if ((_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Jump
            && _playerState != PlayerState.Fall)
            ||(_playerState==PlayerState.Die))
            return;


        // ���� �̵��� ����...
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


        // �¿� �̵��� ���� ����
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

        // �밢�� ��츦 ����Ͽ�, ��� �������� ������ �ӵ��� ������ �Ѵ�.
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

        // �ڷ� ���� �� �̵��ӵ� 25% ����
        if(forceForward < 0)
        {
            backMovingCorrectionValue = 0.75f;
        }


        // �ִ�ӵ� ���� & ������ ����
        // : �ִ�ӵ��� ���� ���� ��쿡�� addForce ����
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
        if (_playerState == PlayerState.Die)
            return;

        // �߶�
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

        // ����
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
         * Input 20~22 : ���� �⺻���� 1Ÿ~3Ÿ
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
        attack2_effect.transform.position = gameObject.transform.position + 0.66f*gameObject.transform.up + playerForward * movePower / 22500;
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
        effectPos += 0.33f*gameObject.transform.right + 0.5f*gameObject.transform.up + playerForward * movePower / 22500;
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
        if ((_playerState != PlayerState.Idle
            && _playerState != PlayerState.Walk
            && _playerState != PlayerState.Defend) || _playerState == PlayerState.Die)
            return;

        // ��Ŭ�� ����
        if (Input.GetMouseButtonDown(1)
            && animator.GetInteger("Input") >= 0 && animator.GetInteger("Input") <=5 
            && m_PlayerStatus.Stamina > 10)
        {
            animator.SetInteger("Input", 30);
            _playerState = PlayerState.Defend;
            m_PlayerStatus.isDefending = true;
            m_PlayerStatus.noDefendTime = 0f;

            m_PlayerStatus.Stamina -= 10;
            parryingTime = 0.5f;
        }
        
        // ��Ŭ�� ���� ����
        if((animator.GetInteger("Input") == 30 && !Input.GetMouseButton(1)) || m_PlayerStatus.Stamina <= 0)
        {
            m_PlayerStatus.isDefending = false;
            animator.SetInteger("Input", 0);
            _playerState = PlayerState.Idle;
            animator.Play("Idle_Battle");
            parryingTime = 0f;
            if(m_PlayerStatus.Stamina <= 0)
                m_PlayerStatus.Stamina = 0f;
        }

        // ��Ŭ�� ����
        else if (animator.GetInteger("Input") == 30)
        {
            parryingTime -= Time.deltaTime;
            m_PlayerStatus.Stamina -= Time.deltaTime * 20;
        }
    }



    void Roll()
        /*
         * Shift�� ������
         * 40 �̻� 70 �̸�
         * (+1) : ������, (+4) ��������, (+9) ����������, (+16) �ڷ�
         * �밢���� �� ������ ��
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

            // �Է¿� ���� ���� ���
            // �¿�
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
            // ����
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

            // �ִϸ��̼� �� ���� ����
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
         * �����Ⱑ ������ �ִϸ��̼ǿ��� ȣ���ϴ� �Լ�
         */
    {
        _playerState = PlayerState.Idle;
        animator.SetInteger("Input", 0);
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
        if(_playerState == PlayerState.Die)
            yield break;
        
        // ���� ��� ����
        _playerState = PlayerState.Hit;
        animator.SetInteger("Input", 100);

        // ���� ���� �� ȸ��
        yield return new WaitForSeconds(time);

        if (_playerState == PlayerState.Die)
            yield break;

        _playerState = PlayerState.Idle;
        animator.SetInteger("Input", 0);
    }

    public void Damaged(float time, Vector3 hitPoint, EnemyEffect _enemyEffect)
        /*
         * �ǰݿ� ���� ����/���/������ ����
         * - time : ���� ���� �ð�
         * - damage : �ǰ� ������
         */
    {

        // ������ �� �ǰ����� ����
        if (_playerState == PlayerState.Roll
            || _playerState == PlayerState.Die)
            return;

        // �� ���� ����
        else if (_playerState == PlayerState.Defend)
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
                if(parryingTime >= 0)
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
        if (avoidance < m_PlayerStatus.avoidanceRate)
        {
            Debug.Log("ȸ��!");
            return;
        }

        // �����
        float damage = _enemyEffect.CalculatedDamage();
        float guardRate = 50f * Mathf.Log(m_PlayerStatus.shield + 10) - 50f;
        damage *= (guardRate / 100);

        // ���� ����
        StartCoroutine(DamagedRigidy(time));


        // ü�� ���� ����
        m_PlayerStatus.HP -= (int)damage;
        Debug.Log($"PlayerHP = {m_PlayerStatus.HP}");

        // ������� �˻�
        if(m_PlayerStatus.HP <= 0)
        {
            Debug.Log("You Die");
            _playerState = PlayerState.Die;
            animator.SetInteger("Input", -999);
        }
    }

}