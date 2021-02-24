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

    

    void Start()
    {
        InitList();
    }

    void InitList()
        /*
         * ui상의 리스트들을 배치
         */
    {
        RenewSumLevelOfType();

        for(int i=0; i<abilities.Count; i++)
        {
            GameObject SandGlass_list = GameObject.Instantiate(AbilityList);
            SandGlass_list.transform.parent = ScrollViewContent.transform;
            SandGlass_list.name = "SandGlass_list" + i.ToString();


            Text[] dataText = SandGlass_list.GetComponentsInChildren<Text>();
            //순서대로 type, tier, name, desc, level

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

            // 버튼 활성화 조건 체크
            Transform button = SandGlass_list.transform.Find("Button");
            button.GetComponent<Button>().interactable = false;

            int tier = int.Parse(dataText[1].text);

            switch(tier)
            {
                case 0:
                    button.GetComponent<Button>().interactable = true;
                    break;

                case 1:
                    // 1티어 : '시간 수거' 능력 10레벨 이상
                    if(int.Parse(abilities[0]["level"].ToString()) >= 10)
                    {
                        button.GetComponent<Button>().interactable = true;
                    }
                    break;

                case 2:
                    // 2티어 : 동일계열 능력 레벨 합 10 이상
                    int index = 10;
                    foreach(typeOfAbility t in System.Enum.GetValues(typeof(typeOfAbility)))
                    {
                        if(t.ToString() == abilities[i]["type"].ToString())
                        {
                            index = (int)t;
                            break;
                        }
                    }

                    if (SumLevelOfType[index] >= 10)
                    {
                        button.GetComponent<Button>().interactable = true;
                    }
                    break;

                case 3:
                    break;

                default:
                    break;
            }

            // 각 버튼에 onClick Listener 추가
            //button.GetComponent<Button>().onClick.AddListener(delegate { AbilityLevelUp(i); });
            button.GetComponent<Button>().onClick.AddListener(() => { AbilityLevelUp(i); });

            Debug.Log($"index = {i}");

        }
    }

    void AbilityLevelUp(int index)
    {
        index -= 39;
        Debug.Log($"index = {index}");

        int cost = int.Parse(abilities[index]["cost"].ToString());

        //if (cost > PlayerStatus.remainPoint)
        //    return;
        

        PlayerStatus.remainPoint -= cost;
        int level = int.Parse(abilities[index]["level"].ToString());
        level++;
        abilities[index]["level"] = level;
        DataSave();

        //PlayerStatus 에 적용 해주세요

        // ui 적용
        GameObject.Find("SandGlass_list" + index.ToString()).transform.Find("data_level").GetComponent<Text>().text = level.ToString();

        // 버튼 활성화 조건 재체크

    }


}
