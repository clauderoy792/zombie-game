using System;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class XmlParser
{
	#region MEMBERS
	
	//
	XmlReader mReader;
	XmlWriter mWriter;
	string mFileName;
	TextAsset mTextAsset = null;
	
	//
	Action<string> mOnXmlTagOpen;
	Action<string> mOnXmlTagClose;
	Action<Dictionary<string,string>> mOnXmlAttributes;
	Action<string> mOnXmlValue;
	
	#endregion
	
	#region CONSTRUCTORS
	
	/// <summary>
	/// Initializes a new instance of the <see cref="XmlParser"/> class.
	/// </summary>
	/// <param name='aFileName'>
	/// A file name.
	/// </param>
	/// <param name='aType'>
	/// A type.
	/// </param>
	public XmlParser(string aFileName)
	{
		//
		mFileName = aFileName;
	}
	
	public XmlParser(TextAsset aAsset)
	{
		mTextAsset = aAsset;
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	/// <summary>
	/// Processes the read.
	/// </summary>
	public void Read(Action<string> aOnXmlTagOpen,Action<Dictionary<string,string>> aOnXmlAttributes,Action<string> aOnXmlValue,Action<string> aOnXmlTagClose = null)
	{
		//
		mOnXmlTagClose = aOnXmlTagClose;
		mOnXmlTagOpen = aOnXmlTagOpen;
		mOnXmlAttributes = aOnXmlAttributes;
		mOnXmlValue = aOnXmlValue;
		
		FileStream stream = null;
		
		try
		{
			//Read from text asset
			if (mTextAsset != null)
			{
				//
				StringReader stringReader = new StringReader(mTextAsset.text);
				
				mReader = XmlReader.Create(stringReader);
			}
			//Read directly from file
			else
			{
				//
				stream = new FileStream(mFileName,
									FileMode.Open,FileAccess.ReadWrite,FileShare.None);
				
				mReader = XmlReader.Create(stream);
			}
		
			while (mReader.Read())
			{
				//
				if (mReader.IsStartElement())
				{
					if (mOnXmlTagOpen != null)
					{
						mOnXmlTagOpen(mReader.Name);
					}
				}
				
				//
				if (mReader.NodeType == XmlNodeType.EndElement)
				{
					if (mOnXmlTagClose != null)
					{
						mOnXmlTagClose(mReader.Name);
					}
				}
				
				//
				if (mReader.HasAttributes)
				{
					if (mOnXmlAttributes != null)
					{
						Dictionary<string,string> attributes = new Dictionary<string, string>();
						
						//
						mReader.MoveToFirstAttribute();
						
						//
						for(int i = 0;i < mReader.AttributeCount;i++)
						{
							//
							attributes.Add(mReader.Name,mReader.Value);
							
							mReader.MoveToNextAttribute();
						}
						
						mOnXmlAttributes(attributes);
					}
				}
				
				//
				if (mReader.HasValue && mReader.NodeType == XmlNodeType.Text)
				{
					if (mOnXmlValue != null)
					{
						mOnXmlValue(mReader.Value.Trim());
					}
				}
			}
		}
		catch(Exception e)
		{
			Debug.Log("Could not load "+ mFileName + " : "+e.Message);
		}
		
		if (mReader != null)
		{
			mReader.Close();
		}
		
		if (stream != null)
		{
			stream.Close();
		}
	}
	
	#endregion
	
}
