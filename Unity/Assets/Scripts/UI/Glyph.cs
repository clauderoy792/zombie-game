using UnityEngine;
using System;

//
[System.Serializable]
public class Glyph
{
	[SerializeField]int mCharID;
	[SerializeField]Rect mUV;
	[SerializeField]int mYOffset;
	[SerializeField]int mWidth;
	[SerializeField]int mHeight;

	//
	public Glyph(char aChar, int aYOffset, Rect aUV)
	{
		this.mCharID = aChar.GetHashCode();
		this.mYOffset = aYOffset;
		this.mUV = aUV;
	}
	
	/// <summary>
	/// Sets the uv.
	/// </summary>
	/// <param name="aNewUv">A new uv.</param>
	public void SetUv(Rect aNewUv)
	{
		mUV = aNewUv;
	}
	
	/// <summary>
	/// Sets the Y offset.
	/// </summary>
	/// <param name="aOffset">A offset.</param>
	public void SetYOffset(int aOffset)
	{
		mYOffset = aOffset;
	}

	/// <summary>
	/// Sets the width.
	/// </summary>
	/// <param name="aWidth">A width.</param>
	public void SetWidth (int aWidth)
	{
		mWidth = aWidth;
	}

	/// <summary>
	/// Sets the height.
	/// </summary>
	/// <param name="aHeight">A height.</param>
	public void SetHeight (int aHeight)
	{
		mHeight = aHeight;
	}
	
	/// <summary>
	/// Gets the character ID.
	/// </summary>
	/// <value>The char I.</value>
	public int CharID
	{
		get{return mCharID;}
	}
	
	/// <summary>
	/// Gets the Atlas UV.
	/// </summary>
	/// <value>The U.</value>
	public Rect UV
	{
		get{return mUV;}
	}
	
	/// <summary>
	/// Gets the original character height.
	/// </summary>
	/// <value>The height of the original.</value>
	public int YOffset
	{
		get{return mYOffset;}
	}

	/// <summary>
	/// Gets the width.
	/// </summary>
	/// <value>The width.</value>
	public int Width
	{
		get{return mWidth;}
	}

	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>The height.</value>
	public int Height
	{
		get{return mHeight;}
	}
}