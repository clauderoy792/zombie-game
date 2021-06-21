using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildVersion : EditorWindow 
{
	
	string[] buildType = new string[]
	{
		"Major feature",
		"Minor feature",
		"Bug Fix"
	};
	int mSelection = 2;
	string mNewVersion = "";
	
	//
	[MenuItem("Tools/Generate Build Version")]
	static void Init()
	{
		GetWindow(typeof(BuildVersion));
	}
	
	//
	void OnGUI()
	{
		mSelection = GUILayout.SelectionGrid(mSelection, buildType, buildType.Length, "toggle");
		
		if(GUILayout.Button("Generate Build version"))
		{
			try
			{
				string lastVersion = PlayerSettings.bundleVersion;
				bool noBuildVersion = string.IsNullOrEmpty(lastVersion);
				
				string[] splitVersion = lastVersion.Split('.');
				
				int major = noBuildVersion ? 0 : int.Parse(splitVersion[0]);
				int minor = noBuildVersion ? 0 : int.Parse(splitVersion[1]);
				int bug = noBuildVersion ? 0 : int.Parse(splitVersion[2]);
				
				switch(mSelection) 
				{
				case 0:
					major ++;
					minor = 0;
					bug = 0;
					break;		
				case 1: 
					minor ++;
					bug = 0;
					break;	
				case 2:
					bug ++;
					break;	
				}
				
				//
				mNewVersion = major.ToString() + "." + minor.ToString() + "." + bug.ToString();
				
				//
				PlayerSettings.bundleVersion = mNewVersion;
			}
			catch(System.Exception e)
			{
				Debug.LogError("AUTO BUILD VERSION ERROR: Current build version must be in x.x.x format. " + e.Message);
			}
		}
		
		//
		GUILayout.Label ("New build version: " + mNewVersion);
		
		if(!string.IsNullOrEmpty(mNewVersion))
		{
			GUILayout.Label ("Ready for build.");
		}
	}
}
