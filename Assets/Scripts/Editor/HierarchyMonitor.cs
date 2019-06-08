using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
public static class HierarchyMonitor
{
	static HierarchyMonitor()
	{
		EditorApplication.hierarchyChanged += OnHierarchyChanged;
	}

	static void OnHierarchyChanged()
	{
		foreach (GameObject blah in Object.FindObjectsOfType(typeof(GameObject)))
		{
			if (blah.name.Contains("_POS.slb"))
			{
				UpdatePositionVisuals(blah);
			}
		}
	}
	
	static void UpdatePositionVisuals(GameObject parent)
	{
		foreach (Transform entry in parent.transform)
		{
			MeshRenderer meshRenderer = entry.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				Debug.Log("No MeshRenderer on " + entry.name);
				continue;
			}
			
			// lol
			
			// start point
			if (entry.name.StartsWith("str"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/str", typeof(Material)) as Material;
			}
			
			// look point
			else if (entry.name.StartsWith("lok"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/lok", typeof(Material)) as Material;
			}
			
			// hive start
			else if (entry.name.StartsWith("hs"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/hs", typeof(Material)) as Material;
			}
			
			// token
			else if (entry.name.StartsWith("et"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/et", typeof(Material)) as Material;
			}
			
			// pickup health
			else if (entry.name.StartsWith("ph"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/ph", typeof(Material)) as Material;
			}
			
			// pickup energy
			else if (entry.name.StartsWith("pe"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/pe", typeof(Material)) as Material;
			}
			
			// pickup air
			else if (entry.name.StartsWith("pa"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/pa", typeof(Material)) as Material;
			}
			
			// ammo
			else if (entry.name.StartsWith("am"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/am", typeof(Material)) as Material;
			}
			
			// arrow
			else if (entry.name.StartsWith("ar"))
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/ar", typeof(Material)) as Material;
			}
			
			else
			{
				meshRenderer.material = Resources.Load("_Editor/Position Markers/Default", typeof(Material)) as Material;
			}
		}
	}
}