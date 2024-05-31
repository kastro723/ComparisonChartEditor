// Ver. 1.2.0
// Updated: 2024-05-31

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SettingsWindow : EditorWindow
{
    private static ComparisonChartEditor parentWindow;
    private static List<GoalParameter> goalParameters = new List<GoalParameter>();
    private Vector2 scrollPosition;
    private Vector2 editScrollPosition;
    private bool isEditPanelExpanded = false;
    private int selectedGoalParameterIndex = 0;
    private static SettingsWindow window;

    private double tempA, tempB, tempC, tempD; // 임시 값

    public static void ShowWindow(ComparisonChartEditor parent)
    {
        parentWindow = parent;
        window = GetWindow<SettingsWindow>("Chart Settings");
        window.maxSize = new Vector2(400, 165);
        window.minSize = new Vector2(400, 165);

        goalParameters = new List<GoalParameter>(parentWindow.goalParameter);

        if (goalParameters.Count == 0)
        {
            goalParameters.Add(null);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Goal Parameters", EditorStyles.boldLabel);

        DrawLine();

        ScriptableObjectFieldArray();

        DrawLine();

        if (GUILayout.Button("Add Goal Parameter"))
        {
            goalParameters.Add(null);
            UpdateChartEditButtonState();
        }

        if (!isEditPanelExpanded)
        {
            if (GUILayout.Button("Update Chart"))
            {
                parentWindow.goalParameter = new List<GoalParameter>(goalParameters);
                parentWindow.UpdateDataSets();
                //Close();
            }
            else
            {
                GUI.FocusControl(null); // 필드 선택 초기화
            }
        }

        GUI.enabled = goalParameters.Exists(gp => gp != null);
        if (GUILayout.Button("Edit and Export Goal Parameter"))
        {
            isEditPanelExpanded = !isEditPanelExpanded;
            if (isEditPanelExpanded)
            {
                selectedGoalParameterIndex = 0;
                LoadSelectedParameterValues();
            }
            UpdateWindowSize();
        }
        GUI.enabled = true;

        if (isEditPanelExpanded)
        {
            //DrawLine();
            DrawEditPanel();
        }
        //if (!isEditPanelExpanded)
        //{
        //    if (GUILayout.Button("Update Chart"))
        //    {
        //        parentWindow.goalParameter = new List<GoalParameter>(goalParameters);
        //        parentWindow.UpdateDataSets();
        //        //Close();
        //    }
        //    else
        //    {
        //        GUI.FocusControl(null); // 필드 선택 초기화
        //    }
        //}

    }

    private void ScriptableObjectFieldArray()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(75));
        GUILayout.BeginVertical();

        int removeIndex = -1;
        for (int i = 0; i < goalParameters.Count; i++)
        {
            GUILayout.BeginHorizontal();
            goalParameters[i] = (GoalParameter)EditorGUILayout.ObjectField(goalParameters[i], typeof(GoalParameter), false);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                removeIndex = i;
            }
            GUILayout.EndHorizontal();
        }

        if (removeIndex != -1)
        {
            goalParameters.RemoveAt(removeIndex);
            UpdateChartEditButtonState();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    private void DrawEditPanel()
    {
        //editScrollPosition = GUILayout.BeginScrollView(editScrollPosition, GUILayout.Height(150));
        GUILayout.BeginVertical();

        GUILayout.Label("Edit Goal Parameters", EditorStyles.boldLabel);

        List<string> goalParameterNamesList = new List<string>();
        List<GoalParameter> nonNullGoalParameters = new List<GoalParameter>();

        for (int i = 0; i < goalParameters.Count; i++)
        {
            if (goalParameters[i] != null)
            {
                goalParameterNamesList.Add(goalParameters[i].name);
                nonNullGoalParameters.Add(goalParameters[i]);
            }
        }

        string[] goalParameterNames = goalParameterNamesList.ToArray();

        if (goalParameterNames.Length == 0)
        {
            goalParameterNames = new[] { "Unnamed" };
        }

        int newSelectedIndex = EditorGUILayout.Popup("Select Parameter", selectedGoalParameterIndex, goalParameterNames);
        if (newSelectedIndex != selectedGoalParameterIndex)
        {
            selectedGoalParameterIndex = newSelectedIndex;
            LoadSelectedParameterValues();
        }

        if (selectedGoalParameterIndex < nonNullGoalParameters.Count)
        {
            tempA = EditorGUILayout.DoubleField("Parameter A", tempA);
            tempB = EditorGUILayout.DoubleField("Parameter B", tempB);
            tempC = EditorGUILayout.DoubleField("Parameter C", tempC);
            tempD = EditorGUILayout.DoubleField("Parameter D", tempD);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset Preview"))
            {
                parentWindow.goalParameter = new List<GoalParameter>(goalParameters);
                parentWindow.UpdateDataSets();
                //Close();
            }

            if (GUILayout.Button("Chart Preview"))
            {
                GoalParameter selectedParameter = nonNullGoalParameters[selectedGoalParameterIndex];
                GoalParameter tempParameter = ScriptableObject.CreateInstance<GoalParameter>();
                tempParameter.name = selectedParameter.name;
                tempParameter.a = tempA;
                tempParameter.b = tempB;
                tempParameter.c = tempC;
                tempParameter.d = tempD;

                List<GoalParameter> tempParameters = new List<GoalParameter>(goalParameters);
                tempParameters[selectedGoalParameterIndex] = tempParameter;
                parentWindow.goalParameter = tempParameters;
                parentWindow.UpdateDataSets();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Parameter Save and Chart Apply"))
            {
                GoalParameter selectedParameter = nonNullGoalParameters[selectedGoalParameterIndex];
                selectedParameter.a = tempA;
                selectedParameter.b = tempB;
                selectedParameter.c = tempC;
                selectedParameter.d = tempD;

                EditorUtility.SetDirty(selectedParameter);
                parentWindow.goalParameter = new List<GoalParameter>(goalParameters);
                parentWindow.UpdateDataSets();
            }

            // CSV Export Button
            if (GUILayout.Button("Export to CSV"))
            {
                ExportSelectedParameterToCSV(nonNullGoalParameters[selectedGoalParameterIndex]);
            }

        }

        GUILayout.EndVertical();
        //GUILayout.EndScrollView();
    }

    private void ExportSelectedParameterToCSV(GoalParameter parameter)
    {
        string path = EditorUtility.SaveFilePanel("Save CSV", Application.persistentDataPath, "goal_parameter.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        using (StreamWriter writer = new StreamWriter(path))
        {
            //writer.WriteLine("X,Goal,a,b,c,d");
            writer.WriteLine("Formular Name");
            writer.WriteLine($"{parameter.name}");
            writer.WriteLine();
            writer.WriteLine("a,b,c,d");
            writer.WriteLine($"{parameter.a},{parameter.b},{parameter.c},{parameter.d}");
            writer.WriteLine();
            writer.WriteLine($"X,Goal");
            for (int x = 0; x <= 100; x++)
            {
                double goal = parentWindow.GetNextGoal(parameter, x, 0);
                writer.WriteLine($"{x},{goal}");
            }
        }

        EditorUtility.RevealInFinder(path);
    }


    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }

    private void UpdateChartEditButtonState()
    {
        bool hasValidGoalParameter = goalParameters.Exists(gp => gp != null);
        if (!hasValidGoalParameter && isEditPanelExpanded)
        {
            isEditPanelExpanded = false;
            UpdateWindowSize();
        }
    }

    private void UpdateWindowSize()
    {
        if (isEditPanelExpanded)
        {
            window.maxSize = new Vector2(400, 325);
            window.minSize = new Vector2(400, 325);
        }
        else
        {
            window.maxSize = new Vector2(400, 165);
            window.minSize = new Vector2(400, 165);
        }
    }

    private void LoadSelectedParameterValues()
    {
        if (selectedGoalParameterIndex < goalParameters.Count && goalParameters[selectedGoalParameterIndex] != null)
        {
            GoalParameter selectedParameter = goalParameters[selectedGoalParameterIndex];
            tempA = selectedParameter.a;
            tempB = selectedParameter.b;
            tempC = selectedParameter.c;
            tempD = selectedParameter.d;
        }
    }
}
