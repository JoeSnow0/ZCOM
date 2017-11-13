using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ObjectiveMaker : EditorWindow {

    string path = "Assets/Scripts/Gameplay/Objectives/";
    string objectiveName = "";

    [MenuItem("Map Tools/Objective Maker")]
    static void Init()
    {
        ObjectiveMaker window = (ObjectiveMaker)GetWindow(typeof(ObjectiveMaker));
        window.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Name");
        objectiveName = EditorGUILayout.TextField(objectiveName);
        if (GUILayout.Button("Create script"))
        {
            path = "Assets/Scripts/Gameplay/Objectives/" + objectiveName + ".cs";
            if (!File.Exists(path))
            {
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine("using System.Collections;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using UnityEngine;\n");
                writer.WriteLine("public class " + objectiveName + " : Objective {\n");
                writer.WriteLine("    void Start() {");
                writer.WriteLine("        InitializeObjective();");
                writer.WriteLine("        SetDescription(\"Default description\");");
                writer.WriteLine("    }\n");
                writer.WriteLine("	void Update () {\n");
                writer.WriteLine("	}");
                writer.WriteLine("}");
                writer.Close();

                AssetDatabase.Refresh();
            }
        }
    }
}
