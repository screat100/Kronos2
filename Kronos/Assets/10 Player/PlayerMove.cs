using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float speedRate = 1.0f;

    private Rigidbody rigidbody;
    float maxSpeed = 5f;
    bool canJump = true;
    bool canMove = true;
    bool canAttack = true;

    Animator animator;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
    }

    void Update()
    {
        MouseRotation();
        if(canMove)
            KeyboardMove();
        Jump();
        Attack();
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "ground")
        {
            canJump = true;
            canAttack = true;

            // 착지 애니메이션
            if (animator.GetInteger("act") == 11)
            {
                animator.SetInteger("act", 12);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            if(canMove)
                canJump = true;

            // 착지 애니메이션
            if (animator.GetInteger("act") == 11 || animator.GetInteger("act") == 10)
            {
                animator.SetInteger("act", 12);
            }
            
            
            if (animator.GetInteger("act") == 12 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                animator.SetInteger("act", 0);
            }
            

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            if(canJump)
            {
                animator.SetInteger("act", 11);
            }

            canJump = false;
            canAttack = false;
        }
    }

    void MouseRotation()
        /*
         * 마우스 수평 움직임으로 캐릭터 회전
         */
    {
        float MouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up * MouseX);
    }

    void KeyboardMove()
    /*
     * WASD 키로 캐릭터 (수평좌표) 움직임
     */
    {
        // 전후 이동에 관한...
        float forceForward = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            forceForward = 100000f;
            if(animator.GetInteger("act")<=5)
                animator.SetInteger("act", 1);
        }
        else if (Input.GetKey(KeyCode.S)) 
        { 
            forceForward = -100000f;
            if (animator.GetInteger("act") <= 5)
                animator.SetInteger("act", 4);
        }


        // 좌우 이동에 관한 변수
        float forceRight = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            forceRight = -100000f;
            if (animator.GetInteger("act") <= 5)
                animator.SetInteger("act", 2);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            forceRight = 100000f;
            if (animator.GetInteger("act") <= 5)
                animator.SetInteger("act", 3);
        }

        // 대각선 경우를 고려하여, 모든 방향으로 동일한 속도를 내도록 한다.
        if(forceForward != 0 && forceRight != 0)
        {
            forceForward = forceForward / Mathf.Sqrt(2);
            forceRight = forceRight / Mathf.Sqrt(2);
        }

        else if (forceForward == 0 && forceRight == 0 && animator.GetInteger("act") <= 4)
        {
            animator.SetInteger("act", 0);
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


        // 입력 없으면 속도 감소
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.97f,
            rigidbody.velocity.y,
            rigidbody.velocity.z * 0.97f);
        
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rigidbody.AddForce(new Vector3(0, 1, 0) * 5000f);
            animator.SetInteger("act", 10);
            Debug.Log("jump");
        }

        // 추락
        if (rigidbody.velocity.y < -0.005f && (animator.GetInteger("act") == 10 || animator.GetInteger("act") <= 5))
        {
            animator.SetInteger("act", 11);
        }
    }

    void MoveWithAttack(Vector3 playerForward)
    {
        // 공격 중 움직임을 더함
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(playerForward * 7500);
        }

        else if (Input.GetKey(KeyCode.S)) { }

        else
        {
            rigidbody.AddForce(playerForward * 1500);
        }
    }

    void Attack()
    {
        if(animator.GetInteger("act") >=20 && animator.GetInteger("act") <= 22 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            rigidbody.velocity *= 0.97f;
            canJump = false;
            canMove = false;
        }


        if(Input.GetMouseButtonDown(0) && canAttack)
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
            Vector3 playerForward = gameObject.transform.forward;
            playerForward.y = 0;

            if (animator.GetInteger("act") <= 5)
            {
                animator.SetInteger("act", 20);
                MoveWithAttack(playerForward);
            }

            else if (animator.GetInteger("act") == 20
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
            {
                animator.SetInteger("act", 21);
                MoveWithAttack(playerForward);
            }

            else if (animator.GetInteger("act") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
            {
                animator.SetInteger("act", 22);
                MoveWithAttack(playerForward);
            }




        }

        else if (animator.GetInteger("act") == 20 
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack01"))
        {
            animator.SetInteger("act", 0);
            canJump = true;
            canMove = true;
        }

        else if (animator.GetInteger("act") == 21
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
        {
            animator.SetInteger("act", 0);
            canJump = true;
            canMove = true;
        }

        else if (animator.GetInteger("act") == 22
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack03"))
        {
            animator.SetInteger("act", 0);
            canJump = true;
            canMove = true;
        }
    }
}
