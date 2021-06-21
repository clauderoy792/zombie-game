using UnityEngine;
using System.Collections;

public class ResearchLab : WorkingRoom {
	
	#region CONSTANTS
	
	const int PERCENT_CHANCE_FIND_INGREDIENT = 10;
	const float INTERVAL_SECONDS_BETWEEN_RANDOM = 10;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private float mCurrentSearchTime = 0;
	private Ingredient mCurrentIngredient = null;
	private bool mIsSearchingForNewIngredient = false;

	#endregion
	
	#region ACCESSORS
	
	public float SearchTime
	{
		get{return mCurrentSearchTime;}
	}

	public bool IsSearchingForNewIngredient
	{
		get{return mIsSearchingForNewIngredient;}
		set
		{
			mIsSearchingForNewIngredient = value;

			//Reset current ingredient
			if (mIsSearchingForNewIngredient)
			{
				mCurrentIngredient = null;
			}
		}
	}

	public Ingredient CurrentIngredient
	{
		get{return mCurrentIngredient;}
		set
		{
			if (mCurrentIngredient != value)
			{
				//Reset progress.
				mCurrentProductivityProgress = 0;
			}

			mCurrentIngredient = value;

			//If we set a valid ingredient, stop searching for new one.
			if (mCurrentIngredient != null)
			{
				mIsSearchingForNewIngredient = false;
			}
		}
	}
	
	#endregion
	
	#region WORK

	/// <summary>
	/// Sets the current ingredient and quantity. It will also set the time that the production will take.
	/// </summary>
	/// <param name="aIngredient">A ingredient.</param>
	/// <param name="aQuantity">A quantity.</param>
	public void SetCurrentIngredientAndQuantity(Ingredient aIngredient, int aQuantity = 1)
	{
		//
		mCurrentIngredient = aIngredient;
		mQuantityToProduce = aQuantity;
	}

	public override void StartWork ()
	{
		//
		mIsWorking = true;
		mProgressBar.Show();

		if (mIsSearchingForNewIngredient)
		{
			//Dont hardcore search progress -CR.
			SetProductivityNeeded(100);
		}
		else if (mCurrentIngredient != null)
		{
			SetProductivityNeeded(mCurrentIngredient.GetProductCost());
		}
	}

	public override void StopWork ()
	{
		if (mIsSearchingForNewIngredient)
		{
			mProgressBar.Hide();
		}
		mIsWorking = false;
	}

	protected override void CompleteTask ()
	{
		//
		base.CompleteTask();

		if (mIsSearchingForNewIngredient)
		{
			Debug.Log("TRYING TO FIND AN INGREDIENT");
			if (Random.Range(0,100) <PERCENT_CHANCE_FIND_INGREDIENT)
			{
				Ingredient i = CraftingManager.Instance.FindNewIngredient();
				
				Debug.Log("FOUND INGREDIENT : "+ i.Name);
				//Found new ingredient.
				GameManager.Instance.UserStats.AddIngredient(i.ID);
			}
		}
		else
		{
			//Found known ingredient.
			GameManager.Instance.UserStats.AddIngredient(mCurrentIngredient.ID,mQuantityToProduce);
		}
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new ResearchLabSerializationInfo(this);
	}
	
	public override void Deserialize (RoomSerializationInfo aInfo)
	{
		base.Deserialize(aInfo);
		ResearchLabSerializationInfo info = aInfo as ResearchLabSerializationInfo;
		
		mCurrentSearchTime = info.mSearchTime;
	}
	
	#endregion
}
