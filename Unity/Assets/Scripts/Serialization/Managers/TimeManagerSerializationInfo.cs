using UnityEngine;
using System.Collections;
using System;
using HCUtils;

[System.Serializable]
public class TimeManagerSerializationInfo {

	#region PUBLIC_MEMBERS
	
	public SimpleDateTime mCurrentTime;
	
	//
	public Timer mTimerCharactersPay;
	public Timer mGameUpdateTimer;
	
	//
	public RandomTimer mUpcomingCivilianTimer;
	
	//
	public float mCurrentSeconds;

	#endregion
	
	#region CONSTRUCTORS
	
	public TimeManagerSerializationInfo()
	{
		//Timers
		mTimerCharactersPay 	= TimeManager.Instance.TimerCharactersPay;
		mGameUpdateTimer		= TimeManager.Instance.GameUpdateTimer;
		
		//Random Timers
		mUpcomingCivilianTimer 	= TimeManager.Instance.UpcominCivilianTimer;
		
		//
		mCurrentTime 			= TimeManager.Instance.CurrentTime;
	}
	
	#endregion
}
