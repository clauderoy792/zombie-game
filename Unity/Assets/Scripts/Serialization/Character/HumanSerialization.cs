using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class HumanSerializationInfo : CharacterSerializationInfo {
	
	#region MEMBERS
	
	public HumanStats mStats;
	public JobStatsSerializationInfo mJobStats;
	public EGender mGender;
	public ERace mRace;
	public float mWorkstationX;
	public float mWorkstationY;
	public CharacterVisualSerializationInfo mVisual;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public HumanSerializationInfo(Human aHuman) : base(aHuman)
	{
		//
		mStats = aHuman.Stats;
		
		//
		mJobStats = new JobStatsSerializationInfo(aHuman.JobStats);

		//
		mVisual = aHuman.Visual.Serialize();

		//
		mGender = aHuman.Gender;
		mRace = aHuman.Race;
	}
	
	#endregion
}
