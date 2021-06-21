using UnityEngine;
using System.Collections;

public class VirusLab: WorkingRoom {

	#region PUBLIC_MEMBERS

	public SpriteRenderer tank;

	#endregion

	#region PRIVATE_MEMBERS

	Virus				mCurrentVirus					= null;

	#endregion

	#region ACCESSORS

	public Virus CurrentVirus
	{
		get{return mCurrentVirus;}
		set
		{
			if (mCurrentVirus != value)
			{
				//Reset current progress.
				mCurrentProductivityProgress = 0;
			}
			
			mCurrentVirus = value;
		}
	}

	#endregion

	#region TANK_MANAGEMENT

	private void SetTankColor(EZombieType aZombieType)
	{
		Color color = Color.white;
		switch(aZombieType)
		{
			case EZombieType.Skinny : color = new Color(1f,0.9803922f,0.3882353f,1f); break;
			case EZombieType.WellFed : color = new Color(0.5372549f,1f,0.3882353f,1f); break;
			case EZombieType.Killer : color = new Color(0.3882353f,1f,0.4705882f,1f); break;
			case EZombieType.Monster : color = new Color(0.3882353f,0.8745098f,1f,1f); break;
			case EZombieType.Chaotic : color = new Color(0.5137255f,0.5176471f,1f,1f); break;
			case EZombieType.Butcher : color = new Color(0.772549f,0.5137255f,1f,1f); break;
			case EZombieType.Badass : color = new Color(1f,0.5137255f,0.945098f,1f); break;
			case EZombieType.BrainMaster : color = new Color(1f,0.3529412f,0.3529412f,1f); break;
			case EZombieType.Beast : color = new Color(1f,0.6627451f,0.3529412f,1f); break;
			case EZombieType.Executor : color = new Color(0.345098f,0.345098f,0.345098f,1f); break;
		}

		tank.color = color;
	}

	#endregion

	#region WORK_MANAGEMENT

	/// <summary>
	/// Sets the current ingredient and quantity. It will also set the time that the production will take.
	/// </summary>
	/// <param name="aIngredient">A ingredient.</param>
	/// <param name="aQuantity">A quantity.</param>
	public void SetCurrentVirusAndQuantity(Virus aVirus, int aQuantity = 1)
	{
		//
		mCurrentVirus = aVirus;
		mQuantityToProduce = aQuantity;
	}

	public override void StartWork ()
	{
		if (mCurrentVirus != null && GameManager.Instance.UserStats.CanProduce(mCurrentVirus,mQuantityToProduce))
		{
			SetTankColor(mCurrentVirus.ZombieType);
			SetProductivityNeeded(mCurrentVirus.GetProductCost()*mQuantityToProduce);
			mProgressBar.Show();
			mIsWorking = true;
		}
	}
	
	public override void StopWork ()
	{
		mIsWorking = false;
	}

	#endregion
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new VirusLabSerializationInfoSerializationInfo(this);
	}
	
	#endregion

	#region PRODUCTION_MANAGEMENT

	protected override void CompleteTask ()
	{
		//
		base.CompleteTask();

		if (mCurrentVirus != null)
		{
			GameManager.Instance.UserStats.AddVirus(mCurrentVirus.ID,true,mQuantityToProduce);
			mCurrentVirus.SetIngredientsUsed();

			//Reset quantity
			mQuantityToProduce = 1;
			//
			StopWork();
		}
	}

	#endregion
}
