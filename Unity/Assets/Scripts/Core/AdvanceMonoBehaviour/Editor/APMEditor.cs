using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class APMEditor : EditorWindow 
{
	string mTitleText = "";
	int mTitleWidth = 350;
	Vector2 mScrollPos;
	Dictionary<int, bool> mFoldouts;

	[MenuItem("Debug/Pool Debugger Tool")]
	static void Init()
	{
		GetWindow(typeof(APMEditor));
	}

	//
	void OnGUI()
	{
		if(EditorApplication.isPlaying)
		{
			if(AutomaticPoolSystem.Exist())
			{
				mTitleText = "Pool Debugger Tool is connected.";
				mTitleWidth = 220;
			}
			else
			{
				mTitleText = "There is no Pool currently used in this scene.";
				mTitleWidth = 300;
			}
		}
		else
		{
			mTitleText = "You must be in PLAY mode to use this Debugger Tool.";
			mTitleWidth = 350;
		}

		GUILayout.Space(20);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField(mTitleText, EditorStyles.boldLabel, GUILayout.Width(mTitleWidth));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		if(EditorApplication.isPlaying && AutomaticPoolSystem.Exist())
		{
			//
			GUILayout.Label(string.Format("Total number of POOLED objects: {0}", AutomaticPoolSystem.Instance.NumberOfObjectsInPool));
			GUILayout.Label(string.Format("Total number of UNIQUE objects: {0}", AutomaticPoolSystem.Instance.GetAllPooledObject().Count));
			GUILayout.Label(string.Format("Total number of ACTIVE objects: {0}", AutomaticPoolSystem.Instance.NumberOfActiveObjects));
			GUILayout.Label(string.Format("Total number of INACTIVE objects: {0}", AutomaticPoolSystem.Instance.NumberOfInactiveObjects));

			GUILayout.Space(20);

			//
			if(mFoldouts != null)
			{
				mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, true);
				foreach(var keyValue in AutomaticPoolSystem.Instance.GetAllPooledObject())
				{
					if(mFoldouts.ContainsKey(keyValue.Key))
					{
						mFoldouts[keyValue.Key] = EditorGUILayout.Foldout(mFoldouts[keyValue.Key], keyValue.Key.ToString());

						if(mFoldouts[keyValue.Key])
						{
							foreach(var go in keyValue.Value)
							{
								if(go != null)
								{
									GUILayout.BeginHorizontal();
									GUILayout.Space(30);

									if(go.GetComponent<UniquePoolID>())
									{
										GUI.color = go.GetComponent<UniquePoolID>().Inactive ? Color.grey : Color.white;
									}

									if(GUILayout.Button(go.name, "BOX"))
									{
										EditorGUIUtility.PingObject(go);
									}

									GUI.color = Color.white;

									GUILayout.EndHorizontal();
								}
							}
						}
					}
				}

				GUILayout.EndScrollView();
          	}

			if(Event.current.type != EventType.Layout)
			{
				//
				UpdateFoldouts();
			}
		}
	}

	void OnInspectorUpdate()
	{
		Repaint();
	}

	void UpdateFoldouts()
	{
		//Fetch Dictionary
		if(mFoldouts == null)
		{
			mFoldouts = new Dictionary<int, bool>();

			foreach(var keyValue in AutomaticPoolSystem.Instance.GetAllPooledObject())
			{
				mFoldouts.Add(keyValue.Key, false);
			}
		}
		else
		{
			Dictionary<int, bool> tmpDictionary = new Dictionary<int, bool>();
			foreach(var keyValue in AutomaticPoolSystem.Instance.GetAllPooledObject())
			{
				tmpDictionary.Add(keyValue.Key, false);
			}

			foreach(var keyValue in mFoldouts)
			{
				if(tmpDictionary.ContainsKey(keyValue.Key))
				{
					tmpDictionary[keyValue.Key] = keyValue.Value;
				}
			}

			mFoldouts = tmpDictionary;
		}
	}
}
