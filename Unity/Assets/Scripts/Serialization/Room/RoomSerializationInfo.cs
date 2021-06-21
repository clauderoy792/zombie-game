using System;
using HCUtils;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class RoomSerializationInfo  {
	
	#region PUBLIC_MEMBERS

	//
	public List<int> mCharactersUsingRoom;

	//
	public float mX;
	public float mY;
	
	//
	public int mMaxHumansPerRoom;
	public int mWidth;
	public int mHeight;
	public int mNbJunks;
	
	//
	public ERoomType mRoomType;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public RoomSerializationInfo(Room aRoom)
	{
		//
		mCharactersUsingRoom = new List<int>();
		
		foreach(Character c in aRoom.CharactersUsingRoom)
		{
			mCharactersUsingRoom.Add(c.ID);
		}
		
		//
		mX = aRoom.GridPosition.x;
		mY = aRoom.GridPosition.y;
		
		//
		mNbJunks = aRoom.NumberOfJunks;
		
		//
		mMaxHumansPerRoom = aRoom.MaxNumberOfHumans;
		mWidth = aRoom.Width;
		mHeight = aRoom.Height;
		
		//
		mRoomType = aRoom.Type;;
	}
	
	#endregion
}
