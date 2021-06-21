using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HCUtils;

[Serializable]
public class Contract : ISerializable
{
	#region CONSTANTS

	private const float PENALITY_PERCENT = 0.1f;

	#endregion

	#region PRIVATE_MEMBERS

	private Dictionary<EZombieType, int> mZombiesNeeded;
	private SimpleDateTime mDeadline;
	private int mNbWeek;
	private int mPay;
	private bool mKeep = false;
	private string mCompanyName = "";
	private EContractStatus mStatus = EContractStatus.PENDING;

	#endregion

	#region CONSTRUCTORS

	//
	public Contract(string aCompanyName, Dictionary<EZombieType, int> aZombiesOrder, int aNbWeek, int aPay)
	{
		mCompanyName = aCompanyName;
		mZombiesNeeded = aZombiesOrder;
		mNbWeek = aNbWeek;
		mPay = aPay;
	}

	#endregion

	#region ACCESSORS

	//
	public string CompanyName
	{
		get{return mCompanyName;}
		set{mCompanyName = value;}
	}
	
	//
	public Dictionary<EZombieType, int> ZombiesNeeded
	{
		get{return mZombiesNeeded;}
		set{mZombiesNeeded = value;}
	}

	public int Penality
	{
		get{return (int)(PENALITY_PERCENT*mPay);}
	}

	//
	public int Pay
	{
		get{return mPay;}
		set{mPay = value;}
	}

	//
	public bool Keep
	{
		get{return mKeep;}
		set{mKeep = value;}
	}

	//
	public EContractStatus Status
	{
		get{return mStatus;}
	}

	//
	public SimpleDateTime DeadLine
	{
		get{return mDeadline;}
	}
	
	//
	public override string ToString ()
	{
		string mainInfo = string.Format ("CompanyName: {0}\nDuration: {1} weeks\nPay: {2}", CompanyName, mNbWeek, Pay);
		
		foreach(var keyvalue in ZombiesNeeded)
		{
			string output = string.Format("\n\nNumber of Zombies x{0}\nType: {1}", keyvalue.Value, keyvalue.Key);
			mainInfo += output;
		}
		
		mainInfo += "\n\n";
		
		return mainInfo;
	}

	#endregion

	#region PUBLIC_METHODS

	public void AcceptContract()
	{
		mStatus = EContractStatus.ACCEPTED;

		//
		mDeadline = new SimpleDateTime( TimeManager.Instance.CurrentTime);
		mDeadline.AddWeeks(mNbWeek);

		//
		ContractManager.Instance.AddContract(this);
	}

	public bool CompleteContract()
	{
		bool returnValue = false;

		if (CanCompleteContract())
		{
			//
			GameManager.Instance.UserStats.AddMoney(ECurrency.Gold,mPay);
			returnValue = true;
		}

		ContractManager.Instance.RemoveContract(this);

		return returnValue;
	}

	public void FailContract()
	{
		Debug.Log("Failed contract : \n"+ToString());
		mStatus = EContractStatus.FAILED;
		GameManager.Instance.UserStats.FailContract(this);
		ContractManager.Instance.RemoveContract(this);
	}

	public bool CanCompleteContract()
	{
		bool returnValue = true;

		foreach(var pair in mZombiesNeeded)
		{
			//If we have that zombie and that we have the good quantity.
			if (!GameManager.Instance.UserStats.Zombies.ContainsKey(pair.Key) || GameManager.Instance.UserStats.Zombies[pair.Key] < pair.Value)
			{
				returnValue = false;
				break;
			}
		}

		return returnValue;
	}

	#endregion

	#region SERIALIZATION
	
	protected Contract(SerializationInfo info, StreamingContext context)
	{
		//
		mCompanyName 			= info.GetString("companyName");
		mZombiesNeeded 			= ((SerializableDictionary<EZombieType,int>)info.GetValue("zombiesOrder",typeof(SerializableDictionary<EZombieType,int>))).ToDictionary();
		mPay 					= info.GetInt32("pay");
		mDeadline 				= (SimpleDateTime)info.GetValue("deadline",typeof(SimpleDateTime));
	}
	
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//
		info.AddValue("companyName", mCompanyName);
		info.AddValue("zombiesOrder", new SerializableDictionary<EZombieType,int>(mZombiesNeeded));
		info.AddValue("pay", mPay);
		info.AddValue("deadline",mDeadline);
	}
	
	#endregion
}
