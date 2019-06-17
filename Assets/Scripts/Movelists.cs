using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Xml;

// whaaaaaaaaaateverrrrrrrrrr

public class Movelists : MonoBehaviour
{
	public void SlbToXml()
	{
		string importPath = EditorUtility.OpenFilePanel("Load SLB", "", "slb");
		if (importPath.Length == 0)
		{
			return;
		}
		
		FileStream fileStream = new FileStream(importPath, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		
		XmlDocument xmlDocument = new XmlDocument();
		
		// INITIAL DATA
		char[] charArray = new char[4];
		charArray = binaryReader.ReadChars(4);
		Array.Reverse(charArray);
		string id = new string(charArray);
		
		XmlElement root = xmlDocument.CreateElement("movelist");
		root.SetAttribute("id", id);
		xmlDocument.AppendChild(root);
		
		binaryReader.ReadUInt32(); // unused entry count
		UInt32 entryCount1 = binaryReader.ReadUInt32();
		UInt32 tableOffset1 = binaryReader.ReadUInt32();
		
		binaryReader.ReadUInt32(); // unused entry count
		UInt32 entryCount2 = binaryReader.ReadUInt32();
		UInt32 tableOffset2 = binaryReader.ReadUInt32();
		
		// TABLE 1
		XmlElement table1 = xmlDocument.CreateElement("table1");
		xmlDocument.DocumentElement.AppendChild(table1);
		fileStream.Seek(tableOffset1, SeekOrigin.Begin);
		for (int i1 = 0; i1 < entryCount1; i1++)
		{
			XmlElement entry = xmlDocument.CreateElement("entry");
			table1.AppendChild(entry);
			
			char[] charArray1 = new char[4];
			charArray1 = binaryReader.ReadChars(4);
			Array.Reverse(charArray1);
			string id1 = new string(charArray1);
			entry.SetAttribute("id_1", id1);
			
			char[] charArray2 = new char[4];
			charArray2 = binaryReader.ReadChars(4);
			Array.Reverse(charArray2);
			string id2 = new string(charArray2);
			entry.SetAttribute("id_2", id2);
			
			UInt16 flags1 = binaryReader.ReadUInt16();
			entry.SetAttribute("flags_1", Convert.ToString(flags1, 2).PadLeft(16, '0'));
			
			Int16 index = binaryReader.ReadInt16();
			entry.SetAttribute("index", index.ToString());
			
			UInt32 long1 = binaryReader.ReadUInt32();
			entry.SetAttribute("long_1", long1.ToString());
			
			float float1 = binaryReader.ReadSingle();
			entry.SetAttribute("float_1", float1.ToString());
			
			float float2 = binaryReader.ReadSingle();
			entry.SetAttribute("float_2", float2.ToString());
			
			UInt32 flags2 = binaryReader.ReadUInt32();
			entry.SetAttribute("flags_2", Convert.ToString(flags2, 2).PadLeft(32, '0'));
			
			Int16 reservedCounter = binaryReader.ReadInt16();
			entry.SetAttribute("reserved_counter", reservedCounter.ToString());
			fileStream.Seek(2, SeekOrigin.Current); // skip padding
			
			// split trigger stuff, whatever that is
			binaryReader.ReadUInt32(); // unused entry count
			UInt32 splitTriggerEntryCount = binaryReader.ReadUInt32();
			UInt32 splitTriggerOffset = binaryReader.ReadUInt32();
			long rememberMe = fileStream.Position;
			fileStream.Seek(splitTriggerOffset, SeekOrigin.Begin);
			for (int j = 0; j < splitTriggerEntryCount; j++)
			{
				XmlElement splitTrigger = xmlDocument.CreateElement("split_trigger");
				entry.AppendChild(splitTrigger);
				
				UInt32 input = binaryReader.ReadUInt32();
				splitTrigger.SetAttribute("input", Convert.ToString(input, 2).PadLeft(32, '0'));
				
				// this is getting ridiculous
				char[] charArray3 = new char[4];
				charArray3 = binaryReader.ReadChars(4);
				Array.Reverse(charArray3);
				string id3 = new string(charArray3);
				splitTrigger.SetAttribute("id", id3);
				
				float sFloat1 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_1", sFloat1.ToString());
				
				float sFloat2 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_2", sFloat2.ToString());
				
				float sFloat3 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_3", sFloat3.ToString());
				
				byte sFlags = binaryReader.ReadByte();
				splitTrigger.SetAttribute("flags", Convert.ToString(sFlags, 2).PadLeft(8, '0'));
				
				fileStream.Seek(3, SeekOrigin.Current); // skip padding
			}
			fileStream.Seek(rememberMe, SeekOrigin.Begin);
		}
		
		// TABLE 2
		// I'M GOING TO BED, COPY PASTE TIME, DON'T CARE
		XmlElement table2 = xmlDocument.CreateElement("table2");
		xmlDocument.DocumentElement.AppendChild(table2);
		fileStream.Seek(tableOffset2, SeekOrigin.Begin);
		for (int i2 = 0; i2 < entryCount2; i2++)
		{
			XmlElement entry = xmlDocument.CreateElement("entry");
			table2.AppendChild(entry);
			
			char[] charArray1 = new char[4];
			charArray1 = binaryReader.ReadChars(4);
			Array.Reverse(charArray1);
			string id1 = new string(charArray1);
			entry.SetAttribute("id_1", id1);
			
			char[] charArray2 = new char[4];
			charArray2 = binaryReader.ReadChars(4);
			Array.Reverse(charArray2);
			string id2 = new string(charArray2);
			entry.SetAttribute("id_2", id2);
			
			UInt16 flags1 = binaryReader.ReadUInt16();
			entry.SetAttribute("flags_1", Convert.ToString(flags1, 2).PadLeft(16, '0'));
			
			Int16 index = binaryReader.ReadInt16();
			entry.SetAttribute("index", index.ToString());
			
			UInt32 long1 = binaryReader.ReadUInt32();
			entry.SetAttribute("long_1", long1.ToString());
			
			float float1 = binaryReader.ReadSingle();
			entry.SetAttribute("float_1", float1.ToString());
			
			float float2 = binaryReader.ReadSingle();
			entry.SetAttribute("float_2", float2.ToString());
			
			UInt32 flags2 = binaryReader.ReadUInt32();
			entry.SetAttribute("flags_2", Convert.ToString(flags2, 2).PadLeft(32, '0'));
			
			Int16 reservedCounter = binaryReader.ReadInt16();
			entry.SetAttribute("reserved_counter", reservedCounter.ToString());
			fileStream.Seek(2, SeekOrigin.Current); // skip padding
			
			// split trigger stuff, whatever that is
			binaryReader.ReadUInt32(); // unused entry count
			UInt32 splitTriggerEntryCount = binaryReader.ReadUInt32();
			UInt32 splitTriggerOffset = binaryReader.ReadUInt32();
			
			UInt32 extra = binaryReader.ReadUInt32();
			entry.SetAttribute("extra", extra.ToString());
			
			long rememberMe = fileStream.Position;
			fileStream.Seek(splitTriggerOffset, SeekOrigin.Begin);
			for (int j = 0; j < splitTriggerEntryCount; j++)
			{
				XmlElement splitTrigger = xmlDocument.CreateElement("split_trigger");
				entry.AppendChild(splitTrigger);
				
				UInt32 input = binaryReader.ReadUInt32();
				splitTrigger.SetAttribute("input", Convert.ToString(input, 2).PadLeft(32, '0'));
				
				// this is getting ridiculous
				char[] charArray3 = new char[4];
				charArray3 = binaryReader.ReadChars(4);
				Array.Reverse(charArray3);
				string id3 = new string(charArray3);
				splitTrigger.SetAttribute("id", id3);
				
				float sFloat1 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_1", sFloat1.ToString());
				
				float sFloat2 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_2", sFloat2.ToString());
				
				float sFloat3 = binaryReader.ReadSingle();
				splitTrigger.SetAttribute("float_3", sFloat3.ToString());
				
				byte sFlags = binaryReader.ReadByte();
				splitTrigger.SetAttribute("flags", Convert.ToString(sFlags, 2).PadLeft(8, '0'));
				
				fileStream.Seek(3, SeekOrigin.Current); // skip padding
			}
			fileStream.Seek(rememberMe, SeekOrigin.Begin);
		}
		
		string pathWithoutFile = Path.GetDirectoryName(importPath);
		string exportPath = EditorUtility.SaveFilePanel("Save XML", pathWithoutFile, "Movelist.xml", "xml");
		if (exportPath.Length == 0)
		{
			return;
		}
		
		// thanks random guy who answered that stackoverflow post, we couldn't have done it without you
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "	",
			NewLineOnAttributes = true
			
		};
		using (XmlWriter writer = XmlWriter.Create(exportPath, settings))
		{
			xmlDocument.Save(writer);
		}
		Debug.Log("Saved " + exportPath);
	}
	
