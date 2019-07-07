using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplinePath))]
public class SplinePathEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SplinePath splinePath = (SplinePath)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		GUILayout.Label("Splines in CHAR SLBs have times of zero.");
		if (GUILayout.Button("Set Point Times to Zero"))
		{
			splinePath.SetTimesToZero();
		}
		GUILayout.Space(10);
		GUILayout.Label("Splines in SPLINE SLBs have incremental\ntimes.");
		if (GUILayout.Button("Set Point Times Incrementally"))
		{
			splinePath.SetTimesIncrementally();
		}
		GUILayout.Space(10);
		GUILayout.Label("Splines in CAM SLBs have times calculated\nbased on the length of the spline, but\ncalculating that *properly* would require\nthings I haven't gotten around to doing yet.\nThe button below can give you close-ish\nresults though. I just wouldn't recommend\ndoing it on existing camera splines unless\nyou REALLY want to, cause it'll be making\nthem slightly wrong.");
		if (GUILayout.Button("Yes I actually wanna do that"))
		{
			splinePath.SetTimesToNotQuiteRightButCloseEnoughValuesForCams();
		}
	}
}
