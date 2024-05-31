// Ver. 1.2.0
// Updated: 2024-05-31

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using log4net.Core;

public class ComparisonChartEditor : EditorWindow
{
    private List<Vector2[]> dataSets = new List<Vector2[]>();
    private int selectedX = -1;
    private bool showVerticalLine = false;

    private double baseValue = 10.447;
    private int diff = 0;
    private int x = 100;

    public List<GoalParameter> goalParameter = new List<GoalParameter>();

    [MenuItem("Window/Comparison Chart")]
    public static void ShowWindow()
    {
        var window = GetWindow<ComparisonChartEditor>("Comparison Chart");
        window.maxSize = new Vector2(window.maxSize.x, window.maxSize.y);
        window.minSize = new Vector2(600, 400);
    }

    void OnEnable()
    {
        if (goalParameter == null)
        {
            goalParameter = new List<GoalParameter>();
        }
        UpdateDataSets();
    }

    void OnGUI()
    {
        GUILayout.Label("Ver. 1.1.0", EditorStyles.boldLabel);
        DrawLine();

        if (GUILayout.Button("Chart Settings"))
        {
            SettingsWindow.ShowWindow(this);
        }

        DrawAxes();

        for (int i = 0; i < dataSets.Count; i++)
        {
            Color color = Color.HSVToRGB((float)i / goalParameter.Count, 0.8f, 0.8f);
            DrawLineChart(dataSets[i], color, goalParameter[i]?.name ?? "Unnamed");
        }

        if (showVerticalLine && selectedX >= 0)
        {
            DrawVerticalLine(selectedX);
        }

        DrawAxisLabels();
        DrawLegends();
        DrawAxisTitle();
    }

    public void UpdateDataSets()
    {
        dataSets.Clear();
        foreach (var parameter in goalParameter)
        {
            if (parameter != null)
            {
                Vector2[] dataSet = new Vector2[x+1];
                for (int level = 0; level <= x; level++)
                {
                    double goal = GetNextGoal(parameter, level, diff);
                    dataSet[level] = new Vector2(level, (float)goal); // 명시적인 형 변환 추가
                }
                dataSets.Add(dataSet);
            }
        }
        Repaint();
    }

    public double GetNextGoal(GoalParameter parameter, int level, int diff)
    {
        return baseValue * Math.Exp(parameter.a * parameter.c * parameter.d * level) + parameter.b + (diff * 10);
    }

    void DrawAxisTitle()
    {
        GUI.Label(new Rect(5, 38, 140, 20), "[Goal]", EditorStyles.boldLabel);
        GUI.Label(new Rect(position.width / 2, position.height - 20, 140, 20), "[X]", EditorStyles.boldLabel);
    }

    void DrawLegends()
    {
        float startX = 50;
        float startY = 60;
        float heightStep = 20;

        GUI.Label(new Rect(startX, startY, 140, 20), "<Legends>", EditorStyles.boldLabel);

        for (int i = 0; i < goalParameter.Count; i++)
        {
            if (goalParameter[i] != null)
            {
                Color color = Color.HSVToRGB((float)i / goalParameter.Count, 0.8f, 0.8f);
                EditorGUI.DrawRect(new Rect(startX, startY + (heightStep * (i + 1)) + 5, 10, 10), color);
                string legendName = string.IsNullOrEmpty(goalParameter[i].name) ? "Unnamed" : goalParameter[i].name;
                GUI.Label(new Rect(startX + 15, startY + (heightStep * (i + 1)), 130, 20), legendName);
            }
        }

        GUI.contentColor = Color.white;
    }

    void DrawLineChart(Vector2[] points, Color color, string label)
    {
        float height = position.height - 60;
        float width = position.width - 70;

        float xScale = width / 100f;
        float minY = (float)GetMinY(); // 명시적인 형 변환 추가
        float maxY = (float)GetMaxY(); // 명시적인 형 변환 추가
        float yScale = (height - 40) / (maxY - minY);

        Handles.color = color;
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 prev = new Vector3(40 + (i - 1) * xScale, height - (points[i - 1].y - minY) * yScale + 20, 0);
            Vector3 curr = new Vector3(40 + i * xScale, height - (points[i].y - minY) * yScale + 20, 0);
            Handles.DrawLine(prev, curr);
        }
    }

    double GetMaxY()
    {
        double maxY = double.MinValue;
        foreach (var dataSet in dataSets)
        {
            foreach (var point in dataSet)
            {
                if (point.y > maxY)
                    maxY = point.y;
            }
        }
        return maxY;
    }

    double GetMinY()
    {
        double minY = double.MaxValue;
        foreach (var dataSet in dataSets)
        {
            foreach (var point in dataSet)
            {
                if (point.y < minY)
                    minY = point.y;
            }
        }
        return minY;
    }

    void DrawAxes()
    {
        float height = position.height - 60;
        float width = position.width - 30;

        Handles.color = Color.black;
        Handles.DrawLine(new Vector3(40, height + 20, 0), new Vector3(width, height + 20, 0));
        Handles.DrawLine(new Vector3(40, height + 20, 0), new Vector3(40, 60, 0));
        Handles.DrawLine(new Vector3(width, height + 20, 0), new Vector3(width, 60, 0));
        Handles.DrawLine(new Vector3(40, 60, 0), new Vector3(width, 60, 0));
    }

    void DrawAxisLabels()
    {
        float height = position.height - 60;
        float width = position.width - 30;
        int xLabelStep = 5;

        for (int i = 0; i <= x; i += xLabelStep)
        {
            float x = 40 + i * (width - 40) / 100f;
            if (GUI.Button(new Rect(x - 15, height + 25, 32, 20), i.ToString()))
            {
                selectedX = i;
                showVerticalLine = true;
                Repaint();

                EditorApplication.delayCall += ShowDialogAfterRepaint;
            }
        }

        double minY = Math.Floor(GetMinY()); // 소숫점 버림
        double maxY = Math.Floor(GetMaxY()); // 소숫점 버림
        int numYLabels = 10; // Y축 레이블의 개수
        double yLabelStep = (maxY - minY) / numYLabels;

        float yScale = (height - 40) / (float)(maxY - minY);

        for (int i = 0; i <= numYLabels; i++)
        {
            float y = height - i * (height - 40) / numYLabels + 20;
            if (y < 20 || y > height + 20) continue;
            GUI.Label(new Rect(10, y - 10, 50, 20), (minY + i * yLabelStep).ToString("F0"));
        }
    }


    void ShowDialogAfterRepaint()
    {
        EditorApplication.delayCall -= ShowDialogAfterRepaint;
        ShowXValuesInDialog(selectedX);
        Repaint();
    }

    void ShowXValuesInDialog(int x)
    {
        string message = $"Selected X: {x}\n";
        foreach (var parameter in goalParameter)
        {
            if (parameter != null)
            {
                double goal = GetNextGoal(parameter, x, diff);
                message += $"{parameter.name} Goal: {goal}\n";
            }
        }
        EditorUtility.DisplayDialog("X Value Details", message, "OK");
        showVerticalLine = false;
        Repaint();
    }

    void DrawVerticalLine(int x)
    {
        float height = position.height - 60;
        float width = position.width - 60;

        float xScale = (width - 10) / 100f;
        float xPos = 40 + x * xScale;

        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(xPos, height + 20, 0), new Vector3(xPos, 60, 0));
    }

    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }
}