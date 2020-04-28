using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Graph", menuName = "NodeGraph")]
public class NodeGraph : ScriptableObject
{
    [HideInInspector]public List<BaseNode> allNodes = new List<BaseNode>();

    [TextArea(1, 3)]
    public string description;
}
