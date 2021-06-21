using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Stats {

	int mBaseStats;
	int mStatsGained;

	#region ACCESSORS

	public int BaseStats
	{
		get{return mBaseStats;}
	}

	public int StatsGained
	{
		get{return mStatsGained;}
	}

	#endregion

	public Stats(int aBaseStats)
	{
		mBaseStats = Mathf.Clamp(aBaseStats,0,HumanStats.HUMAN_STATS_MAXIMUM);
		mStatsGained = 0;
	}

	public void IncrementStats(int aIncrement = 1)
	{
		if (GetTotal() < HumanStats.HUMAN_STATS_MAXIMUM)
		{
			if (aIncrement + mStatsGained + mBaseStats <= HumanStats.HUMAN_STATS_MAXIMUM)
			{
				mStatsGained += aIncrement;
			}
			else
			{
				//We set the maximum gained stats.
				mStatsGained = HumanStats.HUMAN_STATS_MAXIMUM - mBaseStats;
			}
		}
	}

	public int GetTotal()
	{
		return mBaseStats +mStatsGained;
	}

	#region OVERRIDEN_METHODS

	public override string ToString ()
	{
		return GetTotal().ToString();
	}

	#endregion
}
