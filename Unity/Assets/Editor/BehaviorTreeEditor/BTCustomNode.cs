using UnityEngine;
using System.Linq;
using System.Collections;


public class BTCustomNode : BTParentNode
{
	public BTCustomNode() : base("Custom")
	{
		Icon = Resources.Load("Icons/IcoCustom") as Texture2D;
	}
	
	public override void Save (System.Xml.XmlWriter aWriter, System.Collections.Generic.List<BTNode> aOriginalList)
	{
		//
		aWriter.WriteStartElement("Custom");
		
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
	public new static BTCustomNode Load(System.Xml.XmlNode aXmlNode)
	{
		BTCustomNode node = new BTCustomNode();
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
