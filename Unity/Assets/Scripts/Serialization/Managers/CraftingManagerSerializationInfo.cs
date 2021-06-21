using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CraftingManagerSerializationInfo {

	#region PUBLIC_MEMBERS
	
	public List<Ingredient> mAllIngredients;
	public List<Virus> mAllViruses;
	public RandomNameGenerator mRandomNameGenerator;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public CraftingManagerSerializationInfo()
	{
		//
		mAllIngredients 			= CraftingManager.Instance.Ingredients;
		mAllViruses					= CraftingManager.Instance.Viruses;
		mRandomNameGenerator		= CraftingManager.Instance.VirusNameGenerator;
	}
	
	#endregion
}
