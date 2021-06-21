using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ingredient : IProductable
{
	#region CONSTANTS
	
	private const int STATS_MINIMUM = 0;
	private const int STATS_MAXIMUM = 100;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	//
	private string mName = "";
	
	//
	private bool mHasBeenDiscovered = false;
	private bool mHasBeenUsed = false;
	
	//
	private int mId = -1;
	private int mIntellect = 0;
	private int mRage = 0;
	private int mInfectivity = 0;
	private int mStench = 0;

	//
	private float mProductionCost;

	#endregion
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	/// 
	/// 
	public string Name
	{
		get{return mName;}
	}
	
	public int ID
	{
		get{return mId;}
	}

	public bool HasBeenDiscovered
	{
		get{return mHasBeenDiscovered;}
		set{mHasBeenDiscovered = value;}
	}
	
	public bool HasBeenUsed
	{
		get{return mHasBeenUsed;}
		set{mHasBeenUsed = value;}
	}
	
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

	#endregion
	
	#region CONSTRUCTORS
	
	public Ingredient(string aName,int aId,int aIntellect,int aRage,int aInfectivity,int aStench, float aProductionTime)
	{
		//
		mId = aId;

		//
		mName = aName;
		
		//
		mProductionCost = ProductivityManager.GetProductionCostForSeconds(aProductionTime);
		
		//
		mIntellect 		= Mathf.Clamp(aIntellect,STATS_MINIMUM,STATS_MAXIMUM);
		aRage 			= Mathf.Clamp(aRage,STATS_MINIMUM,STATS_MAXIMUM);
		aInfectivity 	= Mathf.Clamp(aInfectivity,STATS_MINIMUM,STATS_MAXIMUM);
		aStench 		= Mathf.Clamp(aStench,STATS_MINIMUM,STATS_MAXIMUM);
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public void SetStats(string aName,int aIntellect,int aRage,int aInfectivity,int aStench)
	{
		mName = aName;
		mIntellect = 	Mathf.Clamp(aIntellect,STATS_MINIMUM,STATS_MAXIMUM);
		mRage = 		Mathf.Clamp (aRage,STATS_MINIMUM,STATS_MAXIMUM);
		mInfectivity = 	Mathf.Clamp(aInfectivity,STATS_MINIMUM,STATS_MAXIMUM);
		mStench = 		Mathf.Clamp(aStench,STATS_MINIMUM,STATS_MAXIMUM);
	}
	
	public void SetStats(int aIntellect,int aRage,int aInfectivity,int aStench)
	{
	}
	
	#endregion

	#region INTERFACES_METHODS
	
	public float GetProductCost()
	{
		return mProductionCost;
	}
	
	#endregion
}
