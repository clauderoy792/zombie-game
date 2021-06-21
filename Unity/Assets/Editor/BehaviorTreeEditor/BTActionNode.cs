using UnityEngine;
using System.Collections.Generic;

public class BTActionNode : BTNode
{
	public BTActionNode() : base("New Action")
	{
		Icon = Resources.Load("Icons/IcoAction") as Texture2D;
	}
	
	public override void Save (System.Xml.XmlWriter aWriter, List<BTNode> aOriginalList)
	{
		//
		aWriter.WriteStartElement("Action");
		
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
		
		//
		aWriter.WriteEndElement();
	}
	
	public static BTActionNode Load(System.Xml.XmlNode aXmlNode)
	{
		BTActionNode node = new BTActionNode();
		node.Name = aXmlNode.Attributes["Name"].Value;
		node.UniqueIdentifier = aXmlNode.Attributes["UID"].Value;
		node.mBaseRect = new Rect(	float.Parse(aXmlNode.Attributes["RectX"].Value), 
									float.Parse(aXmlNode.Attributes["RectY"].Value), 
									float.Parse(aXmlNode.Attributes["RectWidth"].Value), 
									float.Parse(aXmlNode.Attributes["RectHeight"].Value));
		
		node.mPosition = new Vector2(float.Parse(aXmlNode.Attributes["PosX"].Value), float.Parse(aXmlNode.Attributes["PosY"].Value));
		node.mParentID = int.Parse(aXmlNode.Attributes["Parent"].Value);
		
		return node;
	}
}
