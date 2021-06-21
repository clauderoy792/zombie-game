using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class CraftingManager : MonoBehaviour
{
	#region CONSTANTS
	
	//
	public int BASE_NB_INGREDIENTS = 5;
	public int BASE_NB_INGREDIENTS_QUANTITY = 2;
	
	#endregion
	
	#region STATIC_MEMBERS
	
	static CraftingManager sInstance;
	
	#endregion
	
	#region PUBLIC_MEMBERS
	
	public TextAsset ingredientXML;
	
	#endregion

	List<Virus> mAllViruses;
	List<Ingredient> mAllIngredients;

	RandomNameGenerator mVirusNameGenerator;
	
	#region MONO METHODS
	
	//
	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			
			//
			mAllViruses = new List<Virus>();
			mAllIngredients = new List<Ingredient>();
		}

		//
		mVirusNameGenerator = new RandomNameGenerator();
		
		//
		InitData();
	}
	
	#endregion
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static CraftingManager Instance
	{
		get {return sInstance;}
	}
	
	public List<Virus> Viruses
	{
		get{return mAllViruses;}
	}
	
	public List<Ingredient> Ingredients
	{
		get{return mAllIngredients;}
	}

	public RandomNameGenerator VirusNameGenerator
	{
		get{return mVirusNameGenerator;}
	}
	
	#endregion
	
	#region PUBLIC METHODS

	
	/// <summary>
	/// Blends the ingredients.
	/// </summary>
	/// <returns>
	/// The ingredients.
	/// </returns>
	/// <param name='ing1'>
	/// Ing1.
	/// </param>
	/// <param name='ing2'>
	/// Ing2.
	/// </param>
	/// <param name='ing3'>
	/// Ing3.
	/// </param>
	public Virus CreateVirus(Ingredient ing1, Ingredient ing2, Ingredient ing3)
	{
		Virus returnValue = null;
	
		string id = Virus.GetID(ing1,ing2,ing3);

		returnValue = mAllViruses.Find(v => v.ID == id);
		
		return returnValue;
	}
	
	/// <summary>
	/// Finds a new ingredient and adds 1 of it to the list of ingredients.
	/// </summary>
	/// <returns>
	/// The new ingredient.
	/// </returns>
	public Ingredient FindNewIngredient()
	{
		Ingredient returnValue = null;
		List<int> checkedValue = new List<int>();
		int randomValue = 0;
		
		for(int i = 0;i< mAllIngredients.Count;i++)
		{
			//Find a valid value
			do
			{
				randomValue = Random.Range(0,mAllIngredients.Count);
			}
			while (checkedValue.Contains(randomValue));
			
			//If the ingredient has not been discovered
			if (!mAllIngredients[randomValue].HasBeenDiscovered)
			{
				returnValue = mAllIngredients[randomValue];
				returnValue.HasBeenDiscovered = true;

				//
				GameManager.Instance.UserStats.AddIngredient(mAllIngredients[randomValue].ID);

				break;
			}
			
			checkedValue.Add(randomValue);
		}
		
		
		return returnValue;
	}

	public Ingredient GetIngredientForId(int aId)
	{
		return mAllIngredients.Find(i => i.ID == aId);
	}

	public Virus GetVirusForId(string aId)
	{
		return mAllViruses.Find(i => i.ID == aId);
	}
	
	#endregion
	
	#region PRIVATE METHODS

	/// <summary>
	/// Inits the data.
	/// </summary>
	void InitData()
	{
		//
		mAllIngredients = IngredientXMLParser.Instance.GetIngredients(ingredientXML);

		//
		mAllViruses = new List<Virus>();

		//Create all viruses
		foreach(Ingredient i1 in mAllIngredients)
		{
			foreach(Ingredient i2 in mAllIngredients)
			{
				foreach(Ingredient i3 in mAllIngredients)
				{
					//TODO SET Seconds -CR
					mAllViruses.Add(new Virus(mVirusNameGenerator.GetUniqueRandomName(),i1,i2,i3,10));
				}
			}
		}
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public void AddBaseIngredients()
	{
		int i = 0;
		foreach(var ingredient in mAllIngredients)
		{
			if (i < BASE_NB_INGREDIENTS)
			{
				ingredient.HasBeenDiscovered = true;
				GameManager.Instance.UserStats.AddIngredient(ingredient.ID,BASE_NB_INGREDIENTS_QUANTITY);
			}
			else
			{
				break;
			}
			i++;
		}
	}
	
	#endregion

	#region SERIALIZATION
	
	public CraftingManagerSerializationInfo Serialize()
	{
		return new CraftingManagerSerializationInfo();
	}
	
	public void Deserialize(CraftingManagerSerializationInfo aInfo)
	{
		//
		mAllIngredients 			= aInfo.mAllIngredients;
		mAllViruses 				= aInfo.mAllViruses;
	}
	
	#endregion
}
