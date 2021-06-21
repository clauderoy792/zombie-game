using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HCLocalization {

	#region STATIC_MEMBERS
	
	static HCLocalization sInstance;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	Dictionary<string,Dictionary<string,string>> mLanguagesKeyValues;
	ELanguage mCurrentLanguage;
	
	#endregion
	
	#region ACCESSORS
	
	public static HCLocalization Instance
	{
		get 
		{
			if (sInstance == null)
			{
				sInstance = new HCLocalization();
			}
			
			return sInstance;
		}
	}
	
	public ELanguage CurrentLanguage
	{
		get{return mCurrentLanguage;}
		set {mCurrentLanguage = value;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public HCLocalization()
	{
		mCurrentLanguage = (ELanguage)0;
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public string GetString(EString aString)
	{
		return mLanguagesKeyValues[mCurrentLanguage.ToString()][aString.ToString()];
	}
	
	public void InitializeData()
	{
		mLanguagesKeyValues = LocalizationXMLParser.Instance.LoadData(true);
	}
	
	#endregion
}
