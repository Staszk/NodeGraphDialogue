using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace NodeDialogueSystem {
	public class NodeGraph : EditorWindow
	{
		private List<Node> nodes;

		private GUIStyle nodeStyle;

		[MenuItem("Window/DialogueGraph")]
		private static void OpenWindow()
		{
			NodeGraph window = GetWindow<NodeGraph>();
			window.titleContent = new GUIContent("Dialogue Node Graph");
		}

		private void OnEnable()
		{
			nodeStyle = new GUIStyle();
			nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
			nodeStyle.border = new RectOffset(12, 12, 12, 12);
		}

		private void OnGUI()
		{
			DrawNodes();

			ProcessNodeEvents(Event.current);
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

			nodes.Add(new Node(mousePosition, 200, 50, nodeStyle));
		}
	}
}
