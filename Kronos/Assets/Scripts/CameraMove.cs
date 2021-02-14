using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Player;

    [SerializeField]
    float radius = 6f;

    float angleY = 90f;

    void Start()
    {
    }

    private void LateUpdate()
    {
        // 상하 각도
        angleY -= 15f*Input.GetAxis("Mouse Y");

        // 상하각도 제한
        if (angleY > 165f)
            angleY = 165f;
        else if (angleY < 10f)
            angleY = 10f;

        // 캐릭터가 바라보는 방향의 반대편에 카메라를 위치
        Vector3 playerForward = Player.transform.forward;
        playerForward.y = 0;
        playerForward = playerForward.normalized; // (x, 0, z), x^2 +z^2 = 1


        // 캐릭터와 카메라 사이에 물체가 위치하면 반지름 조정
        RaycastHit hit;
        if(Physics.Raycast(Player.transform.position, transform.position, out hit, radius, LayerMask.GetMask("Wall")))
        {
            float dist = (hit.point - Player.transform.position).magnitude * 0.8f;

            float planeRange = Mathf.Cos(angleY / 180f) * dist;
            transform.position = new Vector3(
                Player.transform.position.x - playerForward.x * planeRange,
                Player.transform.position.y + Mathf.Sin(angleY / 180f) * dist,
                Player.transform.position.z - playerForward.z * planeRange
                );
        }

        else
        {
            float planeRange = Mathf.Cos(angleY / 180f) * radius;
            transform.position = new Vector3(
                Player.transform.position.x - playerForward.x * planeRange,
                Player.transform.position.y + Mathf.Sin(angleY / 180f) * radius,
                Player.transform.position.z - playerForward.z * planeRange
                );
        }

        // 플레이어의 발 위치에서 y축으로 +1만큼의 위치를 쳐다본다.
        Vector3 LookTaregt = Player.transform.position;
        LookTaregt.y += 1f;
        transform.LookAt(LookTaregt);

    }

    void Update()
    {
        
    }
}
