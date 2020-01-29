using System;
using UnityEditor;
using UnityEngine;

namespace NodeDialogueSystem {
	public class ConsoleWindowClone : EditorWindow
	{
		private Rect upperPanel;
		private Rect lowerPanel;
		private Rect resizer;
		private Rect menuBar;

		private float sizeRatio = 0.5f;
		private bool isResizing = false;

		private float resizerHeight = 5f;
		private float menuBarHeight = 20f;

		private GUIStyle resizerStyle;

		[MenuItem("Window/Console Clone")]
		private static void OpenWindow()
		{
			ConsoleWindowClone window = GetWindow<ConsoleWindowClone>();
			window.titleContent = new GUIContent("Console Log Clone");
		}

		private void OnEnable()
		{
			resizerStyle = new GUIStyle();
			resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
		}

		private void OnGUI()
		{
			DrawMenuBar();
			DrawUpperPanel();
			DrawLowerPanel();
			DrawResizer();

			ProcessEvents(Event.current);

			if (GUI.changed)
				Repaint();
		}

		private void DrawMenuBar()
		{
			menuBar = new Rect(0, 0, position.width, menuBarHeight);

			GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
			GUILayout.EndArea();
		}

		private void DrawUpperPanel()
		{
			upperPanel = new Rect(0, 0, position.width, position.height * sizeRatio);

			GUILayout.BeginArea(upperPanel);
			GUILayout.Label("Upper Panel");
			GUILayout.EndArea();
		}

		private void DrawLowerPanel()
		{
			lowerPanel = new Rect(0, position.height * sizeRatio, position.width, position.height * (1 - sizeRatio) - menuBarHeight);

			GUILayout.BeginArea(lowerPanel);
			GUILayout.Label("Lower Panel");
			GUILayout.EndArea();
		}

		private void DrawResizer()
		{
			resizer = new Rect(0, (position.height * sizeRatio) - resizerHeight, position.width, 10f);

			GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up * resizerHeight), new Vector2(position.width, 2)), resizerStyle);
			GUILayout.EndArea();

			EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
		}

		private void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0 && resizer.Contains(e.mousePosition))
					{
						isResizing = true;
					}
					break;
				case EventType.MouseUp:
					isResizing = false;
					break;
			}

			Resize(e);
		}

		private void Resize(Event e)
		{
			if (isResizing)
			{
				sizeRatio = e.mousePosition.y / position.height;
				Repaint();
			}
		}
	}
}
