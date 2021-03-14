using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager_fire07 : MonoBehaviour
{
    [SerializeField]
    List<GameObject> movingBlocks;


    [SerializeField]
    GameObject ExplosionParticle;

    [SerializeField]
    List<Vector3> ExplosionsPos;

    float time;
    float explosionTime;

    void Start()
    {
        time = 0;
        for(int i=0; i<movingBlocks.Count; i++)
        {
            movingBlocks[i].transform.position = new Vector3(
                movingBlocks[i].transform.position.x,
                0.001f,
                movingBlocks[i].transform.position.z
                );
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        explosionTime += Time.deltaTime;

        MoveBlock();
        Explosion();
    }

    void MoveBlock()
        /*
         * 2초간 대기,
         * 3초동안 상승,
         * 2초간 대기,
         * 3초동안 하강
         */
    {
        float moveDist = 0f;

        if (time >= 2f && time <= 5f)
        {
            moveDist += 2 * Time.deltaTime;
        }

        else if (time >= 5f && time <= 7f)
        {
            moveDist = 0f;
        }

        else if (time >= 7f && time <= 10f)
        {
            moveDist -= 2 * Time.deltaTime;
        }

        else if (time >= 10f)
        {
            time = 0f;
        }

        for (int i = 0; i < movingBlocks.Count; i++)
        {
            movingBlocks[i].transform.position = new Vector3(
                movingBlocks[i].transform.position.x,
                movingBlocks[i].transform.position.y + moveDist,
                movingBlocks[i].transform.position.z);
        }

    }

    void Explosion()
        /*
         * 6초마다 동시에 폭발 발생
         */
    {
        if(explosionTime >= 6f)
        {
            for (int i = 0; i < ExplosionsPos.Count; i++)
            {
                GameObject Explosion = GameObject.Instantiate(ExplosionParticle);
                Explosion.transform.name = "TrapExplosion";
                Explosion.transform.position = ExplosionsPos[i];
                Explosion.transform.parent = null;

                Destroy(Explosion, 1f);
            }

            explosionTime = 0f;
        }
    }
}
