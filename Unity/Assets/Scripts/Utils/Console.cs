using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour
{
	static Console mInstance;
	private Vector2 mScroll;
	private List<string> mLogs = new List<string>();
	private GUIStyle mStyle;
	private bool mIsOpen;

	//
	void Awake()
	{
		mStyle = new GUIStyle();
		mStyle.normal.textColor = Color.white;
		mStyle.fontSize = Screen.height/22;
	}

	//
	void OnGUI()
	{
		if(mInstance && mInstance.mIsOpen)
		{
			float width = Screen.width/1.5f;
			float height = Screen.height/2;
			float buttonHeight = 0.15f*height;
			float buttonWidth = 0.2f*width;

			//
			if (GUI.Button(new Rect(Screen.width - width-buttonWidth,Screen.height-buttonHeight*2,buttonWidth,buttonHeight),"Clear"))
			{
				mInstance.mLogs.Clear();
			}

			//
			if (GUI.Button(new Rect(Screen.width - width-buttonWidth,Screen.height-buttonHeight,buttonWidth,buttonHeight),"Close"))
			{
				mInstance.mIsOpen = false;
			}

			//
			GUILayout.BeginArea(new Rect(Screen.width - width, Screen.height - height, width, height), "", "BOX");
			GUILayout.BeginScrollView(mScroll);

			for(int i = mInstance.mLogs.Count - 1; i >= 0; i--)
			{
				GUILayout.Label(mInstance.mLogs[i], mStyle);
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	//
	public static void WriteLine(object aValue)
	{
		if(!mInstance)
		{
			GameObject go = new GameObject("Console");
			Console console = go.AddComponent<Console>();
			mInstance = console;
		}

		mInstance.mIsOpen = true;
		mInstance.mLogs.Add(System.DateTime.Now.ToString("HH:mm:ss") + ": " + aValue.ToString());
	}
}
