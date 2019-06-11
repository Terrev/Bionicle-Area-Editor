using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlaneTrigger))]
public class PlaneTriggerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		PlaneTrigger planeTrigger = (PlaneTrigger)target;
		if (GUILayout.Button("Apply transformation"))
		{
			planeTrigger.ApplyTransformation();
		}
	}
}