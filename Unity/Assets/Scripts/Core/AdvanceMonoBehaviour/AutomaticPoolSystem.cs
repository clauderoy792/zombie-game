using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//
public class AutomaticPoolSystem
{
	private static AutomaticPoolSystem mInstance;
	private Dictionary<int, List<GameObject>> mPool; 
	private List<GameObject> mAllObjectsInPool;

	private AutomaticPoolSystem()
	{
		mPool = new Dictionary<int, List<GameObject>>();
		mAllObjectsInPool = new List<GameObject>();
	}
	
	public static AutomaticPoolSystem Instance
	{
		get
		{
			if(mInstance == null)
			{
				mInstance = new AutomaticPoolSystem();
			}
			return mInstance;
		}
	}
	
	//
	public int NumberOfObjectsInPool
	{
		get{return mAllObjectsInPool.Count;}
	}

	//
	public int NumberOfActiveObjects
	{
		get
		{
			int output = 0;
			for(int i = 0; i < mAllObjectsInPool.Count; i++)
			{
				if(mAllObjectsInPool[i] != null && mAllObjectsInPool[i].GetComponent<UniquePoolID>())
				{
					if(!mAllObjectsInPool[i].GetComponent<UniquePoolID>().Inactive)
					{
						output++;
					}
				}
			}

			return output;
		}
	}

	//
	public int NumberOfInactiveObjects
	{
		get
		{
			int output = 0;
			for(int i = 0; i < mAllObjectsInPool.Count; i++)
			{
				if(mAllObjectsInPool[i] != null && mAllObjectsInPool[i].GetComponent<UniquePoolID>())
				{
					if(mAllObjectsInPool[i].GetComponent<UniquePoolID>().Inactive)
					{
						output++;
					}
				}
			}
			
			return output;
		}
	}

	//
	public static bool Exist()
	{
		return mInstance != null;
	}

	//
	public Dictionary<int, List<GameObject>> GetAllPooledObject()
	{
		Dictionary<int, List<GameObject>> output = new Dictionary<int, List<GameObject>>();

		// Copy current pool
		foreach(var keyValue in mPool)
		{
			output.Add(keyValue.Key, new List<GameObject>());
		}

		// Add all active object (Which are not in pool dictionary by default)
		foreach(var obj in mAllObjectsInPool)
		{
			UniquePoolID poolComponent = obj.GetComponent<UniquePoolID>();

			if(poolComponent)
			{
				if(output.ContainsKey(poolComponent.ID))
				{
					output[poolComponent.ID].Add(obj);
				}
				else
				{
					Debug.LogError("There is a pooled object in the scene which is not contains in the current pool.");
				}
			}
			else
			{
				Debug.LogError("There is a pooled object with a missing UniquePoolID component.");
			}
		}

		return output;
	}

	public Object InstantiateObject(GameObject obj, Vector3 pos, Quaternion rot)
	{
		int id = obj.GetInstanceID();
		
		if(mPool.ContainsKey(id))
		{
			if(mPool[id].Count > 0)
			{
				GameObject go = mPool[id][0];
				mPool[id].RemoveAt(0);

				//
				if(go.transform)
				{
					go.transform.position = pos;
					go.transform.rotation = rot;
				}
				
				//
				go.SetActive(true);
				go.GetComponent<UniquePoolID>().Inactive = false;
				
				return go;
			}
			else
			{
				GameObject go = GameObject.Instantiate(obj, pos, rot) as GameObject;

				//
				mAllObjectsInPool.Add(go);

				//
				UniquePoolID poolComponent = go.AddComponent<UniquePoolID>();
				poolComponent.ID = id;
				
				//
				go.SetActive(true);
				poolComponent.Inactive = false;
				
				return go;
			}
		}
		else
		{
			GameObject go = GameObject.Instantiate(obj, pos, rot) as GameObject;
			int UID = obj.GetInstanceID(); 

			//
			mAllObjectsInPool.Add(go);

			//
			UniquePoolID poolComponent = go.AddComponent<UniquePoolID>();
			poolComponent.ID = UID;
			
			//
			mPool.Add(UID, new List<GameObject>());
			
			//
			go.SetActive(true);
			poolComponent.Inactive = false;
			
			return go;
		}
	}
	
	public void DestroyObject(GameObject obj)
	{
		//
		UniquePoolID poolComponent = obj.GetComponent<UniquePoolID>();
		
		if(poolComponent)
		{
			if(mPool.ContainsKey(poolComponent.ID))
			{
				obj.SetActive(false);
				poolComponent.Inactive = true;
				mPool[poolComponent.ID].Add(obj);
			}
			else
			{
				//
				mAllObjectsInPool.Remove(obj);
				GameObject.Destroy(obj);
			}
		}
		else
		{
			//
			mAllObjectsInPool.Remove(obj);
			GameObject.Destroy(obj);
		}
	}
	
	public IEnumerator WaitAndDestroyObject(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);

		//
		UniquePoolID poolComponent = obj.GetComponent<UniquePoolID>();
		
		if(poolComponent)
		{
			if(mPool.ContainsKey(poolComponent.ID))
			{
				obj.SetActive(false);
				poolComponent.Inactive = true;
				mPool[poolComponent.ID].Add(obj);
			}
			else
			{
				//
				mAllObjectsInPool.Remove(obj);
				GameObject.Destroy(obj);
			}
		}
		else
		{
			//
			mAllObjectsInPool.Remove(obj);
			GameObject.Destroy(obj);
		}
		
	}

}