using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

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
		if (gameVersion.Equals("alpha", StringComparison.OrdinalIgnoreCase))
		{
			path = Application.dataPath + "/Resources/" + gameVersion + "/cinematics/" + levelName + "/" + cinematicName + "_CHAR.slb";
		}
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
		
		// READ BASIC INFO
		UInt32 entryCount = binaryReader.ReadUInt32();
		UInt32 tableOffset = binaryReader.ReadUInt32();
		
		// GO TO BEGINNING OF TABLE
		fileStream.Seek(tableOffset, SeekOrigin.Begin);
		
		// LOOP THROUGH ENTRIES
		for (int i = 0; i < entryCount; i++)
		{
			// ANIM HIERARCHY
			string animHierarchy = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// CHARACTER
			string characterName = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// ANIM BAKED
			string animBaked = Utilities.CharArrayToString(binaryReader.ReadChars(4));
			
			// MASK SWITCH TIMES
			float maskSwitchTime1 = binaryReader.ReadSingle();
			float maskSwitchTime2 = binaryReader.ReadSingle();
			
			// PUT CHARACTER/MARKER IN SCENE
			GameObject newGameObject = new GameObject(animHierarchy);
			newGameObject.transform.parent = slbParent.transform;
			// CINCHARACTER COMPONENT FOR EXTRA DATA
			CinCharacter cinCharacter = newGameObject.AddComponent<CinCharacter>() as CinCharacter;
			cinCharacter.characterName = characterName;
			cinCharacter.animBaked = animBaked;
			cinCharacter.maskSwitchTime1 = maskSwitchTime1;
			cinCharacter.maskSwitchTime2 = maskSwitchTime2;
			
			// LOCATION TABLE TIME
			UInt32 locationTableEntryCount = binaryReader.ReadUInt32();
			UInt32 locationTableOffset = binaryReader.ReadUInt32();
			long rememberMe = fileStream.Position;
			fileStream.Seek(locationTableOffset, SeekOrigin.Begin);
			for (int j = 0; j < locationTableEntryCount; j++)
			{
				float time = binaryReader.ReadSingle();
				Vector3 location = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				float orientationX = binaryReader.ReadSingle();
				float orientationY = -binaryReader.ReadSingle();
				float orientationZ = -binaryReader.ReadSingle();
				
				GameObject locationGameObject;
				GameObject character = (GameObject)Resources.Load(gameVersion + "/characters/" + characterName + "/" + characterName, typeof(GameObject));
				if (character == null)
				{
					character = (GameObject)Resources.Load(gameVersion + "/characters/" + characterName + "/Xs/" + characterName, typeof(GameObject));
				}
				if (character == null)
				{
					Debug.LogWarning("Could not load character model for " + characterName);
					locationGameObject = Instantiate(Resources.Load("_Editor/Character Marker", typeof(GameObject)), location, Quaternion.Euler(new Vector3(0.0f, orientationY, 0.0f)), newGameObject.transform) as GameObject;
					locationGameObject.name = "Location";
				}
				else
				{
					locationGameObject = Instantiate(character, location, Quaternion.Euler(new Vector3(0.0f, orientationY, 0.0f)), newGameObject.transform) as GameObject;
					locationGameObject.name = "Location";
				}
				CinCharacterLocation cinCharacterLocation = locationGameObject.AddComponent<CinCharacterLocation>() as CinCharacterLocation;
				cinCharacterLocation.time = time;
				cinCharacterLocation.unusedOrientationX = orientationX;
				cinCharacterLocation.unusedOrientationZ = orientationZ;
			}
			fileStream.Seek(rememberMe, SeekOrigin.Begin);
		}
		// SHRUG
		// some stackoverflow post said closing the reader SHOULD close the stream but I don't trust "should" enough lol
		binaryReader.Close();
		fileStream.Close();
		Debug.Log("Loaded " + path);
	}
	
	public void SaveCinCharSlb()
	{
		// LOOK OVER SCENE AND ERROR CHECK
		// get slb parent
		GameObject parentGameObject = GameObject.Find("/" + cinematicName + "_CHAR.slb");
		if (parentGameObject == null)
		{
			Debug.LogError("Couldn't find GameObject named " + cinematicName + "_CHAR.slb");
			return;
		}
		// grab the slb's gameobjects/entries
		List<GameObject> entries = new List<GameObject>();
		foreach (Transform entry in parentGameObject.transform)
		{
			entries.Add(entry.gameObject);
		}
		// make sure all entry gameobjects have CinCharacter components (and grab them all)
		List<CinCharacter> cinCharacters = new List<CinCharacter>();
		foreach (GameObject entry in entries)
		{
			CinCharacter cinCharacter = entry.GetComponent<CinCharacter>();
			if (cinCharacter == null)
			{
				Debug.LogError("No CinCharacter component on " + entry.name);
				return;
			}
			cinCharacters.Add(cinCharacter);
			
			// error checking for CinCharacterLocation components too
			foreach (Transform locationEntry in entry.transform)
			{
				CinCharacterLocation cinCharacterLocation = locationEntry.GetComponent<CinCharacterLocation>();
				if (cinCharacterLocation == null)
				{
					Debug.LogError("No CinCharacterLocation component on " + entry.name + "/" + locationEntry.name);
					return;
				}
			}
		}
		
		// OK NOW FOR ACTUAL FILE STUFF
		// GET FILE PATH
		string path = EditorUtility.SaveFilePanel("Save SLB", "", cinematicName + "_CHAR.slb", "slb");
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
		int locationEntriesCount = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			// ANIM HIERARCHY
			binaryWriter.Write(Utilities.StringToCharArray(entries[i].name));
			
			// CHARACTER
			binaryWriter.Write(Utilities.StringToCharArray(cinCharacters[i].characterName));
			
			// ANIM BAKED
			binaryWriter.Write(Utilities.StringToCharArray(cinCharacters[i].animBaked));
			
			// MASK SWITCH TIMES
			binaryWriter.Write(cinCharacters[i].maskSwitchTime1);
			binaryWriter.Write(cinCharacters[i].maskSwitchTime2);
			
			// LOCATION TABLE STUFF
			// entry count
			binaryWriter.Write(entries[i].transform.childCount);
			// offset
			binaryWriter.Write(8 + (entries.Count * 28) + (locationEntriesCount * 28)); // offset
			locationEntriesCount += entries[i].transform.childCount;
		}
		
		// ACTUAL LOCATION STUFF
		foreach (GameObject entry in entries)
		{
			foreach (Transform locationEntry in entry.transform)
			{
				CinCharacterLocation cinCharacterLocation = locationEntry.GetComponent<CinCharacterLocation>();
				
				// TIME
				binaryWriter.Write(cinCharacterLocation.time);
				
				// POSITION
				binaryWriter.Write(Utilities.DumbCheck(-locationEntry.localPosition.x));
				binaryWriter.Write(Utilities.DumbCheck(locationEntry.localPosition.y));
				binaryWriter.Write(Utilities.DumbCheck(locationEntry.localPosition.z));
				
				// ORIENTATION
				binaryWriter.Write(cinCharacterLocation.unusedOrientationX);
				binaryWriter.Write(Utilities.ClampRotation(Utilities.DumbCheck(-locationEntry.localEulerAngles.y)));
				binaryWriter.Write(-cinCharacterLocation.unusedOrientationZ);
			}
		}
		
		// pointers to offsets or whatever
		
		// character table offset
		binaryWriter.Write(4);
		
		// offsets
		for (int j = 0; j < entries.Count; j++)
		{
			int offset = 8; // initial data
			if (j > 0)
			{
				offset += j * 28; // skip forward as many entries as we need
			}
			offset += 24; // entry data up to offset
			binaryWriter.Write(offset);
		}
		
		// offset entry count
		binaryWriter.Write(1 + entries.Count);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		// CONTINUED SHRUG
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
		
		if (overwriteSlbInResources)
		{
			string path2 = Application.dataPath + "/Resources/" + gameVersion + "/levels/" + levelName + "/" + levelName + "/" + cinematicName + "_CHAR.slb";
			if (gameVersion.Equals("alpha", StringComparison.OrdinalIgnoreCase))
			{
				path2 = Application.dataPath + "/Resources/" + gameVersion + "/cinematics/" + levelName + "/" + cinematicName + "_CHAR.slb";
			}
			File.Copy(path, path2, true);
			Debug.Log("Copied to " + path2);
		}
	}
	
	#endregion
	///////////////////////////////////////////////////////////////////
	
}
