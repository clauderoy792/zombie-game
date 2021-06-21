using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionary<K,V> 
{

	private List<K> mKeys;
	private List<V> mValues;

	public SerializableDictionary(IDictionary<K,V> aDic)
	{
		if (aDic != null)
		{
			mKeys = new List<K>();
			mValues = new List<V>();

			foreach(KeyValuePair<K,V> pair in aDic)
			{
				mKeys.Add(pair.Key);
				mValues.Add(pair.Value);
			}
		}
	}

	public Dictionary<K,V> ToDictionary()
	{
		Dictionary<K,V> returnValue = null;

		if (mKeys != null)
		{
			returnValue = new Dictionary<K, V>();

			for(int i = 0;i < mKeys.Count;i++)
			{
				returnValue.Add(mKeys[i],mValues[i]);
			}
		}

		return returnValue;
	}
}
