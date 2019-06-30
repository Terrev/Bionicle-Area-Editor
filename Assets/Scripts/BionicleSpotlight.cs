using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BionicleSpotlight : MonoBehaviour
{
	public float intensity = 1.0f;
	public Color color = Color.yellow;
	public float theta = 20.0f; // inner
	public float phi = 22.0f; // outer
	public float range = 50;
	float adjustedRange = 150.0f;
	
	[System.NonSerialized]
	public Transform directionMarker;
	
	Color colorLastUpdate = Color.yellow;
	Color colorWithoutAlpha = Color.yellow;
	
	void OnDrawGizmos()
	{
		adjustedRange = range * 3.0f; // game seems to triple the range...?
		
		if (color != colorLastUpdate)
		{
			colorWithoutAlpha = new Color(color.r, color.g, color.b, 1.0f);
			colorLastUpdate = color;
		}
		
		if (directionMarker == null)
		{
			directionMarker = transform.Find("Direction");
		}
		if (directionMarker == null)
		{
			return;
		}
		
		Gizmos.color = colorWithoutAlpha;
		
		Gizmos.DrawLine(transform.position, directionMarker.position);
		
		// QUATERNION VERSION OF DIRECTION VECTOR
		Quaternion mainRotation = Quaternion.LookRotation(directionMarker.localPosition);
		
		// THIS IS THEORETICALLY THE DIRECTION MARKER POSITION AS WELL
		// BUT THEY CAN BE OUT OF SYNC
		Vector3 endOfRange = new Vector3(0.0f, 0.0f, adjustedRange);
		endOfRange = mainRotation * endOfRange + transform.position;
		
		// REALLY REALLY DUMB
		
		// THETA POINTS
		Vector3 thetaPoint1 = new Vector3(0.0f, 0.0f, adjustedRange);
		Vector3 thetaPoint2 = thetaPoint1;
		Vector3 thetaPoint3 = thetaPoint1;
		Vector3 thetaPoint4 = thetaPoint1;
		// ROTATIONS TO EXPAND THEM OUT
		Quaternion thetaRotation1 = Quaternion.Euler(new Vector3(theta / 2.0f, 0.0f, 0.0f));
		Quaternion thetaRotation2 = Quaternion.Euler(new Vector3(-theta / 2.0f, 0.0f, 0.0f));
		Quaternion thetaRotation3 = Quaternion.Euler(new Vector3(0.0f, theta / 2.0f, 0.0f));
		Quaternion thetaRotation4 = Quaternion.Euler(new Vector3(0.0f, -theta / 2.0f, 0.0f));
		// APPLY THOSE ROTATIONS AND THE BASE POSITION
		thetaPoint1 = thetaRotation1 * thetaPoint1 + transform.position;
		thetaPoint2 = thetaRotation2 * thetaPoint2 + transform.position;
		thetaPoint3 = thetaRotation3 * thetaPoint3 + transform.position;
		thetaPoint4 = thetaRotation4 * thetaPoint4 + transform.position;
		// ROTATE AROUND TO ALIGN WITH DIRECTION MARKER
		thetaPoint1 = RotatePointAroundPivot(thetaPoint1, transform.position, mainRotation);
		thetaPoint2 = RotatePointAroundPivot(thetaPoint2, transform.position, mainRotation);
		thetaPoint3 = RotatePointAroundPivot(thetaPoint3, transform.position, mainRotation);
		thetaPoint4 = RotatePointAroundPivot(thetaPoint4, transform.position, mainRotation);
		// DRAW
		Gizmos.DrawLine(transform.position, thetaPoint1);
		Gizmos.DrawLine(transform.position, thetaPoint2);
		Gizmos.DrawLine(transform.position, thetaPoint3);
		Gizmos.DrawLine(transform.position, thetaPoint4);
		Gizmos.DrawLine(thetaPoint1, thetaPoint3);
		Gizmos.DrawLine(thetaPoint1, thetaPoint4);
		Gizmos.DrawLine(thetaPoint2, thetaPoint3);
		Gizmos.DrawLine(thetaPoint2, thetaPoint4);
		Gizmos.DrawLine(endOfRange, thetaPoint1);
		Gizmos.DrawLine(endOfRange, thetaPoint2);
		Gizmos.DrawLine(endOfRange, thetaPoint3);
		Gizmos.DrawLine(endOfRange, thetaPoint4);
		
		// NOW WE DO IT AGAIN FOR PHI
		// HOORAY
		Vector3 phiPoint1 = new Vector3(0.0f, 0.0f, adjustedRange);
		Vector3 phiPoint2 = phiPoint1;
		Vector3 phiPoint3 = phiPoint1;
		Vector3 phiPoint4 = phiPoint1;
		// ROTATIONS TO EXPAND THEM OUT
		Quaternion phiRotation1 = Quaternion.Euler(new Vector3(phi / 2.0f, 0.0f, 0.0f));
		Quaternion phiRotation2 = Quaternion.Euler(new Vector3(-phi / 2.0f, 0.0f, 0.0f));
		Quaternion phiRotation3 = Quaternion.Euler(new Vector3(0.0f, phi / 2.0f, 0.0f));
		Quaternion phiRotation4 = Quaternion.Euler(new Vector3(0.0f, -phi / 2.0f, 0.0f));
		// APPLY THOSE ROTATIONS AND THE BASE POSITION
		phiPoint1 = phiRotation1 * phiPoint1 + transform.position;
		phiPoint2 = phiRotation2 * phiPoint2 + transform.position;
		phiPoint3 = phiRotation3 * phiPoint3 + transform.position;
		phiPoint4 = phiRotation4 * phiPoint4 + transform.position;
		// ROTATE AROUND TO ALIGN WITH DIRECTION MARKER
		phiPoint1 = RotatePointAroundPivot(phiPoint1, transform.position, mainRotation);
		phiPoint2 = RotatePointAroundPivot(phiPoint2, transform.position, mainRotation);
		phiPoint3 = RotatePointAroundPivot(phiPoint3, transform.position, mainRotation);
		phiPoint4 = RotatePointAroundPivot(phiPoint4, transform.position, mainRotation);
		// DRAW
		Gizmos.DrawLine(transform.position, phiPoint1);
		Gizmos.DrawLine(transform.position, phiPoint2);
		Gizmos.DrawLine(transform.position, phiPoint3);
		Gizmos.DrawLine(transform.position, phiPoint4);
		Gizmos.DrawLine(phiPoint1, phiPoint3);
		Gizmos.DrawLine(phiPoint1, phiPoint4);
		Gizmos.DrawLine(phiPoint2, phiPoint3);
		Gizmos.DrawLine(phiPoint2, phiPoint4);
		Gizmos.DrawLine(endOfRange, phiPoint1);
		Gizmos.DrawLine(endOfRange, phiPoint2);
		Gizmos.DrawLine(endOfRange, phiPoint3);
		Gizmos.DrawLine(endOfRange, phiPoint4);
		
	}
	
	// THANKS RANDOM UNITY ANSWERS GUY FOR SAVING ME A FEW MINUTES
	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = angles * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	public void CalculateRangeFromDirection()
	{
		range = directionMarker.localPosition.magnitude;
	}
	
	public void CalculateDirectionFromRange()
	{
		Vector3 normalizedDirection = Vector3.Normalize(directionMarker.localPosition);
		directionMarker.localPosition = normalizedDirection * range;
	}
}
