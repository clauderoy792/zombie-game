using UnityEngine;
using System.Collections;

public class GameManager 
{
	#region STATIC_MEMBERS
	
	static GameManager sInstance;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private UserStats mUserStats;
	
	#endregion
	
	#region ACCESSORS
	
	public static GameManager Instance
	{
		get 
		{
			if (sInstance == null)
			{
				sInstance = new GameManager();
			}
			
			return sInstance;
		}
	}
	
	public UserStats UserStats
	{
		get{return mUserStats;}
		set{mUserStats = value;}
	}
	
	#endregion

	#region CONSTRUCTORS
	
	private GameManager()
	{
		mUserStats = new UserStats();
	}
	
	#endregion

	#region PUBLIC_METHODS

	public bool IsGamePaused()
	{
		return Time.timeScale == 0;
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	/// <param name="aValue">If set to <c>true</c> the game is paused, else the game is resumed.</param>
	public void PauseGame(bool aValue = true)
	{
		if (aValue)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}

	#endregion
}
