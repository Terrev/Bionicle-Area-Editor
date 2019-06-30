using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BionicleSpotlight))]
public class BionicleSpotlightEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		BionicleSpotlight bionicleSpotlight = (BionicleSpotlight)target;
		if (bionicleSpotlight.directionMarker != null)
		{
			GUILayout.Label("Direction length: " + bionicleSpotlight.directionMarker.localPosition.magnitude);
		}
		// \n
		GUILayout.Label("The length of the direction vector doesn't\nmatter, but in vanilla game files it's the\nsame as the range. However, the game\nseems to internally triple the range.\n\nThis editor displays cones 3 times longer\nthan the defined range to show you what\nthe result will be in-game.");
		// gonna keep actual functionality in BionicleSpotlight even if it's simple
		if (GUILayout.Button("Calculate range from direction length"))
		{
			bionicleSpotlight.CalculateRangeFromDirection();
		}
		if (GUILayout.Button("Calculate direction length from range"))
		{
			bionicleSpotlight.CalculateDirectionFromRange();
		}
	}
}