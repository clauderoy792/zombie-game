using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Virus : IProductable{
	
	#region STATIC_METHODS
	
	public static string GetID(Ingredient i1,Ingredient i2,Ingredient i3)
	{
		return i1.ID+"."+i2.ID+"."+i3.ID;
	}
	
	#endregion
	
	#region PRIVATE_MEMERS
	
	//
	private string mId;
	private string mName;

	int[] mIngredients;
	
	//
	private EZombieType mZombieType;

	//
	private float mProductionCost;
	private float mZombieProductionCost;
	
	#endregion
	
	#region ACCESSORS
	
	public string ID
	{
		get{return mId;}
	}
	
	public string Name
	{
		get{return mName;}
	}

	public int[] Ingredients
	{
		get{return mIngredients;}
	}

	public EZombieType ZombieType
	{
		get{return mZombieType;}
	}

	public float ZombieProductionCost
	{
		get{return mZombieProductionCost;}
	}

	#endregion
	
	#region CONSTRUCTOR
	
	public Virus(string aName,Ingredient aIng1,Ingredient aIng2,Ingredient aIng3,float aProductionTime)
	{
		mName = aName;
		mId = GetID(aIng1,aIng2,aIng3);

		mProductionCost = ProductivityManager.GetProductionCostForSeconds(aProductionTime);
		mIngredients = new int[] {aIng1.ID,aIng2.ID,aIng3.ID};

		//
		mZombieType = CalculateZombieType(aIng1,aIng2,aIng3);
	}
	
	#endregion

	#region PUBLIC_METHODS

	public void SetIngredientsUsed()
	{
		foreach(var v in mIngredients)
		{
			Ingredient i = CraftingManager.Instance.GetIngredientForId(v);

			if (i != null)
			{
				i.HasBeenUsed = true;
			}
			else
			{
				Debug.LogError("Could not set ingredient");
			}
		}
	}

	#endregion

	#region PRIVATE_METHODS

	EZombieType CalculateZombieType(Ingredient aIng1,Ingredient aIng2,Ingredient aIng3)
	{
		EZombieType returnValue;
		float divider = 120;
		float total = 0;
		
		//
		total += aIng1.Rage;
		total += aIng1.Stench;
		total += aIng1.Intellect;
		total += aIng1.Infectivity;
		
		//
		total += aIng2.Rage;
		total += aIng2.Stench;
		total += aIng2.Intellect;
		total += aIng2.Infectivity;
		
		//
		total += aIng3.Rage;
		total += aIng3.Stench;
		total += aIng3.Intellect;
		total += aIng3.Infectivity;
		
		//
		returnValue = (EZombieType)(Mathf.FloorToInt(total/divider));
		
		//if value was 1200 and was floor to 10 (EZombieType.Count)
		if (returnValue == EZombieType.COUNT)
		{
			returnValue = EZombieType.Executor;
		}

		//TODO : Dont hardcode zombie production cost -CR
		mZombieProductionCost = ((int)returnValue * 3)+100;
		if (mProductionCost == 0)
		{
		Debug.LogError("cost:"+mZombieProductionCost);
		}

		return returnValue;
	}

	#endregion

	#region INTERFACES_METHODS

	public float GetProductCost()
	{
		return mProductionCost;
	}

	#endregion
}
