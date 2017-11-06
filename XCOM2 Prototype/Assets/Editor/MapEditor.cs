using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class MapEditor : EditorWindow {

    string path = "Assets/Scenes/GridOutput.txt";
    string tileIndex = "1";
    string fileName = "GridOutput.txt";
    string description = "//";
    
    [MenuItem("Map Tools/Map Editor")]
    static void Init()
    {
        MapEditor window = (MapEditor)GetWindow(typeof(MapEditor));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Output name");
        fileName = EditorGUILayout.TextField(fileName);
        EditorGUILayout.LabelField("Index");
        tileIndex = EditorGUILayout.TextField(tileIndex);
        EditorGUILayout.LabelField("Description");
        description = EditorGUILayout.TextField(description);

        path = "Assets/Scenes/" + fileName + ".txt";


        if (GUILayout.Button("Generate"))
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine();
            writer.WriteLine("//" + description);

            foreach (GameObject tile in Selection.gameObjects)
            {
                ClickebleTile currentTile = tile.transform.parent.GetComponent<ClickebleTile>();
                writer.WriteLine("tiles[" + currentTile.tileX + ", " + currentTile.tileY + "] = " + tileIndex + ";");
            }
            writer.Close();
        }
    }
}
