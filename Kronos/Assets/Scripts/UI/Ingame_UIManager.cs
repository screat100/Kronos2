using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingame_UIManager : MonoBehaviour
{
    public GameObject RemainTime_Slider;
    public GameObject RemainTime_text;

    public GameObject HP_Slider;

    public GameObject Stamina_Slider;

    public GameObject SandGlass_Slider;
    public GameObject SandGlass_text;

    PlayerStatus m_PlayerStatus;


    void Start()
    {
        m_PlayerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        int remainTime = (int)(m_PlayerStatus.timeRemain);
        RemainTime_Slider.GetComponent<Image>().fillAmount = m_PlayerStatus.timeRemain / m_PlayerStatus.timeMax;
        RemainTime_text.GetComponent<Text>().text = remainTime.ToString();

        HP_Slider.GetComponent<Image>().fillAmount = (float)m_PlayerStatus.HP / (float)m_PlayerStatus.HPMax;

        Stamina_Slider.GetComponent<Image>().fillAmount = m_PlayerStatus.Stamina / m_PlayerStatus.StaminaMax;

        SandGlass_Slider.GetComponent<Image>().fillAmount = m_PlayerStatus.exp / m_PlayerStatus.expMax;
        SandGlass_text.GetComponent<Text>().text = ((int)m_PlayerStatus.level).ToString();
    }


}
