using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandGlassManager : PlayerSandGlass
{
    [SerializeField]
    GameObject AbilityList;

    [SerializeField]
    GameObject ScrollViewContent;

    Player m_Player;



    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();
        Debug.Log(m_Player.remainPoint);
        InitList();
    }

    void InitList()
        /*
         * ui상에 능력 리스트들을 배치
         * - 각 능력 리스트 내 텍스트들 작성
         */
    {
        RenewSumLevelOfType();

        for(int i=0; i<abilities.Count; i++)
        {
            GameObject SandGlass_list = GameObject.Instantiate(AbilityList);
            SandGlass_list.transform.SetParent(ScrollViewContent.transform);
            SandGlass_list.name = "SandGlass_list" + i.ToString();


            //순서대로 type, tier, name, desc, level
            Text[] dataText = SandGlass_list.GetComponentsInChildren<Text>();

            string type = abilities[i]["type"].ToString();
            string type_kor;

            // 한글로 변경
            switch(type)
            {
                case "none":
                    type_kor = "-";
                    break;

                case "attack":
                    type_kor = "공격";
                    break;

                case "health":
                    type_kor = "체력";
                    break;

                case "skill":
                    type_kor = "스킬강화";
                    break;

                case "critical":
                    type_kor = "치명";
                    break;

                case "shield":
                    type_kor = "방어";
                    break;

                case "utility":
                    type_kor = "유틸리티";
                    break;

                default:
                    type_kor = type;
                    break;
            }

            dataText[0].text = type_kor;
            dataText[1].text = abilities[i]["tier"].ToString();
            dataText[2].text = abilities_text[i]["name"].ToString();
            dataText[3].text = abilities_text[i]["desc"].ToString();
            dataText[4].text = abilities[i]["level"].ToString();

            // 각 버튼에 onClick Listener 추가 및 코스트 반영
            GameObject button = SandGlass_list.transform.Find("Button").gameObject;
            button.transform.Find("Text").GetComponent<Text>().text = abilities[i]["cost"].ToString();


            int temp = i; // i로 parameter를 넣으면 오류 발생 (# Closure Problem)
            button.gameObject.GetComponent<Button>().onClick.AddListener(() => { AbilityLevelUp(temp); });
        }

        CheckButtonActivateCrieta();
    }

    bool CheckButtonActivateCrieta()
        /*
         * 모든 능력들의 버튼 활성화 조건을 체크
         * - 보유한 모래시계 수가 해당 능력의 비용 이상이고
         * - 각 티어별 해금 조건을 충족했으며
         * - 현재 보유한 능력 레벨이 마스터레벨 미만
         */
    {
        for(int i=0; i<abilities.Count; i++)
        {
            // 1. 이름으로 Child Object를 찾는다
            Transform temp = ScrollViewContent.transform.Find("SandGlass_list" + i.ToString()).transform;

            // 2. 해당 object의 child 버튼을 찾는다
            GameObject button = temp.Find("Button").gameObject;

            // 3. 기본적으로 interact를 끈다.
            button.GetComponent<Button>().interactable = false;

            // 조건 검사를 위한 변수들
            bool costCheck = false;
            bool tierCheck = false;
            bool masterCheck = false;

            // 4. 비용조건 체크
            if(m_Player.remainPoint >= int.Parse(abilities[i]["cost"].ToString()))
                costCheck = true;
            
            // 5. 티어 조건 체크
            int tier = int.Parse(abilities[i]["tier"].ToString());
            switch (tier)
            {
                case 0:
                    tierCheck = true;
                    break;

                case 1:
                    // 1티어 : '시간 수거' 능력 10레벨 이상
                    if (int.Parse(abilities[0]["level"].ToString()) >= 10)
                    {
                        tierCheck = true;
                    }
                    break;

                case 2:
                    // 2티어 : 동일계열 능력 레벨 합 10 이상
                    int index = 10;
                    foreach (typeOfAbility t in System.Enum.GetValues(typeof(typeOfAbility)))
                    {
                        if (t.ToString() == abilities[i]["type"].ToString())
                        {
                            index = (int)t;
                            break;
                        }
                    }

                    if (SumLevelOfType[index] >= 10)
                    {
                        tierCheck = true;
                    }
                    break;

                case 3:
                    break;

                default:
                    break;
            }

            // 6. 마스터레벨 조건 체크
            int masterLevel = int.Parse(abilities[i]["maxLevel"].ToString());
            int nowLevel = int.Parse(abilities[i]["level"].ToString());
            if(nowLevel < masterLevel)
            {
                masterCheck = true;
            }

            // 7. 세 조건 모두 통과시 활성화
            if (costCheck && tierCheck && masterCheck)
                button.GetComponent<Button>().interactable = true;
        }


        return true;
    }


    void AbilityLevelUp(int index)
    {
        Debug.Log($"index = {index}");

        int cost = int.Parse(abilities[index]["cost"].ToString());
        m_Player.remainPoint -= cost;
        Debug.Log(m_Player.remainPoint);

        int level = int.Parse(abilities[index]["level"].ToString());
        level++;
        abilities[index]["level"] = level;
        DataSave();

        //PlayerStatus 에 적용 해주세요

        // ui 적용
        GameObject.Find("SandGlass_list" + index.ToString()).transform.Find("data_level").GetComponent<Text>().text = level.ToString();

        // 버튼 활성화 조건 재체크
        CheckButtonActivateCrieta();

    }


}
