using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineNode
{
    public Component Component;
    public Coroutine Routine;
    public IEnumerator Enumerator;
    public string Name;
}

public class MyCoroutineManager : SingletonMono<MyCoroutineManager>
{
    private Dictionary<Component, LinkedList<CoroutineNode>> mContainer = new Dictionary<Component, LinkedList<CoroutineNode>>();

    public Coroutine StartMyCoroutine(Component comp, IEnumerator enumerator, string name = "")
    {
        CoroutineNode node = new CoroutineNode();
        LinkedListNode<CoroutineNode> listNode = null;   
        if(mContainer.ContainsKey(comp))
        {
            listNode = mContainer[comp].AddLast(node);
        }
        else
        {
            mContainer[comp] = new LinkedList<CoroutineNode>();
            listNode = mContainer[comp].AddLast(node);
        }

        node.Component = comp;
        node.Enumerator = enumerator;
        node.Name = name;
        node.Routine = StartCoroutine(WrapperCorotine(listNode));
        return node.Routine;
    }

    private IEnumerator WrapperCorotine(LinkedListNode<CoroutineNode> node)
    {
        Component component = node.Value.Component;
        IEnumerator enumerator = node.Value.Enumerator;

        while(enumerator.MoveNext())
        {
            yield return enumerator.Current;

            if(component == null || component.gameObject == null || !component.gameObject.activeInHierarchy)
            {
                break;
            }
        }

        if(mContainer.ContainsKey(component))
        {
            LinkedList<CoroutineNode> list = mContainer[component];
            list.Remove(node);
            if(list.Count <= 0)
            {
                mContainer.Remove(component);
            }
        }
    }

    public void StopMyCoroutine(Component comp)
    {
        if (mContainer.ContainsKey(comp))
        {
            foreach (CoroutineNode node in mContainer[comp])
            {
                StopCoroutine(node.Routine);
            }

            mContainer.Remove(comp);
        }
    }
    public void StopMyCoroutine(Component comp, Coroutine routine)
    {
        if (mContainer.ContainsKey(comp))
        {
            LinkedList<CoroutineNode> list = mContainer[comp];
            var curNode = list.First;
            while(curNode != null)
            {
                CoroutineNode info = curNode.Value;
                if (info.Routine == routine)
                {
                    StopCoroutine(info.Routine);
                    list.Remove(curNode);
                    break;
                }
                curNode = curNode.Next;
            }

            if (list.Count <= 0)
            {
                mContainer.Remove(comp);
            }
        }
    }
    public void StopMyCoroutine(Component comp, string name)
    {
        if (mContainer.ContainsKey(comp))
        {
            LinkedList<CoroutineNode> list = mContainer[comp];
            var curNode = list.First;
            while (curNode != null)
            {
                CoroutineNode info = curNode.Value;
                if (info.Name.Equals(name))
                {
                    StopCoroutine(info.Routine);
                    list.Remove(curNode);
                    break;
                }
                curNode = curNode.Next;
            }

            if(list.Count <= 0)
            {
                mContainer.Remove(comp);
            }
        }
    }

}

