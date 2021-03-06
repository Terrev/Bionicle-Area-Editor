﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BionicleObject : MonoBehaviour
{
	public float unknown = -1.0f; // standard value, unused by the game according to JMMB so it shouldn't matter anyway
	public int flags = 16; // flags for common objects like the hammers in onu-koro
	
	[System.NonSerialized]
	public Transform collisionPoint1;
	[System.NonSerialized]
	public Transform collisionPoint2;
	
	int lolCounter; // lol
	
	void OnDrawGizmos()
	{
		lolCounter++;
		if (lolCounter == 300) // dumb way of doing this only occasionally
		{
			CheckForCollisionPoints();
			lolCounter = 0;
		}
		
		if (collisionPoint1 != null && collisionPoint2 != null)
		{
			Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
			
			// bottom
			Vector3 corner1 = collisionPoint1.position;
			Vector3 corner2 = new Vector3(collisionPoint1.position.x, collisionPoint1.position.y, collisionPoint2.position.z);
			Vector3 corner3 = new Vector3(collisionPoint2.position.x, collisionPoint1.position.y, collisionPoint2.position.z); 
			Vector3 corner4 = new Vector3(collisionPoint2.position.x, collisionPoint1.position.y, collisionPoint1.position.z);
			
			// top
			Vector3 corner5 = new Vector3(collisionPoint1.position.x, collisionPoint2.position.y, collisionPoint1.position.z);
			Vector3 corner6 = new Vector3(collisionPoint1.position.x, collisionPoint2.position.y, collisionPoint2.position.z);
			Vector3 corner7 = collisionPoint2.position;
			Vector3 corner8 = new Vector3(collisionPoint2.position.x, collisionPoint2.position.y, collisionPoint1.position.z);
			
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
	
	public void CheckForCollisionPoints()
	{
		collisionPoint1 = transform.Find("Collision Points/Collision Point 1");
		collisionPoint2 = transform.Find("Collision Points/Collision Point 2");
	}
}
