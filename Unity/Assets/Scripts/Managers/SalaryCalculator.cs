using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SalaryCalculator
{
	#region CONSTANTS

	const float PERCENT_COST_WHEN_STATS_GAINED = 0.7f;

	#endregion

	#region STATIC_MEMBERS

	private static SalaryCalculator sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	private Dictionary<ECharacterType, int>	mBaseSalaries;
	private float[] 						mSalaryMultiplicator;

	#endregion

	#region ACCESSORS

	public static SalaryCalculator Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new SalaryCalculator();
			}

			return sInstance;
		}
	}
	#endregion

	#region CONSTRUCTORS

	private SalaryCalculator ()
	{
		mBaseSalaries = new Dictionary<ECharacterType, int>();

		//Hardcode base salaries.
		//TODO Fetch from internet -CR.
		mBaseSalaries.Add(ECharacterType.Janitor			,2500);
		mBaseSalaries.Add(ECharacterType.Receptionist		,3500);
		mBaseSalaries.Add(ECharacterType.ResearchScientist	,10000);
		mBaseSalaries.Add(ECharacterType.SecurityGuard		,5000);
		mBaseSalaries.Add(ECharacterType.VirusScientist		,15000);
		mBaseSalaries.Add(ECharacterType.Worker				,5000);

		//Multiplicator to take to multiply salary with average.
		mSalaryMultiplicator = new float[]
		{
			1.2f,
			1.4f,
			1.7f,
			2.1f,
			2.4f,
			2.7f,
			2.9f,
			3.1f,
			3.3f,
			3.5f,
			4f
		};
	}

	#endregion

	#region PUBLIC_METHODS

	public int CalculateSalary(Human aHuman)
	{
		int returnValue = 0;
		KeyValuePair<int,int> stats = aHuman.GetProductivityStatsDivided();

		if (aHuman != null && aHuman.Type != ECharacterType.Civilian && mBaseSalaries.ContainsKey(aHuman.Type))
		{
			//
			returnValue =  stats.Key * mBaseSalaries[aHuman.Type] + (int)(stats.Value * mBaseSalaries[aHuman.Type]*PERCENT_COST_WHEN_STATS_GAINED);

			//Add percent cost for each stats so better character will be paid more
			returnValue += (int)(returnValue*mSalaryMultiplicator[(int)((stats.Key+stats.Key)/2f)]);
		}

		return returnValue;
	}

	#endregion
}
