using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private Rigidbody rigidbody;
    float maxSpeed = 5f;
    float speedRate = 1.0f;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Debug.Log(gameObject.transform.forward);
    }

    void Update()
    {
        MouseRotation();
        KeyboardMove();
    }

    void MouseRotation()
    {
        float MouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up * MouseX);
    }

    void KeyboardMove()
    {
        // 전후 이동에 관한 변수
        float forceForward = 0f;

        if (Input.GetKey(KeyCode.W))
            forceForward = 100000f;
        else if (Input.GetKey(KeyCode.S))
            forceForward = -100000f;

        // 좌우 이동에 관한 변수
        float forceRight = 0f;

        if (Input.GetKey(KeyCode.A))
            forceRight = -100000f;
        else if (Input.GetKey(KeyCode.D))
            forceRight = 100000f;

        // 대각선 경우를 고려하여, 모든 방향으로 동일한 속도를 내도록 한다.
        if(forceForward != 0 && forceRight != 0)
        {
            forceForward = forceForward / Mathf.Sqrt(2);
            forceRight = forceRight / Mathf.Sqrt(2);
        }

        // 뒤로 걷을 때 이동속도 25% 감소
        if(forceForward < 0)
        {
            forceForward *= 0.75f;
        }


        // 물리에 적용
        Vector3 playerForward = gameObject.transform.forward;
        playerForward.y = 0;
        playerForward = playerForward.normalized;

        Vector3 playerRight = gameObject.transform.right;
        playerRight.y = 0;
        playerRight = playerRight.normalized;

        rigidbody.AddForce(playerForward * speedRate * forceForward * Time.deltaTime);
        rigidbody.AddForce(playerRight * speedRate * forceRight * Time.deltaTime);

        /*
        // 입력 없으면 속도 감소 (입력 있다면 늘어날테니까 적용 안될듯)
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.98f,
            rigidbody.velocity.y,
            rigidbody.velocity.z * 0.98f);
        */

        // 최대속도 제한
        if (rigidbody.velocity.x > maxSpeed * speedRate)
            rigidbody.velocity = new Vector3(maxSpeed * speedRate, rigidbody.velocity.y, rigidbody.velocity.z);

        else if (rigidbody.velocity.x < -maxSpeed * speedRate)
            rigidbody.velocity = new Vector3(-maxSpeed * speedRate, rigidbody.velocity.y, rigidbody.velocity.z);


        if (rigidbody.velocity.z > maxSpeed * speedRate)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, maxSpeed * speedRate);
        else if(rigidbody.velocity.z < -maxSpeed * speedRate)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, -maxSpeed * speedRate);

    }
}
