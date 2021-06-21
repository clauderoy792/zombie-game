using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanStatsGenerator
{
	#region CONSTANTS

	#endregion

	#region STATIC_MEMBERS

	private static HumanStatsGenerator sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	/// <summary>
	/// The m probability table. Keys from this table can only be between 0 and 1.
	/// </summary>
	Dictionary<float,KeyValuePair<int,int>> mProbabilityTable;

	#endregion

	#region ACCESSORS

	public static HumanStatsGenerator Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new HumanStatsGenerator();
			}

			return sInstance;
		}
	}
	#endregion

	#region CONSTRUCTORS

	private HumanStatsGenerator ()
	{
		mProbabilityTable = new Dictionary<float, KeyValuePair<int, int>>();

		//TODO Get from internet -CR
		mProbabilityTable.Add(0.4f,new KeyValuePair<int, int>(1,3));
		mProbabilityTable.Add(0.7f,new KeyValuePair<int, int>(1,5));
		mProbabilityTable.Add(0.9f,new KeyValuePair<int, int>(2,6));
		mProbabilityTable.Add(1f,new KeyValuePair<int, int>(3,8));
	}

	#endregion

	#region PUBLIC_METHODS

	public HumanStats GenerateRandomStats()
	{
		HumanStats returnValue = null;

		int[] stats = new int[3];

		//Get all random stats needed
		for(int i =0;i<stats.Length;i++)
		{
			float rand = Random.Range(0f,1f);

			//Get corresponding entry in the probability table
			foreach(KeyValuePair<float,KeyValuePair<int,int>> pair in mProbabilityTable)
			{
				if (rand <= pair.Key)
				{
					stats[i] = Random.Range(pair.Value.Key,pair.Value.Value);
					break;
				}
			}
		}

		returnValue = new HumanStats(stats[0],stats[1],stats[2]);

		return returnValue;
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion
}
