using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConsoleDebugger : MonoBehaviour
{
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
    {
        { LogType.Assert, Color.white },
        { LogType.Error, Color.red },
        { LogType.Exception, Color.red },
        { LogType.Log, Color.white },
        { LogType.Warning, Color.yellow },
    };

    bool show;
    bool collapse;

    const int margin = 20;
    const int windowHeight = 200;
    const int windowWidth = 500;


    List<Log> logs = new List<Log>();
    Vector2 scrollPosition;

    Rect windowRect = new Rect(margin, Screen.height - windowHeight - margin, windowWidth, windowHeight);
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    private void OnGUI()
    {
        if (!show)
            return;

        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }


    void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;
                if (messageSameAsPrevious)
                {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);

        }
        GUILayout.EndScrollView();

        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        GUI.DragWindow(titleBarRect);
    }

    private void Awake()
    {
        Application.logMessageReceived += LogCalled;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            show = !show;
        }
    }


    void LogCalled(string logString, string stackTrace, LogType type)
    {
        logs.Add(new Log()
        {
            message = logString,
            stackTrace = stackTrace,
            type = type
        });
    }


}
