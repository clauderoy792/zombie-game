using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HCUtils;

[Serializable]
public class UserStats : ISerializable,IDeserializationCallback {

	#region CONSTANTS

	private const int NUMBER_OF_CONTRACT = 3;
	private const int NUMBER_OF_SECONDS_BEFORE_CONTRACT_REFRESH = 10;

	#endregion

	#region PRIVATE_MEMBERS
	
	private int mZombiesSold;
	private int mGold;
	private int mBrainz;
	private EContractDifficulty mContractDifficulty = EContractDifficulty.Easy;
	private Contract[] mContracts;
	private HCUtils.Timer mContractTimer;
	private Dictionary<EZombieType,int> mZombies;

	//Possessed ingredients with id as key and quantity as value.
	private Dictionary<int, int> 	mIngredients;
	
	//Possessed viruses with id as key and quantity as value.
	private Dictionary<string, int>		mViruses;

	#endregion
	
	#region PUBLIC_MEMBERS
	
	#endregion
	
	#region ACCESSORS
	
	public int ZombiesSold
	{
		get {return mZombiesSold;}
	}
	
	public int Gold
	{
		get {return mGold;}
	}
	
	public int Brainz
	{
		get{return mBrainz;}
	}

	public Contract[] Contracts
	{
		get{return mContracts;}
	}

	public HCUtils.Timer ContractTimer
	{
		get{return mContractTimer;}
	}

	public Dictionary<EZombieType,int> Zombies
	{
		get{return mZombies;}
	}

	public Dictionary<string,int> Viruses
	{
		get{return mViruses;}
	}