	public void XmlToSlb()
	{
		string importPath = EditorUtility.OpenFilePanel("Load XML", "", "xml");
		if (importPath.Length == 0)
		{
			return;
		}
		
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(File.ReadAllText(importPath));
		
		string pathWithoutFile = Path.GetDirectoryName(importPath);
		string exportPath = EditorUtility.SaveFilePanel("Save SLB", pathWithoutFile, "Movelist_OUTPUT.slb", "slb");
		if (exportPath.Length == 0)
		{
			return;
		}
		
		FileStream fileStream = new FileStream(exportPath, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		
		// ID
		XmlElement root = (XmlElement)xmlDocument.DocumentElement;
		char[] charArray = root.Attributes["id"].Value.ToCharArray(0, 4);
		Array.Reverse(charArray);
		binaryWriter.Write(charArray);
		
		// TABLE 1
		XmlElement table1 = (XmlElement)root.SelectSingleNode(".//table1");
		binaryWriter.Write(table1.ChildNodes.Count); // entry count (unused)
		binaryWriter.Write(table1.ChildNodes.Count); // entry count
		binaryWriter.Write(28); // table offset
		
		// TABLE 2
		XmlElement table2 = (XmlElement)root.SelectSingleNode(".//table2");
		binaryWriter.Write(table2.ChildNodes.Count); // entry count (unused)
		binaryWriter.Write(table2.ChildNodes.Count); // entry count
		binaryWriter.Write(28 + (table1.ChildNodes.Count * 44)); // table offset
		
		int table1SplitTriggerCount = 0;
		int table2SplitTriggerCount = 0;
		
		// TABLE 1 CONTENTS
		foreach (XmlNode movelistEntry in table1.ChildNodes)
		{
			char[] charArray1 = movelistEntry.Attributes["id_1"].Value.ToCharArray(0, 4);
			Array.Reverse(charArray1);
			binaryWriter.Write(charArray1);
			
			char[] charArray2 = movelistEntry.Attributes["id_2"].Value.ToCharArray(0, 4);
			Array.Reverse(charArray2);
			binaryWriter.Write(charArray2);
			
			binaryWriter.Write(Convert.ToUInt16(movelistEntry.Attributes["flags_1"].Value, 2));
			
			binaryWriter.Write(Convert.ToInt16(movelistEntry.Attributes["index"].Value));
			
			binaryWriter.Write(Convert.ToUInt32(movelistEntry.Attributes["long_1"].Value));
			
			binaryWriter.Write(Convert.ToSingle(movelistEntry.Attributes["float_1"].Value));
			
			binaryWriter.Write(Convert.ToSingle(movelistEntry.Attributes["float_2"].Value));
			
			binaryWriter.Write(Convert.ToUInt32(movelistEntry.Attributes["flags_2"].Value, 2));
			
			binaryWriter.Write(Convert.ToInt16(movelistEntry.Attributes["reserved_counter"].Value));
			
			binaryWriter.Write(new byte[2]); // padding
			
			binaryWriter.Write(movelistEntry.ChildNodes.Count); // entry count (unused)
			binaryWriter.Write(movelistEntry.ChildNodes.Count); // entry count
			
			binaryWriter.Write(28 + (table1.ChildNodes.Count * 44) + (table2.ChildNodes.Count * 48) + (table1SplitTriggerCount * 24)); // offset
			if (movelistEntry.HasChildNodes)
			{
				foreach (XmlNode splitTriggerNode in movelistEntry.ChildNodes)
				{
					table1SplitTriggerCount++;
				}
			}
		}
		
		// TABLE 2 CONTENTS
		foreach (XmlNode movelistEntry in table2.ChildNodes)
		{
			char[] charArray1 = movelistEntry.Attributes["id_1"].Value.ToCharArray(0, 4);
			Array.Reverse(charArray1);
			binaryWriter.Write(charArray1);
			
			char[] charArray2 = movelistEntry.Attributes["id_2"].Value.ToCharArray(0, 4);
			Array.Reverse(charArray2);
			binaryWriter.Write(charArray2);
			
			binaryWriter.Write(Convert.ToUInt16(movelistEntry.Attributes["flags_1"].Value, 2));
			
			binaryWriter.Write(Convert.ToInt16(movelistEntry.Attributes["index"].Value));
			
			binaryWriter.Write(Convert.ToUInt32(movelistEntry.Attributes["long_1"].Value));
			
			binaryWriter.Write(Convert.ToSingle(movelistEntry.Attributes["float_1"].Value));
			
			binaryWriter.Write(Convert.ToSingle(movelistEntry.Attributes["float_2"].Value));
			
			binaryWriter.Write(Convert.ToUInt32(movelistEntry.Attributes["flags_2"].Value, 2));
			
			binaryWriter.Write(Convert.ToInt16(movelistEntry.Attributes["reserved_counter"].Value));
			
			binaryWriter.Write(new byte[2]); // padding
			
			binaryWriter.Write(movelistEntry.ChildNodes.Count); // entry count (unused)
			binaryWriter.Write(movelistEntry.ChildNodes.Count); // entry count
			
			binaryWriter.Write(28 + (table1.ChildNodes.Count * 44) + (table2.ChildNodes.Count * 48) + (table1SplitTriggerCount * 24) + (table2SplitTriggerCount * 24)); // offset
			if (movelistEntry.HasChildNodes)
			{
				foreach (XmlNode splitTriggerNode in movelistEntry.ChildNodes)
				{
					table2SplitTriggerCount++;
				}
			}
			
			binaryWriter.Write(Convert.ToUInt32(movelistEntry.Attributes["extra"].Value));
		}
		
		// TABLE 1 SPLIT TRIGGERS
		foreach (XmlNode movelistEntry in table1.ChildNodes)
		{
			if (movelistEntry.HasChildNodes)
			{
				foreach (XmlNode splitTriggerNode in movelistEntry.ChildNodes)
				{
					binaryWriter.Write(Convert.ToUInt32(splitTriggerNode.Attributes["input"].Value, 2));
					
					char[] charArrayRegret = splitTriggerNode.Attributes["id"].Value.ToCharArray(0, 4);
					Array.Reverse(charArrayRegret);
					binaryWriter.Write(charArrayRegret);
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_1"].Value));
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_2"].Value));
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_3"].Value));
					
					binaryWriter.Write(Convert.ToByte(splitTriggerNode.Attributes["flags"].Value, 2));
					
					binaryWriter.Write(new byte[3]); // padding
				}
			}
		}
		
		// TABLE 2 SPLIT TRIGGERS
		// GOD I LOVE COPY PASTE i"M GONNA COPY PASTE THE BED ON TOP of ME
		foreach (XmlNode movelistEntry in table2.ChildNodes)
		{
			if (movelistEntry.HasChildNodes)
			{
				foreach (XmlNode splitTriggerNode in movelistEntry.ChildNodes)
				{
					binaryWriter.Write(Convert.ToUInt32(splitTriggerNode.Attributes["input"].Value, 2));
					
					char[] charArrayRegret = splitTriggerNode.Attributes["id"].Value.ToCharArray(0, 4);
					Array.Reverse(charArrayRegret);
					binaryWriter.Write(charArrayRegret);
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_1"].Value));
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_2"].Value));
					
					binaryWriter.Write(Convert.ToSingle(splitTriggerNode.Attributes["float_3"].Value));
					
					binaryWriter.Write(Convert.ToByte(splitTriggerNode.Attributes["flags"].Value, 2));
					
					binaryWriter.Write(new byte[3]); // padding
				}
			}
		}
		
		// OFFSETS
		binaryWriter.Write(12);
		binaryWriter.Write(24);
		
		for (int i = 0; i < table1.ChildNodes.Count; i++)
		{
			int offset = 28; // initial data
			if (i > 0)
			{
				offset += i * 44; // skip forward as many entries as we need
			}
			offset += 40; // entry data up to first offset
			binaryWriter.Write(offset);
		}
		
		for (int j = 0; j < table2.ChildNodes.Count; j++)
		{
			int offset = 28 + (table1.ChildNodes.Count * 44); // initial data
			if (j > 0)
			{
				offset += j * 48; // skip forward as many entries as we need
			}
			offset += 40; // entry data up to first offset
			binaryWriter.Write(offset);
			offset += 4; // extra
		}
		
		// OFFSET COUNT
		binaryWriter.Write(2 + table1.ChildNodes.Count + table2.ChildNodes.Count);
		
		// FOOTER
		binaryWriter.Write(0xC0FFEE);
		
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + exportPath);
	}
}
