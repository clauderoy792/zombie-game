using UnityEngine;
using HCUtils;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class WorkingRoom : Room {

	#region MONO_METHODS

	protected override void Awake ()
	{
		base.Awake ();
	}

	#endregion

	#region PROTECTED_MEMBERS
	
	protected int mMaxNumberOfWorkers = 3;
	protected int mQuantityToProduce;
	protected float mProductivityCostNeeded;
	protected float mCurrentProductivityProgress = 0;
	protected bool mIsWorking;

	//
	protected List<Human> mHumansWorkingInRoom;
	protected ReadOnlyCollection<Human> mHumansWorkingInRoomReadOnly;
	
	protected List<Human> mHumansCurrentlyWorkingInRoom;
	protected ReadOnlyCollection<Human> mHumansCurrentlyWorkingInRoomReadOnly;

	#endregion
	
	#region ABSTRACT_METHODS
	
	public abstract void StartWork();
	
	public abstract void StopWork();
	
	#endregion
	
	#region ACCESSORS
	
	public int MaxNumberOfWorkers
	{
		get {return mMaxNumberOfWorkers;}
	}

	public int QuantityToProduce
	{
		get{return mQuantityToProduce;}
		set{mQuantityToProduce = value;}
	}

	public float ProductivityCostNeeded
	{
		get{return mProductivityCostNeeded;}
	}

	public float CurrentProductivityProgress
	{
		get{return mCurrentProductivityProgress;}
	}
	
	public ReadOnlyCollection<Human> HumansWorkingInRoom
	{
		get {return mHumansWorkingInRoomReadOnly;}
	}
	
	public ReadOnlyCollection<Human> HumansCurrentlyWorkingInRoom
	{
		get {return mHumansCurrentlyWorkingInRoomReadOnly;}
	}

	public bool IsWorking
	{
		get{return mIsWorking;}
	}
	
	#endregion

	
	#region INITIALIZATION
	
	public override void Initialize (float aX, float aY, ERoomType aType)
	{
		base.Initialize (aX, aY, aType);
		
		//
		InitWorkingRooms();
	}
	
	void InitWorkingRooms()
	{
		//
		mHumansWorkingInRoom = new List<Human>();
		mHumansWorkingInRoomReadOnly = mHumansWorkingInRoom.AsReadOnly();
		
		//
		mHumansCurrentlyWorkingInRoom = new List<Human>();
		mHumansCurrentlyWorkingInRoomReadOnly = mHumansCurrentlyWorkingInRoom.AsReadOnly();	
	}
	
	#endregion
	
	#region OVERRIDEN_METHODS
	
	protected override void Update ()
	{
		base.Update ();

		//Add work progression.
		if (mIsWorking)
		{
			for(int i = 0;i <mHumansCurrentlyWorkingInRoom.Count;i++)
			{
				AddProgress(mHumansCurrentlyWorkingInRoom[i].GetProductivityStats()*Time.deltaTime);
			}
		}
	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();
	}
	
	#endregion
	
	#region CHARACTER_MANAGEMENT
	
	public virtual void AddWorkingHuman(Human aHuman)
	{
		if (!IsFullOfWorkers())
		{
			aHuman.JobStats.Job = mRoomType.GetJob();
			mHumansWorkingInRoom.Add(aHuman);
		}
		else
		{
			Debug.LogError("Cannot add worker in room, the room is already full");
		}
	}
	
	public virtual void AddCurrentlyWorkingHuman(Human aHuman)
	{
		mHumansCurrentlyWorkingInRoom.Add(aHuman);
	}
	
	public virtual void RemoveWorkingHuman(Human aHuman)
	{
		mHumansWorkingInRoom.Remove(aHuman);
	}
	
	public virtual void RemoveCurrentlyWorkingHuman(Human aHuman)
	{
		mHumansCurrentlyWorkingInRoom.Remove(aHuman);
	}
	
	#endregion
	
	#region WORK_MANAGEMENT

	protected float GetCostRebate(float aCost)
	{
		return (int)(aCost*0.1f);
	}

	/// <summary>
	/// Sets the productivity needed and process a rebates.
	/// </summary>
	/// <param name="aProductivity">A productivity.</param>
	public void SetProductivityNeeded(float aProductivity)
	{
		mProductivityCostNeeded = Mathf.Clamp(aProductivity - GetCostRebate(aProductivity),0,int.MaxValue);
	}

	public bool IsFullOfWorkers()
	{
		return mHumansWorkingInRoom.Count == mMaxNumberOfWorkers;
	}

	public float GetWorkProgression01()
	{
		return Mathf.Clamp01(mCurrentProductivityProgress/mProductivityCostNeeded);
	}

	protected virtual void CompleteTask()
	{
		//
		StopWork();

		//
		mProgressBar.Hide();
	}

	protected void AddProgress(float aStats)
	{
		mCurrentProductivityProgress += aStats;

		mProgressBar.Progress = GetWorkProgression01();

		if (mCurrentProductivityProgress >= mProductivityCostNeeded)
		{
			mCurrentProductivityProgress = 0;

			//
			CompleteTask();
		}
	}

	#endregion
	
	#region SERIALIZATION

	public override RoomSerializationInfo Serialize ()
	{
		return base.Serialize ();
	}
	
	public override void Deserialize(RoomSerializationInfo aInfo)
	{
		base.Deserialize(aInfo);

		WorkingRoomSerializationInfo info = aInfo as WorkingRoomSerializationInfo;

		//
		mMaxNumberOfWorkers 			= info.mMaxNumberOfWorkers;
		mQuantityToProduce 				= info.mQuantityToProduce;
		mProductivityCostNeeded 		= info.mProductivityCostNeeded;
		mCurrentProductivityProgress 	= info.mCurrentProductivityProgress;
		mIsWorking						= info.mIsWorking;

		//
		InitWorkingRooms();
	}
	
	#endregion
}
