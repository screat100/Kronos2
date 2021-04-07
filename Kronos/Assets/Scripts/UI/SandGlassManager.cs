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
         * ui�� �ɷ� ����Ʈ���� ��ġ
         * - �� �ɷ� ����Ʈ �� �ؽ�Ʈ�� �ۼ�
         */
    {
        RenewSumLevelOfType();

        for(int i=0; i<abilities.Count; i++)
        {
            GameObject SandGlass_list = GameObject.Instantiate(AbilityList);
            SandGlass_list.transform.SetParent(ScrollViewContent.transform);
            SandGlass_list.name = "SandGlass_list" + i.ToString();


            //������� type, tier, name, desc, level
            Text[] dataText = SandGlass_list.GetComponentsInChildren<Text>();

            string type = abilities[i]["type"].ToString();
            string type_kor;

            // �ѱ۷� ����
            switch(type)
            {
                case "none":
                    type_kor = "-";
                    break;

                case "attack":
                    type_kor = "����";
                    break;

                case "health":
                    type_kor = "ü��";
                    break;

                case "skill":
                    type_kor = "��ų��ȭ";
                    break;

                case "critical":
                    type_kor = "ġ��";
                    break;

                case "shield":
                    type_kor = "���";
                    break;

                case "utility":
                    type_kor = "��ƿ��Ƽ";
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

            // �� ��ư�� onClick Listener �߰� �� �ڽ�Ʈ �ݿ�
            GameObject button = SandGlass_list.transform.Find("Button").gameObject;
            button.transform.Find("Text").GetComponent<Text>().text = abilities[i]["cost"].ToString();


            int temp = i; // i�� parameter�� ������ ���� �߻� (# Closure Problem)
            button.gameObject.GetComponent<Button>().onClick.AddListener(() => { AbilityLevelUp(temp); });
        }

        CheckButtonActivateCrieta();
    }

    bool CheckButtonActivateCrieta()
        /*
         * ��� �ɷµ��� ��ư Ȱ��ȭ ������ üũ
         * - ������ �𷡽ð� ���� �ش� �ɷ��� ��� �̻��̰�
         * - �� Ƽ� �ر� ������ ����������
         * - ���� ������ �ɷ� ������ �����ͷ��� �̸�
         */
    {
        for(int i=0; i<abilities.Count; i++)
        {
            // 1. �̸����� Child Object�� ã�´�
            Transform temp = ScrollViewContent.transform.Find("SandGlass_list" + i.ToString()).transform;

            // 2. �ش� object�� child ��ư�� ã�´�
            GameObject button = temp.Find("Button").gameObject;

            // 3. �⺻������ interact�� ����.
            button.GetComponent<Button>().interactable = false;

            // ���� �˻縦 ���� ������
            bool costCheck = false;
            bool tierCheck = false;
            bool masterCheck = false;

            // 4. ������� üũ
            if(m_Player.remainPoint >= int.Parse(abilities[i]["cost"].ToString()))
                costCheck = true;
            
            // 5. Ƽ�� ���� üũ
            int tier = int.Parse(abilities[i]["tier"].ToString());
            switch (tier)
            {
                case 0:
                    tierCheck = true;
                    break;

                case 1:
                    // 1Ƽ�� : '�ð� ����' �ɷ� 10���� �̻�
                    if (int.Parse(abilities[0]["level"].ToString()) >= 10)
                    {
                        tierCheck = true;
                    }
                    break;

                case 2:
                    // 2Ƽ�� : ���ϰ迭 �ɷ� ���� �� 10 �̻�
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

            // 6. �����ͷ��� ���� üũ
            int masterLevel = int.Parse(abilities[i]["maxLevel"].ToString());
            int nowLevel = int.Parse(abilities[i]["level"].ToString());
            if(nowLevel < masterLevel)
            {
                masterCheck = true;
            }

            // 7. �� ���� ��� ����� Ȱ��ȭ
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

        //PlayerStatus �� ���� ���ּ���

        // ui ����
        GameObject.Find("SandGlass_list" + index.ToString()).transform.Find("data_level").GetComponent<Text>().text = level.ToString();

        // ��ư Ȱ��ȭ ���� ��üũ
        CheckButtonActivateCrieta();

    }


}
