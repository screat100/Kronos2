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
        if(Player.GetComponent<Player>().state.ToString() ==  "Die")
        {
            return;
        }

        // X, Y�� ���콺 �Է¿� ���� AngleX, AngleY ����
        // X�� �������� �÷��̾� ĳ������ ȸ������ ����
        float MouseXInput = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitive * 100f;
        angleY -= Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitive * 100f;

        Player.transform.Rotate(Vector3.up, MouseXInput);


        // ���� ����
        if (angleY > 80)
            angleY = 80;

        else if (angleY < 0)
            angleY = 0;



        // Ÿ����ǥ�� �÷��̾� ��ǥ���� Y������ +1 (���ٴ��� �ٶ󺸴� ���� ����)
        Vector3 TargetPos = new Vector3(Player.transform.position.x,
            Player.transform.position.y + 1f,
            Player.transform.position.z);


        // ������ angleX, angleY�� ���� ī�޶� ��ġ ����
        float CamPosBack = radius * Mathf.Cos(angleY / 180 * 3.14f);
        float CamPosUp = radius * Mathf.Sin(angleY / 180 * 3.14f);
        Vector3 resultPos = TargetPos
            - Player.transform.forward * CamPosBack
            + Player.transform.up * CamPosUp;

        // ī�޶�� �÷��̾� ���̿� ���� ������ �Ÿ� ����
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
