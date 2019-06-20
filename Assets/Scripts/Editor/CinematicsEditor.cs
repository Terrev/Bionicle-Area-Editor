using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Cinematics))]
public class CinematicsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Cinematics cinematics = (Cinematics)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		if (GUILayout.Button("Load Cinematic Characters SLB"))
		{
			cinematics.LoadCinCharSlb();
		}
		//GUILayout.Space(10);
		if (GUILayout.Button("Save Cinematic Characters SLB"))
		{
			//cinematics.SaveCinCharSlb();
		}
	}
}