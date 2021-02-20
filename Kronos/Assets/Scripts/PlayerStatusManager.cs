using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    bool timeFlowOn;

    public bool isDefending;
    public float noDefendTime;


    private void Awake()
    {
        PlayerStatus.init();
        timeFlowOn = true;
        isDefending = false;
    }

    private void Update()
    {
        if(timeFlowOn)
            PlayerStatus.timeRemain -= Time.deltaTime;

        if (!isDefending)
        {
            noDefendTime += Time.deltaTime;

            if(noDefendTime >= 3)
            {
                PlayerStatus.Stamina += 10*Time.deltaTime;
            }
        }
    }


}
