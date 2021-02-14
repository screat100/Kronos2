using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    bool timeFlowOn;

    private void Awake()
    {
        PlayerStatus.init();
        timeFlowOn = true;
    }

    private void Update()
    {
        if(timeFlowOn)
            PlayerStatus.timeRemain -= Time.deltaTime;
    }


}
