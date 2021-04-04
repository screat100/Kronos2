using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyInformation : UIManager
{
    /*
     * 내 정보 ui를 관리하는 스크립트
     * - 보유한 레벨과 point를 실시간 갱신
     * - 플레이어 스탯 정보를 표기
     * - 모래시계 또는 성물 탭을 이동
     * - 모래시계 : 
     * - 성물 :
     * - [닫기] 누르면 UI 끄기
     */

    bool tapToSandglass;

    [SerializeField]
    GameObject MyInformation_Taps_SandGlass;

    [SerializeField]
    GameObject MyInformation_Taps_HolyThings;

    Text[] numbers; //스테이터스. 순서대로 시간,공격력,방어력,공격속도,이동속도,치확,치뎀,회피,쿨감
    Text remain; // 남은 포인트
    Button[] taps; //탭 버튼
    Button closer;

    PlayerStatus m_PlayerStatus;

    private void Start()
    {
        tapToSandglass = true;

        m_PlayerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();

        remain = GameObject.Find("MyInformation_remain_text").GetComponent<Text>();
        numbers = GameObject.Find("MyInformation_ability_number").transform.GetComponentsInChildren<Text>();
        taps = GameObject.Find("MyInformation_Taps_tapButtons").transform.GetComponentsInChildren<Button>();
        closer = GameObject.Find("MyInformation_close").GetComponent<Button>();

        taps[0].onClick.AddListener(delegate { OnClickTap("SandGlass"); });
        taps[1].onClick.AddListener(delegate { OnClickTap("HolyThings"); });
        closer.onClick.AddListener(Close);

        RefreshInformation();
    }

    private void OnEnable()
    {
        // 활성화됐을 때 자동으로 탭 중 하나를 결정
        if(tapToSandglass)
        {
            OpenUI(MyInformation_Taps_SandGlass);
            CloseUI(MyInformation_Taps_HolyThings);
        }

        else
        {
            OpenUI(MyInformation_Taps_HolyThings);
            CloseUI(MyInformation_Taps_SandGlass);
        }

        Time.timeScale = 0;
    }

    public void RefreshInformation()
    {
        // (남은 포인트)/(레벨)
        remain.text = m_PlayerStatus.remainPoint.ToString() + " / " + m_PlayerStatus.level.ToString();

        numbers[0].text = m_PlayerStatus.timeMax.ToString();
        numbers[1].text = m_PlayerStatus.attack.ToString();
        numbers[2].text = m_PlayerStatus.shield.ToString();
        numbers[3].text = m_PlayerStatus.attackSpeed.ToString();
        numbers[4].text = m_PlayerStatus.moveSpeed.ToString();
        numbers[5].text = m_PlayerStatus.criticalProb.ToString();
        numbers[6].text = m_PlayerStatus.criticalDamage.ToString();
        numbers[7].text = m_PlayerStatus.avoidanceRate.ToString();
        numbers[8].text = m_PlayerStatus.coolTimeDecreaseRate.ToString();
    }

    void OnClickTap(string tap)
    {
        switch(tap)
        {
            case "SandGlass":
                OpenUI(MyInformation_Taps_SandGlass);
                CloseUI(MyInformation_Taps_HolyThings);
                tapToSandglass = true;
                break;

            case "HolyThings":
                OpenUI(MyInformation_Taps_HolyThings);
                CloseUI(MyInformation_Taps_SandGlass);
                tapToSandglass = false;
                break;

            default:
                Debug.Log("wrong tap name");
                break;
        }
    }

    void Close()
    {
        Time.timeScale = 1f;
        CloseUI(gameObject);
    }
}
