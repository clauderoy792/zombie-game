using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenesManager : MonoBehaviour {
	
	#region ENUM
	
	public enum EScene
	{
		MainMenu,
		InGame,
		UIScene
	}
	
	#endregion
	
	#region MEMBERS
	
	private List<EScene> mGameFlow;
	
	private EScene mCurrentScene;
	
	#endregion
	
	#region MONO_METHODS
	
	void Awake()
	{
		if (mInstance != null)
		{
			Debug.LogError("There is already another instance of ScenesManager.");
		}
		
		mInstance = this;
		
		mGameFlow = new List<EScene>();
		
		mCurrentScene = (EScene)0;
	}
	
	#endregion
	
	#region STATIC_MEMBERS
	
	private static ScenesManager mInstance;
	
	#endregion
	
	#region ACCESSORS
	
	public static ScenesManager Instance
	{
		get {return mInstance;}
	}
	
	#endregion
	
	#region PUBLIC_FUNCTIONS
	
	public void LoadScreen(ScenesManager.EScene aScene)
	{
		//
		ManageGameFlow(aScene);
		
		Application.LoadLevel(aScene.ToString());
		
		//
		mCurrentScene = aScene;
	}
	
	public void LoadPreviousScreen()
	{
		if (mGameFlow.Count > 0)
		{
			Application.LoadLevel(mGameFlow[mGameFlow.Count-1].ToString());
			
			//
			mCurrentScene = mGameFlow[mGameFlow.Count-1];
			
			//Remove game flow from stack.
			mGameFlow.RemoveAt(mGameFlow.Count-1);
		}
		else
		{
			Debug.LogError("There is no previous screen to load");
		}
	}
	
	#endregion
	
	#region PRIVATE_METHODS
	
	/// <summary>
	/// Manages the game flow to make sure that when you press back, you come back to the "real" previous screen.
	/// </summary>
	/// <param name='aScene'>
	/// A scene.
	/// </param>
	private void ManageGameFlow(EScene aNextScreen)
	{
		if (mGameFlow.Contains(aNextScreen))
		{
			int index = mGameFlow.IndexOf(aNextScreen);
			
			for(int i = mGameFlow.Count-1; i >=index;i--)
			{
				mGameFlow.RemoveAt(i);
			}
		}
		else
		{
			mGameFlow.Add(mCurrentScene);
		}
	}
	
	#endregion
}
