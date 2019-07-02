using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
	
	Transform thetaCone1;
	Transform thetaCone2;
	Transform phiCone1;
	Transform phiCone2;
	Material lightMaterial;
	Material darkMaterial;
	
	float intensityLastUpdate = 1.0f;
	
	Color colorLastUpdate = Color.yellow;
	Color colorWithoutAlpha = Color.yellow;
	
	void Update()
	{
		// don't rotate
		if (transform.localRotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.identity;
		}
	}
	
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
		
		if (thetaCone1 == null)
		{
			thetaCone1 = transform.Find("Ignore Me 1");
		}
		
		if (thetaCone2 == null)
		{
			thetaCone2 = transform.Find("Ignore Me 2");
		}
		
		if (phiCone1 == null)
		{
			phiCone1 = transform.Find("Ignore Me 3");
		}
		
		if (phiCone2 == null)
		{
			phiCone2 = transform.Find("Ignore Me 4");
		}
		
		if (lightMaterial == null)
		{
			lightMaterial = Resources.Load("_Editor/Spotlight Cone Light", typeof (Material)) as Material;
		}
		
		if (darkMaterial == null)
		{
			darkMaterial = Resources.Load("_Editor/Spotlight Cone Dark", typeof (Material)) as Material;
		}
		
		Gizmos.color = colorWithoutAlpha;
		
		Gizmos.DrawLine(transform.position, directionMarker.position);
		
		// QUATERNION VERSION OF DIRECTION VECTOR
		Quaternion mainRotation = Quaternion.LookRotation(directionMarker.localPosition);
		
		Vector3 endOfRange = new Vector3(0.0f, 0.0f, adjustedRange);
		endOfRange = mainRotation * endOfRange + transform.position;
		
		// REALLY REALLY DUMB WIREFRAME VISUALS
		// NOW MOSTLY GONE but we still use a few of the points calculated for the new cone visuals
		
		// THETA POINTS
		Vector3 thetaPoint1 = new Vector3(0.0f, 0.0f, adjustedRange);
		Vector3 thetaPoint2 = thetaPoint1;
		//Vector3 thetaPoint3 = thetaPoint1;
		//Vector3 thetaPoint4 = thetaPoint1;
		// ROTATIONS TO EXPAND THEM OUT
		Quaternion thetaRotation1 = Quaternion.Euler(new Vector3(theta / 2.0f, 0.0f, 0.0f));
		Quaternion thetaRotation2 = Quaternion.Euler(new Vector3(-theta / 2.0f, 0.0f, 0.0f));
		//Quaternion thetaRotation3 = Quaternion.Euler(new Vector3(0.0f, theta / 2.0f, 0.0f));
		//Quaternion thetaRotation4 = Quaternion.Euler(new Vector3(0.0f, -theta / 2.0f, 0.0f));
		// APPLY THOSE ROTATIONS AND THE BASE POSITION
		thetaPoint1 = thetaRotation1 * thetaPoint1 + transform.position;
		thetaPoint2 = thetaRotation2 * thetaPoint2 + transform.position;
		//thetaPoint3 = thetaRotation3 * thetaPoint3 + transform.position;
		//thetaPoint4 = thetaRotation4 * thetaPoint4 + transform.position;
		// ROTATE AROUND TO ALIGN WITH DIRECTION MARKER
		thetaPoint1 = RotatePointAroundPivot(thetaPoint1, transform.position, mainRotation);
		thetaPoint2 = RotatePointAroundPivot(thetaPoint2, transform.position, mainRotation);
		//thetaPoint3 = RotatePointAroundPivot(thetaPoint3, transform.position, mainRotation);
		//thetaPoint4 = RotatePointAroundPivot(thetaPoint4, transform.position, mainRotation);
		// DRAW
		/*
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
		*/
		
		// NOW WE DO IT AGAIN FOR PHI
		// HOORAY
		Vector3 phiPoint1 = new Vector3(0.0f, 0.0f, adjustedRange);
		Vector3 phiPoint2 = phiPoint1;
		//Vector3 phiPoint3 = phiPoint1;
		//Vector3 phiPoint4 = phiPoint1;
		// ROTATIONS TO EXPAND THEM OUT
		Quaternion phiRotation1 = Quaternion.Euler(new Vector3(phi / 2.0f, 0.0f, 0.0f));
		Quaternion phiRotation2 = Quaternion.Euler(new Vector3(-phi / 2.0f, 0.0f, 0.0f));
		//Quaternion phiRotation3 = Quaternion.Euler(new Vector3(0.0f, phi / 2.0f, 0.0f));
		//Quaternion phiRotation4 = Quaternion.Euler(new Vector3(0.0f, -phi / 2.0f, 0.0f));
		// APPLY THOSE ROTATIONS AND THE BASE POSITION
		phiPoint1 = phiRotation1 * phiPoint1 + transform.position;
		phiPoint2 = phiRotation2 * phiPoint2 + transform.position;
		//phiPoint3 = phiRotation3 * phiPoint3 + transform.position;
		//phiPoint4 = phiRotation4 * phiPoint4 + transform.position;
		// ROTATE AROUND TO ALIGN WITH DIRECTION MARKER
		phiPoint1 = RotatePointAroundPivot(phiPoint1, transform.position, mainRotation);
		phiPoint2 = RotatePointAroundPivot(phiPoint2, transform.position, mainRotation);
		//phiPoint3 = RotatePointAroundPivot(phiPoint3, transform.position, mainRotation);
		//phiPoint4 = RotatePointAroundPivot(phiPoint4, transform.position, mainRotation);
		// DRAW
		/*
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
		*/
		
		// CONE TIME
		Vector3 thetaCenter = new Vector3((thetaPoint1.x + thetaPoint2.x) / 2.0f, (thetaPoint1.y + thetaPoint2.y) / 2.0f, (thetaPoint1.z + thetaPoint2.z) / 2.0f);
		float thetaWidth = Vector3.Distance(thetaPoint1, thetaPoint2) / 2.0f;
		float thetaCone1Height = Vector3.Distance(thetaCenter, transform.position);
		float thetaCone2Height = Vector3.Distance(thetaCenter, endOfRange);
		if (thetaCone1 != null)
		{
			thetaCone1.position = thetaCenter;
			thetaCone1.localScale = new Vector3(thetaWidth, thetaWidth, thetaCone1Height);
			thetaCone1.LookAt(transform.position);
		}
		if (thetaCone2 != null)
		{
			thetaCone2.position = thetaCenter;
			thetaCone2.localScale = new Vector3(thetaWidth, thetaWidth, thetaCone2Height);
			thetaCone2.LookAt(endOfRange);
		}
		
		Vector3 phiCenter = new Vector3((phiPoint1.x + phiPoint2.x) / 2.0f, (phiPoint1.y + phiPoint2.y) / 2.0f, (phiPoint1.z + phiPoint2.z) / 2.0f);
		float phiWidth = Vector3.Distance(phiPoint1, phiPoint2) / 2.0f;
		float phiCone1Height = Vector3.Distance(phiCenter, transform.position);
		float phiCone2Height = Vector3.Distance(phiCenter, endOfRange);
		if (phiCone1 != null)
		{
			phiCone1.position = phiCenter;
			phiCone1.localScale = new Vector3(phiWidth, phiWidth, phiCone1Height);
			phiCone1.LookAt(transform.position);
		}
		if (phiCone2 != null)
		{
			phiCone2.position = phiCenter;
			phiCone2.localScale = new Vector3(phiWidth, phiWidth, phiCone2Height);
			phiCone2.LookAt(endOfRange);
		}
		
		if (intensity != intensityLastUpdate)
		{
			// not bothering to cache meshrenderers. practically speaking it causes no performance issues. bite me.
			if (intensity > 0.0f)
			{
				thetaCone1.gameObject.GetComponent<MeshRenderer>().material = lightMaterial;
				thetaCone2.gameObject.GetComponent<MeshRenderer>().material = lightMaterial;
				phiCone1.gameObject.GetComponent<MeshRenderer>().material = lightMaterial;
				phiCone2.gameObject.GetComponent<MeshRenderer>().material = lightMaterial;
			}
			else
			{
				thetaCone1.gameObject.GetComponent<MeshRenderer>().material = darkMaterial;
				thetaCone2.gameObject.GetComponent<MeshRenderer>().material = darkMaterial;
				phiCone1.gameObject.GetComponent<MeshRenderer>().material = darkMaterial;
				phiCone2.gameObject.GetComponent<MeshRenderer>().material = darkMaterial;
			}
			intensityLastUpdate = intensity;
		}
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
