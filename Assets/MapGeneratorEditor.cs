using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
	
	#if UNITY_EDITOR
	override public void  OnInspectorGUI () {
		MapGenerator mapGenerator = (MapGenerator)target;
		if(GUILayout.Button("GenerateMap")) {
			mapGenerator.GenerateMap();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
		DrawDefaultInspector();
	}
	#endif
}