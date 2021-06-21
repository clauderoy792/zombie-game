using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class JobStatsSerializationInfo {
	
	#region PUBLIC_MEMBERS

	//
	public float mWorkstationX;
	public float mWorkstationY;
	
	//
	public ECharacterType mJobType;
	
	#endregion
	
	#region CONSTRUCTOR
	
	public JobStatsSerializationInfo(JobStats aStats)
	{
		//
		mJobType = aStats.Job;
		
		//
		if (aStats.WorkStation != null)
		{
			mWorkstationX = aStats.WorkStation.GridPosition.x;
			mWorkstationY = aStats.WorkStation.GridPosition.y;
		}
		else
		{
			mWorkstationX = -1;
			mWorkstationY = -1;
		}
	}
	
	#endregion
}
