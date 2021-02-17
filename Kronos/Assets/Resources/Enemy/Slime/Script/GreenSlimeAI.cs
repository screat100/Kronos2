using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlimeAI : MonoBehaviour
{
    private Sequence root = new Sequence();
    private Selector selector = new Selector();
    private Selector selectPattern = new Selector();

    private Sequence seqAttack = new Sequence();
    private Sequence seqMove = new Sequence();
    private Sequence seqPatrol = new Sequence();

    private Sequence seqDead = new Sequence();


    private Dead m_Dead = new Dead();

    private Attack m_Attack = new Attack();
    private MonsterMove m_MonsterMove = new MonsterMove();
    private Patrol m_Patrol = new Patrol();

    private GreenSlime m_Slime;
    private IEnumerator behaviorProcess;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("슬라임");
        m_Slime = gameObject.GetComponent<GreenSlime>();
        root.AddChild(selector);
        selector.AddChild(seqDead);
        selector.AddChild(selectPattern);

        //노드
        m_Dead.Enemy = m_Slime;
        m_Attack.Enemy = m_Slime;
        m_MonsterMove.Enemy = m_Slime;
        m_Patrol.Enemy = m_Slime;

        seqDead.AddChild(m_Dead);

        selectPattern.AddChild(seqMove);
        selectPattern.AddChild(seqAttack);
        selectPattern.AddChild(seqPatrol);

        seqMove.AddChild(m_MonsterMove);
        seqAttack.AddChild(m_Attack);
        seqPatrol.AddChild(m_Patrol);
        

        behaviorProcess = BehaviorProcess();
        StartCoroutine(behaviorProcess);
    }
    public IEnumerator BehaviorProcess()
    {
        while (root.Invoke())
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.GetComponentInChildren<MeshCollider>().isTrigger = true;
        Destroy(gameObject, 5f);
        Debug.Log("슬라임 쥬금");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
