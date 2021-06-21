using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ZombieStats 
{
	#region CONSTANTS
	
	private const int STATS_MINIMUM = 0;
	private const int STATS_MAXIMUM = 10;
	
	#endregion
	
	#region PRIVATE_MEMBERS

	//
	private EZombieType mType;
	
	private int 	mIntellect = 0;
	private int 	mRage = 0;
	private int 	mInfectivity = 0;
	private int 	mStench = 0;
	
	
	#endregion
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets or sets the intellect.
	/// </summary>
	/// <value>
	/// The intellect.
	/// </value>
	public int Intellect
	{
		get{return mIntellect;}
	}
	
	/// <summary>
	/// Gets or sets the rage.
	/// </summary>
	/// <value>
	/// The rage.
	/// </value>
	public int Rage
	{
		get{return mRage;}
	}
	
	/// <summary>
	/// Gets or sets the infectivity.
	/// </summary>
	/// <value>
	/// The infectivity.
	/// </value>
	public int Infectivity
	{
		get{return mInfectivity;}
	}
	
	/// <summary>
	/// Gets or sets the stench.
	/// </summary>
	/// <value>
	/// The stench.
	/// </value>
	public int Stench
	{
		get{return mStench;}
	}
	
	//
	public EZombieType Type
	{
		get {return mType;}
		set {mType = value;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public ZombieStats(int aIntellect,int aRage,int aInfectivity,int aStench)
	{
		//
		mIntellect 		= Mathf.Clamp(aIntellect,STATS_MINIMUM,STATS_MAXIMUM);
		mRage 			= Mathf.Clamp(aRage,STATS_MINIMUM,STATS_MAXIMUM);
		mInfectivity 	= Mathf.Clamp(aInfectivity,STATS_MINIMUM,STATS_MAXIMUM);
		mStench 		= Mathf.Clamp(aStench,STATS_MINIMUM,STATS_MAXIMUM);

		mType = GetZombieType();
	}
	
	#endregion

	#region PRIVATE_METHODS

	private EZombieType GetZombieType()
	{
		EZombieType returnValue;
		float divider = 120;
		float total = 0;
		
		//
		total += Rage;
		total += Stench;
		total += Intellect;
		total += Infectivity;

		//
		returnValue = (EZombieType)(Mathf.FloorToInt(total/divider));
		
		//if value was 1200 and was floor to 10 (EZombieType.Count)
		if (returnValue == EZombieType.COUNT)
		{
			returnValue = EZombieType.Executor;
		}
		
		return returnValue;
	}

	#endregion

	#region OVERRIDEN_METHODS
	
	public override bool Equals (object obj)
	{
		if(!(obj is ZombieStats))
		{
			return false;
		}
		
		ZombieStats zombieObj = obj as ZombieStats;
		
		if(	this.Intellect == zombieObj.Intellect &&
			this.Infectivity == zombieObj.Infectivity &&
			this.Stench == zombieObj.Stench && 
			this.Rage == zombieObj.Rage)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public override int GetHashCode ()
	{
		return mIntellect.GetHashCode() + mInfectivity.GetHashCode()*2 - mStench.GetHashCode()*3+mRage.GetHashCode()*4;
	}
	
	#endregion
}
