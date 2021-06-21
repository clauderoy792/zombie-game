using UnityEngine;
using System.Collections;
using HCUtils;

public class Construction : Room {

	#region PRIVATE_MEMBERS

	Timer mProgressTimer = null;
	ERoomType mRoomToConstruct = ERoomType.None;

	//
	private float baseTime = 15;
	private float timeFactor = 5;
	#endregion

	#region ACCESSORS

	public Timer ProgressTimer
	{
		get{return mProgressTimer;}
	}

	public ERoomType RoomToConstruct
	{
		get{return mRoomToConstruct;}
	}

	#endregion

	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();

		mProgressTimer = new Timer(0,OnConstructionTimerEnd,false);
	}

	protected override void Update ()
	{
		base.Update ();

		if (mProgressTimer.IsRunning)
		{
			mProgressBar.Progress = mProgressTimer.GetPercentage01();
		}
	}

	protected override void OnDestroy ()
	{
		//
		mProgressTimer.Dispose();

		base.OnDestroy ();
	}

	#endregion
		
	#region PUBLIC_METHODS

	public void StartConstruction(ERoomType aType)
	{
		//
		mRoomToConstruct = aType;

		//
		mProgressTimer.SetDuration(baseTime+RoomManager.Instance.NbRoomCreated*timeFactor);
		mProgressTimer.Start();

		//
		mProgressBar.Show();
	}

	#endregion

	#region CONSTRUCTION_MANAGEMENT

	void OnConstructionTimerEnd()
	{
		//Add Room And destroy itself.
		mProgressTimer.Stop();

		//
		RoomManager.Instance.ReplaceRoom(this,mRoomToConstruct,true);
	}

	#endregion

	#region SERIALIZATION

	public override void Deserialize (RoomSerializationInfo aInfo)
	{
		base.Deserialize (aInfo);

		ConstructionSerializationInfo info = aInfo as ConstructionSerializationInfo;

		//
		mProgressTimer 			= info.mProgressTimer;
		mRoomToConstruct 		= info.mRoomToConstruct;
	}

	public override RoomSerializationInfo Serialize ()
	{
		return new ConstructionSerializationInfo(this);
	}
	
	#endregion
}
