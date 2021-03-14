using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }


    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    [SerializeField]
    List<GameObject> dontDestroyObj;

    void Start()
    {
        Init();

        // 마우스 커서 숨김 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Serialize로 가져온 오브젝트들
        for (int i=0; i<dontDestroyObj.Count; i++)
        {
            DontDestroyOnLoad(dontDestroyObj[i]);
        }
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        // Managers는 Empty Object로부터 가져오는데, 만약 해당 오브젝트가 없어서 불러올 수 없다면 새로 생성한다.
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");

            if(go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }
    }
}
