using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

// WIP WIP WIP WIP WIP WIP

public class Cinematics : MonoBehaviour
{
	public string gameVersion = "beta";
	public string levelName = "lev1";
	public string cinematicName = "cin1";
	public bool overwriteSlbInResources = true;
	
	///////////////////////////////////////////////////////////////////
	#region CIN_CHAR
	
	public void LoadCinCharSlb()
	{
		// PREPARE FOR FILE READING
		string path = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + levelName + "/" + cinematicName + "_CHAR.slb";
		if (!File.Exists(path))
		{
			Debug.LogError("File not found: " + path);
			return;
		}
		FileStream fileStream = new FileStream(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		// MAKE SLB PARENT GAMEOBJECT
		// doing this after opening the file so we don't just get an empty gameobject if we entered an invalid name or something
		GameObject slbParent = new GameObject(cinematicName + "_CHAR.slb");
		
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
			
			// PUT CHARACTER/MARKER IN SCENE
			GameObject newGameObject;
			GameObject character = (GameObject)Resources.Load(gameVersion + "/characters/" + identifier + "/" + identifier, typeof(GameObject));
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
	
	public void SaveCinCharSlb()
	{
		/*
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
			binaryWriter.Write(DumbCheck(-entries[i].transform.localPosition.x));
			binaryWriter.Write(DumbCheck(entries[i].transform.localPosition.y));
			binaryWriter.Write(DumbCheck(entries[i].transform.localPosition.z));
			
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
		*/
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
}
