using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class BTNode 
{
	protected Rect mBaseRect = new Rect(0,0,60,60);
	protected Vector2 mPosition;
	protected Texture2D mIcon;
	protected string mName = "";
	protected string mUniqueIdentifier = "";
	protected bool mNameEditEnable = true;
	protected bool mDeletable = true;
	protected bool mMovable = true;
	protected BTParentNode mParent = null;
	protected int mParentID = -1;
	
	
	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <value>
	/// The position.
	/// </value>
	public Vector2 Position
	{
		get{return mPosition;}
		set
		{
			mPosition = value;
			mBaseRect = new Rect(mPosition.x-mBaseRect.width/2, mPosition.y-mBaseRect.height/2, mBaseRect.width, mBaseRect.height);
		}
	}
	
	/// <summary>
	/// Sets the boundaries.
	/// </summary>
	/// <value>
	/// The boundaries.
	/// </value>
	public Rect Rectangle
	{
		get{return mBaseRect;}
	}
	
	/// <summary>
	/// Gets or sets the icon.
	/// </summary>
	/// <value>
	/// The icon.
	/// </value>
	public Texture2D Icon
	{
		get{return mIcon;}
		set{mIcon = value;}
	}
	
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name
	{
		get{return mName;}
		set
		{
			string n = value.Replace(" ", "_");
			mName = n;
		}
	}
	
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string UniqueIdentifier
	{
		get{return mUniqueIdentifier;}
		set
		{
			string n = value.Replace(" ", "_");
			mUniqueIdentifier = n;
		}
	}

	/// <summary>
	/// Gets or sets this node parent.
	/// </summary>
	/// <value>
	/// The parent.
	/// </value>
	public BTParentNode Parent
	{
		get{return mParent;}
		set{mParent = value;}
	}
	
	/// <summary>
	/// Gets or sets the parent I.
	/// </summary>
	/// <value>
	/// The parent I.
	/// </value>
	public int ParentID
	{
		get{return mParentID;}
		set{mParentID = value;}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BTNode"/> name can be edited.
	/// </summary>
	/// <value>
	/// <c>true</c> if name edit enable; otherwise, <c>false</c>.
	/// </value>
	public bool NameEditEnable
	{
		get{return mNameEditEnable;}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BTNode"/> is deletable.
	/// </summary>
	/// <value>
	/// <c>true</c> if deletable; otherwise, <c>false</c>.
	/// </value>
	public bool Deletable
	{
		get{return mDeletable;}
	}
	
	public bool Movable
	{
		get{return mMovable;}
	}
		
	/// <summary>
	/// Initializes a new instance of the <see cref="BTNode"/> class.
	/// </summary>
	/// <param name='aName'>
	/// A name.
	/// </param>
	public BTNode(string aName)
	{
		mName = aName;
	}
	
	//
	public virtual void ConnectHierarchy(List<BTNode> aNodes)
	{
		mParent = mParentID == -1 ? null : aNodes[mParentID] as BTParentNode;
	}
	
	
	/// <summary>
	/// Draws this control GUI. (Need to be called in OnGUI function)
	/// </summary>
	public virtual void DrawGUI(Vector2 aOffset)
	{
		if(mIcon != null)
			GUI.Box(new Rect(mBaseRect.x+aOffset.x, mBaseRect.y+aOffset.y, mBaseRect.width, mBaseRect.height), mIcon);
		else
			GUI.Box(new Rect(mBaseRect.x+aOffset.x, mBaseRect.y+aOffset.y, mBaseRect.width, mBaseRect.height), "");
		
		//
		GUI.skin.label.alignment = TextAnchor.MiddleCenter; 
		GUI.Label(new Rect(mBaseRect.x + aOffset.x - 20, mBaseRect.y + aOffset.y - 20, mBaseRect.width + 40, 20), mName);
	}
	
	public abstract void Save(System.Xml.XmlWriter aWriter, List<BTNode> aOriginalList);
}
