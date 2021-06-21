using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class HumanStats
{
	#region CONSTANTS
	
	private const int STATS_MINIMUM = 0;
	
	//Stats like Hungriness,toilet, etc
	private const int BASE_STATS_MAXIMUM = 100;
	
	//Stats like intellect,strenght, etc
	public const int HUMAN_STATS_MAXIMUM = 10;
	
	#endregion
	
	//
	private bool mIsAtHome = false;
	private bool mHasJob = false;
	
	//
	private float mHungriness = 100;
	private float mToilet = 100;
	private float mExhaustion = 100;
	
	//Job Stats
	private Stats mIntellect;
	private Stats mAwareness;
	private Stats mHandiness;

	#region ACCESSORS

	public bool IsAtHome
	{
		get {return mIsAtHome;}
		set {mIsAtHome = value;}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this instance has job.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance has job; otherwise, <c>false</c>.
	/// </value>
	public bool HasJob
	{
		get{return mHasJob;}
		set{mHasJob = value;}
	}
	
	/// <summary>
	/// Gets or sets the hungriness.
	/// </summary>
	/// <value>
	/// The hungriness.
	/// </value>
	public float Hungriness
	{
		get{return mHungriness;}
		set{mHungriness = Mathf.Clamp(value,0,BASE_STATS_MAXIMUM);}
	}
	
	/// <summary>
	/// Gets or sets the toilet.
	/// </summary>
	/// <value>
	/// The toilet.
	/// </value>
	public float Toilet
	{
		get{return mToilet;}
		set{mToilet = Mathf.Clamp(value,0,BASE_STATS_MAXIMUM);}
	}
	
	/// <summary>
	/// Gets or sets the exhaustion.
	/// </summary>
	/// <value>
	/// The exhaustion.
	/// </value>
	public float Exhaustion
	{
		get{return mExhaustion;}
		set{mExhaustion = Mathf.Clamp(value,0,BASE_STATS_MAXIMUM);}
	}
	
	/// <summary>
	/// Gets or sets the intellect.
	/// </summary>
	/// <value>
	/// The intellect.
	/// </value>
	public Stats Intellect
	{
		get{return mIntellect;}
	}
	
	/// <summary>
	/// Gets or sets the awareness.
	/// </summary>
	/// <value>
	/// The awareness.
	/// </value>
	public Stats Awareness
	{
		get{return mAwareness;}
	}
	
	/// <summary>
	/// Gets or sets the handiness.
	/// </summary>
	/// <value>
	/// The handiness.
	/// </value>
	public Stats Handiness
	{
		get{return mHandiness;}
	}

	#endregion

	#region CONSTRUCTORS

	public HumanStats(int aBaseIntellect,int aBaseAwareness,int aBaseHandiness)
	{
		mAwareness = new Stats(aBaseAwareness);
		mIntellect = new Stats(aBaseIntellect);
		mHandiness = new Stats(aBaseHandiness);
	}

	#endregion
}
