using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlime : MonoBehaviour
{
    private float MonsterHP;
    
    public float MoveSpeed = 1;
    public float DetectRange=5f;
    public float AttackRange=1.2f;
    GameObject player;

    // patrol 방향 정하는 bool
    bool L_dir = true;
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

    Animator animator;

    // 이펙트
    [SerializeField]
    GameObject attackEffect;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
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
                L_dir = true;
            }
            else
            {
                L_dir = false;
            }
            animator.SetBool("Sense", false);
            yield return new WaitForSeconds(4f);
            animator.SetBool("Sense", true);
            dir_lock = false;
        }
    }
    private void Animation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"))
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
            RaycastHit hit;
            Vector3 obj = transform.position + Vector3.up;
            if (Physics.Raycast(obj, transform.forward, out hit, DetectRange))
            {
                if (hit.collider.gameObject == player)
                {
                    Debug.Log("Detect");
                    Detect = true;
                    return true;
                }
            }

            /*
             * 몬스터에게 너무 가까이 있어도 탐지// 거리는 DectectRange의 1/2
             */
            Vector3 mag = player.transform.position - gameObject.transform.position;
            if (Vector3.Magnitude(mag) <= DetectRange / 2f)
            {
                Detect = true;
                return true;
            }

            /*
             * 순찰 status: 0
             * 좌우(랜덤) 일정시간 이동
             */
            if (L_dir)
            {
                _enemyState = EnemyState.Patrol;
                Vector3 target = gameObject.transform.position + Vector3.left;

                gameObject.transform.LookAt(target);
                transform.position = Vector3.MoveTowards(gameObject.transform.position, target, MoveSpeed * Time.deltaTime);
            }
            else
            {
                _enemyState = EnemyState.Patrol;
                Vector3 target = gameObject.transform.position + Vector3.right;

                gameObject.transform.LookAt(target);
                transform.position = Vector3.MoveTowards(gameObject.transform.position, target, MoveSpeed * Time.deltaTime);
            }


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
            return false;
        }
        return true;
    }
  
    public bool GetHit()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"))
        {
            StartCoroutine(GetHitdelay(0.2f));
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
