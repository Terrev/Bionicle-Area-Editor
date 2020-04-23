using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TestCinCamExport))]
public class TestCinCamExportEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TestCinCamExport testCinCamExport = (TestCinCamExport)target;
		if (GUILayout.Button("Export"))
		{
			testCinCamExport.Export();
		}
	}
}