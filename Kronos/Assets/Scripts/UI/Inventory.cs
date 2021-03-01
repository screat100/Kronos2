using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : UIManager
{
    /*
     * 인벤토리 ui를 관리하는 스크립트
     * - 한 게임에서 획득한 아이템 및 스킬 정보를 가짐
     * - 
     */


    [SerializeField]
    GameObject itemList;

    [SerializeField]
    GameObject skillList;

    bool tapToItemList;

    void Start()
    {
        tapToItemList = true;
    }

    void Update()
    {
        
    }

    void OnClickTap(string tap)
    {
        switch (tap)
        {
            case "itemList":
                OpenUI(itemList);
                CloseUI(skillList);
                tapToItemList = true;
                break;

            case "skillList":
                OpenUI(skillList);
                CloseUI(itemList);
                tapToItemList = false;
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
