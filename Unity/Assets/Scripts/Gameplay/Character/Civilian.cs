using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HCUtils;

public class Civilian : Human {
	
	#region CONSTANTS
	
	private const float TIME_BEFORE_GOING_HOME_RANDOM = 10;
	private const int GOING_HOME_RANDOM_CHANCE = 40;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private Reception mReception;
	private bool mIsWaitingAtReception;
	private bool mInitiallyMovedTowardReception = false;
	private Vector2 mWaitingPosition;
	private RandomTimer mGoingHomeTimer;
	
	#endregion
	
	#region ACCESSORS
	
	public bool InitiallyMovedTowardReception
	{
		get{return mInitiallyMovedTowardReception;}
	}
	
	public bool IsWaitingAtReception
	{
		get{return mIsWaitingAtReception;}
	}
	
	public Vector2 WaitingPosition
	{
		get{return mWaitingPosition;}
		set{mWaitingPosition = value;}
	}
	
	public RandomTimer GoingHomeTimer
	{
		get{return mGoingHomeTimer;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();
		
		mDropJunkChance = 0;
		
		mOnEndMove += OnEndMove;
		
		//
		List<Room> receptions = RoomManager.Instance.GetSameTypeRoom(ERoomType.Reception);
		
		//
		if (receptions.Count > 0)
		{
			mReception = receptions[0] as Reception;
		}
	}
	
	public override void OnDestroy ()
	{
		base.OnDestroy ();

		if ( mGoingHomeTimer != null)
		{
			mGoingHomeTimer.Stop();
			mGoingHomeTimer.Dispose();
		}

		mOnEndMove -= OnEndMove;
	}
	
	protected override void Update()
	{
		base.Update();

		if (!mInitiallyMovedTowardReception && mCurrentRoom == mReception)
		{
			//
			mInitiallyMovedTowardReception = true;
			mWaitingPosition = mReception.GetNextWaitingPosition();
			MoveToLocalPoint(mWaitingPosition);
			mReception.AddWaitingCivilian(this);
		}
	}
	
	#endregion
	
	#region PUBLIC_METHODS

	public void InitializeCharacterWithoutMoving()
	{
		//Start at a random time in idle anim.
		//TODO Set random time in anim. -CR
		mInitiallyMovedTowardReception = true;
		mIsWaitingAtReception = true;
		mWaitingPosition = mReception.GetNextWaitingPosition();
		LocalTransformPosition = new Vector3(mWaitingPosition.x,mWaitingPosition.y,mTransform.localPosition.z);
		SetCharacterRoomAndGridPosition();
		mReception.AddWaitingCivilian(this);

		//Breed at at random pace
		float random = Random.Range(0f,1f);
		mController.Play("Idle",0,random);

		OnEndMove();

		//Prevent the character from leaving.
		SetCanLeave(false);
	}
	
	public void SetCanLeave(bool aValue)
	{
		if (aValue)
		{
			mGoingHomeTimer.Start();
		}
		else
		{
			mGoingHomeTimer.Stop();
		}
	}
	
	public void MoveTowardReception()
	{
		if (mReception != null && !mIsWaitingAtReception)
		{
			mWaitingPosition = new Vector2(mReception.LocalTransformPosition.x+Room.UNIT_CELL_WIDTH/2,mReception.LocalTransformPosition.y);
			MoveToLocalPoint(mWaitingPosition);	
		}
		else
		{
			Debug.LogError("Could not find reception for Civilian");
		}
	}

	/// <summary>
	/// Removes from reception directly. Dont call this if you want your character to go back home.
	/// </summary>
	public void RemoveFromReception()
	{
		mReception.RemoveWaitingCivilian(this);
		mReception.RemoveGoingHomeCharacter(this);
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public override void Initialize ()
	{
		base.Initialize();

		mGoingHomeTimer = new RandomTimer(this,TIME_BEFORE_GOING_HOME_RANDOM,OnGoingHome,GOING_HOME_RANDOM_CHANCE,true);
	}
	
	#endregion
	
	#region PRIVATE_METHODS
	
	public void GoHome(bool aRemoveFromReception = true)
	{
		mGoingHomeTimer.Stop();
		mIsWaitingAtReception = false;
		float xLeft = Mathf.Clamp(BuildingManager.Instance.LeftBorder - Room.UNIT_CELL_WIDTH, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		float xRight = Mathf.Clamp(BuildingManager.Instance.RightBorder + Room.UNIT_CELL_WIDTH, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		mLeftHomePosition = new Vector2( xLeft, 0 );
		mRightHomePosition = new Vector2( xRight, 0 );
		
		if(aRemoveFromReception)
		{
			//
			mReception.RemoveWaitingCivilian(this);
		}
		
		//
		MoveToLocalPoint((GridPosition.x <= PathFinder.GRID_WIDTH) ? new Vector2(mLeftHomePosition.x, mLeftHomePosition.y - Room.UNIT_CELL_HEIGHT) : new Vector2(mRightHomePosition.x, mRightHomePosition.y - Room.UNIT_CELL_HEIGHT));
		mOnEndMove += DeleteCharacter;
	}
	
	protected override void DeleteCharacter()
	{
		//TODO Quick fix for Character's OnEndMove called when whe stop the current movement
		if (mTransform.localPosition.x == mLeftHomePosition.x && mTransform.localPosition.y +Room.UNIT_CELL_HEIGHT== mLeftHomePosition.y)
		{
			mOnEndMove -= DeleteCharacter;
			
			Destroy(mGameObject);
			CharacterManager.Instance.RemoveCharacter(this);
			mReception.RemoveGoingHomeCharacter(this);
		}
	}

	#endregion
	
	#region EVENT
	
	void OnGoingHome()
	{
		GoHome();
	}
	
	public void OnEndMove()
	{
		mOnEndMove -= OnEndMove;

		//Face left
		if (mTransform.localScale.x != 1)
		{
			mIsMoving = false;
			mIsWaitingAtReception = true;
			FaceLeft();
		}
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public override CharacterSerializationInfo Serialize ()
	{
		return new CivilianSerializationInfo(this);
	}
	
	public override void Deserialize (CharacterSerializationInfo aInfo)
	{
		base.Deserialize (aInfo);
		
		CivilianSerializationInfo info = aInfo as CivilianSerializationInfo;
		
		mInitiallyMovedTowardReception = info.mInitiallyMovedTowardReception;
		
		mGoingHomeTimer = info.mGoingHomeTimer;
		
		mIsWaitingAtReception = info.mIsWaitingInReception;
		
		mWaitingPosition  = new Vector2(info.mWaitingPosition.x,info.mWaitingPosition.y);
		
		if (!mIsWaitingAtReception)
		{
			MoveToLocalPoint(mWaitingPosition);
		}
	}

	
	#endregion
}
