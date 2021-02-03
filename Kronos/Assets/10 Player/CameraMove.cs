using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Player;

    float radius = 6f;
    float angle = 90f;

    void Start()
    {
    }

    private void FixedUpdate()
    {

        // Camera position
        angle -= 15f*Input.GetAxis("Mouse Y");

        if (angle > 165f)
            angle = 165f;
        else if (angle < 0f)
            angle = 0f;


        Vector3 playerForward = Player.transform.forward;
        playerForward.y = 0;
        playerForward = playerForward.normalized; // (x, 0, z), x^2 +z^2 = 1

        float planeRange = Mathf.Cos(angle / 180f) * radius;

        gameObject.transform.position = new Vector3(
            Player.transform.position.x - playerForward.x * planeRange,
            Mathf.Sin(angle / 180f) * radius,
            Player.transform.position.z - playerForward.z * planeRange
            );

        gameObject.transform.LookAt(Player.transform.position);

        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 1f,
            gameObject.transform.position.z);

    }

    void Update()
    {
        
    }
}
