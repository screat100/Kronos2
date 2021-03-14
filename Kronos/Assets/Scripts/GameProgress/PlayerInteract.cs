using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상호작용이 가능한 오브젝트에 .cs 파일을 생성한 뒤,
 * 이 클래스를 상속하면
 * 'OnInteract()'함수 부분만 작성하면 된다.
 */

public abstract class PlayerInteract : MonoBehaviour
{
    protected GameObject interactPopup;

    bool canInteract;


    private void Start()
    {
        interactPopup = GameObject.Find("Canvas").transform.Find("Ingame").Find("Ingame_Interact").gameObject;
        interactPopup.SetActive(false);
        canInteract = false;
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract)
        {
            OnInteract();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // 플레이어 접촉 시 UI 활성화
        if(other.transform.tag == "Player")
        {
            InteractPopup();
            interactPopup.SetActive(true);
            canInteract = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        // 플레이어 exit = 비활성화
        if (other.transform.tag == "Player")
        {
            interactPopup.SetActive(false);
            canInteract = false;
        }
    }


    // 하위(자식) 클래스에서 구현

    // 상호작용 내용 변경
    public abstract void InteractPopup();

    // 상호작용 키 입력 시 실행
    public abstract void OnInteract();

}
