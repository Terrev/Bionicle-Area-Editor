using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

// I WOULD LIKE TO THANK MY CTRL, C, AND V KEYS FOR THEIR HARD WORK AND DEDICATION TO THIS PROJECT

public class Areas : MonoBehaviour
{
	public string gameVersion = "beta";
	public string levelName = "lev1";
	public string areaName = "atrm";
	public bool overwriteSlbInResources = true;
	
	// axes at origin
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.right);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}
	
	
	
	///////////////////////////////////////////////////////////////////
	#region TEST
	
	public void LoadEntireGame()
	{
		levelName = "fren";
		LoadEntireLevel();
		if (gameVersion != "alpha")
		{
			levelName = "lev0";
			LoadEntireLevel();
		}
		levelName = "lev1";
		LoadEntireLevel();
		levelName = "lev2";
		LoadEntireLevel();
		levelName = "lev3";
		LoadEntireLevel();
		levelName = "lev4";
		LoadEntireLevel();
		levelName = "lev5";
		LoadEntireLevel();
		levelName = "lev6";
		LoadEntireLevel();
		levelName = "lev7";
		LoadEntireLevel();
	}
	
	public void LoadEntireLevel()
	{
		string[] areas = Directory.GetDirectories(Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName);
		for (int i = 0; i < areas.Length; i++)
		{
			areaName = areas[i].Substring(areas[i].Length - 4);
			if (areaName != levelName && areaName != "ures") // textures folder lol
			{
				//LoadObjSlb();
				//LoadPosSlb();
				//LoadCharSlb();
				//LoadTriggerSlb();
				LoadLightSlb();
			}
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region OBJ
	
	public void LoadObjSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_OBJ.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_OBJ.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 8;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// LOCATION
			Vector3 location = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// ORIENTATION
			Vector3 orientation = new Vector3(binaryReader.ReadSingle(), -binaryReader.ReadSingle(), -binaryReader.ReadSingle());
			
			// UNKNOWN
			float unknown = binaryReader.ReadSingle();
			
			// COLLISION POINT 1
			Vector3 collisionPoint1 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// COLLISION POINT 2
			Vector3 collisionPoint2 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// FLAGS
			UInt32 flags = binaryReader.ReadUInt32();
			
			// INSTANTIATE IN SCENE
			GameObject newGameObject;
			GameObject obj = (GameObject)Resources.Load(gameVersion + "/levels/" + levelName + "/" + areaName + "/" + identifier, typeof(GameObject));
			// for the alpha's Xs folders
			if (obj == null)
			{
				obj = (GameObject)Resources.Load(gameVersion + "/levels/" + levelName + "/" + areaName + "/Xs/" + identifier, typeof(GameObject));
			}
			// if THAT didn't work
			if (obj == null)
			{
				Debug.LogWarning("Could not load model for " + identifier + ", please make sure the .x is converted");
				newGameObject = Instantiate(Resources.Load("_Editor/Object Marker", typeof(GameObject)), location, Quaternion.Euler(orientation), slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			else
			{
				newGameObject = Instantiate(obj, location, Quaternion.Euler(orientation), slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			// BIONICLEOBJECT COMPONENT FOR EXTRA DATA
			BionicleObject bionicleObject = newGameObject.AddComponent<BionicleObject>() as BionicleObject;
			bionicleObject.unknown = unknown;
			bionicleObject.flags = (int)flags;
			// COLLISION POINT GAMEOBJECTS
			if (collisionPoint1 != Vector3.zero || collisionPoint2 != Vector3.zero)
			{
				// PARENT FOR DUMB ROTATION HACK
				GameObject collisionPointParent = new GameObject("Collision Points");
				collisionPointParent.AddComponent<CollisionPointHack>();
				collisionPointParent.transform.parent = newGameObject.transform;
				collisionPointParent.transform.localPosition = Vector3.zero;
				collisionPointParent.transform.localRotation = Quaternion.identity;
				
				// POINT 1
				GameObject blah = Instantiate(Resources.Load("_Editor/Collision Point", typeof(GameObject))) as GameObject;
				blah.name = "Collision Point 1";
				blah.transform.parent = collisionPointParent.transform;
				blah.transform.localPosition = collisionPoint1;
				blah.transform.localRotation = Quaternion.identity;
				
				// POINT 2
				GameObject blah2 = Instantiate(Resources.Load("_Editor/Collision Point", typeof(GameObject))) as GameObject;
				blah2.name = "Collision Point 2";
				blah2.transform.parent = collisionPointParent.transform;
				blah2.transform.localPosition = collisionPoint2;
				blah2.transform.localRotation = Quaternion.identity;
			}
		}
		// SHRUG
		// some stackoverflow post said closing the reader SHOULD close the stream but I don't trust "should" enough lol
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveObjSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_OBJ.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_OBJ.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// make sure all entry gameobjects have BionicleObject components (and grab them all)
		List<BionicleObject> bionicleObjects = new List<BionicleObject>();
		foreach (GameObject entry in entries)
		{
			BionicleObject bionicleObject = entry.GetComponent<BionicleObject>();
			if (bionicleObject == null)
			{
				Debug.LogError("No BionicleObject component on " + entry.name);
				return;
			}
			bionicleObjects.Add(bionicleObject);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_OBJ.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(0); // identifier - is this really used for anything?
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(22); // table offset
		binaryWriter.Write(new byte[6]); // padding, apparently unread... what's the best way to do this? oh well, this works
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// LOCATION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// ORIENTATION
			binaryWriter.Write(Utilities.ClampRotation(Utilities.DumbCheck(entries[i].transform.localEulerAngles.x)));
			binaryWriter.Write(Utilities.ClampRotation(Utilities.DumbCheck(-entries[i].transform.localEulerAngles.y)));
			binaryWriter.Write(Utilities.ClampRotation(Utilities.DumbCheck(-entries[i].transform.localEulerAngles.z)));
			
			// UNKNOWN
			binaryWriter.Write(bionicleObjects[i].unknown);
			
			// COLLISION POINT 1
			Transform transform1 = entries[i].transform.Find("Collision Points/Collision Point 1");
			if (transform1 == null)
			{
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(Utilities.DumbCheck(-transform1.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(transform1.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(transform1.localPosition.z));
			}
			
			// COLLISION POINT 2
			Transform transform2 = entries[i].transform.Find("Collision Points/Collision Point 2");
			if (transform2 == null)
			{
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(Utilities.DumbCheck(-transform2.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(transform2.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(transform2.localPosition.z));
			}
			
			// FLAGS
			binaryWriter.Write(bionicleObjects[i].flags);
		}
		
		// moar padding, apparently? JMMB says it's cause the footer has to be aligned nicely
		binaryWriter.Write(new byte[2]);
		
		// pointer thingy (lol)
		binaryWriter.Write(12);
		
		// entry count (lol again)
		binaryWriter.Write(1);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_OBJ.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region POS
	
	public void LoadPosSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_POS.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_POS.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 4;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POSITION
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// FLAGS
			UInt32 flags = binaryReader.ReadUInt32();
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(Resources.Load("_Editor/Position Markers/Position Marker", typeof(GameObject)), position, Quaternion.identity, slbParent.transform) as GameObject;
			
			newGameObject.name = identifier + " " + flags;
		}
		// SHRUUUUG
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SavePosSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_POS.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_POS.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_POS.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(12); // table offset
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// POSITION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// FLAGS
			string flagsString = entries[i].name.Substring(5, entries[i].name.Length - 5);
			binaryWriter.Write(UInt32.Parse(flagsString));
		}
		
		// pointer thingy (lol)
		binaryWriter.Write(8);
		
		// entry count (lol again)
		binaryWriter.Write(1);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_POS.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region LIGHT
	
	public void LoadLightSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_LIGHT.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_LIGHT.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 4;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POSITION
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// BLAH BLAH LIGHTS BLAH
			float intensity = binaryReader.ReadSingle();
			float range = binaryReader.ReadSingle();
			Color color = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			UInt32 flags = binaryReader.ReadUInt32();
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(Resources.Load("_Editor/Light Marker", typeof(GameObject)), position, Quaternion.identity, slbParent.transform) as GameObject;
			newGameObject.name = identifier;
			BionicleLight bionicleLight = newGameObject.GetComponent<BionicleLight>();
			bionicleLight.intensity = intensity;
			bionicleLight.range = range;
			bionicleLight.color = color;
			if (flags == 96)
			{
				bionicleLight.lightType = LightType.Point;
			}
			else if (flags == 129)
			{
				bionicleLight.lightType = LightType.Directional;
			}
			else
			{
				Debug.LogWarning("Unknown light flags on " + identifier + ", defaulting to standard point light");
				bionicleLight.lightType = LightType.Point;
			}
		}
		// SHRUUUUG
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveLightSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_LIGHT.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_LIGHT.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// components
		List<BionicleLight> bionicleLights = new List<BionicleLight>();
		foreach (GameObject entry in entries)
		{
			BionicleLight bionicleLight = entry.GetComponent<BionicleLight>();
			if (bionicleLight == null)
			{
				Debug.LogError("No BionicleLight component on " + entry.name);
				return;
			}
			bionicleLights.Add(bionicleLight);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_LIGHT.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(12); // table offset
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// POSITION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// YOU KNOW WHAT TO EXPECT
			binaryWriter.Write(bionicleLights[i].intensity);
			binaryWriter.Write(bionicleLights[i].range);
			binaryWriter.Write(bionicleLights[i].color.r);
			binaryWriter.Write(bionicleLights[i].color.g);
			binaryWriter.Write(bionicleLights[i].color.b);
			binaryWriter.Write(bionicleLights[i].color.a);
			if (bionicleLights[i].lightType == LightType.Point)
			{
				binaryWriter.Write(96);
			}
			else // directional
			{
				binaryWriter.Write(129);
			}
		}
		
		// pointer thingy (lol)
		binaryWriter.Write(8);
		
		// entry count (lol again)
		binaryWriter.Write(1);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_LIGHT.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region SPOT
	
	public void LoadSpotSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_SPOT.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_SPOT.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 4;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POSITION / DIRECTION
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Vector3 direction = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// BLAH BLAH LIGHTS BLAH
			float intensity = binaryReader.ReadSingle();
			Color color = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			float theta = binaryReader.ReadSingle(); // INNER
			float phi = binaryReader.ReadSingle(); // OUTER
			float range = binaryReader.ReadSingle();
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(Resources.Load("_Editor/Spotlight Marker", typeof(GameObject)), position, Quaternion.identity, slbParent.transform) as GameObject;
			newGameObject.name = identifier;
			Transform directionMarker = newGameObject.transform.Find("Direction");
			directionMarker.localPosition = direction;
			BionicleSpotlight bionicleSpotlight = newGameObject.GetComponent<BionicleSpotlight>();
			bionicleSpotlight.intensity = intensity;
			bionicleSpotlight.color = color;
			bionicleSpotlight.theta = theta;
			bionicleSpotlight.phi = phi;
			bionicleSpotlight.range = range;
		}
		// SHRUUUUG
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveSpotSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_SPOT.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_SPOT.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// components
		List<BionicleSpotlight> bionicleSpotlights = new List<BionicleSpotlight>();
		foreach (GameObject entry in entries)
		{
			BionicleSpotlight bionicleSpotlight = entry.GetComponent<BionicleSpotlight>();
			if (bionicleSpotlight == null)
			{
				Debug.LogError("No BionicleSpotlight component on " + entry.name);
				return;
			}
			bionicleSpotlights.Add(bionicleSpotlight);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_SPOT.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(12); // table offset
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// POSITION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// DIRECTION
			Transform directionMarker = entries[i].transform.Find("Direction");
			if (directionMarker == null)
			{
				Debug.LogWarning("Direction marker not found on " + entries[i].name);
				binaryWriter.Write(-100.0f); // random values, whatever, shouldn't happen
				binaryWriter.Write(-100.0f);
				binaryWriter.Write(100.0f);
			}
			else
			{
				binaryWriter.Write(Utilities.DumbCheck(-directionMarker.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(directionMarker.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(directionMarker.localPosition.z));
			}
			
			// YOU KNOW WHAT TO EXPECT
			binaryWriter.Write(bionicleSpotlights[i].intensity);
			binaryWriter.Write(bionicleSpotlights[i].color.r);
			binaryWriter.Write(bionicleSpotlights[i].color.g);
			binaryWriter.Write(bionicleSpotlights[i].color.b);
			binaryWriter.Write(bionicleSpotlights[i].color.a);
			binaryWriter.Write(bionicleSpotlights[i].theta);
			binaryWriter.Write(bionicleSpotlights[i].phi);
			binaryWriter.Write(bionicleSpotlights[i].range);
			
		}
		
		// pointer thingy (lol)
		binaryWriter.Write(8);
		
		// entry count (lol again)
		binaryWriter.Write(1);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_SPOT.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region TRIGGER
	
	public void LoadTriggerSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_TRIGGER.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_TRIGGER.slb");
		GameObject boxesParent = new GameObject("Boxes");
		GameObject planesParent = new GameObject("Planes");
		boxesParent.transform.parent = slbParent.transform;
		planesParent.transform.parent = slbParent.transform;
		
		// READ BASIC INFO
		UInt32 boxEntryCountUnused = binaryReader.ReadUInt32();
		UInt32 boxEntryCount = binaryReader.ReadUInt32();
		UInt32 boxTableOffset = binaryReader.ReadUInt32();
		
		UInt32 planeEntryCountUnused = binaryReader.ReadUInt32();
		UInt32 planeEntryCount = binaryReader.ReadUInt32();
		UInt32 planeTableOffset = binaryReader.ReadUInt32();
		
		// BOX TABLE FIRST, GO TO START OF IT
		fileStream.Seek(boxTableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH BOX ENTRIES
		for (int i = 0; i < boxEntryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POINT 1
			Vector3 point1 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// POINT 2
			Vector3 point2 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// INSTANTIATE IN SCENE
			GameObject newGameObject = new GameObject(identifier);
			newGameObject.transform.parent = boxesParent.transform;
			BoxTrigger boxTrigger = newGameObject.AddComponent<BoxTrigger>() as BoxTrigger;
			
			GameObject blah1 = Instantiate(Resources.Load("_Editor/Box Trigger Corner", typeof(GameObject))) as GameObject;
			blah1.name = "Point 1";
			blah1.transform.parent = newGameObject.transform;
			blah1.transform.localPosition = point1;
			blah1.transform.localRotation = Quaternion.identity;
			
			GameObject blah2 = Instantiate(Resources.Load("_Editor/Box Trigger Corner", typeof(GameObject))) as GameObject;
			blah2.name = "Point 2";
			blah2.transform.parent = newGameObject.transform;
			blah2.transform.localPosition = point2;
			blah2.transform.localRotation = Quaternion.identity;
			
			/*
			GameObject boxVisual = Instantiate(Resources.Load("_Editor/Box Trigger Visual", typeof(GameObject))) as GameObject;
			boxVisual.name = "Visual";
			boxVisual.transform.parent = newGameObject.transform;
			boxVisual.transform.localScale = new Vector3(Mathf.Abs(point1.x - point2.x), Mathf.Abs(point1.y - point2.y), Mathf.Abs(point1.z - point2.z));
			boxVisual.transform.localPosition = new Vector3((point1.x + point2.x) / 2.0f, (point1.y + point2.y) / 2.0f, (point1.z + point2.z) / 2.0f);
			*/
		}
		
		// PLANE TABLE TIME
		fileStream.Seek(planeTableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH PLANE ENTRIES
		for (int i = 0; i < planeEntryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POINTS
			Vector3 point1 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Vector3 point2 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Vector3 point3 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Vector3 point4 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// VECTOR THING?
			Vector3 planeNormal = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// AREA, START, LOOK
			string area = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			string startPoint = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			string lookPoint = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// INSTANTIATE IN SCENE
			GameObject newGameObject = new GameObject(identifier);
			newGameObject.transform.parent = planesParent.transform;
			PlaneTrigger planeTrigger = newGameObject.AddComponent<PlaneTrigger>() as PlaneTrigger;
			planeTrigger.area = area;
			planeTrigger.startPoint = startPoint;
			planeTrigger.lookPoint = lookPoint;
			planeTrigger.originalPlaneNormal = planeNormal;
			
			// MOAR COPYPASTE
			GameObject blah1 = Instantiate(Resources.Load("_Editor/Plane Trigger Corner", typeof(GameObject))) as GameObject;
			blah1.name = "Point 1";
			blah1.transform.parent = newGameObject.transform;
			blah1.transform.localPosition = point1;
			blah1.transform.localRotation = Quaternion.identity;
			
			GameObject blah2 = Instantiate(Resources.Load("_Editor/Plane Trigger Corner", typeof(GameObject))) as GameObject;
			blah2.name = "Point 2";
			blah2.transform.parent = newGameObject.transform;
			blah2.transform.localPosition = point2;
			blah2.transform.localRotation = Quaternion.identity;
			
			GameObject blah3 = Instantiate(Resources.Load("_Editor/Plane Trigger Corner", typeof(GameObject))) as GameObject;
			blah3.name = "Point 3";
			blah3.transform.parent = newGameObject.transform;
			blah3.transform.localPosition = point3;
			blah3.transform.localRotation = Quaternion.identity;
			
			GameObject blah4 = Instantiate(Resources.Load("_Editor/Plane Trigger Corner", typeof(GameObject))) as GameObject;
			blah4.name = "Point 4";
			blah4.transform.parent = newGameObject.transform;
			blah4.transform.localPosition = point4;
			blah4.transform.localRotation = Quaternion.identity;
		}
		
		// SHRUG
		// some stackoverflow post said closing the reader SHOULD close the stream but I don't trust "should" enough lol
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveTriggerSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_TRIGGER.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_TRIGGER.slb");
			return;
		}
		
		// grab the slb's gameobjects/entries
		Transform boxesParent = parentGameObject.transform.Find("Boxes");
		Transform planesParent = parentGameObject.transform.Find("Planes");
		List<GameObject> boxEntries = new List<GameObject>();
		List<GameObject> planeEntries = new List<GameObject>();
		List<BoxTrigger> boxTriggers = new List<BoxTrigger>();
		List<PlaneTrigger> planeTriggers = new List<PlaneTrigger>();
		if (boxesParent == null)
		{
			Debug.LogError("Couldn't find Boxes GameObject");
			return;
		}
		if (planesParent == null)
		{
			Debug.LogError("Couldn't find Planes GameObject");
			return;
		}
		foreach (Transform box in boxesParent)
		{
			boxEntries.Add(box.gameObject);
			BoxTrigger boxTrigger = box.GetComponent<BoxTrigger>();
			if (boxTrigger == null)
			{
				Debug.LogError("No BoxTrigger component on " + box.name);
				return;
			}
			if (!boxTrigger.CheckPoints())
			{
				Debug.LogError("One or more missing points on " + box.name);
				return;
			}
			boxTrigger.ApplyTransformation();
			if (!boxTrigger.CheckIfCube())
			{
				Debug.LogWarning(box.name + " is not a cube");
			}
			boxTriggers.Add(boxTrigger);
		}
		foreach (Transform plane in planesParent)
		{
			planeEntries.Add(plane.gameObject);
			PlaneTrigger planeTrigger = plane.GetComponent<PlaneTrigger>();
			if (planeTrigger == null)
			{
				Debug.LogError("No PlaneTrigger component on " + plane.name);
				return;
			}
			if (!planeTrigger.CheckPoints())
			{
				Debug.LogError("One or more missing points on " + plane.name);
				return;
			}
			planeTrigger.ApplyTransformation();
			planeTriggers.Add(planeTrigger);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_TRIGGER.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(boxEntries.Count); // entry count (unused)
		binaryWriter.Write(boxEntries.Count); // entry count
		binaryWriter.Write(24); // table offset
		
		binaryWriter.Write(planeEntries.Count); // entry count (unused)
		binaryWriter.Write(planeEntries.Count); // entry count
		binaryWriter.Write(24 + (28 * boxEntries.Count)); // table offset (initial data + length of box entries)
		
		// BOX ENTRIES
		for (int i = 0; i < boxEntries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(boxEntries[i].name));
			
			// AAAAAAAAAAAAA
			binaryWriter.Write(Utilities.DumbCheck(-boxTriggers[i].point1.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(boxTriggers[i].point1.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(boxTriggers[i].point1.localPosition.z));
			
			binaryWriter.Write(Utilities.DumbCheck(-boxTriggers[i].point2.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(boxTriggers[i].point2.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(boxTriggers[i].point2.localPosition.z));
		}
		
		// PLANE ENTRIES
		for (int i = 0; i < planeEntries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(planeEntries[i].name));
			
			// AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
			binaryWriter.Write(Utilities.DumbCheck(-planeTriggers[i].point1.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point1.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point1.localPosition.z));
			
			binaryWriter.Write(Utilities.DumbCheck(-planeTriggers[i].point2.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point2.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point2.localPosition.z));
			
			binaryWriter.Write(Utilities.DumbCheck(-planeTriggers[i].point3.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point3.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point3.localPosition.z));
			
			binaryWriter.Write(Utilities.DumbCheck(-planeTriggers[i].point4.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point4.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].point4.localPosition.z));
			
			binaryWriter.Write(Utilities.DumbCheck(-planeTriggers[i].planeNormal.x));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].planeNormal.y));
			binaryWriter.Write(Utilities.DumbCheck(planeTriggers[i].planeNormal.z));
			
			// a
			binaryWriter.Write(Utilities.StringToCharArray(planeTriggers[i].area));
			binaryWriter.Write(Utilities.StringToCharArray(planeTriggers[i].startPoint));
			binaryWriter.Write(Utilities.StringToCharArray(planeTriggers[i].lookPoint));
		}
		
		// pointer thingy (lol)
		binaryWriter.Write(8);
		// wow look another one
		binaryWriter.Write(20);
		
		// entry count (lol again)
		binaryWriter.Write(2);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_TRIGGER.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region CHAR
	
	public void LoadCharSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_CHAR.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_CHAR.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 4;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POSITION
			// some slb templates say position, others say location, doesn't matter but SHRUG
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// ORIENTATION
			// apparently unread/unused for characters
			Vector3 orientation = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// UNKNOWN
			// also unread
			float unknown = binaryReader.ReadSingle();
			
			// TRIGGER BOX TABLE (lev1\hk01)
			UInt32 triggerBoxEntryCountUnused = binaryReader.ReadUInt32();
			UInt32 triggerBoxEntryCount = binaryReader.ReadUInt32();
			UInt32 triggerBoxOffset = binaryReader.ReadUInt32();
			if (triggerBoxEntryCountUnused != 0 || triggerBoxEntryCount != 0)
			{
				Debug.LogWarning("Trigger box data found for " + identifier + ", this data is not supported yet and will be lost if you re-save the file");
			}
			
			// SPLINE PATHS TABLE (lev3\gly1, lev5\lep1)
			UInt32 splinePathEntryCount = binaryReader.ReadUInt32();
			UInt32 splinePathOffset = binaryReader.ReadUInt32();
			if (splinePathEntryCount != 0)
			{
				Debug.LogWarning("Spline data found, this data is not supported yet and will be lost if you re-save the file");
			}
			
			// -----------------------------------------------
			// TODO/WIP
			// EXTRA DATA
			// -----------------------------------------------
			
			// READ TRIGGER BOXES
			/*
			List<string> triggerBoxes = new List<string>();
			if (triggerBoxEntryCount != 0)
			{
				fileStream.Seek(triggerBoxOffset, SeekOrigin.Begin);
				for (int j = 0; j < triggerBoxEntryCount; j++)
				{
					triggerBoxes.Add(Utilities.CharArrayToString(binaryReader.ReadChars(4)));
				}
			}
			*/
			// then add component with triggerBoxes contents later
			// tbh it's just the exporting part I don't wanna deal with but eh it probably won't be that bad, I just wanna get this out the door
			
			// READ SPLINE PATHS
			// was working on this at like 3 AM a few weeks ago and abruptly stopped cause BED
			/*
			List<string> splinePaths = new List<string>(); // however we wanna store this
			if (splinePathEntryCount != 0)
			{
				fileStream.Seek(splinePathOffset, SeekOrigin.Begin);
				for (int k = 0; k < splinePathEntryCount; k++)
				{
					UInt32 splinePointsEntryCount = binaryReader.ReadUInt32();
					UInt32 splinePointsOffset = binaryReader.ReadUInt32();
					UInt32 unusedPointsOffset = binaryReader.ReadUInt32(); // always 0, reserved pointer space for game engine apparently
					splinePaths.Add("the stuff above");
				}
				
				// for each thing in splinePaths above
				// blahblahblahblah
				fileStream.Seek(splinePointsOffset, SeekOrigin.Begin);
				for (int l = 0; l < splinePointsEntryCount; l++)
				{
					float time = binaryReader.ReadSingle();
					float valueThingy = binaryReader.ReadSingle();
				}
			}
			*/
			
			
			// PUT CHARACTER/MARKER IN SCENE
			GameObject newGameObject;
			GameObject character = (GameObject)Resources.Load(gameVersion + "/characters/" + identifier + "/" + identifier, typeof(GameObject));
			if (character == null)
			{
				character = (GameObject)Resources.Load(gameVersion + "/characters/" + identifier + "/Xs/" + identifier, typeof(GameObject));
			}
			if (character == null)
			{
				Debug.LogWarning("Could not load character model for " + identifier);
				newGameObject = Instantiate(Resources.Load("_Editor/Character Marker", typeof(GameObject)), position, Quaternion.identity, slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			else
			{
				newGameObject = Instantiate(character, position, Quaternion.identity, slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			// BIONICLECHARACTER COMPONENT FOR EXTRA DATA
			BionicleCharacter bionicleCharacter = newGameObject.AddComponent<BionicleCharacter>() as BionicleCharacter;
			bionicleCharacter.unusedOrientation = orientation;
			bionicleCharacter.unknown = unknown;
		}
		// SHRUG
		// some stackoverflow post said closing the reader SHOULD close the stream but I don't trust "should" enough lol
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveCharSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_CHAR.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_CHAR.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// make sure all entry gameobjects have BionicleCharacter components (and grab them all)
		List<BionicleCharacter> bionicleCharacters = new List<BionicleCharacter>();
		foreach (GameObject entry in entries)
		{
			BionicleCharacter bionicleCharacter = entry.GetComponent<BionicleCharacter>();
			if (bionicleCharacter == null)
			{
				Debug.LogWarning("No BionicleCharacter component on " + entry.name + ", adding default/empty component");
				bionicleCharacter = entry.AddComponent<BionicleCharacter>();
			}
			bionicleCharacters.Add(bionicleCharacter);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_CHAR.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(12); // table offset
		
		// calculate for use later
		int charTableLength = 12 + (entries.Count * 52); // 12 for initial data, and each entry is 52 long
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// POSITION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// ORIENTATION (unused)
			binaryWriter.Write(bionicleCharacters[i].unusedOrientation.x);
			binaryWriter.Write(bionicleCharacters[i].unusedOrientation.y);
			binaryWriter.Write(bionicleCharacters[i].unusedOrientation.z);
			
			// UNKNOWN
			binaryWriter.Write(bionicleCharacters[i].unknown);
			
			// TEMP TRIGGER BOXES
			binaryWriter.Write(0); // entry count (unused)
			binaryWriter.Write(0); // entry count
			binaryWriter.Write(charTableLength); // offset points to end of character table/start of offset stuff to signify no data, according to shadowknight
			
			// TEMP SPLINE PATHS TABLE
			binaryWriter.Write(0); // entry count
			binaryWriter.Write(charTableLength); // same as before
			
		}
		
		// pointers to offsets or whatever
		
		// character table offset
		binaryWriter.Write(8);
		
		// offsets for trigger box and spline path tables
		// idk dude
		for (int j = 0; j < entries.Count; j++)
		{
			int offset = 12; // initial data
			if (j > 0)
			{
				offset += j * 52; // skip forward as many entries as we need
			}
			offset += 40; // entry data up to first offset
			binaryWriter.Write(offset);
			offset += 8; // onwards to the next offset
			binaryWriter.Write(offset);
		}
		
		// offset entry count
		binaryWriter.Write(1 + (entries.Count * 2));
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_CHAR.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region HIVE
	
	public void LoadHiveSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_HIVE.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_HIVE.slb");
		
		// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
		binaryReader.BaseStream.Position = 4;
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// IDENTIFIER
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// POSITION
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// ORIENTATION
			float orientation = -binaryReader.ReadSingle();
			
			// COLLISION POINT 1
			Vector3 collisionPoint1 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// COLLISION POINT 2
			Vector3 collisionPoint2 = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// HEALTH
			Int32 health = binaryReader.ReadInt32();
			
			// SPAWN ID
			string characterToSpawn = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// MAX SPAWNS
			int maxSpawns = (int)binaryReader.ReadByte();
			// padding
			fileStream.Seek(3, SeekOrigin.Current);
			
			// PHYSICS GROUP ID
			string spawnPoint = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// INSTANTIATE IN SCENE
			GameObject newGameObject;
			GameObject obj = (GameObject)Resources.Load(gameVersion + "/levels/" + levelName + "/" + areaName + "/" + identifier, typeof(GameObject));
			// for the alpha's Xs folders - the alpha doesn't even HAVE hives but whatever
			if (obj == null)
			{
				obj = (GameObject)Resources.Load(gameVersion + "/levels/" + levelName + "/" + areaName + "/Xs/" + identifier, typeof(GameObject));
			}
			// if THAT didn't work
			if (obj == null)
			{
				Debug.LogWarning("Could not load model for " + identifier + ", please make sure the .x is converted (if it has one)");
				newGameObject = Instantiate(Resources.Load("_Editor/Object Marker", typeof(GameObject)), position, Quaternion.Euler(0.0f, orientation, 0.0f), slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			else
			{
				newGameObject = Instantiate(obj, position, Quaternion.Euler(0.0f, orientation, 0.0f), slbParent.transform) as GameObject;
				newGameObject.name = identifier;
			}
			// HIVE COMPONENT FOR EXTRA DATA
			Hive hive = newGameObject.AddComponent<Hive>() as Hive;
			hive.health = health;
			hive.characterToSpawn = characterToSpawn;
			hive.maxSpawns = maxSpawns;
			hive.spawnPoint = spawnPoint;
			// COLLISION POINT GAMEOBJECTS
			// PARENT FOR DUMB ROTATION HACK
			GameObject collisionPointParent = new GameObject("Collision Points");
			collisionPointParent.AddComponent<CollisionPointHack>();
			collisionPointParent.transform.parent = newGameObject.transform;
			collisionPointParent.transform.localPosition = Vector3.zero;
			collisionPointParent.transform.localRotation = Quaternion.identity;
			
			// POINT 1
			GameObject blah = Instantiate(Resources.Load("_Editor/Collision Point", typeof(GameObject))) as GameObject;
			blah.name = "Collision Point 1";
			blah.transform.parent = collisionPointParent.transform;
			blah.transform.localPosition = collisionPoint1;
			blah.transform.localRotation = Quaternion.identity;
			
			// POINT 2
			GameObject blah2 = Instantiate(Resources.Load("_Editor/Collision Point", typeof(GameObject))) as GameObject;
			blah2.name = "Collision Point 2";
			blah2.transform.parent = collisionPointParent.transform;
			blah2.transform.localPosition = collisionPoint2;
			blah2.transform.localRotation = Quaternion.identity;
		}
		// SHRUUUUG
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	
	public void SaveHiveSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_HIVE.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_HIVE.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// make sure all entry gameobjects have Hive components (and grab them all)
		List<Hive> hives = new List<Hive>();
		foreach (GameObject entry in entries)
		{
			Hive hive = entry.GetComponent<Hive>();
			if (hive == null)
			{
				Debug.LogError("No Hive component on " + entry.name);
				return;
			}
			hives.Add(hive);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_HIVE.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count (unused)
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(12); // table offset
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// LOCATION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			
			// ORIENTATION
			binaryWriter.Write(Utilities.ClampRotation(Utilities.DumbCheck(-entries[i].transform.localEulerAngles.y)));
			
			// COLLISION POINT 1
			Transform transform1 = entries[i].transform.Find("Collision Points/Collision Point 1");
			if (transform1 == null)
			{
				Debug.LogWarning("Collision Point 1 not found on " + entries[i].name);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(Utilities.DumbCheck(-transform1.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(transform1.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(transform1.localPosition.z));
			}
			
			// COLLISION POINT 2
			Transform transform2 = entries[i].transform.Find("Collision Points/Collision Point 2");
			if (transform2 == null)
			{
				Debug.LogWarning("Collision Point 2 not found on " + entries[i].name);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(Utilities.DumbCheck(-transform2.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(transform2.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(transform2.localPosition.z));
			}
			
			// HEALTH
			binaryWriter.Write(hives[i].health);
			
			// SPAWN ID
			binaryWriter.Write(Utilities.StringToCharArray(hives[i].characterToSpawn));
			
			// MAX SPAWNS
			binaryWriter.Write((byte)hives[i].maxSpawns);
			// padding
			binaryWriter.Write(new byte[3]);
			
			// SPAWN POINT
			binaryWriter.Write(Utilities.StringToCharArray(hives[i].spawnPoint));
		}
		
		// pointer thingy (lol)
		binaryWriter.Write(8);
		
		// entry count (lol again)
		binaryWriter.Write(1);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_OBJ.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region SOUNDS
	
	public void LoadSoundsSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_SOUNDS.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(areaName + "_SOUNDS.slb");
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// READ MAIN ENTRY
			UInt32 filePathPointer = binaryReader.ReadUInt32();
			string identifier = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			Int32 variety = binaryReader.ReadInt32();
			UInt32 priority = binaryReader.ReadUInt32();
			float volume = binaryReader.ReadSingle();
			Int32 pitch = binaryReader.ReadInt32();
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Vector3 front = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			Int32 insideAngle = binaryReader.ReadInt32();
			Int32 outsideAngle = binaryReader.ReadInt32();
			float outsideVolume = binaryReader.ReadSingle();
			float minDistance = binaryReader.ReadSingle();
			float maxDistance = binaryReader.ReadSingle();
			
			// FILE NAME STRING
			long rememberMe = fileStream.Position;
			fileStream.Seek(filePathPointer, SeekOrigin.Begin);
			byte stringLength = binaryReader.ReadByte();
			string filePath = new string(binaryReader.ReadChars(stringLength));
			fileStream.Seek(rememberMe, SeekOrigin.Begin);
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(Resources.Load("_Editor/Sound Marker", typeof(GameObject)), position, Quaternion.identity, slbParent.transform) as GameObject;
			newGameObject.name = identifier;
			BionicleSound bionicleSound = newGameObject.AddComponent<BionicleSound>() as BionicleSound;
			bionicleSound.filePath = filePath;
			bionicleSound.variety = variety;
			bionicleSound.priority = (int)priority;
			bionicleSound.volume = volume;
			bionicleSound.pitch = pitch;
			bionicleSound.front = front;
			bionicleSound.insideAngle = insideAngle;
			bionicleSound.outsideAngle = outsideAngle;
			bionicleSound.outsideVolume = outsideVolume;
			bionicleSound.minDistance = minDistance;
			bionicleSound.maxDistance = maxDistance;
		}
		// SHRUUUUG
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveSoundsSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + areaName + "_SOUNDS.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + areaName + "_SOUNDS.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// component check
		List<BionicleSound> bionicleSounds = new List<BionicleSound>();
		foreach (GameObject entry in entries)
		{
			BionicleSound bionicleSound = entry.GetComponent<BionicleSound>();
			if (bionicleSound == null)
			{
				Debug.LogError("No BionicleSound component on " + entry.name);
				return;
			}
			bionicleSounds.Add(bionicleSound);
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", areaName + "_SOUNDS.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}
		
		// WRITE THE FILE
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		binaryWriter.Write(entries.Count); // entry count
		binaryWriter.Write(8); // table offset
		
		// ENTRIES
		for (int i = 0; i < entries.Count; i++)
		{
			// TEMP POINTER
			binaryWriter.Write(0);
			// IDENTIFIER
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			binaryWriter.Write(bionicleSounds[i].variety);
			binaryWriter.Write((UInt32)bionicleSounds[i].priority);
			binaryWriter.Write(bionicleSounds[i].volume);
			binaryWriter.Write(bionicleSounds[i].pitch);
			// POSITION
			binaryWriter.Write(Utilities.DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(Utilities.DumbCheck(entries[i].transform.localPosition.z));
			// FRONT
			binaryWriter.Write(Utilities.DumbCheck(-bionicleSounds[i].front.x));
			binaryWriter.Write(Utilities.DumbCheck(bionicleSounds[i].front.y));
			binaryWriter.Write(Utilities.DumbCheck(bionicleSounds[i].front.z));
			// BLAH
			binaryWriter.Write(bionicleSounds[i].insideAngle);
			binaryWriter.Write(bionicleSounds[i].outsideAngle);
			binaryWriter.Write(bionicleSounds[i].outsideVolume);
			binaryWriter.Write(bionicleSounds[i].minDistance);
			binaryWriter.Write(bionicleSounds[i].maxDistance);
		}
		// FILE PATHS
		List<long> offsets = new List<long>();
		for (int i = 0; i < bionicleSounds.Count; i++)
		{
			offsets.Add(fileStream.Position);
			char[] charArray = bionicleSounds[i].filePath.ToCharArray();
			binaryWriter.Write((byte)charArray.Length);
			binaryWriter.Write(charArray);
			binaryWriter.Write((byte)0); // whatever lol
		}
		// GO BACK AND WRITE OFFSETS
		fileStream.Seek(8, SeekOrigin.Begin);
		for (int i = 0; i < entries.Count; i++)
		{
			binaryWriter.Write((Int32)offsets[i]);
			fileStream.Seek(64, SeekOrigin.Current);
		}
		// pad to multiple of 4
		fileStream.Seek(0, SeekOrigin.End);
		binaryWriter.Write(new byte[fileStream.Length % 4]);
		
		// offset stuff
		binaryWriter.Write(4);
		for (int j = 0; j < entries.Count; j++)
		{
			int offset = 8; // initial data
			if (j > 0)
			{
				offset += j * 68; // skip forward as many entries as we need
			}
			binaryWriter.Write(offset);
		}
		
		// offset entry count
		binaryWriter.Write(1 + (entries.Count));
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + areaName + "/" + areaName + "_SOUNDS.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
}
