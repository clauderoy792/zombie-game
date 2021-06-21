using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DictionaryExtension {

	public static void Serialize<K,V>(this Dictionary<K,V> aDic,BinaryFormatter formatter, FileStream fs)
	{
		List<K> keyList = new List<K>();
		List<V> valueList = new List<V>();
		
		foreach(KeyValuePair<K,V> pair in aDic)
		{
			keyList.Add(pair.Key);
			valueList.Add(pair.Value);
		}
		
		//
		formatter.Serialize(fs,keyList);
		formatter.Serialize(fs,valueList);
	}
	
	public static Dictionary<K,V> DeserializeDictionary<K,V>(this BinaryFormatter formatter,FileStream fs)
	{
		Dictionary<K,V> returnValue = new Dictionary<K, V>(); 
		
        List<K> keyList = (List<K>)formatter.Deserialize(fs);
		List<V> valueList = (List<V>)formatter.Deserialize(fs);
		
		for(int i = 0;i < keyList.Count;i++)
		{
			returnValue.Add(keyList[i],valueList[i]);
		}
		
		return returnValue;
	}
}
