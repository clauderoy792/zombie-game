using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class CharacterNameGenerator {
	
	#region ENUM
	
	/// <summary>
	/// E name type.
	/// </summary>
	private enum ENameType
	{
		FirstName,
		LastName,
		Count
	}
	
	#endregion

	#region MEMBERS
	
	//
	private static CharacterNameGenerator mInstance;
	
	//
	System.Random mRandom;
	
	/// <summary>
	/// Contains 2 list of string for each key in the dictionary. At index 0 of the array, is all the first
	/// names for males, and at index 1 is all the first name of females for their respective races.
	/// </summary>
	private Dictionary<ERace, List<string>[]> mFirstNames;
	
	/// <summary>
	/// A dictionary containing a list of string for each value representing all the 
	/// last names available for their respective race.
	/// </summary>
	private Dictionary<ERace,List<string>> mLastNames;
	
	/// <summary>
	/// A dictionary containing a list of string for all full names for each race that are currently
	/// being used.
	/// </summary>
	private Dictionary<ERace, List<string>[]> mUsedNames;
	
	//
	private XmlParser mXmlParser;
	
	//
	private EGender mCurrentGender;
	private ERace mCurrentRace;
	private bool mIsFirstNames;
	
	#endregion
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static CharacterNameGenerator Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new CharacterNameGenerator();
			}
			
			return mInstance;
		}
	}

	public Dictionary<ERace,List<string>[]> UsedNames
	{
		get{return mUsedNames;}
		set{mUsedNames = value;}
	}

	#endregion
	
	#region CONTRUCTORS
	
	/// <summary>
	/// Initializes a new instance of the <see cref="CharacterNameGenerator"/> class.
	/// </summary>
	private CharacterNameGenerator()
	{
		mRandom = new System.Random();
		
		//
		mLastNames = new Dictionary<ERace, List<string>>();
		mFirstNames = new Dictionary<ERace, List<string>[]>();
		mUsedNames = new Dictionary<ERace, List<string>[]>();
		
		//Initialize names dictionaries.
		for(int i = 0; i < (int)ERace.Count;i++)
		{
			//First names.
			List<string>[] firstNameArray = new List<string>[(int)EGender.Count];
			
			for(int j = 0; j <firstNameArray.Length;j++)
			{
				firstNameArray[j] = new List<string>();
			}
			
			mFirstNames.Add((ERace)i,firstNameArray);
			
			//Last names.
			mLastNames.Add((ERace)i, new List<string>());
			
			//Currently used names.
			List<string>[] nameArray = new List<string>[(int)EGender.Count];
			
			for(int j = 0; j <nameArray.Length;j++)
			{
				nameArray[j] = new List<string>();
			}
			
			mUsedNames.Add((ERace)i,nameArray);
		}
		
		//
		mXmlParser = new XmlParser(XMLAssetManager.Instance.mNameXML);
		
		//
		mXmlParser.Read(OnXmlElementStart,null,OnXmlValueRead);
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	/// <summary>
	/// Gets the total names combination count.
	/// </summary>
	/// <returns>
	/// The total names combination count.
	/// </returns>
	public int GetTotalNamesCombinationCount()
	{
		int count = 0;
		
		for(int i = 0;i < mLastNames.Count;i++)
		{
			count += mLastNames[(ERace)i].Count*mFirstNames[(ERace)i][0].Count;
			count += mLastNames[(ERace)i].Count*mFirstNames[(ERace)i][1].Count;
		}
		
		return count;
	}
	
	/// <summary>
	/// Generates the name.
	/// </summary>
	/// <returns>
	/// The name.
	/// </returns>
	/// <param name='aGender'>
	/// A gender.
	/// </param>
	/// <param name='aRace'>
	/// A race.
	/// </param>
	public string GenerateName(EGender aGender,ERace aRace)
	{
		//
		int gender = (int)aGender;
		string generatedName = "";
		bool nonValidName = true;
		
		//Check if we used all possible combination for race/gender,
		//if so, reset the list.
		if (IsAllUsedNameForRaceGender(aRace,aGender))
		{
			mUsedNames[aRace][gender].Clear();
		}
		
		while (nonValidName)
		{
			generatedName =  mFirstNames[aRace][gender][mRandom.Next(mFirstNames[aRace][gender].Count)] + " " + mLastNames[aRace][mRandom.Next(mLastNames[aRace].Count)];
			
			nonValidName = IsAllUsedNameForRaceGender(aRace,aGender);
		}

		//
		mUsedNames[aRace][gender].Add(generatedName);
		
		return generatedName;
	}
	
	#endregion
		
	#region PRIVATE_METHODS
	
	/// <summary>
	/// Determines whether this instance is all used name for race gender the specified aGender aRace.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is all used name for race gender the specified aGender aRace; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='aGender'>
	/// If set to <c>true</c> a gender.
	/// </param>
	/// <param name='aRace'>
	/// If set to <c>true</c> a race.
	/// </param>
	private bool IsAllUsedNameForRaceGender(ERace aRace,EGender aGender)
	{
		int gender = (int)aGender;
		
		return mUsedNames[aRace][gender].Count >= (mLastNames[aRace].Count*mFirstNames[aRace][gender].Count);
	}
		
	#endregion
	
	#region XML_METHODS
	
	/// <summary>
	/// Raises the xml element start event.
	/// </summary>
	/// <param name='aElement'>
	/// A element.
	/// </param>
	void OnXmlElementStart(string aElement)
	{
		//
		aElement = aElement.ToLower();
		
		//
		Array races = Enum.GetValues(typeof(ERace));
		Array genders = Enum.GetValues(typeof(EGender));
		
		//Check For Race tag
		for(int i = 0;i < races.Length;i++)
		{
			string v =races.GetValue(i).ToString().ToLower();
			
			if (v == aElement)
			{
				mCurrentRace = (ERace)i;
				break;
			}
		}
		
		//Check for gender tag
		for(int i = 0;i < genders.Length;i++)
		{
			string v =genders.GetValue(i).ToString().ToLower();
			
			if (v == aElement)
			{
				mCurrentGender = (EGender)i;
				break;
			}
		}
		
		//Check if it is first names
		mIsFirstNames = aElement == "male" || aElement == "female";
	}
	
	/// <summary>
	/// Raises the xml value read event.
	/// </summary>
	/// <param name='aValue'>
	/// A value.
	/// </param>
	void OnXmlValueRead(string aValue)
	{
		string[] values =aValue.Split(',');
		
		//Trim all strings
		for(int i = 0;i < values.Length;i++)
		{
			values[i] = values[i].Trim().Replace("\n", String.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty);
		}

		if (mIsFirstNames)
		{
			//First Names
			mFirstNames[mCurrentRace][(int)mCurrentGender].AddRange(values);
		}
		else
		{
			//Last Names
			mLastNames[mCurrentRace].AddRange(values);
		}
	}

	#endregion
}
