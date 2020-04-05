using System;
using UnityEngine;

namespace NodeDialogueSystem
{
	public enum ConnectionPointType { In, Out }

	public class ConnectionPoint
	{
		public ConnectionPointType type;

		public Node node;
        public Rect rect;

		public GUIStyle style;

		public ConnectionPoint(Node node, ConnectionPointType type)
		{
			this.node = node;
			this.type = type;
			rect = new Rect(0, 0, 10f, 10f);
		}
	}
}
