using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node
{
    public abstract bool Invoke();
}
public class CompositeNode : Node
{
    public override bool Invoke()
    {
        throw new NotImplementedException();
    }

    public void AddChild(Node node)
    {
        childrens.Push(node);
    }

    public Stack<Node> GetChildrens()
    {
        return childrens;
    }
    private Stack<Node> childrens = new Stack<Node>();
}

public class Selector : CompositeNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke())
            {

                return true;
            }
        }
        return false;
    }
}

public class Sequence : CompositeNode
{
    public override bool Invoke()
    {
        bool p = false;
        foreach (var node in GetChildrens())
        {
            if (node.Invoke() == false)
            {
                p = true;
            }
        }
        return !p;
    }
}

public class Dead : Node
{
    public GreenSlime Enemy
    {
        set { _Enemy = value; }
    }
    private GreenSlime _Enemy;
    public override bool Invoke()
    {
        return _Enemy.Dead();
    }
}

public class MonsterMove : Node
{
    public GreenSlime Enemy
    {
        set { _Enemy = value; }
    }
    private GreenSlime _Enemy;
    public override bool Invoke()
    {
        return _Enemy.MonsterMove();
    }
}
public class Attack : Node
{
    public GreenSlime Enemy
    {
        set { _Enemy = value; }
    }
    private GreenSlime _Enemy;
    public override bool Invoke()
    {
        return _Enemy.Attack();
    }
}

public class Patrol : Node
{
    public GreenSlime Enemy
    {
        set { _Enemy = value; }
    }
    private GreenSlime _Enemy;
    public override bool Invoke()
    {
        return _Enemy.Patrol();
    }
}