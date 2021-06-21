using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContractGenerator
{
	#region CONSTANTS
	
	string[] mCompanyName = new string[]
	{
		"Stink Toes inc.",
		"Half Head Studio",
		"Happy Corpse inc.",
		"Mind Blown Corp.",
		"Sneeze inc.",
		"Fleshless inc.",
		"20th Undead Fox",
		"Subbrains",
		"Putrefy Entertainment",
		"Epic Skull",
		"Will It Bend Company",
		"Flying bodies inc.",
		"The Hell Company",
		"No More Heaven inc.",
		"I'm the BOSS inc.",
		"Brainless Corp.",
		"Lost Eyeball Corp.",
		"iFlesh",
		"T-Bone Company",
		"Left 4 Alive",
		"Microshot Corp.",
		"Mc BlowAll",
		"The Running Dead",
		"Rotten Teeth inc.",
	};
	
	Dictionary<EContractDifficulty, OrderChances> mOrderNumberChances;
	
	const int ZOMBIE_PRICE = 10;
	const float DURATION_FACTOR = 25;
	
	#endregion

	#region STATIC_MEMBERS

	private static ContractGenerator sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	#endregion

	#region ACCESSORS

	public static ContractGenerator Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new ContractGenerator();
			}

			return sInstance;
		}
	}
	#endregion

	#region CONSTRUCTORS

	private ContractGenerator ()
	{
		mOrderNumberChances = new Dictionary<EContractDifficulty, OrderChances>();
		mOrderNumberChances.Add(EContractDifficulty.Easy			, new OrderChances(100,0,0));
		mOrderNumberChances.Add(EContractDifficulty.Normal			, new OrderChances(100,20,0));
		mOrderNumberChances.Add(EContractDifficulty.Intermediate	, new OrderChances(100,60,0));
		mOrderNumberChances.Add(EContractDifficulty.Hard			, new OrderChances(100,100,50));
		mOrderNumberChances.Add(EContractDifficulty.VeryHard		, new OrderChances(100,100,100));
	}

	#endregion

	#region PUBLIC_METHODS

	public Contract GetRandomContract(EContractDifficulty aDifficulty)
	{
		Dictionary<EZombieType,int> zombiesNeeded = GetRandomZombieOrder(aDifficulty);

		return new Contract(mCompanyName[Random.Range(0, mCompanyName.Length)],
		                    zombiesNeeded,
		                    GetDuration(zombiesNeeded, aDifficulty),
		                    GetPayForContract( zombiesNeeded, aDifficulty ));
	}

	#endregion

	#region PRIVATE_METHODS
	
	private Dictionary<EZombieType, int> GetRandomZombieOrder(EContractDifficulty aDifficulty)
	{
		Dictionary<EZombieType, int> returnValue = new Dictionary<EZombieType, int>();
		int orderChance = Random.Range(0,100);
		int numberOfOrder = 0;
		
		if(orderChance < mOrderNumberChances[aDifficulty].order3Chance)
		{
			numberOfOrder = 3;
		}
		else if(orderChance < mOrderNumberChances[aDifficulty].order2Chance)
		{
			numberOfOrder = 2;
		}
		else
		{
			numberOfOrder = 1;
		}
		
		for(int i = 0; i < numberOfOrder; i++)
		{
			bool alreadyContained = true;
			EZombieType currentType;

			while(alreadyContained)
			{
				currentType = (EZombieType)Random.Range(0, (int)aDifficulty);	

				//
				if(!returnValue.ContainsKey(currentType))
				{
					alreadyContained = false;
					returnValue.Add(currentType, Random.Range(1*(int)aDifficulty, 2*(int)aDifficulty));
				}
			}
		}

		return returnValue;
	}
	
	//
	private int GetPayForContract(Dictionary<EZombieType, int> aZombies, EContractDifficulty aDifficulty)
	{
		int result = 0;
		int numberOfZombie = 0;
		
		foreach(var keyValue in aZombies)
		{
			int zombieStatsValue = (int)(keyValue.Key);
			numberOfZombie += keyValue.Value;
			result += (keyValue.Value * zombieStatsValue) * ZOMBIE_PRICE;
		}
		
		result += (numberOfZombie * (int)aDifficulty) * 100;
		
		return result;
	}
	
	/// <summary>
	/// Gets the duration in week.
	/// </summary>
	/// <returns>The duration.</returns>
	/// <param name="aZombies">A zombies.</param>
	/// <param name="aDifficulty">A difficulty.</param>
	private int GetDuration(Dictionary<EZombieType, int> aZombies, EContractDifficulty aDifficulty)
	{
		float factor = DURATION_FACTOR / (int)aDifficulty;
		
		int numberOfZombies = 0;
		foreach(var keyValue in aZombies)
		{
			numberOfZombies += keyValue.Value;
		}
	
		return (int)(numberOfZombies * factor);
	}

	
	#endregion
}

public struct OrderChances
{
	public int order1Chance;
	public int order2Chance;
	public int order3Chance;
	
	//
	public OrderChances(int aOrder1Chance, int aOrder2Chance, int aOrder3Chance)
	{
		this.order1Chance = aOrder1Chance;
		this.order2Chance = aOrder2Chance;
		this.order3Chance = aOrder3Chance;
	}
}