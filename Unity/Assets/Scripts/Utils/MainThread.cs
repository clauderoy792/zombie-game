using UnityEngine;
using System.Collections;
using HCUtils;

public class MainThread : MonoBehaviour
{
	#region CONSTANTS

	#endregion

	#region STATIC_MEMBERS

	private static MainThread sInstance;

	#endregion

	#region ACCESSORS

	public static MainThread Instance
	{
		get {return sInstance;}
	}

	#endregion

	#region MONO_METHODS

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
		}
		else
		{
			Debug.Log("There is already an instance of the MainThread in the scene.");
		}
	}
	
	void Update()
	{
		//
		TimerManager.Instance.Update(Time.deltaTime);
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion
}
