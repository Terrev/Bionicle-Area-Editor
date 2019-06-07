using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class Manager : MonoBehaviour
{
	public string gameVersion = "beta";
	public string levelName = "lev1";
	public string areaName = "atrm";
	
	public bool overwriteSlbInResources = false;
	
	public GameObject positionMarker;
	public GameObject characterMarker;
	
	
	
	///////////////////////////////////////////////////////////////////
	#region OBJ
	
	public void LoadObjSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_OBJ.slb";
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
			// read the characters then turn them into a string we can use more easily
			char[] charArray = new char[4];
			charArray = binaryReader.ReadChars(4);
			Array.Reverse(charArray);
			string identifier = new string(charArray);
			
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
			GameObject newGameObject = Instantiate(Resources.Load(gameVersion + "/" + levelName + "/" + areaName + "/" + identifier, typeof(GameObject)), location, Quaternion.Euler(orientation), slbParent.transform) as GameObject;
			newGameObject.name = identifier;
			// BIONICLEOBJECT COMPONENT FOR EXTRA DATA
			BionicleObject bionicleObject = newGameObject.AddComponent<BionicleObject>() as BionicleObject;
			bionicleObject.unknown = unknown;
			bionicleObject.flags = (int)flags;
			// COLLISION POINT GAMEOBJECTS
			if (collisionPoint1 != Vector3.zero)
			{
				GameObject blah = new GameObject("Collision Point 1");
				blah.transform.parent = newGameObject.transform;
				blah.transform.localPosition = collisionPoint1;
				blah.transform.localRotation = Quaternion.identity;
			}
			if (collisionPoint2 != Vector3.zero)
			{
				GameObject blah = new GameObject("Collision Point 2");
				blah.transform.parent = newGameObject.transform;
				blah.transform.localPosition = collisionPoint2;
				blah.transform.localRotation = Quaternion.identity;
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
			char[] charArray = entries[i].name.ToCharArray(0, 4);
			Array.Reverse(charArray);
			binaryWriter.Write(charArray);
			
			// LOCATION
			binaryWriter.Write(DumbCheck(-entries[i].transform.position.x));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.y));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.z));
			
			// ORIENTATION
			binaryWriter.Write(ClampRotation(DumbCheck(entries[i].transform.eulerAngles.x)));
			binaryWriter.Write(ClampRotation(DumbCheck(-entries[i].transform.eulerAngles.y)));
			binaryWriter.Write(ClampRotation(DumbCheck(-entries[i].transform.eulerAngles.z)));
			
			// UNKNOWN
			binaryWriter.Write(bionicleObjects[i].unknown);
			
			// COLLISION POINT 1
			Transform transform1 = entries[i].transform.Find("Collision Point 1");
			if (transform1 == null)
			{
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(DumbCheck(-transform1.localPosition.x));
				binaryWriter.Write(DumbCheck(transform1.localPosition.y));
				binaryWriter.Write(DumbCheck(transform1.localPosition.z));
			}
			
			// COLLISION POINT 2
			Transform transform2 = entries[i].transform.Find("Collision Point 2");
			if (transform2 == null)
			{
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
				binaryWriter.Write(0.0f);
			}
			else
			{
				binaryWriter.Write(DumbCheck(-transform2.localPosition.x));
				binaryWriter.Write(DumbCheck(transform2.localPosition.y));
				binaryWriter.Write(DumbCheck(transform2.localPosition.z));
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
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_OBJ.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	// get rid of -0s unity may introduce
	float DumbCheck(float input)
	{
		if (input == -0.0f)
		{
			return 0.0f;
		}
		else
		{
			return input;
		}
	}
	
	// clamp rotation values to avoid weird numbers - highest object rotation value in the vanilla game is 170, lowest is -90, unity sometimes gives values like -302.5 instead of the original 57.5
	float ClampRotation(float input)
	{
		if (input > 360.0f || input < -360.0f)
		{
			Debug.LogWarning("lol wtf is this rotation, lemme know if you see this");
			return input;
		}
		if (input > 180.0f)
		{
			return input - 360.0f;
		}
		if (input < -180.0f)
		{
			return input + 360.0f;
		}
		return input;
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
	
	
	///////////////////////////////////////////////////////////////////
	#region POS
	
	public void LoadPosSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_POS.slb";
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
			// read the characters then turn them into a string we can use more easily
			char[] charArray = new char[4];
			charArray = binaryReader.ReadChars(4);
			Array.Reverse(charArray);
			string identifier = new string(charArray);
			
			// POSITION
			Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			
			// FLAGS
			UInt32 flags = binaryReader.ReadUInt32();
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(positionMarker, position, Quaternion.identity, slbParent.transform);
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
			char[] charArray = entries[i].name.ToCharArray(0, 4);
			Array.Reverse(charArray);
			binaryWriter.Write(charArray);
			
			// POSITION
			binaryWriter.Write(DumbCheck(-entries[i].transform.position.x));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.y));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.z));
			
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
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_POS.slb";
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
		string path = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_CHAR.slb";
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
			// read the characters then turn them into a string we can use more easily
			char[] charArray = new char[4];
			charArray = binaryReader.ReadChars(4);
			Array.Reverse(charArray);
			string identifier = new string(charArray);
			
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
					char[] anotherCharArray = new char[4];
					anotherCharArray = binaryReader.ReadChars(4);
					Array.Reverse(anotherCharArray);
					string triggerBox = new string(anotherCharArray);
					triggerBoxes.Add(triggerBox);
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
			
			
			// PUT MARKER IN SCENE
			GameObject newGameObject = Instantiate(characterMarker, position, Quaternion.identity, slbParent.transform);
			newGameObject.name = identifier;
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
			char[] charArray = entries[i].name.ToCharArray(0, 4);
			Array.Reverse(charArray);
			binaryWriter.Write(charArray);
			
			// POSITION
			binaryWriter.Write(DumbCheck(-entries[i].transform.position.x));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.y));
			binaryWriter.Write(DumbCheck(entries[i].transform.position.z));
			
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
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/" + levelName + "/" + areaName + "/" + areaName + "_CHAR.slb";
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	
	#endregion
	///////////////////////////////////////////////////////////////////
}
