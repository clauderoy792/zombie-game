using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public class Reception : Room 
{	
		
	#region PRIVATE_MEMBERS
	
	float mRelativeStartPositionX;
	float mWaitingPositionX;
	int mMaxNbWaitingCivilian;
	int mNbSpawnedCivilian;
	List<Vector2> mWaitingPositions;
	List<Character> mGoingHomeCharacters;
	ReceptionSerializationInfo mRecepSerInfo;
	
	#endregion
	
	#region ACCESSORS
	
	public float WaitingPositionX
	{
		get{return mWaitingPositionX;}
	}
	
	public float RelativeStartPositionX
	{
		get {return mRelativeStartPositionX;}
	}
	
	public List<Character> GoingHomeCharacters
	{
		get{return mGoingHomeCharacters;}
	}
	
	public int MaxNbWaitingCivilian
	{
		get{return mMaxNbWaitingCivilian;}
	}
	
	public int NbSpawnedCivilian
	{
		get{return mNbSpawnedCivilian;}
	}
	
	public int CurrentNbWaitingCivilian
	{
		get{return mCharactersUsingRoom.Count;}
	}
	
	public List<Vector2> WaitingPositions
	{
		get{return mWaitingPositions;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();
		
		//Initialize waiting position
		mWaitingPositionX = 0.2f*Room.UNIT_CELL_WIDTH;
		mMaxNbWaitingCivilian = 10;
		mWaitingPositions = new List<Vector2>(mMaxNbWaitingCivilian);
		mGoingHomeCharacters = new List<Character>(10);
	}
	
	protected override void Start ()
	{
		base.Start ();
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public override void Initialize (float aX, float aY, ERoomType aType)
	{
		base.Initialize (aX, aY, aType);
		
		//
		mRelativeStartPositionX = mGridPosition.x*Room.UNIT_CELL_WIDTH+mWaitingPositionX+GraphicsManager.HUMAN_BOX_COLLIDER_SIZE+ 0.225f*Room.UNIT_CELL_WIDTH;
		
		//Calculate all waiting positions
		for(int i = 0;i<mMaxNbWaitingCivilian;i++)
		{
			//
			mWaitingPositions.Add(new Vector2(mRelativeStartPositionX+i*(GraphicsManager.HUMAN_BOX_COLLIDER_SIZE+GraphicsManager.HUMAN_SIZE*0.15f),mGridPosition.y*Room.UNIT_CELL_HEIGHT));
		}
	}
	
	public override void PostInitialization ()
	{
		base.PostInitialization ();

		if (mRecepSerInfo.mGoingHomeCharacters != null && mRecepSerInfo.mGoingHomeCharacters.Count > 0)
		{
			foreach(int i in mRecepSerInfo.mGoingHomeCharacters)
			{
				mGoingHomeCharacters.Add(CharacterManager.Instance.GetCharById(i));
			}
			
			foreach(Character c in mGoingHomeCharacters)
			{
				(c as Civilian).GoHome(false);
			}
		}
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public Vector2 GetNextWaitingPosition()
	{
		Vector2 returnValue = Vector2.zero;

		if (mCharactersUsingRoom.Count < mWaitingPositions.Count)
		{
			returnValue = mWaitingPositions[mCharactersUsingRoom.Count];
		}
		else
		{
			Debug.LogError("Could not get next waiting position.");
		}

		return returnValue;
	}
	
	public void IncrementSpawnedCivilian()
	{
		mNbSpawnedCivilian++;
	}

	public void SetCiviliansCanLeave(bool aValue)
	{
		foreach(Character character in mCharactersUsingRoom)
		{
			Civilian c = character as Civilian;

			if (c != null)
			{
				c.SetCanLeave(aValue);
			}
		}
	}

	public bool AddWaitingCivilian(Civilian aCivilian)
	{
		bool returnValue = false;
		
		if (mCharactersUsingRoom.Count < mMaxNbWaitingCivilian)
		{
			int count = mCharactersUsingRoom.Count;
			mCharactersUsingRoom.Add(aCivilian);
			returnValue = true;
			if (count == mCharactersUsingRoom.Count)
			{
				Debug.Log("WTF");
			}
		}
		else
		{
			Debug.LogError("Failed to add waiting civilian in list");
		}
		
		return returnValue;
	}
	
	public void RemoveWaitingCivilian(Civilian aCharacter)
	{
		int charIndex = mCharactersUsingRoom.IndexOf(aCharacter);
		
		if (charIndex != -1)
		{
			//Tell characters that were behind to move forward
			for(int i = charIndex+1;i <mCharactersUsingRoom.Count;i++)
			{
				mCharactersUsingRoom[i].MoveToLocalPoint(mWaitingPositions[i-1]);
				(mCharactersUsingRoom[i] as Civilian).WaitingPosition = mWaitingPositions[i-1];
				mCharactersUsingRoom[i-1] = mCharactersUsingRoom[i];
			}
			
			//Remove at last index.
			mCharactersUsingRoom.RemoveAt(mCharactersUsingRoom.Count-1);
			
			//
			mGoingHomeCharacters.Add(aCharacter);
			
			mNbSpawnedCivilian--;
		}
		else
		{
			Debug.LogError("Could not remove character from Reception's queue. Char's ID is : "+aCharacter.ID);
		}
	}
	
	public void RemoveGoingHomeCharacter(Character aCharacter)
	{
		mGoingHomeCharacters.Remove(aCharacter);
	}
	
	public bool CanSpawnCivilian()
	{
		return mNbSpawnedCivilian < mMaxNbWaitingCivilian;
	}
	
	#endregion

	#region OVERRIDEN_METHODS

	public override bool RemoveCharacter (Character aCharacter)
	{
		//When removing character, dont remove it from the charaters using room.
		return true;
	}

	#endregion
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new ReceptionSerializationInfo(this);
	}
	
	public override void Deserialize (RoomSerializationInfo aInfo)
	{
		base.Deserialize (aInfo);
		
		//
		mRecepSerInfo = aInfo as ReceptionSerializationInfo;
		
		//
		mWaitingPositionX = mRecepSerInfo.mWaitingPositionX;
		
		//
		mNbSpawnedCivilian = mRecepSerInfo.mNbSpawnedCivilian;
		
		//
		mWaitingPositions = mRecepSerInfo.mWaitingPositions.Deserialize();
	}
	
	#endregion
}
