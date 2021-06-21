using UnityEngine;
using System.Collections;

[System.Serializable]
public class SimpleDateTime {
	
	#region CONSTANTS
	
	private const int NB_WEEKS_IN_MONTH = 4;
	private const int NB_MONTH_IN_YEAR = 12;
	
	#endregion

	#region PRIVATE_MEMBERS
	
	private int mWeek;
	private int mMonth;
	private int mYear;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public SimpleDateTime(int aWeek,int aMonth,int aYear)
	{
		mWeek = aWeek;
		mMonth = aMonth;
		mYear = aYear;
	}

	public SimpleDateTime(SimpleDateTime aOther)
	{
		mWeek = aOther.mWeek;
		mMonth = aOther.mMonth;
		mYear = aOther.mYear;
	}
	
	#endregion
	
	#region ACCESSORS
	
	public int Week
	{
		get{return mWeek;}	
	}
	
	public int Month
	{
		get{return mMonth;}	
	}
	
	public int Year
	{
		get{return mYear;}	
	}
	
	#endregion
	
	#region PUBLIC_METHODS

	public void AddWeeks(int aNbWeek)
	{
		aNbWeek = Mathf.Clamp(aNbWeek,0,int.MaxValue);

		for(int i = 0;i <aNbWeek;i++)
		{
			IncrementWeek();
		}
	}

	public bool IsGreaterOrEqual(SimpleDateTime aDateTime)
	{
		bool returnValue = false;

		if (mYear > aDateTime.mYear)
		{
			returnValue = true;
		}
		else if (mYear == aDateTime.mYear && mMonth > aDateTime.mMonth)
		{
			returnValue = true;
		}
		else if (mYear == aDateTime.mYear && mMonth == aDateTime.mMonth  &&mWeek >= aDateTime.mWeek)
		{
			returnValue = true;
		}

		return returnValue;
	}

	public void IncrementWeek()
	{
		mWeek++;
		
		//
		if (mWeek > NB_WEEKS_IN_MONTH)
		{
			mWeek = 1;
			mMonth++;
		}
		
		//
		if (mMonth > NB_MONTH_IN_YEAR)
		{
			mMonth = 1;
			mYear++;
		}
	}
	
	#endregion
	
	#region OVERRIDEN_METHODS

	public override string ToString ()
	{
		return string.Format ("[SimpleDateTime: Week={0}, Month={1}, Year={2}]", Week, Month, Year);
	}
	
	
	#endregion
}
