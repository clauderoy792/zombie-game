using UnityEngine;
using System.Collections;

public class JobStats 
{
	#region PRIVATE_MEMBERS
	//TODO Change work station to WorkingRoom
	WorkingRoom mWorkStation;
	ECharacterType mJobType;
	
	#endregion
	
	#region ACCESSORS
	
	public WorkingRoom WorkStation
	{
		get{return mWorkStation;}
		set{mWorkStation = value;}
	}
	
	public ECharacterType Job
	{
		get{return mJobType;}
		set{mJobType = value;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public JobStats()
	{
		
	}
	
	public JobStats(JobStatsSerializationInfo aStats)
	{
		//
		mJobType = aStats.mJobType;
		
		//
		if (aStats.mWorkstationX != -1 && aStats.mWorkstationY != -1)
		{
			mWorkStation = RoomManager.Instance.GetRoom(aStats.mWorkstationX,aStats.mWorkstationY) as WorkingRoom;
		}
	}
	
	#endregion
	
}
