using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class ElevatorRouter : ISerializable{

	#region CONSTANTS
	
	private const int NO_DESTINATION = -1;
	private const float TIME_BEFORE_TELEPORT = 5f;

	#endregion

	#region MEMBERS

	private List<Character> 		mCharacters = null;
	private List<ElevatorWaitInfo> 	mWaitInfos = null;
	private ElevatorWaitInfo?		mCurrentElevatorInfo = null;
	private float					mLastTimeIdle;
	private bool 					mIsGoingUp = false;
	private bool					mIsTeleporting = false;
	private bool					mWillGoUpAfterUltimateDestination = false;
	private EElevatorState 			mCurrentState = EElevatorState.Idle;
	private int 					mUltimateDestination;
	private int 					mCurrentFloor;
	private int						mCurrentDestination;
	
	#endregion

	#region TEMP_MEMBERS

	List<int> characters = null;

	#endregion

	#region ACCESSORS

	public EElevatorState CurrentState
	{
		get{return mCurrentState;}
	}

	public int CurrentFloor
	{
		get{return mCurrentFloor;}
	}

	public int CurrentDestination
	{
		get{return mCurrentDestination;}
	}

	public List<Character> Characters
	{
		get{return mCharacters;}
	}

	public bool IsGoingUp
	{
		get{return mIsGoingUp;}
	}

	#endregion

	#region CONSTRUCTOR

	public ElevatorRouter()
	{
		mCharacters						= new List<Character>();
		mWaitInfos 						= new List<ElevatorWaitInfo>();
		mCurrentState 					= EElevatorState.Idle;
		mCurrentDestination				= NO_DESTINATION;
		mUltimateDestination			= NO_DESTINATION;
	}

	#endregion

	#region PUBLIC_METHODS

	/// <summary>
	/// Gets the next destination. Will return -1 if it is not idle.
	/// </summary>
	/// <returns>The next destination.</returns>
	public int GetNextDestination()
	{
		int returnValue = -1;

		//
		switch(mCurrentState)
		{
			case EElevatorState.Idle:
				
				break;
			case EElevatorState.Going_Up:
				break;
			case EElevatorState.Going_Down:
				break;
		}

		return returnValue;
	}

	public void CallElevator(int aCharId,int aAtFloor, int aGoingToFloor)
	{
		//Prevent same character from calling elevator multiple time
		bool charContained = false;
		int oldCharInfoIndex = -1;

		for(int i = 0;i < mWaitInfos.Count;i++)
		{
			if (mWaitInfos[i].mCharacterId == aCharId)
			{
				charContained = true;
				oldCharInfoIndex = i;
				break;
			}
		}

		//
		if (!charContained)
		{
			ElevatorWaitInfo info = new ElevatorWaitInfo(aCharId, aAtFloor,aGoingToFloor);

			//Add elevator info.
			mWaitInfos.Add(info);
			int nb = 0;
			List<int> ids = new List<int>();
			for(int i = 0;i < mWaitInfos.Count;i++)
			{
				if (mWaitInfos[i].mAtFloor == aAtFloor)
				{
					ids.Add( aCharId);
					nb++;
				}
			}

			if (nb == 4 && aAtFloor != 0)
			{
				List<Character> chars = new List<Character>();
				for(int i =0;i<ids.Count;i++)
				{
					chars.Add(CharacterManager.Instance.GetCharById(ids[i]));
				}
			}

			//If elevator is idle.
			if (mCurrentState == EElevatorState.Idle)
			{
				SetDestinationAndState();
			}
		}
		else
		{
			//Remove character's old entry and add new one
			ElevatorWaitInfo info = mWaitInfos[oldCharInfoIndex];
			mWaitInfos.RemoveAt(oldCharInfoIndex);

			//Change going to floor.
			info.UpdateGoingToFloor(aGoingToFloor);

			//Add at the end
			mWaitInfos.Add(info);
		}
	}

	public void UpdateCharactersTransformPosition()
	{
		for(int i = 0;i < mCharacters.Count;i++)
		{
			//Place Change the character transform
			mCharacters[i].LocalTransformPosition = new Vector3(mCharacters[i].LocalTransformPosition.x,mCurrentFloor*Room.UNIT_CELL_HEIGHT,mCharacters[i].LocalTransformPosition.z);
			mCharacters[i].SetCharacterRoomAndGridPosition();
		}
	}

	#endregion

	#region CHARACTER_ACCESS

	public bool CanGoIn(Character aChar)
	{
		bool returnValue = false;
		bool charGoingUp = IsCharacterGoingUp(aChar);

		//If the elevator is going up or down and that the character destination is in th same direciton
		if (mIsGoingUp == charGoingUp)
		{
			returnValue = true;
		}
		else if (mUltimateDestination != NO_DESTINATION && charGoingUp == mWillGoUpAfterUltimateDestination)
		{
			//If were going to go in the same direction after moving to the ultimate destination.
			returnValue = true;
		}

		return returnValue;
	}

	#endregion

	#region ADD_REMOVE_CHARACTERS
	
	public void AddCharacter(Character aCharacter)
	{
		mCharacters.Add(aCharacter);
	}
	
	public void RemoveCharactersThatNeedToGetOut()
	{
		//Remove characters that are at destination.
		for(int i = 0;i <mCharacters.Count;i++)
		{
			if (mCharacters[i].DestinationFloor == mCurrentFloor)
			{
				//
				mWaitInfos.Remove(new ElevatorWaitInfo(mCharacters[i].ID, mCharacters[i].AtFloorWhenCallingElevator,mCharacters[i].DestinationFloor));

				//
				mCharacters[i].IsInElevator = false;

				//Place Change the character transform
				mCharacters[i].LocalTransformPosition = new Vector3(mCharacters[i].LocalTransformPosition.x,mCurrentFloor*Room.UNIT_CELL_HEIGHT,mCharacters[i].InitZ);
				
				mCharacters.RemoveAt(i);
				i--;
			}
		}
	}

	public bool RemoveCharacter(int aId)
	{
		bool returnValue = false;

		//
		for(int i = 0;i <mWaitInfos.Count;i++)
		{
			if (mWaitInfos[i].mCharacterId == aId)
			{
				mWaitInfos.RemoveAt(i);
				returnValue = true;
				break;
			}
		}

		return returnValue;
	}

	#endregion

	#region DESINATIONS_AND_STATE_MANAGEMENT

	public void SetDestinationAndState()
	{
		//
		if (mCurrentFloor == mCurrentDestination || mCurrentDestination == NO_DESTINATION)
		{
			if (IsThereCharacterGoingInTheSameDirection())
			{
				//Continue in the same direction
				SetNextDestinationOnSameState();
			}
			else if (mCharacters.Count > 0)
			{
				//go drop a character that is currently is the elevator
				SetNextDestinationWithNextPriority();
			}
			else if (mWaitInfos.Count > 0)
			{
				//Go get a waiting character
				SetNextDestinationForWaitingCharacter();
			}
		}

		//If no one in elevator
		if (mWaitInfos.Count == 0 && mCharacters.Count == 0)
		{
			ResetStateAndDestination();
		}
	}

	void SetDestinationAndStateWithoutConsideringCharactersIn()
	{

	}

	void SetNextDestinationOnSameState()
	{
		if (mCurrentState == EElevatorState.Going_Up)
		{
			//
			mCurrentDestination++;
		}
		else if (mCurrentState == EElevatorState.Going_Down)
		{
			//
			mCurrentDestination--;
		}

		//
		mIsGoingUp = mCurrentDestination > mCurrentFloor;
	}

	void SetNextDestinationWithNextPriority()
	{
		if (mWaitInfos.Count > 0)
		{
			mCurrentElevatorInfo = mWaitInfos[0];

			//
			if (mCurrentFloor != mCurrentElevatorInfo.Value.mAtFloor)
			{
				//At floor
				mCurrentDestination = mCurrentElevatorInfo.Value.mAtFloor;
			}
			else 
			{
				//Going to floor
				mCurrentDestination = mCurrentElevatorInfo.Value.mGoingToFloor;
			}

			//
			mCurrentState = mCurrentDestination > mCurrentFloor ? EElevatorState.Going_Up : EElevatorState.Going_Down;
			mIsGoingUp = mCurrentDestination > mCurrentFloor;
		}
		else
		{
			//
			ResetStateAndDestination();
		}
	}

	void SetNextDestinationForWaitingCharacter()
	{
		//
		mCurrentElevatorInfo = mWaitInfos[0];

		//Set Current destination
		mCurrentDestination = mCurrentElevatorInfo.Value.mAtFloor;
		mUltimateDestination = mCurrentDestination;
		mWillGoUpAfterUltimateDestination = mCurrentElevatorInfo.Value.mGoingToFloor > mUltimateDestination;

		
		//
		if (mCurrentDestination > mCurrentFloor)
		{
			mCurrentState = EElevatorState.Going_Up;
		}
		else if (mCurrentDestination < mCurrentFloor)
		{
			mCurrentState = EElevatorState.Going_Down;
		}
		else
		{
			mCurrentState = EElevatorState.Idle;
		}

		//If the destination is different than the current floor set the going up value with the
		//at floor of the character else set it with the going to floor of the character.
		if (mCurrentDestination != mCurrentFloor)
		{
			//
			mIsGoingUp = mCurrentDestination > mCurrentFloor;
		}
		else
		{
			//
			mIsGoingUp = mCurrentElevatorInfo.Value.mGoingToFloor > mCurrentFloor;
		}
	}
	
	void ResetStateAndDestination()
	{
		mCurrentState = EElevatorState.Idle;
		mCurrentDestination = NO_DESTINATION;
		mUltimateDestination = NO_DESTINATION;
		mCurrentElevatorInfo = null;
		mLastTimeIdle = Time.time;
	}

	public bool ShouldStopHere()
	{
		bool returnValue = false;

		//
		if (mUltimateDestination != NO_DESTINATION && mCurrentFloor == mUltimateDestination)
		{
			returnValue = true;
		}
		else
		{
			//if we arrived at some one's destnation
			for(int i =0; i<mCharacters.Count;i++)
			{
				if (mCharacters[i].DestinationFloor == mCurrentFloor)
				{
					returnValue = true;
					break;
				}
			}

			if (!returnValue)
			{
				//If there is people that wants to go in the same direction as we are going now
				for(int i =0; i<mWaitInfos.Count;i++)
				{
					if (mCurrentFloor == mWaitInfos[i].mAtFloor && mIsGoingUp == mWaitInfos[i].mIsGoingUp)
					{
						returnValue = true;
						break;
					}
				}
			}
		}

		return returnValue;
	}

	public void ArriveToNextFloor()
	{
		//Before arriving to next floor, reset ultimate destination if we were there before
		if (mUltimateDestination != NO_DESTINATION && mCurrentFloor == mUltimateDestination)
		{
			mUltimateDestination = NO_DESTINATION;
		}

		if (mCurrentDestination != NO_DESTINATION)
		{
			if (mCurrentDestination > mCurrentFloor)
			{
				mCurrentFloor++;
			}
			else if (mCurrentFloor > mCurrentDestination)
			{
				mCurrentFloor--;
			}
		}
		else
		{
			Debug.LogError("Could not change floor , current destination : "+mCurrentDestination+", current floor : "+mCurrentFloor);
		}

		//
		//SetDestinationAndState();
	}


	#endregion

	#region UTILS_METHODS

	bool IsThereCharacterGoingInTheSameDirection()
	{
		bool returnValue = false;

		for(int i = 0;i<mCharacters.Count;i++)
		{
			if (mCurrentState == EElevatorState.Going_Up)
			{
				//Is there a character that wants to go up;
				if (mCharacters[i].DestinationFloor > mCurrentFloor)
				{
					returnValue = true;
					break;
				}
			}
			else if (mCurrentState == EElevatorState.Going_Down)
			{
				//Is there a character that wants to go down;
				if (mCharacters[i].DestinationFloor < mCurrentFloor)
				{
					returnValue = true;
					break;
				}
			}
		}

		return returnValue;
	}

	bool CanRemoveCurrentFloorFromDestinations()
	{
		bool returnValue = true;
		
		for(int i = 0;i < mWaitInfos.Count;i++)
		{
			//There is still people that needs to go to this destination so dont remove it.
			if (mIsGoingUp == mWaitInfos[i].mIsGoingUp && mWaitInfos[i].mGoingToFloor == mCurrentFloor)
			{
				returnValue = false;
				break;
			}
		}
		
		return returnValue;
	}

	public bool IsCharacterGoingUp(Character aChar)
	{
		return (aChar.DestinationFloor > aChar.AtFloorWhenCallingElevator);
	}

	#endregion

	#region MERGE_MANAGEMENT

	public void Merge(ElevatorRouter aOther)
	{
		//Add other characters.
		mCharacters.AddRange(aOther.mCharacters);

		//Add destinations
		mWaitInfos.AddRange(aOther.mWaitInfos);

		for(int i = 0;i < aOther.mCharacters.Count;i++)
		{
			//Set Position of characters.
			Vector3 pos = aOther.mCharacters[i].LocalTransformPosition;
			aOther.mCharacters[i].LocalTransformPosition = new Vector3(pos.x,mCurrentFloor*Room.UNIT_CELL_HEIGHT, pos.z);
			aOther.mCharacters[i].SetCharacterRoomAndGridPosition();
		}
	}

	#endregion

	#region DISPOSE

	public void Dispose()
	{
		mCharacters.Clear();
		mWaitInfos.Clear();
	}

	#endregion

	#region SERIALIZATION

	protected ElevatorRouter(SerializationInfo info, StreamingContext context)
	{
		mCharacters = new List<Character>();

		characters = info.GetValue("characters",typeof(List<int>)) as List<int>;

		//
		for(int i = 0;i< characters.Count;i++)
		{
			mCharacters.Add(CharacterManager.Instance.GetCharById(characters[i]));
		}

		//
		mWaitInfos 							= info.GetValue("waitInfos",typeof(List<ElevatorWaitInfo>)) as List<ElevatorWaitInfo>;
		mLastTimeIdle 						= (float)info.GetValue("lastTimeIdle",typeof(float));
		mIsGoingUp 							= info.GetBoolean("isGoingUp");
		mIsGoingUp 							= info.GetBoolean("isTeleporting");
		mWillGoUpAfterUltimateDestination 	= info.GetBoolean("willGoUpAfter");
		mCurrentState 						= (EElevatorState)info.GetValue("currentState",typeof(EElevatorState));
		mUltimateDestination 				= info.GetInt32("ultimateDestination");
		mCurrentFloor 						= info.GetInt32("currentFloor");
		mCurrentDestination 				= info.GetInt32("currrentDestination");
	}
	
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//
		List<int> ids = new List<int>();

		//Characters
		for(int i = 0;i < mCharacters.Count;i++)
		{
			ids.Add(mCharacters[i].ID);
		}

		info.AddValue("characters",ids);
		info.AddValue("waitInfos",mWaitInfos);
		info.AddValue("lastTimeIdle",mLastTimeIdle);
		info.AddValue("isGoingUp",mIsGoingUp);
		info.AddValue("isTeleporting",mIsTeleporting);
		info.AddValue("willGoUpAfter",mWillGoUpAfterUltimateDestination);
		info.AddValue("currentState",mCurrentState);
		info.AddValue("ultimateDestination",mUltimateDestination);
		info.AddValue("currentFloor",mCurrentFloor);
		info.AddValue("currrentDestination",mCurrentDestination);
	}

	#endregion
}
