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

    Player m_Player;


    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        int remainTime = (int)(m_Player.time_p);
        RemainTime_Slider.GetComponent<Image>().fillAmount = m_Player.time_p / m_Player.time;
        RemainTime_text.GetComponent<Text>().text = remainTime.ToString();

        HP_Slider.GetComponent<Image>().fillAmount = (float)m_Player.HP_p / (float)m_Player.HP;

        Stamina_Slider.GetComponent<Image>().fillAmount = m_Player.stamina_p / m_Player.stamina;

        SandGlass_Slider.GetComponent<Image>().fillAmount = m_Player.exp / m_Player.expMax;
        SandGlass_text.GetComponent<Text>().text = ((int)m_Player.level).ToString();
    }


}
