using UnityEngine;
using System;
using System.Collections;
using HCUtils;

[System.Serializable]
public class TimeManager
{
	#region CONSTANTS
	
	//
	public const float SECONDS_TO_INCREMENT_WEEKS			= 30;
	private const int NB_WEEKS_TO_PAY_CHARACTERS			= 4;

	#endregion

	#region STATIC_MEMBERS

	private static TimeManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS
	
	private SimpleDateTime mCurrentTime;
	
	//Upcoming civilians
	private float mTimeBeforeCivilianTimerElapse = 10;
	private int mCivilianTimerSuccessRate = 30;
	
	[NonSerializedAttribute]
	private Reception mReception;
	
	//
	private Timer mTimerCharactersPay;
	private Timer mGameUpdateTimer;
	
	//
	private	RandomTimer mUpcomingCivilianTimer;

	#endregion

	#region ACCESSORS

	public static TimeManager Instance
	{
		get 
		{
			if (sInstance == null)
			{
				sInstance = new TimeManager();
			}
			
			return sInstance;
		}
	}
	
	public SimpleDateTime CurrentTime
	{
		get{return mCurrentTime;}
	}
	
	public Timer GameUpdateTimer
	{
		get{return mGameUpdateTimer;}
	}
	
	public Timer TimerCharactersPay
	{
		get{return mTimerCharactersPay;}
	}
	
	public RandomTimer UpcominCivilianTimer
	{
		get{return mUpcomingCivilianTimer;}
	}

	#endregion

	#region CONSTRUCTORS

	private TimeManager()
	{
		//Start at week Y1 M1 W1
		mCurrentTime = new SimpleDateTime(1,1,1);
		
		//
		mTimerCharactersPay = new Timer(SECONDS_TO_INCREMENT_WEEKS*NB_WEEKS_TO_PAY_CHARACTERS,PayEmployees,true);
		mGameUpdateTimer	= new Timer(SECONDS_TO_INCREMENT_WEEKS,UpdateGameTime,true);
		
		//
		mUpcomingCivilianTimer = new RandomTimer(mTimeBeforeCivilianTimerElapse,SpawnCivilian,mCivilianTimerSuccessRate,true);
		
		
		//
		mTimerCharactersPay.Start();
		mUpcomingCivilianTimer.Start();
	}

	#endregion
	
	#region GAME_TIME_MANAGEMENT
	
	public void StartUpdatingGameTime()
	{
		mGameUpdateTimer.Start();
	}
	
	void UpdateGameTime()
	{
		//
		mCurrentTime.IncrementWeek();
		ContractManager.Instance.UpdateContracts(mCurrentTime);
	}
	
	#endregion

	#region TIMED_EVENTS
	
	void PayEmployees()
	{
		int cost = 0;
		
		foreach(var employee in CharacterManager.Instance.Characters)
		{
			if (employee is Human)
			{
				cost += (employee as Human).Salary;
			}
		}

		if (GameManager.Instance.UserStats.CanBuy(ECurrency.Gold,cost))
		{
			GameManager.Instance.UserStats.RemoveMoney(ECurrency.Gold,cost);
			Debug.Log("GOTTA PAY EMPLOYEES : "+cost);
		}
		else
		{
			//TODO Put something to help the player with more money ? -CR
			Debug.Log("CAN'T PAY EMPLOYEES : "+cost);
		}
	}
			
	void SpawnCivilian()
	{
		if (mReception == null)
		{
			mReception = RoomManager.Instance.GetSameTypeRoom(ERoomType.Reception)[0] as Reception;
		}
		
		if (mReception != null)
		{
			if (mReception.CanSpawnCivilian())
			{
				//
				Civilian civilian = CharacterManager.Instance.AddCharacter(ECharacterType.Civilian,mReception) as Civilian;

				if (civilian != null)
				{
					//Offset the characters.
					civilian.LocalTransformPosition = new Vector3(BuildingManager.Instance.LeftBorder-2*Room.UNIT_CELL_WIDTH,0,civilian.LocalTransformPosition.z);

					//
					civilian.SetCharacterRoomAndGridPosition();

					//
					civilian.MoveTowardReception();
					
					//
					mReception.IncrementSpawnedCivilian();
				}
				else
				{
					Debug.LogError("Could not spawn civilian");
				}
			}
		}
		else
		{
			Debug.Log("Could not find reception");
		}
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public TimeManagerSerializationInfo Serialize()
	{
		return new TimeManagerSerializationInfo();
	}
	
	public void Deserialize(TimeManagerSerializationInfo aInfo)
	{
		//
		mCurrentTime 			= aInfo.mCurrentTime;
		
		//Timers
		mTimerCharactersPay 	= aInfo.mTimerCharactersPay;
		mGameUpdateTimer		= aInfo.mGameUpdateTimer;
		
		//Random timers
		mUpcomingCivilianTimer 	= aInfo.mUpcomingCivilianTimer;
		
		//Update timer's delay
		mGameUpdateTimer.SetDuration(SECONDS_TO_INCREMENT_WEEKS);
	}
	
	#endregion
}
