using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContractManager : MonoBehaviour
{
	#region CONSTANTS

	#endregion

	#region STATIC_MEMBERS

	private static ContractManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	List<Contract> mContracts;

	#endregion

	#region ACCESSORS

	public static ContractManager Instance
	{
		get {return sInstance;}
	}

	public List<Contract> Contracts
	{
		get{return mContracts;}
		set{mContracts = value;}
	}

	#endregion

	#region MONO_METHODS

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;

			mContracts = new List<Contract>();
		}
		else
		{
			Debug.Log("There is already an instance of the ContractManager in the scene.");
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void UpdateContracts(SimpleDateTime aDateTime)
	{
		for(int i = 0;i <mContracts.Count;i++)
		{
			if (TimeManager.Instance.CurrentTime.IsGreaterOrEqual(mContracts[i].DeadLine))
			{
				mContracts[i].FailContract();
			}
		}
	}

	public void AddContract(Contract aContract)
	{
		if (aContract != null)
		{
			mContracts.Add(aContract);
		}
		else
		{
			Debug.LogError("Could not add null contract in ContractManager");
		}
	}

	public void RemoveContract(Contract aContract)
	{
		mContracts.Remove(aContract);
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion
}
