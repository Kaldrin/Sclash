using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputWindow : EditorWindow
{
    Vector2 scrollPos;
    public List<m_Input> inputs = new List<m_Input>();

    public class m_Input
    {
        public string m_Name;
        public string m_Primary;
        public string m_Secondary;
        public string m_Tertiary;
    };

    public enum InputType
    {
        KeyOrMouseButton,
        MouseMovement,
        JoystickAxis
    };

    public enum JoystickAxis
    {
        XAxis,
        YAxis,
        ThirdAxis,
        FourthAxis,
        FifthAxis,
        SixthAxis,
        SeventhAxis,
        EighthAxis,
        NinthAxis,
        TenthdAxis,
        EleventhAxis,
        TwelfthAxis,
        ThirteenthAxis,
        FourteenthAxis,
        FifteenthAxis,
        SixteenthAxis,
        SeventeenthAxis,
        EighteenthAxis,
        NineteenthAxis,
        TwentiethAxis,
        TwentyFirstAxis,
        TwentySecondAxis,
        TwentyThirdAxis,
        TwentyFourthAxis,
        TwentyFifthAxis,
        TwentySixthAxis,
        TwentySeventhAxis,
        TwentyEighthAxis,
    }

    public enum Joystick
    {
        AllJoysticks,
        Joystick1,
        Joystick2,
    }

    public static void DoRead()
    {
        InputWindow window = EditorWindow.GetWindow<InputWindow>(false, "Input", true);

        Object[] inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset");

        SerializedObject obj = new SerializedObject(inputManager);
        SerializedProperty axisArray = obj.FindProperty("m_Axes");

        for (int i = 0; i < axisArray.arraySize; i++)
        {
            SerializedProperty axis = axisArray.GetArrayElementAtIndex(i);
            string name = axis.FindPropertyRelative("m_Name").stringValue;

            string positive = axis.FindPropertyRelative("positiveButton").stringValue;
            string negative = axis.FindPropertyRelative("negativeButton").stringValue;
            InputType inputType = (InputType)axis.FindPropertyRelative("type").intValue;
            JoystickAxis axisVal = (JoystickAxis)axis.FindPropertyRelative("axis").intValue;
            Joystick joyNum = (Joystick)axis.FindPropertyRelative("joyNum").intValue;

            int l_MatchIndex = window.FindMatch(name);
            if (l_MatchIndex < 0)
            {
                m_Input nInput = new m_Input();
                nInput.m_Name = name;

                if (inputType == InputType.JoystickAxis)
                {
                    nInput.m_Primary = joyNum + " " + axisVal;
                }
                else
                {
                    if (positive != "")
                        nInput.m_Primary = window.UpperFirst(positive);
                    if (negative != "")
                        nInput.m_Primary += " / " + window.UpperFirst(negative);
                }
                window.inputs.Add(nInput);
            }
            else if (window.inputs[l_MatchIndex].m_Secondary == null)
            {
                if (inputType == InputType.JoystickAxis)
                {
                    window.inputs[l_MatchIndex].m_Secondary = joyNum + " " + axisVal;
                }
                else
                {
                    if (positive != "")
                        window.inputs[l_MatchIndex].m_Secondary = window.UpperFirst(positive);
                }
            }
            else
            {
                if (inputType == InputType.JoystickAxis)
                {
                    window.inputs[l_MatchIndex].m_Tertiary = joyNum + " " + axisVal;
                }
                else
                {
                    if (positive != "")
                        window.inputs[l_MatchIndex].m_Tertiary = window.UpperFirst(positive);
                }
            }
        }
    }

    private string UpperFirst(string s)
    {
        char[] word = s.ToCharArray();
        word[0] = char.ToUpper(word[0]);
        return new string(word);
    }

    private int FindMatch(string name)
    {
        for (int i = 0; i < inputs.Count; i++)
            if (inputs[i].m_Name == name)
                return i;

        return -1;
    }

    public void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Input", style);
        DrawVerticalUILine(Color.black, scrollPos.y);
        EditorGUILayout.LabelField("Primary", style);
        DrawVerticalUILine(Color.black, scrollPos.y);
        EditorGUILayout.LabelField("Secondary", style);
        DrawVerticalUILine(Color.black, scrollPos.y);
        EditorGUILayout.LabelField("Tertiary", style);
        EditorGUILayout.EndHorizontal();
        DrawUILine(Color.black);
        foreach (m_Input i in inputs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.m_Name);
            EditorGUILayout.LabelField(i.m_Primary, style);
            EditorGUILayout.LabelField(i.m_Secondary, style);
            EditorGUILayout.LabelField(i.m_Tertiary, style);
            EditorGUILayout.EndHorizontal();
            DrawUILine(Color.grey);
        }

        EditorGUILayout.EndScrollView();
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    public static void DrawVerticalUILine(Color color, float scrollpos = 0, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        r.width = thickness;
        r.x += padding / 2;
        r.y += scrollpos;
        r.height = Screen.height;
        EditorGUI.DrawRect(r, color);
    }

    [MenuItem("Sclash/Input window")]
    public static void ShowWindow()
    {
        InputWindow window = EditorWindow.GetWindow<InputWindow>(false, "Input", true);
        window.minSize = new Vector2(300, 300);
        DoRead();
    }
}