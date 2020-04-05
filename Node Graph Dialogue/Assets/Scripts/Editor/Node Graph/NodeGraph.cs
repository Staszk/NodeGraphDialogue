using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace NodeDialogueSystem {
	public class NodeGraph : EditorWindow
	{
		private List<Node> nodes;
		private List<Connection> connections;

        private Node selectedNode;
		private ConnectionPoint selectedInPoint;
		private ConnectionPoint selectedOutPoint;

		[MenuItem("Node Graph/DialogueGraph")]
		private static void OpenWindow()
		{
			NodeGraph window = GetWindow<NodeGraph>();
			window.titleContent = new GUIContent("Dialogue Node Graph");
		}

		private void OnEnable()
		{

		}

		private void OnGUI()
		{
			DrawNodes();
			DrawConnections();

            ProcessNodeEvents(Event.current);

            if (!GUI.changed)
			    ProcessEvents(Event.current);

			if (GUI.changed) Repaint();
		}

		private void DrawNodes()
		{
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					nodes[i].Draw();
				}
			}
		}

		private void DrawConnections()
		{
			if (connections != null)
			{
				for (int i = 0; i < connections.Count; i++)
				{
					connections[i].Draw();
				}
			}
		}

		private void ProcessNodeEvents(Event e)
		{
			if (nodes != null)
			{
				for (int i = nodes.Count - 1; i >= 0; --i)
				{
					bool guiChanged = nodes[i].ProcessEvents(e);

					if (guiChanged)
					{
						GUI.changed = true;
					}
				}
			}
		}

		private void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 1)
					{
						ProcessContextMenu(e.mousePosition);
					}
					break;
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.Escape:
                            break;
                        default:
                            break;
                    }
                    break;
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu gMenu = new GenericMenu();
			gMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
			gMenu.ShowAsContext();
		}

		private void OnClickAddNode(Vector2 mousePosition)
		{
			if (nodes == null)
			{
				nodes = new List<Node>();
			}

			nodes.Add(new Node(this, mousePosition));
		}

        public void RemoveNode(Node toBeDeleted)
        {
            nodes.Remove(toBeDeleted);
        }

		private void OnClickInPoint(ConnectionPoint inPoint)
		{
			selectedInPoint = inPoint;

			if (selectedOutPoint != null)
			{
				if (selectedOutPoint.node != selectedInPoint.node)
				{
					CreateConnection();
					ClearConnectionSelection();
				}
				else
				{
					ClearConnectionSelection();
				}
			}
		}

		private void OnClickOutPoint(ConnectionPoint outPoint)
		{
			selectedOutPoint = outPoint;

			if (selectedInPoint != null)
			{
				if (selectedOutPoint.node != selectedInPoint.node)
				{
					CreateConnection();
					ClearConnectionSelection();
				}
				else
				{
					ClearConnectionSelection();
				}
			}
		}

		private void OnClickRemoveConnection(Connection connection)
		{
			connections.Remove(connection);
		}

		private void CreateConnection()
		{
			if (connections == null)
			{
				connections = new List<Connection>();
			}

			connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
		}

		private void ClearConnectionSelection()
		{
			selectedInPoint = null;
			selectedOutPoint = null;
		}
	}
}
