using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    

    protected void OpenUI(GameObject go)
    {
        go.SetActive(true);
    }

    protected void CloseUI(GameObject go)
    {
        go.SetActive(false);
    }

}
