using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeDialogueSystem {

	public class Log
	{
		public bool isSelected;
		public string info;
		public string message;
		public LogType type;

		public Log(bool isSelected, string info, string message, LogType type)
		{
			this.isSelected = isSelected;
			this.info = info;
			this.message = message;
			this.type = type;
		}
	}

	public class ConsoleWindowClone : EditorWindow
	{
		private Rect upperPanel;
		private Rect lowerPanel;
		private Rect resizer;
		private Rect menuBar;

		private float sizeRatio = 0.75f;
		private bool isResizing = false;

		private float resizerHeight = 5f;
		private float menuBarHeight = 20f;

		private bool collapse = false;
		private bool clearOnPlay = false;
		private bool clearOnBuild = false;
		private bool pauseOnError = false;
		private bool showLog = false;
		private bool showWarnings = false;
		private bool showErrors = false;

		private Texture2D boxBgOdd;
		private Texture2D boxBgEven;
		private Texture2D boxBgSelected;
		private Texture2D icon;
		private Texture2D errorIcon;
		private Texture2D errorIconSmall;
		private Texture2D warningIcon;
		private Texture2D warningIconSmall;
		private Texture2D infoIcon;
		private Texture2D infoIconSmall;

		private static Texture titleImage;

		private Vector2 upperPanelScroll;
		private Vector2 lowerPanelScroll;

		private GUIStyle resizerStyle;
		private GUIStyle boxStyle;
		private GUIStyle textAreaStyle;

		private List<Log> logs;
		private Log selectedLog;

		[MenuItem("Window/Console Clone")]
		private static void OpenWindow()
		{
			if (titleImage == null)
				titleImage = EditorGUIUtility.Load("icons/UnityEditor.ConsoleWindow.png") as Texture;

			ConsoleWindowClone window = GetWindow<ConsoleWindowClone>();
			window.titleContent = new GUIContent("Console Clone", titleImage);
		}

		private void OnEnable()
		{
			resizerStyle = new GUIStyle();
			resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

			boxStyle = new GUIStyle();
			boxStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f);

			boxBgOdd = EditorGUIUtility.Load("builtin skins/lightskin/images/cn entrybackodd.png") as Texture2D;
			boxBgEven = EditorGUIUtility.Load("builtin skins/lightskin/images/cnentrybackeven.png") as Texture2D;
			boxBgSelected = EditorGUIUtility.Load("builtin skins/lightskin/images/menuitemhover.png") as Texture2D;

			textAreaStyle = new GUIStyle();
			textAreaStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f);
			textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;

			errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
			warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D;
			infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D;

			errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D;
			warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D;
			infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D;

			logs = new List<Log>();
			selectedLog = null;

			Application.logMessageReceived += LogMessageReceived;
		}

		private void OnDisable()
		{
			Application.logMessageReceived -= LogMessageReceived;
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
			GUILayout.BeginHorizontal();

			GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton, GUILayout.Width(40));
			collapse = GUILayout.Toggle(collapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.Width(60));
			clearOnPlay = GUILayout.Toggle(clearOnPlay, new GUIContent("Clear on Play"), EditorStyles.toolbarButton, GUILayout.Width(85));
			clearOnBuild = GUILayout.Toggle(clearOnBuild, new GUIContent("Clear on Build"), EditorStyles.toolbarButton, GUILayout.Width(85));
			pauseOnError = GUILayout.Toggle(pauseOnError, new GUIContent("Error Pause"), EditorStyles.toolbarButton, GUILayout.Width(75));

			GUILayout.FlexibleSpace();

			showLog = GUILayout.Toggle(showLog, new GUIContent("L", infoIconSmall), EditorStyles.toolbarButton, GUILayout.Width(35));
			showWarnings = GUILayout.Toggle(showWarnings, new GUIContent("W", warningIconSmall), EditorStyles.toolbarButton, GUILayout.Width(35));
			showErrors = GUILayout.Toggle(showErrors, new GUIContent("E", errorIconSmall), EditorStyles.toolbarButton, GUILayout.Width(35));

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawUpperPanel()
		{
			upperPanel = new Rect(0, menuBarHeight, position.width, (position.height * sizeRatio) - menuBarHeight);

			GUILayout.BeginArea(upperPanel);
			upperPanelScroll = GUILayout.BeginScrollView(upperPanelScroll);

			for (int i = 0; i < logs.Count; i++)
			{
				if (DrawBox(logs[i].info, logs[i].type, i % 2 == 0, logs[i].isSelected))
				{
					if (selectedLog != null)
					{
						selectedLog.isSelected = false;
					}

					logs[i].isSelected = true;
					selectedLog = logs[i];
					GUI.changed = true;
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void DrawLowerPanel()
		{
			lowerPanel = new Rect(0, position.height * sizeRatio, position.width, position.height * (1 - sizeRatio) - menuBarHeight);

			GUILayout.BeginArea(lowerPanel);
			lowerPanelScroll = GUILayout.BeginScrollView(lowerPanelScroll);

			if (selectedLog != null)
			{
				GUILayout.TextArea(selectedLog.message, textAreaStyle);
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void DrawResizer()
		{
			resizer = new Rect(0, (position.height * sizeRatio) - resizerHeight, position.width, 10f);

			GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up * resizerHeight), new Vector2(position.width, 2)), resizerStyle);
			GUILayout.EndArea();

			EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
		}

		private bool DrawBox(string content, LogType boxType, bool isOdd, bool isSelected)
		{
			if (isSelected)
			{
				boxStyle.normal.background = boxBgSelected;
			}
			else
			{
				if (isOdd)
				{
					boxStyle.normal.background = boxBgOdd;
				}
				else
				{
					boxStyle.normal.background = boxBgEven;
				}
			}

			switch (boxType)
			{
				case LogType.Error: icon = errorIcon; break;
				case LogType.Exception: icon = errorIcon; break;
				case LogType.Assert: icon = errorIcon; break;
				case LogType.Warning: icon = warningIcon; break;
				case LogType.Log: icon = infoIcon; break;
			}

			return GUILayout.Button(new GUIContent(content, icon), boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
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

		private void LogMessageReceived(string condition, string stackTrace, LogType type)
		{
			Log l = new Log(false, condition, stackTrace, type);
			logs.Add(l);
		}
	}
}
