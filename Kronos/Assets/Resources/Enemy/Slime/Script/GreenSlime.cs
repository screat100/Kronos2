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

    //Status -> 0:idle, 1:Walk, 2:Attack, 3:Dead
    int Status = 0;
    bool Detect = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }
    void Start()
    {
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
            gameObject.GetComponent<Animator>().SetBool("Sense", false);
            yield return new WaitForSeconds(4f);
            gameObject.GetComponent<Animator>().SetBool("Sense", true);
            dir_lock = false;
        }
    }
    private void Animation()
    {
        gameObject.GetComponent<Animator>().SetInteger("Status", Status);
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
                Status = 0;
                Vector3 target = gameObject.transform.position + Vector3.left;

                gameObject.transform.LookAt(target);
                transform.position = Vector3.MoveTowards(gameObject.transform.position, target, MoveSpeed * Time.deltaTime);
            }
            else
            {
                Status = 0;
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
            Status = 1;
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
                Status = 2;
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
            Status = 3;
            return false;
        }
        return true;
    }
  
}
