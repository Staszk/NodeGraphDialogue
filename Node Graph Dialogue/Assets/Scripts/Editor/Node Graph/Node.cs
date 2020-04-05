using UnityEngine;
using UnityEditor;

namespace NodeDialogueSystem
{
	public class Node
	{
        private NodeGraph parent;

		public Rect rect;
        protected float width, height;
        public string title;

		public bool isDragged;
		public bool isSelected;

		public ConnectionPoint inPoint;
		public ConnectionPoint outPoint;

		public GUIStyle style;
		public GUIStyle defaultNodeStyle;
		public GUIStyle selectedNodeStyle;

		public Node(NodeGraph parent, Vector2 position)
		{
            this.parent = parent;
            width = 250;
            height = 300;

			rect = new Rect(position.x, position.y, width, height);

            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            style = defaultNodeStyle;

			inPoint = new ConnectionPoint(this, ConnectionPointType.In);
			outPoint = new ConnectionPoint(this, ConnectionPointType.Out);
		}

		public void Drag(Vector2 delta)
		{
			rect.position += delta;
		}

		public void Draw()
		{
			GUI.Box(rect, title, style);
            GUI.TextField(new Rect(rect.x + 30, rect.y + 30, 150, 30), "");
		}

        public void SelectThisNode(bool selected)
        {
            isSelected = selected;

            style = !selected ? defaultNodeStyle : selectedNodeStyle;
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
                            SelectThisNode(true);
						}
						else
						{
                            SelectThisNode(false);
                        }

						GUI.changed = true;
					}
                    else if (e.button == 1)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            SelectThisNode(true);
                            ProcessNodeMenu(e.mousePosition);

                            GUI.changed = true;
                        }
                        else
                        {
                            SelectThisNode(false);
                        }
                            
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

			return GUI.changed;
		}

        private void ProcessNodeMenu(Vector2 mousePos)
        {
            GenericMenu gMenu = new GenericMenu();
            gMenu.AddItem(new GUIContent("Remove Node"), false, () => parent.RemoveNode(this));
            gMenu.ShowAsContext();
        }
	}
}