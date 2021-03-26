using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Player;
    

    [SerializeField]
    float radius = 6f;

    float angleY = 15f;

    [SerializeField]
    [Range(1f, 10f)]
    float mouseSensitive = 1f;

    void Start()
    {
        angleY = 15f;

    }

    private void LateUpdate()
    {
        if(Player.GetComponent<PlayerMove>()._playerState.ToString() ==  "Die")
        {
            return;
        }

        // X, Y축 마우스 입력에 따라 AngleX, AngleY 조정
        // X축 움직임은 플레이어 캐릭터의 회전에도 적용
        float MouseXInput = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitive * 100f;
        angleY -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitive * 100f;

        Player.transform.Rotate(Vector3.up, MouseXInput);


        // 각도 제한
        if (angleY > 80)
            angleY = 80;

        else if (angleY < 0)
            angleY = 0;



        // 타겟좌표는 플레이어 좌표에서 Y축으로 +1 (땅바닥을 바라보는 현상 방지)
        Vector3 TargetPos = new Vector3(Player.transform.position.x,
            Player.transform.position.y + 1f,
            Player.transform.position.z);


        // 결정된 angleX, angleY에 따라 카메라 위치 조정
        float CamPosBack = radius * Mathf.Cos(angleY / 180 * 3.14f);
        float CamPosUp = radius * Mathf.Sin(angleY / 180 * 3.14f);
        Vector3 resultPos = TargetPos
            - Player.transform.forward * CamPosBack
            + Player.transform.up * CamPosUp;

        // 카메라와 플레이어 사이에 벽이 있으면 거리 조절
        Vector3 rayDir = (resultPos - TargetPos).normalized;
        RaycastHit hit;
        if (Physics.Raycast(TargetPos, rayDir, out hit, radius, LayerMask.GetMask("Wall")))
        {
            float distance = (hit.point - Player.transform.position).magnitude * 0.8f;

            CamPosBack = distance * Mathf.Cos(angleY / 180 * 3.14f);
            CamPosUp = distance * Mathf.Sin(angleY / 180 * 3.14f);

            resultPos = TargetPos
                - Player.transform.forward * CamPosBack
                + Player.transform.up * CamPosUp;
        }

        gameObject.transform.position = resultPos;
        gameObject.transform.LookAt(TargetPos);

    }

}
