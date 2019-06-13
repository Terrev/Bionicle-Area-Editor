using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Manager))]
// what a dumb class name
public class ManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GUILayout.Space(10);
		
		Manager manager = (Manager)target;
		/*
		if (GUILayout.Button("Test"))
		{
			manager.LoadALotOfStuff();
		}
		*/
		if (GUILayout.Button("Load Objects SLB"))
		{
			manager.LoadObjSlb();
		}
		if (GUILayout.Button("Load Positions SLB"))
		{
			manager.LoadPosSlb();
		}
		if (GUILayout.Button("Load Characters SLB"))
		{
			manager.LoadCharSlb();
		}
		if (GUILayout.Button("Load Triggers SLB"))
		{
			manager.LoadTriggerSlb();
		}
		GUILayout.Space(10);
		if (GUILayout.Button("Save Objects SLB"))
		{
			manager.SaveObjSlb();
		}
		if (GUILayout.Button("Save Positions SLB"))
		{
			manager.SavePosSlb();
		}
		if (GUILayout.Button("Save Characters SLB"))
		{
			manager.SaveCharSlb();
		}
		if (GUILayout.Button("Save Triggers SLB"))
		{
			manager.SaveTriggerSlb();
		}
	}
}