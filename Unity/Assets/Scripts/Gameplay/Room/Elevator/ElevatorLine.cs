using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using HCUtils;

[Serializable]
public class ElevatorLine :ISerializable,IDeserializationCallback{
	
	#region CONSTANTS
	
	private const int NO_DESTINATION = -1;
	private const float OPEN_DOORS_TIME = 1f;
	private const float CLOSE_DOORS_TIME = 0.5f;
	private const float ELEVATOR_DEPTH = 50;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private ElevatorRouter mElevatorRouter;
	private List<Elevator> mElevators;
	private int mCurrentFloor;
	private int mCurrentDestination;
	private Timer mFloorMovingTimer;
	private Timer mWaitToCloseDoorsTimer;
	private Timer mOpenDoorsTimer;
	private Timer mCloseDoorsTimer;
	private Timer mFloorTeleportTimer;
	private float mSpeed = 2f;
	private bool mIsWaitingForPeopleToGetIn;
	private bool mIsElevatorMoving = false;
	private bool mIsDoorsOpen = false;
	private bool mIsTeleporting = false;
	
	/// <summary>
	/// The time that the elevator stays idle between movements.
	/// </summary>
	private float mWaitTime = 3f;
	
	#endregion
	
	#region CONSTRUCTOR
	
	public ElevatorLine(Elevator aRoot)
	{
		//
		mElevators = new List<Elevator>();
		
		//
		mFloorMovingTimer = new Timer(mSpeed,OnElevatorStop);
		mFloorTeleportTimer = new Timer(mSpeed/2,OnElevatorTeleported);
		mWaitToCloseDoorsTimer = new Timer(mWaitTime,CloseDoors);
		mOpenDoorsTimer = new Timer(OPEN_DOORS_TIME,OnDoorsOpen);
		mCloseDoorsTimer = new Timer(CLOSE_DOORS_TIME,OnDoorsClosed);
		
		//
		mElevators.Add(aRoot);
		
		mCurrentFloor = (int)aRoot.GridPosition.y;

		mElevatorRouter = new ElevatorRouter();
	}
	
	#endregion
	
	#region ACCESSORS
	
	public Room Root
	{
		get{return mElevators[0];}
	}
	
	public int CurrentFloor
	{
		get{return mCurrentFloor;}
	}
	
