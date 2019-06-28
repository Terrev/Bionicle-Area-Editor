using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightType
{
	Point,
	Directional
}

public class BionicleLight : MonoBehaviour
{
	public float intensity = 1.0f;
	public float range = 10.0f;
	public Color color = Color.yellow;
	public LightType lightType = LightType.Point;
	
	Color colorLastUpdate = Color.yellow;
	Color colorWithoutAlpha = Color.yellow;
	
	void OnDrawGizmos()
	{
		if (color != colorLastUpdate)
		{
			colorWithoutAlpha = new Color(color.r, color.g, color.b, 1.0f);
			colorLastUpdate = color;
		}
		Gizmos.color = colorWithoutAlpha;
		if (lightType == LightType.Point)
		{
			Gizmos.DrawLine(transform.position + Vector3.right, transform.position - Vector3.right);
			Gizmos.DrawLine(transform.position + Vector3.up, transform.position - Vector3.up);
			Gizmos.DrawLine(transform.position + Vector3.forward, transform.position - Vector3.forward);
			Gizmos.DrawWireSphere(transform.position, range);
		}
		else // directional
		{
			Gizmos.DrawLine(Vector3.zero, transform.position);
		}
	}
	
	public void Normalize()
	{
		transform.position = Vector3.Normalize(transform.position);
	}
}
