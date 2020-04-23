using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TestCinCamInterpolator))]
public class TestCinCamInterpolatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TestCinCamInterpolator testCinCamInterpolator = (TestCinCamInterpolator)target;
		if (GUILayout.Button("Save"))
		{
			testCinCamInterpolator.Save();
		}
	}
}