﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
	[System.NonSerialized]
	public Transform point1;
	[System.NonSerialized]
	public Transform point2;
	
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
		
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		
		point1.parent = transform;
		point2.parent = transform;
		
		point1.rotation = Quaternion.identity;
		point2.rotation = Quaternion.identity;
		
		point1.localScale = defaultHandleScale;
		point2.localScale = defaultHandleScale;
	}
}