	public int CurrentDestination
	{
		get{return mCurrentDestination;}
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public bool CanAdd(Elevator aElevator)
	{
		bool returnValue = false;

		//
		if (mElevators[0].GridPosition.x == aElevator.GridPosition.x)
		{
			//Add below
			if (mElevators[0].GridPosition.y > 0 && aElevator.GridPosition.y == mElevators[0].GridPosition.y-1)
			{
				returnValue = true;
			}
			//Add above
			else if (mElevators[mElevators.Count-1].GridPosition.y < PathFinder.GRID_HEIGHT-1 && 
			         aElevator.GridPosition.y == mElevators[mElevators.Count-1].GridPosition.y +1)
			{
				returnValue = true;
			}
		}

		return returnValue;
	}
	
	public bool CanGoIn(Character aCharacter)
	{
		bool returnValue = false;

		if (mIsDoorsOpen)
		{
			//if the elevator is waiting for people to get in at my floor
			if (mCurrentFloor == aCharacter.GridPosition.y && mIsWaitingForPeopleToGetIn)
			{
				returnValue = true;
				returnValue = mElevatorRouter.CanGoIn(aCharacter);
			}
		}

		return returnValue;
	}

	public bool RemoveCharacter(Character aChar)
	{
		return mElevatorRouter.RemoveCharacter(aChar.ID);
	}
	
	/// <summary>
	/// Initializes the character in the elevator when the game is loaded.
	/// </summary>
	/// <param name='aChar'>
	/// A char.
	/// </param>
	public void InitializeCharacter(Character aChar)
	{
		//
		mElevatorRouter.AddCharacter(aChar);
		Debug.Log("HERE2");
		aChar.IsInElevator = true;
		aChar.SetTransformDepth(aChar.LocalTransformPosition.z+ELEVATOR_DEPTH);
	}
	
	public bool AddWaitingCharacterInElevator(Character aChar)
	{
		bool returnValue = false;

		if (CanGoIn(aChar))
		{
			//
			mElevatorRouter.AddCharacter(aChar);
			
			//
			aChar.IsInElevator = true;
			aChar.SetTransformDepth(aChar.LocalTransformPosition.z +ELEVATOR_DEPTH);
			
			returnValue = true;
		}
		else
		{
			Debug.LogError("Could not add character in elevator.");
		}
		
		return returnValue;
	}
	
	/// <summary>
	/// Adds the line.
	/// </summary>
	/// <param name='aLine'>
	/// A line.
	/// </param>
	public void AddLine(ElevatorLine aLine)
	{
		//
		aLine.SetElevatorsState(mElevatorRouter.CurrentState);

		//
		mElevators.AddRange(aLine.mElevators);

		//Close elevator's doors
		if (aLine.mIsDoorsOpen && mIsWaitingForPeopleToGetIn)
		{
			aLine.mElevators[aLine.GetCurrentFloorIndexInArray()].CloseDoors();
		}

		//
		mElevatorRouter.Merge(aLine.mElevatorRouter);

		//Remove old line
		ElevatorManager.Instance.RemoveElevator(aLine);
	}
	
	public bool IsAppropriateForCharacter(Character aChar)
	{
		bool returnValue = false;
		
		//
		if (aChar.GridPosition.x == mElevators[0].GridPosition.x)
		{
			//
			returnValue = aChar.GridPosition.y >= mElevators[0].GridPosition.y && aChar.GridPosition.y <= mElevators[mElevators.Count-1].GridPosition.y;
		}
		
		return returnValue;
	}
	 
	public void AddElevator(Elevator aElevator)
	{
		
		if (aElevator.GridPosition.y < mElevators[0].GridPosition.y)
		{
			//Set new elevator as root
			mElevators.Insert(0,aElevator);
		}
		else
		{
			//Add at the end
			mElevators.Add(aElevator);
		}

		//
		aElevator.SetElevatorState(mElevatorRouter.CurrentState);
	}
	
	public void CallElevator(int aCharId,int aAtFloor,int aGoingToFloor)
	{	
		//
		mElevatorRouter.CallElevator(aCharId, aAtFloor,aGoingToFloor);

		//If the elevator is idle and not waiting to go to another floor.
		if (!mIsElevatorMoving && !mIsWaitingForPeopleToGetIn && !mIsDoorsOpen && !mOpenDoorsTimer.IsRunning)
		{
			//If were at the current floor of character, wait for it to go in
			// else start moving towa rd him.
			if (mCurrentFloor == aAtFloor)
			{
				//
				OpenDoors();
			}
			else
			{
				//
				SetElevatorAndMoveToDestination();
			}
		}
	}

	#endregion
	
	#region PRIVATE_METHODS

	void SetElevatorAndMoveToDestination()
	{
		//
		SetDestinationAndState();
		
		if (mElevatorRouter.CurrentState != EElevatorState.Idle)
		{
			//Nobody has to stop here so we can go back toward destination.
			MoveElevatorToDestination();
		}
	}

	void OnElevatorStop()
	{
		mIsElevatorMoving = false;
		mElevatorRouter.ArriveToNextFloor();
		mCurrentFloor = mElevatorRouter.CurrentFloor;

		//
		mElevatorRouter.UpdateCharactersTransformPosition();

		//If people needed to stop/get in here.
		if (mElevatorRouter.ShouldStopHere())
		{
			//
			OpenDoors();
		}
		else
		{
			//
			SetElevatorAndMoveToDestination();
		}
	}
	
	void OnElevatorTeleported()
	{
		mIsElevatorMoving = false;

		//
		if (mCurrentDestination != mCurrentFloor)
		{
			mFloorTeleportTimer.Start();
		}
		else
		{
			//Arrived at destination
			OpenDoors();
		}
	}
	
	void MoveElevatorToDestination()
	{
		mIsElevatorMoving = true;

		if (mIsTeleporting)
		{
			mIsTeleporting = false;
			
			//
			mFloorTeleportTimer.Start();
		}
		else if (!mIsDoorsOpen && !mOpenDoorsTimer.IsRunning)
		{
			//
			mFloorMovingTimer.Start();
		}
	}

	void SetDestinationAndState()
	{
		//
		mElevatorRouter.SetDestinationAndState();
		
		//
		SetElevatorsState(mElevatorRouter.CurrentState);
	}

	#endregion

	#region ELEVATORS_ANIMATIONS_MANAGEMENT
	
	void SetElevatorsState(EElevatorState aState)
	{
		//Tell each elevator to update its
		//visual with the current state.
		foreach(Elevator e in mElevators)
		{
			e.SetElevatorState(aState);
		}
	}
	
	void OpenDoors()
	{
		//
		mElevators[GetCurrentFloorIndexInArray()].OpenDoors();

		//
		mOpenDoorsTimer.Start();
	}
	
	void CloseDoors()
	{
		//
		mElevators[GetCurrentFloorIndexInArray()].CloseDoors();
		
		mIsWaitingForPeopleToGetIn = false;
		
		//
		mCloseDoorsTimer.Start();
	}
	
	#endregion
	
	#region TIMERS_MANAGEMENT
	
	void UpdateTimersDuration()
	{
		//Set new duration for timer if it has changed
		if (mFloorMovingTimer.Duration != mSpeed)
		{
			mFloorMovingTimer.SetDuration(mSpeed);
		}
		
		if (mWaitToCloseDoorsTimer.Duration != mWaitTime)
		{
			mWaitToCloseDoorsTimer.SetDuration(mWaitTime);
		}
		
		if (mCloseDoorsTimer.Duration != CLOSE_DOORS_TIME)
		{
			mCloseDoorsTimer.SetDuration(CLOSE_DOORS_TIME);
		}
		
		if (mOpenDoorsTimer.Duration != OPEN_DOORS_TIME)
		{
			mOpenDoorsTimer.SetDuration(OPEN_DOORS_TIME);
		}
	}
	
	#endregion

	#region TIMERS_HANDLER

	void OnDoorsClosed()
	{
		mIsDoorsOpen = false;

		//
		SetDestinationAndState();

		//Move Elevator
		if (mElevatorRouter.CurrentState != EElevatorState.Idle)
		{
			//
			MoveElevatorToDestination();
		}
		else if (mElevatorRouter.CurrentDestination == mCurrentFloor)
		{
			//
			OpenDoors();
		}
	}
	
	void OnDoorsOpen()
	{
		//
		mIsDoorsOpen = true;
		mIsWaitingForPeopleToGetIn = true;

		//
		mElevatorRouter.RemoveCharactersThatNeedToGetOut();

		//
		SetDestinationAndState();

		//Start waiting to close doors.
		mWaitToCloseDoorsTimer.Start();
	}

	#endregion
	#region DISPOSE
	
	public void Dispose()
	{
		//
		if (mElevatorRouter != null)
			mElevatorRouter.Dispose();

		if (mElevators != null)
			mElevators.Clear();

		if (mFloorMovingTimer != null)
			mFloorMovingTimer.Dispose();

		if (mWaitToCloseDoorsTimer != null)
			mWaitToCloseDoorsTimer.Dispose();

		if (mOpenDoorsTimer != null)
			mOpenDoorsTimer.Dispose();

		if (mCloseDoorsTimer != null)
			mCloseDoorsTimer.Dispose();

		if (mFloorTeleportTimer != null)
			mFloorTeleportTimer.Dispose();

		//
		mElevatorRouter					= null;
		mElevators						= null;
		mFloorMovingTimer				= null;
		mWaitToCloseDoorsTimer			= null;
		mOpenDoorsTimer					= null;
		mCloseDoorsTimer				= null;
		mFloorTeleportTimer				= null;
	}
	
	#endregion

	#region UTILS_METHODS

	int GetCurrentFloorIndexInArray()
	{
		return mCurrentFloor-(int)mElevators[0].GridPosition.y;
	}

	#endregion
	
	#region SERIALIZATION
	
	 protected ElevatorLine(SerializationInfo info, StreamingContext context)
	 {
		mElevators = 				new List<Elevator>();

		//
		mCurrentFloor = 			info.GetInt32("currentFloor");
		mCurrentDestination = 		info.GetInt32("currentDestination");

		//
		mFloorMovingTimer 			= info.GetValue("floorMovingTimer",typeof(Timer)) as Timer;
		mWaitToCloseDoorsTimer		= info.GetValue("waitTimer",typeof(Timer)) as Timer;
		mOpenDoorsTimer 			= info.GetValue("openDoorsTimer",typeof(Timer)) as Timer;
		mCloseDoorsTimer 			= info.GetValue("closeDoorsTimer",typeof(Timer)) as Timer;
		mFloorTeleportTimer			= info.GetValue("floorTeleportTimer",typeof(Timer)) as Timer;

		//
		mSpeed = 					(float)info.GetValue("speed",typeof(float));
		mWaitTime = 				(float)info.GetValue("waitTime",typeof(float));
		
		//
		mIsWaitingForPeopleToGetIn 	= info.GetBoolean("isWaitingForPeopleToGetIn");
		mIsDoorsOpen				= info.GetBoolean("isDoorsOpen");
		mIsTeleporting				= info.GetBoolean("isTeleporting");
		mIsElevatorMoving			= info.GetBoolean("elevatorMoving");

		//
		mElevatorRouter				= info.GetValue("elevatorMotor",typeof(ElevatorRouter)) as ElevatorRouter;
		
		//Initialize Rooms
		int minX					= info.GetInt32("rootX");
		int minY					= info.GetInt32("rootY");
		int maxY					= info.GetInt32("maxY");
		
		//
		for(int i = minY;i<=maxY;i++)
		{
			//
			Elevator e = RoomManager.Instance.GetRoom(minX,i) as Elevator;
			
			//
			if (mIsWaitingForPeopleToGetIn)
			{
				e.SetElevatorState(mElevatorRouter.IsGoingUp ? EElevatorState.Going_Up : EElevatorState.Going_Down);
			}
			else
			{
				e.SetElevatorState(mElevatorRouter.CurrentState);
			}
			
			//
			mElevators.Add(e);
		}
	 }

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//
		info.AddValue("floorMovingTimer",mFloorMovingTimer);
		info.AddValue("waitTimer",mWaitToCloseDoorsTimer);
		info.AddValue("waitTime",mWaitTime);
		info.AddValue("currentFloor", mCurrentFloor);
		info.AddValue("currentDestination", mCurrentDestination);
		info.AddValue("speed", mSpeed);
		info.AddValue("rootX",(int)mElevators[0].GridPosition.x);
		info.AddValue("rootY",(int)mElevators[0].GridPosition.y);
		info.AddValue("maxY",(int)mElevators[mElevators.Count-1].GridPosition.y);
		info.AddValue("isWaitingForPeopleToGetIn",mIsWaitingForPeopleToGetIn);
		info.AddValue("isDoorsOpen",mIsDoorsOpen);
		info.AddValue("openDoorsTimer",mOpenDoorsTimer);
		info.AddValue("closeDoorsTimer",mCloseDoorsTimer);
		info.AddValue("floorTeleportTimer",mFloorTeleportTimer);
		info.AddValue("isTeleporting",mIsTeleporting);
		info.AddValue("elevatorMotor",mElevatorRouter);
		info.AddValue("elevatorMoving",mIsElevatorMoving);
	}
	
	void IDeserializationCallback.OnDeserialization(System.Object sender)
	{
		UpdateTimersDuration();
	}
	
	#endregion
}