	public Dictionary<int,int> Ingredients
	{
		get{return mIngredients;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public UserStats()
	{
		//
		mGold = Economy.BASE_MONEY;

		//
		mContracts = new Contract[NUMBER_OF_CONTRACT];

		//
		RefreshContract();

		//
		mContractTimer = new HCUtils.Timer(NUMBER_OF_SECONDS_BEFORE_CONTRACT_REFRESH, RefreshContract, true);

		//
		mIngredients = new Dictionary<int, int>();
		mViruses = new Dictionary<string, int>();

		//
		mZombies = new Dictionary<EZombieType, int>();

		//Add temporary zombies
		for(int i = 0;i< (int)EZombieType.COUNT;i++)
		{
			mZombies.Add((EZombieType)i,50);
		}

		//Add temporary ingredients
		AddIngredient(0,3);

		//Add temporary virus.
		AddVirus(CraftingManager.Instance.Viruses[0].ID,false,3);
	}
	
	#endregion
	
	#region ZOMBIES_MANAGEMENT
	
	public void AddZombiesSold(int aZombies)
	{
		if (aZombies < 0)
		{
			aZombies = 0;
		}
		
		mZombiesSold += aZombies;
	}
	
	#endregion
	
	#region MONEY MANAGEMENT
	
	public void AddMoney(ECurrency aCurrency,int aAmount)
	{
		if (aAmount < 0)
		{
			aAmount = 0;
		}
		
		//
		if (aCurrency == ECurrency.Gold)
		{
			mGold += aAmount;
		}
		else
		{
			mBrainz += aAmount;
		}
	}
	
	public void RemoveMoney(ECurrency aCurrency,int aAmount)
	{
		//
		if (aAmount < 0)
		{
			aAmount = 0;
		}
		
		//
		if (aCurrency == ECurrency.Gold)
		{
			mGold -= aAmount;
		}
		else
		{
			mBrainz -= aAmount;
		}
	}
	
	public bool CanBuy(ECurrency aCurrency,int aAmount)
	{
		//
		bool returnValue;
		
		//
		if (aCurrency == ECurrency.Gold)
		{
			returnValue = mGold >= aAmount;
		}
		else
		{
			returnValue = mBrainz >= aAmount;
		}
		
		return returnValue;
	}
	
	#endregion

	#region CONTRACT_MANAGEMENT
	private void RefreshContract()
	{
		for(int i = 0; i < mContracts.Length; i++)
		{
			if(mContracts[i] == null || (!mContracts[i].Keep && mContracts[i].Status != EContractStatus.ACCEPTED))
			{
				Contract c = ContractGenerator.Instance.GetRandomContract(mContractDifficulty);
				mContracts[i] = c;
			}
		}
	}

	public void CompleteContract(int aContractPos)
	{
		if (aContractPos < mContracts.Length && aContractPos >= 0)
		{
			//
			mContracts[aContractPos].CompleteContract();

			//Remove zombies from inventory
			foreach(var pair in mContracts[aContractPos].ZombiesNeeded)
			{
				mZombies[pair.Key] -= pair.Value;
			}

			//
			mContracts[aContractPos] = null;
		}
		else
		{
			Debug.LogError("Cannot complete contract, invalid index : "+aContractPos);
		}
	}

	public void RemoveContract(int aContractPos)
	{
		if (aContractPos < mContracts.Length && aContractPos >= 0)
		{
			//
			mContracts[aContractPos] = null;
		}
		else
		{
			Debug.LogError("Cannot remove contract, invalid index : "+aContractPos);
		}
	}

	public void FailContract(Contract aContract)
	{
		for(int i = 0;i <mContracts.Length;i++)
		{
			if (mContracts[i] == aContract)
			{
				GameManager.Instance.UserStats.RemoveMoney(ECurrency.Gold, aContract.Penality);
				mContracts[i] = null;
			}
		}
	}

	#endregion

	#region VIRUS_AND_INGREDIENTS

	public void AddVirus(string aId,bool aRemoveIngredients = true,int aQuantity = 1)
	{
		//
		if (!mViruses.ContainsKey(aId))
		{
			mViruses.Add(aId,0);
		}

		//
		mViruses[aId]+=aQuantity;

		//Remove Ingredients from inventory
		if (aRemoveIngredients)
		{
			foreach(var ing in CraftingManager.Instance.GetVirusForId(aId).Ingredients)
			{
				//
				RemoveIngredient(ing);
			}
		}
	}

	public void AddIngredient(int aId,int aQuantity = 1)
	{
		//
		if (!mIngredients.ContainsKey(aId))
		{
			mIngredients.Add(aId,0);
		}
		
		//
		mIngredients[aId] += aQuantity;
	}

	public bool RemoveVirus(string aId,int aQuantity = 1)
	{
		bool returnValue = false;

		//
		if (mViruses.ContainsKey(aId) && mViruses[aId] > 0)
		{
			mViruses[aId]-= aQuantity;
			returnValue = true;
		}

		return returnValue;
	}

	public bool RemoveIngredient(int aId,int aQuantity = 1)
	{
		bool returnValue = false;
		
		//
		if (mIngredients.ContainsKey(aId) && mIngredients[aId] > 0)
		{
			mIngredients[aId]-= aQuantity;
			returnValue = true;
		}
		
		return returnValue;
	}

	public bool CanProduce(Virus aVirus,int aQuantity = 1)
	{
		int[] ingredients = aVirus.Ingredients;
		bool returnValue = true;

		for(int i = 0;i < ingredients.Length;i++)
		{
			int nbAppearance = 0;

			//If we have ingredients in posession
			if (mIngredients.ContainsKey(ingredients[i]) && mIngredients[ingredients[i]] > 0)
			{
				//Count nb times that ingredient appears.
				for(int j = 0;j < ingredients.Length;j++)
				{
					if (ingredients[j] == ingredients[i])
					{
						nbAppearance++;
					}
				}

				//If we don't have enough ingredient
				if (mIngredients[ingredients[i]]*aQuantity < nbAppearance)
				{
					returnValue = false;
					break;
				}
			}
			else
			{
				returnValue = false;
				break;
			}
		}

		return returnValue;
	}

	#endregion

	#region SERIALIZATION
	
	protected UserStats(SerializationInfo info, StreamingContext context)
	{
		//
		mZombiesSold 			= info.GetInt32("zombieSold");
		mGold 					= info.GetInt32("gold");
		mBrainz 				= info.GetInt32("brainz");
		mContractDifficulty 	= (EContractDifficulty)info.GetInt32("difficulty");
		mContracts 				= (Contract[])info.GetValue("contracts",typeof(Contract[]));
		mContractTimer			= info.GetValue("timer",typeof(Timer)) as Timer;

		//Dictionaries
		mZombies				= ((SerializableDictionary<EZombieType,int>)info.GetValue("zombies",typeof(SerializableDictionary<EZombieType,int>))).ToDictionary();
		mViruses				= ((SerializableDictionary<string,int>)info.GetValue("viruses",typeof(SerializableDictionary<string,int>))).ToDictionary();
		mIngredients			= ((SerializableDictionary<int,int>)info.GetValue("ingredients",typeof(SerializableDictionary<int,int>))).ToDictionary();
	}
	
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//
		info.AddValue("zombieSold", mZombiesSold);
		info.AddValue("gold", mGold);
		info.AddValue("brainz", mBrainz);
		info.AddValue("difficulty", (int)mContractDifficulty);
		info.AddValue("contracts",mContracts);
		info.AddValue("timer", mContractTimer);
		info.AddValue("zombies", new SerializableDictionary<EZombieType,int>(mZombies));
		info.AddValue("viruses", new SerializableDictionary<string,int>(mViruses));
		info.AddValue("ingredients", new SerializableDictionary<int,int>(mIngredients));
	}

	//Called after deserialization finished
	void IDeserializationCallback.OnDeserialization(System.Object sender)
	{
		//Restart contract timer if it was stopped
		if (!mContractTimer.IsRunning)
		{
			mContractTimer.Start();
		}
	}
	
	#endregion
}
