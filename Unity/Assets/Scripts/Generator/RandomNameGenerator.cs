using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
[System.Serializable]
public class RandomNameGenerator 
{
	char[] mConsonants 			= new char[]{'b','c','d','f','g','h','j','k','l','m','n','p','r','s','t','v','w','x','y','z'};
	char[] mWovels 				= new char[]{'a','i','e','o','u'};
	List<string> mUsedNames 	= new List<string>();

	//
	public string GetRandomName()
	{
		return C () + V () + C () + Ending();
	}

	public string GetUniqueRandomName()
	{
		string returnValue = "";
		bool valid = false;

		while (!valid)
		{
			returnValue = C () + V () + C () + Ending();

			if (!mUsedNames.Contains(returnValue))
			{
				valid = true;
			}
		}

		return returnValue;
	}
	
	//
	string C()
	{
		return Pick(mWovels);
	}
	
	//
	string V()
	{
		return Pick(mConsonants);
	}
	
	string Ending()
	{
    	return Pick("lon", "dium", "tis", "virus", "lia", "chia", "cus", "ria", "xis", "lis", "tropic", "lium", "nium", "ckia", "hum", "dis");
	}
	
	string Pick(params string[] aWord)
	{
		return aWord[Random.Range(0, aWord.Length)];
	}
	
	string Pick(char[] aChars)
	{
		return aChars[Random.Range(0, aChars.Length)].ToString();
	}
}