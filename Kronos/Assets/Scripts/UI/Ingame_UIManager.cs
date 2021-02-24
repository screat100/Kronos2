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


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int remainTime = (int)(PlayerStatus.timeRemain);
        RemainTime_Slider.GetComponent<Image>().fillAmount = PlayerStatus.timeRemain / PlayerStatus.timeMax;
        RemainTime_text.GetComponent<Text>().text = remainTime.ToString();

        HP_Slider.GetComponent<Image>().fillAmount = (float)PlayerStatus.HP / (float)PlayerStatus.HPMax;

        Stamina_Slider.GetComponent<Image>().fillAmount = PlayerStatus.Stamina / PlayerStatus.StaminaMax;

        SandGlass_Slider.GetComponent<Image>().fillAmount = PlayerStatus.exp / PlayerStatus.expMax;
        SandGlass_text.GetComponent<Text>().text = ((int)PlayerStatus.level).ToString();

    }
}
