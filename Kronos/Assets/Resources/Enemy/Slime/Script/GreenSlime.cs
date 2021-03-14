using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlime : MonoBehaviour
{
    private float MonsterHP;
    
    public float MoveSpeed = 1;
    public float AttackRange=1.2f;
    [SerializeField] float m_SightAngle = 0f;
    [SerializeField] float m_DetectDistance = 0f;
    [SerializeField] LayerMask m_layerMask = 0;

    GameObject player;
    Rigidbody rigidbody;
    // patrol 방향 정하는 bool
    bool Dir_forward = true;
    bool dir_lock = false;

    //Status -> 0:Patrol, 1:Chase, 2:Attack, 3:Dead, 4:GetHit
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Die,
        GetHit,
    }
    [System.NonSerialized]
    public EnemyState _enemyState;
    bool Detect = false;

    private Animator animator;

    // 이펙트
    [SerializeField]
    GameObject attackEffect;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        _enemyState = EnemyState.Patrol; //기본 상태 idle
        MonsterHP = gameObject.GetComponent<Enemy>().MonsterHP; //hp 설정
    }

    // Update is called once per frame
    void Update()
    {
        MonsterHP = gameObject.GetComponent<Enemy>().MonsterHP; //hp 설정
        Animation(); // 상태별 애니메이션 실행
        StartCoroutine(Select_dir());// 순찰 방향 결정 
    }

    IEnumerator Select_dir()
    {
        if (!dir_lock)
        {
            dir_lock = true;
            int r = Random.Range(1, 3);
            if (r % 2 == 0)
            {
                Dir_forward = true;
            }
            else
            {
                Dir_forward = false;
            }
            animator.SetBool("Sense", false);
            yield return new WaitForSeconds(4f);
            animator.SetBool("Sense", true);
            dir_lock = false;
        }
    }
    private void Animation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit")||_enemyState==EnemyState.Die)
        {
            animator.SetInteger("Status", (int)_enemyState);
        }
    }


    /* 
     * 몬스터 AI 트리에 사용되는 함수들 
     */
    public bool Patrol()
    {

        if (MonsterHP > 0 && !Detect)
        {
            /*
             * 몬스터 시야 전방 탐지범위안에 들어오면 탐지완료
             */

            Collider[] t_cols = Physics.OverlapSphere(transform.position, m_DetectDistance, m_layerMask);
            if (t_cols.Length > 0)
            {
                Transform t_tfPlayer = t_cols[0].transform;

                Vector3 t_direcrion = (t_tfPlayer.position - transform.position).normalized;
                float t_angle = Vector3.Angle(t_direcrion, transform.forward);

                //시야각에 잡히면
                if(t_angle <m_SightAngle * 0.5f){
                    if (Physics.Raycast(transform.position+Vector3.up, t_direcrion, out RaycastHit t_hit, m_DetectDistance))
                    {
                        if (t_hit.transform.CompareTag("Player"))
                        {
                            Debug.Log("Detect");
                            Detect = true;
                            return true;
                        }
                    }
                }
                //너무 가까이 왔으면(탐지거리에 1/3)
                else if(Physics.Raycast(transform.position + Vector3.up, t_direcrion, out RaycastHit t_hit, m_DetectDistance / 3))
                {
                    if (t_hit.transform.CompareTag("Player"))
                    {
                        Debug.Log("Detect");
                        Detect = true;
                        return true;
                    }
                }
            }

            /*
             * 순찰 status: 0
             * 좌우(랜덤) 일정시간 이동
             */
            //벽만나면 빠꾸
            if(Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 2f))
            {
                Debug.Log("빠꾸");
                Dir_forward = !Dir_forward;
            }

            _enemyState = EnemyState.Patrol;
            
            if(!Dir_forward)
            {
                transform.Rotate(Vector3.up * 180);
                Dir_forward = !Dir_forward;
            }
            Vector3 target = gameObject.transform.position + transform.forward;
            gameObject.transform.LookAt(target);
            transform.position = Vector3.MoveTowards(gameObject.transform.position, target, MoveSpeed * Time.deltaTime);

            return true;
        }
        return false;
    }
    public bool MonsterMove()
    {
        if (MonsterHP > 0&&Detect)
        {
            //걷기 status: 1
            _enemyState = EnemyState.Chase;
            Vector3 target = player.transform.position;
            target.y = gameObject.transform.position.y;

            gameObject.transform.LookAt(target);
            transform.position = Vector3.MoveTowards(gameObject.transform.position, target, MoveSpeed * Time.deltaTime);
            return true;
        }
        return false;
    }

    public bool Attack()
    {
        RaycastHit hit;
        Vector3 obj = transform.position + Vector3.up;
        if (Physics.Raycast(obj, transform.forward, out hit, AttackRange))
        {
            if (hit.collider.gameObject == player)
            {
                //공격 status: 2
                _enemyState = EnemyState.Attack;

                return true;
            }
        }
        return false;
    }
    public bool Dead()
    {
        if (MonsterHP <= 0)
        {
            //죽음 status: 3
            _enemyState = EnemyState.Die;
            animator.Play("Die");
            return false;
        }
        return true;
    }
  
    public bool GetHit()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit")&& MonsterHP>0)
        {
            //StartCoroutine(GetHitdelay(0.2f));
            Detect = true;
            return true;
        }
        return false;
    }
    IEnumerator GetHitdelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _enemyState = EnemyState.Chase;
    }
    void OnSlimeAttackEvent()
    {
        // 공격 이펙트
        GameObject _GreenSlimeAttackEffect = GameObject.Instantiate(attackEffect);
        _GreenSlimeAttackEffect.transform.parent = GameObject.Find("@Effect").transform;

        Vector3 effectPos = gameObject.transform.position + gameObject.transform.up * 0.33f + gameObject.transform.forward * 0.25f;
        _GreenSlimeAttackEffect.transform.position = effectPos;


        _GreenSlimeAttackEffect.transform.rotation = gameObject.transform.rotation;
        _GreenSlimeAttackEffect.transform.Rotate(0, 180, 0);


        Destroy(_GreenSlimeAttackEffect, _GreenSlimeAttackEffect.GetComponent<ParticleSystem>().main.duration);

        // 공격 사운드

    }


}
