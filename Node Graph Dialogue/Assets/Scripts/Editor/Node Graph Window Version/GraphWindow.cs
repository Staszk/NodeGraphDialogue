using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GraphWindow : EditorWindow
{
    public enum GraphActions { CreateDialogueNode, DeleteNode, CreateTransition, DeleteTransition, CancelTransition };

    private NodeGraph currentGraph;
    private Vector2 mousePos;
    private bool makingTransition = false;
    private bool nodeIsSelected;
    private BaseNode selectedNode;
    private BaseNode nodeAwaitingTransition;
    private Vector2 panStart;

    [MenuItem("Node Graph/DialogueGraph V2")]
    private static void OpenWindow()
    {
        GraphWindow graph = GetWindow<GraphWindow>();
        graph.minSize = new Vector2(800, 600);
    }

    private void OnGUI()
    {
        Event e = Event.current;
        mousePos = e.mousePosition;

        HandleInput(e);
        DrawNodes();
    }

    #region Draw
    private void DrawNodes()
    {
        GUILayout.BeginHorizontal();
        currentGraph = (NodeGraph)EditorGUILayout.ObjectField(currentGraph, typeof(NodeGraph), false);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        if (currentGraph != null)
        {
            List<BaseNode> nodes = currentGraph.allNodes;

            BeginWindows();

            foreach (BaseNode node in nodes)
            {
                node.DrawCurve();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].nodeRect = GUI.Window(i, nodes[i].nodeRect, DrawNodeWindow, nodes[i].windowTitle);
            }

            EndWindows();
        }
    }

    private void DrawNodeWindow(int id)
    {
        currentGraph.allNodes[id].DrawNode();
        GUI.DragWindow();
    }
    #endregion

    #region Input
    private void HandleInput(Event e)
    {
        // Right Click
        if (e.button == 1)
        {
            if (e.type == EventType.MouseDown)
            {
                RightClick(e);
            }
        }

        // Left Click
        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                //LeftClick(e);
            }
        }

        // Middle Click
        if (e.button == 2)
        {
            if (e.type == EventType.MouseDown)
            {
                panStart = e.mousePosition;
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (currentGraph != null) // if null, there is nothing to pan
                    HandlePanning(e);
            }
        }
    }

    private void HandlePanning(Event e)
    {
        Vector2 panThisFrame = e.mousePosition - panStart;
        panThisFrame *= 0.6f;

        for (int i = 0; i < currentGraph.allNodes.Count; i++)
        {
            currentGraph.allNodes[i].nodeRect.x += panThisFrame.x;
            currentGraph.allNodes[i].nodeRect.y += panThisFrame.y;
        }

        panStart = e.mousePosition;
    }

    private void RightClick(Event e)
    {
        selectedNode = null;
        nodeIsSelected = false;

        if (currentGraph != null)
        {
            for (int i = 0; i < currentGraph.allNodes.Count; i++)
            {
                if (currentGraph.allNodes[i].nodeRect.Contains(e.mousePosition))
                {
                    nodeIsSelected = true;
                    selectedNode = currentGraph.allNodes[i];
                    break;
                }
            }
        }

        if (!nodeIsSelected)
        {
            NewNodeCallbacks(e);
        }
        else
        {
            ExistingNodeCallbacks(e);
        }
    }

    private void LeftClick(Event e)
    {
        //throw new NotImplementedException();
    }

    private void ExistingNodeCallbacks(Event e)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Remove Node"), false, ClickContextCallback, GraphActions.DeleteNode);
        menu.AddSeparator("");
        

        if (!makingTransition)
        {
            menu.AddItem(new GUIContent("Transition/From Here"), false, ClickContextCallback, GraphActions.CreateTransition);
            menu.AddDisabledItem(new GUIContent("Transition/To Here"));
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Transition/From Here"));
            menu.AddItem(new GUIContent("Transition/To Here"), false, ClickContextCallback, GraphActions.CreateTransition);
        }

        menu.ShowAsContext();
        e.Use();
    }

    private void NewNodeCallbacks(Event e)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Dialogue Node"), false, ClickContextCallback, GraphActions.CreateDialogueNode);
        menu.AddSeparator("");
        if (!makingTransition)
        {
            menu.AddDisabledItem(new GUIContent("Cancel Transition"));
        }
        else
        {
            menu.AddItem(new GUIContent("Cancel Transition"), false, ClickContextCallback, GraphActions.CancelTransition);
        }

        menu.ShowAsContext();
        e.Use();
    }

    private void ClickContextCallback(object o)
    {
        GraphActions act = (GraphActions)o;

        switch (act)
        {
            case GraphActions.CreateDialogueNode:
                DialogueNode dialogueNode = CreateInstance<DialogueNode>();
                dialogueNode.nodeRect = new Rect(mousePos.x, mousePos.y, DialogueNode.width, DialogueNode.height);
                dialogueNode.windowTitle = DialogueNode.title;
                currentGraph.allNodes.Add(dialogueNode);
                break;
            case GraphActions.DeleteNode:
                if (selectedNode != null)
                {
                    selectedNode.CleanSelf();
                    currentGraph.allNodes.Remove(selectedNode);
                }
                break;
            case GraphActions.CreateTransition:
                if (!makingTransition)
                {
                    nodeAwaitingTransition = selectedNode;
                    makingTransition = true;
                }
                else
                {
                    if (!nodeAwaitingTransition.IsConnectedTo(selectedNode) && nodeAwaitingTransition != selectedNode)
                    {
                        selectedNode.AddIncomingNode(nodeAwaitingTransition);
                        nodeAwaitingTransition.AddOutgoingNode(selectedNode);
                        makingTransition = false;
                    }
                }
                break;
            case GraphActions.DeleteTransition:
                break;
            case GraphActions.CancelTransition:
                nodeAwaitingTransition = null;
                makingTransition = false;
                break;
            default:
                break;
        }
    }
    #endregion

    #region Helper

    public static void DrawBezierCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height * 0.5f, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height * .5f, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 3);

        if (Handles.Button((start.center + end.center) * 0.5f, Quaternion.identity, 4, 8, Handles.CircleHandleCap))
        {
            
        }
    }

    #endregion
}
