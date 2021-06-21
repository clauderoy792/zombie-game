using UnityEngine;
using System.Collections;

namespace Utils
{
	public static class ArrayExtension {
	
		public static int IndexOf<T>(this T[] array,T aObject) where T : class
		{
			int index = -1;
			
			if (array == null)
			{
				Debug.LogError("The array you are trying to access is null.");
				return index;
			}
			else if (aObject == null)
			{
				Debug.LogError("The object you are trying to access is null.");
				return index;
			}
			
			for(int i = 0;i <array.Length;i++)
			{
				if (array[i] == aObject)
				{
					index = i;
					break;
				}
			}
				
			return index;
		}
		
		public static T ElementAt<T>(this T[] array, int aIndex) where T : class
		{
			T obj = null;
			
			if (aIndex >= 0 && aIndex < array.Length)
			{
				obj = array[aIndex];
			}
			else
			{
				Debug.LogError("Index out of range.");
			}
			
			return obj;
		}
	}
}
