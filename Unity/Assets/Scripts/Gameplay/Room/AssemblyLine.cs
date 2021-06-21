using UnityEngine;
using HCUtils;
using System.Collections;

public class AssemblyLine : WorkingRoom {
	
	//
	ZombieWarehouse 	mProductionRoom					= null;
	Virus				mCurrentVirus					= null;

	#region ACCESSORS
	
	public ZombieWarehouse ProductionRoom
	{
		get {return mProductionRoom;}
	}

	public Virus CurrentVirus
	{
		get{return mCurrentVirus;}
		set
		{
			if (mCurrentVirus != value)
			{
				mCurrentProductivityProgress = 0;
			}

			mCurrentVirus = value;
		}
	}

	#endregion
	
	#region MONO_METHODS

	protected override void Awake ()
	{
		base.Awake ();
	}
	
	protected override void Update ()
	{
		base.Update ();

		//
		FindProductionRoom();
	}
	
	#endregion

	#region CHARACTER_MANAGEMENT
	
	public override void AddCurrentlyWorkingHuman (Human aHuman)
	{
		base.AddCurrentlyWorkingHuman (aHuman);
		
		if (!mIsWorking)
		{
			//
			if (mProductionRoom == null)
			{
				mProductionRoom = RoomManager.Instance.FindProductionRoom();
			}
		}
	}
	
	#endregion
	
	#region PRODUCTION_MANAGEMENT
	
	protected override void CompleteTask ()
	{
		//
		base.CompleteTask();

		if (mHumansCurrentlyWorkingInRoom.Count > 0 && mProductionRoom != null && !mProductionRoom.IsRoomFull())
		{
			mProductionRoom.SpawnZombie();
			GameManager.Instance.UserStats.RemoveVirus(mCurrentVirus.ID);
		}
		
		//
		if (mProductionRoom != null && mProductionRoom.IsRoomFull())
		{
			mProductionRoom = null;
		}
	}

	void FindProductionRoom()
	{
		if (!mIsWorking && mProductionRoom == null)
		{
			mProductionRoom = RoomManager.Instance.FindProductionRoom();
			
			if (mProductionRoom != null && mCurrentVirus != null)
			{
				SetProductivityNeeded(mCurrentVirus.ZombieProductionCost);
				StartWork();
			}
		}
	}

	public override void StartWork ()
	{
		if (mProductionRoom != null && mCurrentVirus != null)
		{
			//TODO Dont hardcode zombie cost. -CR
			mProductivityCostNeeded = mCurrentVirus.ZombieProductionCost;
			Debug.Log("COST : "+mCurrentVirus.ZombieProductionCost);
			mProgressBar.Show();
			mIsWorking = true;
		}
	}

	public override void StopWork ()
	{
		mIsWorking = false;
	}
	
	#endregion

	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new AssemblyLineSerializationInfo(this);
	}
	
	public override void Deserialize (RoomSerializationInfo aInfo)
	{
		base.Deserialize (aInfo);
		
		AssemblyLineSerializationInfo info = aInfo as AssemblyLineSerializationInfo;
		
		mProductionRoom = RoomManager.Instance.GetRoom(info.mProducingRoomX,info.mProducingRoomY) as ZombieWarehouse;
	}
	
	#endregion
}
