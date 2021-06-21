using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterSerializationInfo {
	
	#region PUBLIC_MEMBERS
	
	public string mName = "";
	
	//
	public float mGridPositionX;
	public float mGridPositionY;
	public float mCurrentRoomX;
	public float mCurrentRoomY;
	public float mTransformX;
	public float mTransformY;
	public float mTransformZ;
	public float mInitZ;
	
	public ECharacterType mType;
	
	//
	public bool mIsMovingRandomly;
	public bool mIsInElevator;
	
	//
	public int mDestinationFloor;
	public int mAtFloorWhenCallingElevator;
	public int mID;
	
	//
	public float mMovementSpeed;
	#endregion
	
	#region CONSTRUCTORS
	
	public CharacterSerializationInfo(Character aCharacter)
	{
		//
		mID = aCharacter.ID;
		
		//
		mTransformX = aCharacter.LocalTransformPosition.x;
		mTransformY = aCharacter.LocalTransformPosition.y;
		mTransformZ = aCharacter.LocalTransformPosition.z;
		
		//
		mInitZ 		= aCharacter.InitZ;
		
		//
		mName = aCharacter.Name;
		
		//Elevator
		mIsInElevator = aCharacter.IsInElevator;
		mDestinationFloor = aCharacter.DestinationFloor;
		mAtFloorWhenCallingElevator = aCharacter.AtFloorWhenCallingElevator;
		
		//
		mGridPositionX = aCharacter.GridPosition.x;
		mGridPositionY = aCharacter.GridPosition.y;
		
		mIsMovingRandomly = aCharacter.IsMovingRandomly;
		mMovementSpeed = aCharacter.MovementSpeed;
		
		mType = aCharacter.Type;

		//
		if (aCharacter.CurrentRoom != null)
		{
			mCurrentRoomX = aCharacter.CurrentRoom.GridPosition.x;
			mCurrentRoomY = aCharacter.CurrentRoom.GridPosition.y;
		}
		else
		{
			mCurrentRoomX = -1;
			mCurrentRoomY = -1;
		}
	}
	
	#endregion
}
