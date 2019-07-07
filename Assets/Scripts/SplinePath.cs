using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SplinePath : MonoBehaviour
{
	void Update()
	{
		// don't move
		if (transform.position != Vector3.zero)
		{
			transform.position = Vector3.zero;
		}
		if (transform.localRotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.identity;
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (i != transform.childCount - 1)
			{
				Transform currentPoint = transform.GetChild(i);
				Transform nextPoint = transform.GetChild(i + 1);
				Gizmos.DrawLine(currentPoint.position, nextPoint.position);
			}
		}
	}
	
	public void SetTimesToZero()
	{
		if (EditorUtility.DisplayDialog("Set point times to zero?", "They'll all be 0. All of them. Zilch. Absolutely nuked.", "Yep", "NO"))
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.GetComponent<SplinePoint>().time = 0.0f;
			}
		}
	}
	
	public void SetTimesIncrementally()
	{
		if (EditorUtility.DisplayDialog("Set point times incrementally?", "This will give points times of 0, 1, 2, 3, etc", "Yep", "NO"))
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.GetComponent<SplinePoint>().time = i;
			}
		}
	}
	
	public void SetTimesToNotQuiteRightButCloseEnoughValuesForCams()
	{
		if (EditorUtility.DisplayDialog("Set point times by distance?", "This isn't 100% accurate because splines add a bit to the distance as they curve and smooth things out, and this doesn't account for that yet. It's fairly close though. You can tune it by hand if you want.", "Yep", "NO"))
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				if (i == 0)
				{
					transform.GetChild(i).gameObject.GetComponent<SplinePoint>().time = 0.0f;
				}
				else if (i != transform.childCount - 1)
				{
					transform.GetChild(i).gameObject.GetComponent<SplinePoint>().time = ArcaneRituals(i);
				}
				else
				{
					transform.GetChild(i).gameObject.GetComponent<SplinePoint>().time = 1.0f;
				}
			}
		}
	}
	
	float ArcaneRituals(int howFarToGo)
	{
		float totalLength = 0.0f;
		for (int i = 0; i < transform.childCount; i++)
		{
			if (i != transform.childCount - 1)
			{
				totalLength += Vector3.Distance(transform.GetChild(i).position, transform.GetChild(i + 1).position);
			}
		}
		
		float returnValue = 0.0f;
		for (int i = 0; i < howFarToGo; i++)
		{
			if (i != transform.childCount - 1)
			{
				returnValue += Vector3.Distance(transform.GetChild(i).position, transform.GetChild(i + 1).position);
			}
		}
		
		return returnValue / totalLength;
	}
}
