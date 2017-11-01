using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class TestScript : EditorWindow {

    string path = "Assets/Scenes/GridOutput.txt";
    
    [MenuItem("Map Tools/Map Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TestScript window = (TestScript)EditorWindow.GetWindow(typeof(TestScript));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            StreamWriter writer = new StreamWriter(path, true);

            foreach (GameObject tile in Selection.gameObjects)
            {
                ClickebleTile currentTile = tile.transform.parent.GetComponent<ClickebleTile>();
                writer.WriteLine("tiles[" + currentTile.tileX + ", " + currentTile.tileY + "] = 1;");
            }
            writer.Close();
        }
    }
}
