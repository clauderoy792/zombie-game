using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorkingRoomSerializationInfo : RoomSerializationInfo {

	#region PUBLIC_MEMBERS

	public int mMaxNumberOfWorkers = 3;
	public int mQuantityToProduce;
	public float mProductivityCostNeeded;
	public float mCurrentProductivityProgress = 0;
	public bool mIsWorking;

	#endregion

	public WorkingRoomSerializationInfo(WorkingRoom aRoom) : base(aRoom)
	{
		mMaxNumberOfWorkers 			= aRoom.MaxNumberOfWorkers;
		mQuantityToProduce 				= aRoom.QuantityToProduce;
		mProductivityCostNeeded 		= aRoom.ProductivityCostNeeded;
		mCurrentProductivityProgress 	= aRoom.CurrentProductivityProgress;
		mIsWorking						= aRoom.IsWorking;
	}
}
