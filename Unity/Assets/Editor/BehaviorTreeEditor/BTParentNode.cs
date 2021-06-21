using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class BTParentNode : BTNode
{
	protected List<BTNode> mChildNodes = new List<BTNode>();
	protected List<int> mChildIDs = new List<int>();
	protected List<Rect> mConnectors = new List<Rect>();
		
	
	public List<BTNode> ChildNodes
	{
		get{return mChildNodes;}
		set{mChildNodes = value;}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="BTParentNode"/> class.
	/// </summary>
	/// <param name='aName'>
	/// A name.
	/// </param>
	public BTParentNode(string aName) : base(aName)
	{
		SetNumberOfConnector(3);
	}
	
	public List<Rect> Connector
	{
		get{return mConnectors;}
		set{mConnectors = value;}
	}
	
	public int ConnectorCount
	{
		get{return mConnectors.Count;}
		set
		{
			SetNumberOfConnector(value);
		}
	}
	
	public List<int> ChildIDs
	{
		get{return mChildIDs;}
		set{mChildIDs = value;}
	}
	
	public void SetNumberOfConnector(int aNumber)
	{
		//
		aNumber = Mathf.Clamp(aNumber, 1, 6);
		
		if(aNumber == mConnectors.Count)
		{
			return;
		}
		else if(aNumber > mConnectors.Count)
		{
			int difference = aNumber - mConnectors.Count;
			
			for(int i = 0; i < difference; i++)
			{
				mConnectors.Add(new Rect());
				ChildNodes.Add(null);
			}
		}
		else
		{
			int difference = mConnectors.Count - aNumber;
			ChildNodes.RemoveRange(mConnectors.Count-difference, difference);
			mConnectors.RemoveRange(mConnectors.Count-difference, difference);
		}
		
		
		float spacing = mBaseRect.width / aNumber;
		
		//
		for(int i = 0; i < aNumber; i++)
		{
			mConnectors[i] = new Rect(i*spacing + spacing/2 -5, mBaseRect.height+5, 10,15);
		}
	}
	
	
	public int GetNumberOfConnectors()
	{
		return mConnectors.Count;
	}
	
	public override void ConnectHierarchy (List<BTNode> aNodes)
	{
		base.ConnectHierarchy (aNodes);
		
		SetNumberOfConnector(mChildIDs.Count);
		
		for(int i = 0; i < mChildIDs.Count; i++)
		{
			mChildNodes[i] = (mChildIDs[i] == -1 ? null : aNodes[mChildIDs[i]]);
		}
	}
	
	public override void DrawGUI (Vector2 aOffset)
	{
		base.DrawGUI (aOffset);
	
		GUI.Box(new Rect(mBaseRect.x+aOffset.x, mBaseRect.y+mBaseRect.height+aOffset.y-1, mBaseRect.width, 10), "");
		
		// Draw connectors
		for(int i = 0; i < mConnectors.Count; i++)
		{
			GUI.Box(new Rect(mConnectors[i].x + aOffset.x + mBaseRect.x, mConnectors[i].y + aOffset.y + mBaseRect.y, mConnectors[i].width, mConnectors[i].height), "");
		}
	}
	
	// Export
	public void Export(StreamWriter aFile)
	{
		// Write my child before me
		foreach(BTNode child in mChildNodes)
		{
			if(child == null)
			{
				continue;
			}
			
			if(child is BTParentNode)
			{
				(child as BTParentNode).Export(aFile);
			}
		}
		
		//
		aFile.WriteLine("		"+mName + " " + mUniqueIdentifier + " = new "+mName+"(mCharacter);");
		
		//
		foreach(BTNode child in mChildNodes)
		{
			if(child == null)
			{
				continue;
			}
			
			if(child is BTParentNode)
			{
				aFile.WriteLine("		"+ mUniqueIdentifier + ".AddChild("+child.UniqueIdentifier+");");
			}
			else
			{
				aFile.WriteLine("		"+ mUniqueIdentifier + ".AddChild(new "+child.Name+"(mCharacter));");
			}
		}
	}
	
	//
	public override void Save (System.Xml.XmlWriter aWriter, List<BTNode> aOriginalList)
	{
		//
		aWriter.WriteStartElement("Parent");
		
		//
		aWriter.WriteAttributeString("Name", mName);
		aWriter.WriteAttributeString("UID", mUniqueIdentifier);
		aWriter.WriteAttributeString("RectX", mBaseRect.x.ToString());
		aWriter.WriteAttributeString("RectY", mBaseRect.y.ToString());
		aWriter.WriteAttributeString("RectWidth", mBaseRect.width.ToString());
		aWriter.WriteAttributeString("RectHeight", mBaseRect.height.ToString());
		aWriter.WriteAttributeString("PosX", mPosition.x.ToString());
		aWriter.WriteAttributeString("PosY", mPosition.y.ToString());
		aWriter.WriteAttributeString("Parent", mParent == null ? "-1" : aOriginalList.IndexOf(mParent).ToString() );
		
		string result = string.Join(",",(from n in mChildNodes select (n == null ? "-1" : aOriginalList.IndexOf(n).ToString())).ToArray());
		aWriter.WriteAttributeString("ChildNodes", result);
		
		//
		aWriter.WriteEndElement();
	}
	
	//
	public static BTParentNode Load(System.Xml.XmlNode aXmlNode)
	{
		BTParentNode node = new BTParentNode("");
		node.Name = aXmlNode.Attributes["Name"].Value;
		node.UniqueIdentifier = aXmlNode.Attributes["UID"].Value;
		node.mBaseRect = new Rect(	float.Parse(aXmlNode.Attributes["RectX"].Value), 
									float.Parse(aXmlNode.Attributes["RectY"].Value), 
									float.Parse(aXmlNode.Attributes["RectWidth"].Value), 
									float.Parse(aXmlNode.Attributes["RectHeight"].Value));
		
		node.mPosition = new Vector2(float.Parse(aXmlNode.Attributes["PosX"].Value), float.Parse(aXmlNode.Attributes["PosY"].Value));
		node.mParentID = int.Parse(aXmlNode.Attributes["Parent"].Value);
		node.mChildIDs = (from c in aXmlNode.Attributes["ChildNodes"].Value.Split(',') select int.Parse(c)).ToList();
		
		return node;
	}
}
