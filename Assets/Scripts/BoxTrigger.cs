using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxTrigger : MonoBehaviour
{
	[System.NonSerialized]
	public Transform point1;
	[System.NonSerialized]
	public Transform point2;
	
	Vector3 defaultHandleScale = new Vector3(3.0f, 3.0f, 3.0f);
	
	Transform boxVisual;
	Material normalBoxMaterial;
	Material invalidBoxMaterial;
	Material normalCornerMaterial;
	Material invalidCornerMaterial;
	// this is the part where I stop caring
	bool nothingWeirdHappening = true;
	bool nothingWeirdHappeningLastUpdate = true;
	
	// TRANSPARENT BOX VISUAL
	void LateUpdate()
	{
		MakeSureWeHaveEverything();
		
		if (CheckPoints())
		{
			// set scale and position
			boxVisual.localScale = new Vector3(Mathf.Abs(point1.localPosition.x - point2.localPosition.x), Mathf.Abs(point1.localPosition.y - point2.localPosition.y), Mathf.Abs(point1.localPosition.z - point2.localPosition.z));
			boxVisual.localPosition = new Vector3((point1.localPosition.x + point2.localPosition.x) / 2.0f, (point1.localPosition.y + point2.localPosition.y) / 2.0f, (point1.localPosition.z + point2.localPosition.z) / 2.0f);
			
			RefreshBoxMaterials();
		}
	}
	
	void OnDrawGizmos()
	{
		MakeSureWeHaveEverything();
		
		if (CheckPoints())
		{
			if (CheckValidity() && CheckIfCube())
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.yellow;
			}
			
			// bottom
			Vector3 corner1 = point1.position;
			Vector3 corner2 = new Vector3(point1.position.x, point1.position.y, point2.position.z);
			Vector3 corner3 = new Vector3(point2.position.x, point1.position.y, point2.position.z); 
			Vector3 corner4 = new Vector3(point2.position.x, point1.position.y, point1.position.z);
			
			// top
			Vector3 corner5 = new Vector3(point1.position.x, point2.position.y, point1.position.z);
			Vector3 corner6 = new Vector3(point1.position.x, point2.position.y, point2.position.z);
			Vector3 corner7 = point2.position;
			Vector3 corner8 = new Vector3(point2.position.x, point2.position.y, point1.position.z);
			
			// draw bottom
			Gizmos.DrawLine(corner1, corner2);
			Gizmos.DrawLine(corner2, corner3);
			Gizmos.DrawLine(corner3, corner4);
			Gizmos.DrawLine(corner4, corner1);
			
			// draw top
			Gizmos.DrawLine(corner5, corner6);
			Gizmos.DrawLine(corner6, corner7);
			Gizmos.DrawLine(corner7, corner8);
			Gizmos.DrawLine(corner8, corner5);
			
			// draw sides
			Gizmos.DrawLine(corner1, corner5);
			Gizmos.DrawLine(corner2, corner6);
			Gizmos.DrawLine(corner3, corner7);
			Gizmos.DrawLine(corner4, corner8);
			
			RefreshBoxMaterials();
		}
	}
	
	// sigh
	void MakeSureWeHaveEverything()
	{
		// grab materials
		if (normalBoxMaterial == null)
		{
			normalBoxMaterial = Resources.Load("_Editor/Box Trigger Visual", typeof (Material)) as Material;
		}
		if (invalidBoxMaterial == null)
		{
			invalidBoxMaterial = Resources.Load("_Editor/Box Trigger Visual Invalid", typeof (Material)) as Material;
		}
		if (normalCornerMaterial == null)
		{
			normalCornerMaterial = Resources.Load("_Editor/Box Trigger Corner", typeof (Material)) as Material;
		}
		if (invalidCornerMaterial == null)
		{
			invalidCornerMaterial = Resources.Load("_Editor/Box Trigger Corner Invalid", typeof (Material)) as Material;
		}
		
		// grab box
		if (boxVisual == null)
		{
			boxVisual = transform.Find("Ignore Me");
			// already do this while loading the slb but meh, just in case anything unforeseen happens
			boxVisual.gameObject.hideFlags = HideFlags.HideInHierarchy;
		}
		
		// grab points
		if (!CheckPoints())
		{
			GetPoints();
		}
	}
	
	void RefreshBoxMaterials()
	{
		// lol
		if (CheckValidity() && CheckIfCube())
		{
			nothingWeirdHappening = true;
		}
		else
		{
			nothingWeirdHappening = false;
		}
		if (nothingWeirdHappening != nothingWeirdHappeningLastUpdate)
		{
			if (nothingWeirdHappening)
			{
				boxVisual.gameObject.GetComponent<MeshRenderer>().material = normalBoxMaterial;
				point1.gameObject.GetComponent<MeshRenderer>().material = normalCornerMaterial;
				point2.gameObject.GetComponent<MeshRenderer>().material = normalCornerMaterial;
			}
			else
			{
				boxVisual.gameObject.GetComponent<MeshRenderer>().material = invalidBoxMaterial;
				point1.gameObject.GetComponent<MeshRenderer>().material = invalidCornerMaterial;
				point2.gameObject.GetComponent<MeshRenderer>().material = invalidCornerMaterial;
			}
			nothingWeirdHappeningLastUpdate = nothingWeirdHappening;
		}
	}
	
	public bool CheckPoints()
	{
		if (point1 == null || point2 == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	void GetPoints()
	{
		point1 = transform.Find("Point 1");
		point2 = transform.Find("Point 2");
	}
	
	public bool CheckValidity()
	{
		if (transform.localPosition == Vector3.zero
		&& transform.localRotation == Quaternion.identity
		&& transform.localScale == Vector3.one)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void ApplyTransformation()
	{
		if (!CheckPoints())
		{
			GetPoints();
		}
		
		if (point1 == null)
		{
			Debug.LogError("Couldn't find Point 1 on " + gameObject.name);
			return;
		}
		
		if (point2 == null)
		{
			Debug.LogError("Couldn't find Point 2 on " + gameObject.name);
			return;
		}
		
		if (CheckValidity())
		{
			return;
		}
		
		point1.parent = null;
		point2.parent = null;
		
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		
		point1.parent = transform;
		point2.parent = transform;
		
		point1.localRotation = Quaternion.identity;
		point2.localRotation = Quaternion.identity;
		
		point1.localScale = defaultHandleScale;
		point2.localScale = defaultHandleScale;
	}
	
	public bool CheckIfCube()
	{
		Vector3 boxScale = new Vector3(Mathf.Abs(point1.localPosition.x - point2.localPosition.x), Mathf.Abs(point1.localPosition.y - point2.localPosition.y), Mathf.Abs(point1.localPosition.z - point2.localPosition.z));
		// existing boxes don't have perfectly matching values, so we're allowing some wiggle room
		if (Mathf.Abs(boxScale.x - boxScale.y) > 0.01f)
		{
			return false;
		}
		if (Mathf.Abs(boxScale.y - boxScale.z) > 0.01f)
		{
			return false;
		}
		if (Mathf.Abs(boxScale.z - boxScale.x) > 0.01f)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public void TurnIntoCube()
	{
		if (CheckIfCube())
		{
			Debug.Log(gameObject.name + " is already a cube");
			return;
		}
		// get dimensions of box
		Vector3 boxScale = new Vector3(Mathf.Abs(point1.localPosition.x - point2.localPosition.x), Mathf.Abs(point1.localPosition.y - point2.localPosition.y), Mathf.Abs(point1.localPosition.z - point2.localPosition.z));
		// get biggest value out of length/width/height
		float size = Mathf.Max(boxScale.x, boxScale.y, boxScale.z);
		// get the center of it
		Vector3 center = new Vector3((point1.localPosition.x + point2.localPosition.x) / 2.0f, (point1.localPosition.y + point2.localPosition.y) / 2.0f, (point1.localPosition.z + point2.localPosition.z) / 2.0f);
		// calculate new localPositions
		point1.localPosition = new Vector3(center.x + (size / 2.0f), center.y - (size / 2.0f), center.z - (size / 2.0f));
		point2.localPosition = new Vector3(center.x - (size / 2.0f), center.y + (size / 2.0f), center.z + (size / 2.0f));
	}
}
