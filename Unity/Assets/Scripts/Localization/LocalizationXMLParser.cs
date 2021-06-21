using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LocalizationXMLParser {

	#region CONSTANTS
	
	//
	public const string SCRIPTS_SAVE_PATH = "/Scripts/Localization/";
	public const string XML_SAVE_PATH = "/XML/";
	public const string XML_SAVEFILE = "localization.xml";
	
	//
	const string TAG_LOCALIZATION = "Localization";
	const string TAG_KEY = "Key";
	const string TAG_LANGUAGE = "Language";
	const string ATTR_NAME = "Name";
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private static LocalizationXMLParser sInstance;
	
	Dictionary<string, Dictionary<string, string>> mLanguagesKeyValues;
	XmlParser mXmlParser;
	string mCurrentLanguage = "";
	string mCurrentKey = "";
	bool mIsWritingLanguage;
	
	#endregion
	
	#region ACCESSORS
	
	public static LocalizationXMLParser Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new LocalizationXMLParser();
			}
			
			return sInstance;
		}
	}
	
	#endregion
	
	#region FILE_CREATION_METHODS
	
	void CreateMissingFolders()
	{
		//
		string[] dirs = SCRIPTS_SAVE_PATH.Split('/');
		
		string path = Application.dataPath;
		
		foreach(string directory in dirs)
		{
			if (!string.IsNullOrEmpty(directory))
			{
				path += "/"+directory;
				
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
		}
	}
	
	public void WriteEnum(string aName,string[] aValues)
	{
		//
		CreateMissingFolders();
		
		//
		FileStream stream = new FileStream(Application.dataPath+SCRIPTS_SAVE_PATH+aName+".cs",
							FileMode.Create,FileAccess.ReadWrite,FileShare.None);
		
		//
		TextWriter writer = new StreamWriter(stream);
		
		//Header file
		writer.Write("/****************************************\n");
		writer.Write("* This file has been automatically created\n");
		writer.Write("* by a tool and should not be modified ***\n");
		writer.Write("****************************************/\n");
		
		//
		writer.Write("\n");
		
		//
		writer.Write("public enum "+aName+"\n");
		writer.Write("{\n");
		
		//
		for(int i = 0;i < aValues.Length;i++)
		{
			//
			writer.Write("\t"+aValues[i]);
			
			//
			if(i == 0)
			{
				//Start enum at 0
				writer.Write(" = 0");
			}
			
			//write comma but last element
			if (i < aValues.Length-1)
			{
				writer.Write(",");
			}
			
			//
			writer.Write("\n");
		}
		
		//
		writer.Write("}\n");
		
		//
		writer.Close();
		stream.Close();
	}
	
	#endregion
	
	#region XML_METHODS
	
	public Dictionary<string, Dictionary<string, string>> LoadData(bool aFromGame)
	{
		//
		mLanguagesKeyValues = new Dictionary<string, Dictionary<string, string>>();
			
		//
		if (aFromGame)
		{
			mXmlParser = new XmlParser(XMLAssetManager.Instance.mLocalizationXML);
		}
		else
		{
			mXmlParser = new XmlParser(Application.dataPath+XML_SAVE_PATH+XML_SAVEFILE);
		}
		
		//
		mXmlParser.Read(OnXmlTagOpen,OnXmlAttributes,OnXmlValue);
		
		return mLanguagesKeyValues;
	}
		
	public void SaveData(Dictionary<string, Dictionary<string, string>> data)
	{
		//Make a copy of the file instead progress is lost.
		File.Copy(Application.dataPath+XML_SAVE_PATH+XML_SAVEFILE,Application.dataPath+XML_SAVE_PATH+XML_SAVEFILE+".copy",true);
		
		mLanguagesKeyValues = data;
		
		//
		FileStream stream = new FileStream(Application.dataPath+XML_SAVE_PATH+XML_SAVEFILE,
							FileMode.Create,FileAccess.ReadWrite,FileShare.None);
		
		//Create XML Document
		XmlWriter writer = XmlWriter.Create(stream);
		
		//
		writer.WriteStartElement(TAG_LOCALIZATION);
		
		//Languages
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			//Language Tag
			writer.WriteStartElement(TAG_LANGUAGE);
			writer.WriteAttributeString(ATTR_NAME,languages.Key);
			
			//Keys
			foreach(KeyValuePair<string,string> key in languages.Value)
			{
				//Key Tag
				writer.WriteStartElement(TAG_KEY);
				writer.WriteAttributeString(ATTR_NAME,key.Key);
				
				//Key Value
				writer.WriteRaw(key.Value);
				
				//
				writer.WriteEndElement();
			}
			
			//
			writer.WriteEndElement();
		}
		
		//
		writer.WriteEndElement();
		
		//
		writer.Close();
		stream.Close();
	}
	
	void OnXmlTagOpen(string aTag)
	{
		mIsWritingLanguage = aTag == TAG_LANGUAGE;
	}
	
	void OnXmlValue(string aValue)
	{
		//
		mLanguagesKeyValues[mCurrentLanguage][mCurrentKey] = aValue;
	}
	
	void OnXmlAttributes(Dictionary<string,string> aAttributes)
	{
		//
		if (aAttributes.ContainsKey(ATTR_NAME))
		{
			if (mIsWritingLanguage)
			{
				mLanguagesKeyValues.Add(aAttributes[ATTR_NAME],new Dictionary<string,string>());
				mCurrentLanguage = aAttributes[ATTR_NAME];
			}
			else
			{
				mLanguagesKeyValues[mCurrentLanguage].Add(aAttributes[ATTR_NAME],"");
				mCurrentKey = aAttributes[ATTR_NAME];
			}
		}
	}
	
	#endregion
}