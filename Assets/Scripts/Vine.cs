using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
	public bool bool1;
	public bool bool2;
	public float float1;
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 1.0f);
		// animation seems to distort the vine model an arbitrary amount (and rotates it too), this seems to be a pretty close visual
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0.0f, 12.11f, 0.0f));
		Gizmos.DrawLine(transform.position, transform.position - (transform.right * 4.0f));
	}
}
