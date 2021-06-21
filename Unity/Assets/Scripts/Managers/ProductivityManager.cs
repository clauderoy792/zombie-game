using UnityEngine;
using System.Collections;

public static class ProductivityManager {

	const int STATS_FOR_LEVEL = 10;

	public static float GetProductionCostForSeconds(float aSeconds)
	{
		float returnValue = 0;

		returnValue = aSeconds * STATS_FOR_LEVEL;

		return returnValue;
	}
}
