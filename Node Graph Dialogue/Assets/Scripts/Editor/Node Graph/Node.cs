using UnityEngine;
using UnityEditor;

namespace NodeDialogueSystem
{
	public class Node
	{
		public Rect rect;
		public string title;

		public bool isDragged;
		public bool isSelected;

		public ConnectionPoint inPoint;
		public ConnectionPoint outPoint;

		public GUIStyle style;
		public GUIStyle defaultNodeStyle;
		public GUIStyle selectedNodestyle;

		public Node(Vector2 position, float width, float height, 
			GUIStyle nodeStyle, GUIStyle selectedStyle,
			GUIStyle inPointStyle, GUIStyle outPointStyle, 
			System.Action<ConnectionPoint> OnClickInPoint, System.Action<ConnectionPoint> OnClickOutPoint)
		{
			rect = new Rect(position.x, position.y, width, height);
			style = nodeStyle;
			defaultNodeStyle = nodeStyle;
			selectedNodestyle = selectedStyle;
			inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
			outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
		}

		public void Drag(Vector2 delta)
		{
			rect.position += delta;
		}

		public void Draw()
		{
			inPoint.Draw();
			outPoint.Draw();
			GUI.Box(rect, title, style);
		}

		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						if (rect.Contains(e.mousePosition))
						{
							isDragged = true;
							isSelected = true;
							style = selectedNodestyle;
						}
						else
						{
							isSelected = false;
							style = defaultNodeStyle;
						}

						GUI.changed = true;
					}
					break;

				case EventType.MouseUp:
					isDragged = false;
					break;

				case EventType.MouseDrag:
					if (e.button == 0 && isDragged)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}
					break;
			}

			return false;
		}
	}
}