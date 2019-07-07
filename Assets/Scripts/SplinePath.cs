using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePath : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
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
}
