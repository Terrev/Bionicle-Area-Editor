using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPosDifference : MonoBehaviour
{
	public Transform alpha;
	public Transform vanilla;

	void OnDrawGizmos()
	{
		if (alpha == null || vanilla == null)
		{
			return;
		}
		Gizmos.color = Color.cyan;
		for (int i = 0; i < alpha.transform.childCount; i++)
		{
			Gizmos.DrawLine(alpha.GetChild(i).position, vanilla.GetChild(i).position);
		}
	}
}
