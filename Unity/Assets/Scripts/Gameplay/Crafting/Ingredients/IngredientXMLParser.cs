using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class IngredientXMLParser
{
	#region CONSTANTS
	//
	public const string SAVE_PATH = "/XML/";
	public const string XML_SAVEFILE = "Ingredients.xml";
	
	#endregion
	
	#region PRIVATE MEMBERS
	//
	private static IngredientXMLParser _Instance;
	
	#endregion
	
	#region ACCESSORS
	
	public static IngredientXMLParser Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new IngredientXMLParser();
			}
			
			return _Instance;
		}
	}
	
	#endregion
	
	
	#region FILE_CREATION_METHODS
	
	//
	void CreateMissingFolders()
	{
		//
		string[] dirs = SAVE_PATH.Split('/');
		
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
	
	//
	public void CreateXMLFile(List<Ingredient> aIngredients)
	{
		//
		CreateMissingFolders();
		
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.ConformanceLevel = ConformanceLevel.Fragment;
		settings.OmitXmlDeclaration = true;
		
		using(XmlWriter writer = XmlWriter.Create(Application.dataPath + SAVE_PATH + XML_SAVEFILE, settings))
		{
		    writer.WriteStartElement("Ingredients");
			
			foreach ( Ingredient ingredient in aIngredients )
			{
				//
				writer.WriteStartElement("Ingredient");
				writer.WriteAttributeString("Name", ingredient.Name.ToString());
				writer.WriteAttributeString("Rage", ingredient.Rage.ToString());
				writer.WriteAttributeString("Intellect", ingredient.Intellect.ToString());
				writer.WriteAttributeString("Stench", ingredient.Stench.ToString());
				writer.WriteAttributeString("Infectivity", ingredient.Infectivity.ToString());
				writer.WriteEndElement();
			}
			
		    writer.WriteEndElement();
		}
	}
	
	//
	public List<Ingredient> GetIngredients()
	{
		List<Ingredient> result = new List<Ingredient>();
		
		//
		if(File.Exists(Application.dataPath + SAVE_PATH + XML_SAVEFILE))
		{
			//
			XmlDocument xmlDoc = new XmlDocument();
			
			//
			using(FileStream fs = new FileStream(Application.dataPath + SAVE_PATH + XML_SAVEFILE, FileMode.Open, FileAccess.Read))
			{
				//
				xmlDoc.Load(fs);
				
				//
				XmlNodeList elemList = xmlDoc.ChildNodes[0].ChildNodes;
				
				//
		        for (int i = 0; i < elemList.Count; i++)
		        {
					//
					XmlNode currentElem = elemList[i];
					
					if(currentElem.Name == "Ingredient")
					{
						Ingredient ingredient = new Ingredient(currentElem.Attributes["Name"].Value,
													i,
													int.Parse(currentElem.Attributes["Intellect"].Value),
													int.Parse(currentElem.Attributes["Rage"].Value),
													int.Parse(currentElem.Attributes["Infectivity"].Value),
													int.Parse(currentElem.Attributes["Stench"].Value),10);
						//TODO DONT Hardocode production cost -CR
						
						// Create Ingredients from xml data
						result.Add(ingredient);
					}
		        }
			}
		}
		
		return result;
	}
	
	//
	public List<Ingredient> GetIngredients(TextAsset aTextAsset)
	{
		List<Ingredient> result = new List<Ingredient>();
		
		//
		if(aTextAsset != null)
		{
			//
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(aTextAsset.text);
				
			//
			XmlNodeList elemList = xmlDoc.ChildNodes[0].ChildNodes;
			
			//
	        for (int i = 0; i < elemList.Count; i++)
	        {
				//
				XmlNode currentElem = elemList[i];
				
				if(currentElem.Name == "Ingredient")
				{
					Ingredient ingredient = new Ingredient(currentElem.Attributes["Name"].Value,
													i,
													int.Parse(currentElem.Attributes["Intellect"].Value),
													int.Parse(currentElem.Attributes["Rage"].Value),
													int.Parse(currentElem.Attributes["Infectivity"].Value),
													int.Parse(currentElem.Attributes["Stench"].Value),10);

					//TODO Dont hardcode production cost -CR
					
					//Add ingredient
					result.Add(ingredient);
				}
	        }
		}

		return result;
	}

	#endregion
}
