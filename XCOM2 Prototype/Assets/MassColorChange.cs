using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MassColorChange : MonoBehaviour {

    static List<MassColorChange> trackedObjects = new List<MassColorChange>();

    public ScriptableObject ColorObject;

	// Use this for initialization
	void Start () {
        trackedObjects.Add(this);
	}

    private void OnDestroy()
    {
        trackedObjects.Remove(this);
    }

    void ApplyColorChange()
    {
        Text c = GetComponent<Text>();
        if (c)
            c.color = ColorScriptableObject.primaryColor;


    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (FindColorChange() != null)
    //    {
    //        Debug.Log("Hello World!");
    //    }

    //}
}
