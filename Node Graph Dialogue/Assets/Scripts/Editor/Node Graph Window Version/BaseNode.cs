using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseNode : ScriptableObject
{
    public Rect nodeRect;
    public string windowTitle;

    private List<BaseNode> incomingNodes = new List<BaseNode>();
    private List<BaseNode> outgoingNodes = new List<BaseNode>();

    public virtual void CleanSelf()
    {
        foreach (BaseNode node in incomingNodes)
        {
            node.RemoveOutgoingNode(this);
        }
    }

    public virtual void DrawNode()
    {

    }

    public virtual void DrawCurve()
    {
        foreach (BaseNode node in outgoingNodes)
        {
            GraphWindow.DrawBezierCurve(nodeRect, node.nodeRect);
        }
    }

    public virtual void AddOutgoingNode(BaseNode n)
    {
        outgoingNodes.Add(n);
    }

    public virtual void RemoveOutgoingNode(BaseNode n)
    {
        outgoingNodes.Remove(n);
    }

    public virtual void AddIncomingNode(BaseNode n)
    {
        incomingNodes.Add(n);
    }

    public bool IsConnectedTo(BaseNode n)
    {
        return incomingNodes.Contains(n) || outgoingNodes.Contains(n);
    }
}
