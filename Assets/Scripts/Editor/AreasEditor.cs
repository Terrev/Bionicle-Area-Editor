using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Areas))]
public class AreasEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Areas areas = (Areas)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		/*
		if (GUILayout.Button("Test"))
		{
			areas.LoadEntireGame();
		}
		*/
		if (GUILayout.Button("Load Objects SLB"))
		{
			areas.LoadObjSlb();
		}
		if (GUILayout.Button("Load Positions SLB"))
		{
			areas.LoadPosSlb();
		}
		if (GUILayout.Button("Load Characters SLB"))
		{
			areas.LoadCharSlb();
		}
		if (GUILayout.Button("Load Triggers SLB"))
		{
			areas.LoadTriggerSlb();
		}
		if (GUILayout.Button("Load Hives SLB"))
		{
			areas.LoadHiveSlb();
		}
		GUILayout.Space(10);
		if (GUILayout.Button("Save Objects SLB"))
		{
			areas.SaveObjSlb();
		}
		if (GUILayout.Button("Save Positions SLB"))
		{
			areas.SavePosSlb();
		}
		if (GUILayout.Button("Save Characters SLB"))
		{
			areas.SaveCharSlb();
		}
		if (GUILayout.Button("Save Triggers SLB"))
		{
			areas.SaveTriggerSlb();
		}
		if (GUILayout.Button("Save Hives SLB"))
		{
			areas.SaveHiveSlb();
		}
	}
}