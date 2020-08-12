using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LSystem))]
public class TreeGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        LSystem tree = (LSystem)target;

        // If inspector is changed
        if(DrawDefaultInspector())
        {
            tree.GenerateTree();
        }

        if (GUILayout.Button("Generate"))
        {
            tree.GenerateTree();
        }
    }
}
