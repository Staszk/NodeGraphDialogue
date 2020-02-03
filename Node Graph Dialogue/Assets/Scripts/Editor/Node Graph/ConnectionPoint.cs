using System;
using UnityEngine;

namespace NodeDialogueSystem
{
	public enum ConnectionPointType { In, Out }

	public class ConnectionPoint
	{
		public Rect rect;

		public ConnectionPointType type;

		public Node node;

		public GUIStyle style;

		public Action<ConnectionPoint> OnClickConnectionPoint;

		public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClick)
		{
			this.node = node;
			this.type = type;
			this.style = style;
			OnClickConnectionPoint = OnClick;
			rect = new Rect(0, 0, 10f, 10f);
		}

		public void Draw()
		{
			rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

			switch (type)
			{
				case ConnectionPointType.In:
					rect.x = node.rect.x - rect.width + 8f;
					break;
				case ConnectionPointType.Out:
					rect.x = node.rect.x + node.rect.width - 8f;
					break;
			}

			if (GUI.Button(rect, "", style))
			{
				OnClickConnectionPoint?.Invoke(this);
			}
		}
	}
}
