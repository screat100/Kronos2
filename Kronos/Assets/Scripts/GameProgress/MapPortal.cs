using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPortal : PlayerInteract
{
    [SerializeField]
    string interactPopupMessage;

    [SerializeField]
    string destScene;

    [SerializeField]
    Vector3 destPos;


    public override void InteractPopup()
    {
        interactPopup.transform.Find("content_text").GetComponent<Text>().text = "이동하기";
    }

    public override void OnInteract()
    {
        SceneManager.LoadScene(destScene);
        GameObject.Find("Player").transform.position = destPos;

        interactPopup.SetActive(false);
    }

}
