using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTrigger : MonoBehaviour
{
	public string area = "????";
	public string startPoint = "????";
	public string lookPoint = "????";
	public Vector3 planeNormal = new Vector3(0.0f, 0.0f, 1.0f);
	
	[System.NonSerialized]
	public Transform point1;
	[System.NonSerialized]
	public Transform point2;
	[System.NonSerialized]
	public Transform point3;
	[System.NonSerialized]
	public Transform point4;
	
	Vector3 defaultHandleScale = new Vector3(3.0f, 3.0f, 3.0f);
	
	void OnDrawGizmos()
	{
		if (!CheckPoints())
		{
			GetPoints();
		}
		
		if (CheckPoints())
		{
			if (CheckValidity())
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.yellow;
			}
			
			Gizmos.DrawLine(point1.position, point2.position);
			Gizmos.DrawLine(point2.position, point3.position);
			Gizmos.DrawLine(point3.position, point4.position);
			Gizmos.DrawLine(point4.position, point1.position);
			Gizmos.DrawLine(point1.position, point3.position);
			Gizmos.DrawLine(point2.position, point4.position);
			
			Vector3 centerOfPlane = new Vector3((point1.position.x + point4.position.x) / 2.0f, (point1.position.y + point4.position.y) / 2.0f, (point1.position.z + point4.position.z) / 2.0f);
			
			planeNormal = GetPlaneNormal();
			
			Vector3 normalVisual = centerOfPlane + planeNormal * 10;
			
			Gizmos.DrawLine(centerOfPlane, normalVisual);
		}
	}
	
	public bool CheckPoints()
	{
		if (point1 == null || point2 == null || point3 == null || point4 == null)
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
		point3 = transform.Find("Point 3");
		point4 = transform.Find("Point 4");
	}
	
	public bool CheckValidity()
	{
		if (transform.localPosition == Vector3.zero
		&& transform.localRotation == Quaternion.identity
		&& transform.localScale == Vector3.one
		&& point2.position == new Vector3(point4.position.x, point1.position.y, point4.position.z)
		&& point3.position == new Vector3(point1.position.x, point4.position.y, point1.position.z))
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
		
		if (point3 == null)
		{
			Debug.LogError("Couldn't find Point 3 on " + gameObject.name);
			return;
		}
		
		if (point4 == null)
		{
			Debug.LogError("Couldn't find Point 4 on " + gameObject.name);
			return;
		}
		
		if (CheckValidity())
		{
			return;
		}
		
		point1.parent = null;
		point2.parent = null;
		point3.parent = null;
		point4.parent = null;
		
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		
		point1.parent = transform;
		point2.parent = transform;
		point3.parent = transform;
		point4.parent = transform;
		
		point1.rotation = Quaternion.identity;
		point2.rotation = Quaternion.identity;
		point3.rotation = Quaternion.identity;
		point4.rotation = Quaternion.identity;
		
		point1.localScale = defaultHandleScale;
		point2.localScale = defaultHandleScale;
		point3.localScale = defaultHandleScale;
		point4.localScale = defaultHandleScale;
		
		// regenerate points 2 and 3
		point2.position = new Vector3(point4.position.x, point1.position.y, point4.position.z);
		point3.position = new Vector3(point1.position.x, point4.position.y, point1.position.z);
		
		planeNormal = GetPlaneNormal();
	}
	
	public Vector3 GetPlaneNormal()
	{
		Vector3 side1 = point2.position - point4.position;
		Vector3 side2 = point3.position - point4.position;
		return Vector3.Cross(side1, side2).normalized;
	}
}
