using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BionicleCharacter : MonoBehaviour
{
	public Vector3 unusedOrientation; // unused, putting here cause unity wasn't preserving it correctly if stored in the transform
	public float unknown = -1.0f; // shrug
	public string[] triggerBoxes;
	public GameObject[] paths;
}
