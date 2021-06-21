using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class LocalizationEditorWindow : EditorWindow 
{
	#region CONSTANTS
	
	//
	public const string LANGUAGE_ENUM_NAME = "ELanguage";
	public const string STRING_ENUM_NAME = "EString";
	readonly string[] BASE_LANGUAGES = new string[] {"fr","en","espanol"};
	
	#endregion
	
	#region PRIVATE_MEMBEMRS
	
	//First key is the language, second key in the value dictionary is the language
	[SerializeField]
	Dictionary<string,Dictionary<string,string>> mLanguagesKeyValues;
	
	//
	string[] mLanguages;
	string[] mKeys;
	string[] mTempKeysValue;
	
	//
	string mCurrentEditKey;
	string mCurrentEditLanguage;
	string mNewKey;
	string mNewLanguage;
	string mErrorMessage;
	
	//
	int mCurrentKeyPosition;
	int mCurrentLanguagePosition;
	
	//
	bool mIsShowingMainMenu = true;
	bool mIsShowingLanguages = false;
	bool mIsShowingKeyEdit = false;
	bool mIsShowingErrorMessage = false;
	bool mIsShowingEditLanguages = false;
	
	//
	GUIStyle mErrorStyle;
	
	#endregion
	
	#region MONO_METHODS
	
	[MenuItem ("Tools/Localization Editor")]
	public static void ShowWindow()
	{
		//
		GetWindow(typeof(LocalizationEditorWindow));
	}
	
	void OnEnable()
	{
		//
		mErrorStyle = new GUIStyle();
		mErrorStyle.normal.textColor = Color.red;
		
		if (mLanguagesKeyValues == null)
		{
			//
			LoadData();
			
			//
			SetLanguagesStringArray();
			
			//
			SetKeysStringArray();
		}
	}
	
    void OnGUI () {
		//
		Toolbar();
		
		if (mIsShowingMainMenu)
		{
	        //
			MainMenuUI();
		}
		else if (mIsShowingKeyEdit)
		{
			KeysMenuUI();
		}
		else if (mIsShowingLanguages)
		{
			LanguagesUI();
		}
		else if (mIsShowingEditLanguages)
		{
			EditLanguagesUI();
		}
    }
	
	#endregion
	
	#region UI_METHODS
	
	void MainMenuUI()
	{
		GUILayout.BeginHorizontal();
		
		//
		GUILayout.Label("Keys : ");
		
		//
		mCurrentKeyPosition = EditorGUILayout.Popup(mCurrentKeyPosition,mKeys);
		
		//
		if(GUILayout.Button("Edit"))
		{
			//
			OnBtnEditClick();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		//
		mNewKey = EditorGUILayout.TextField("New Key :", mNewKey);
		
		//
		if(GUILayout.Button("Add"))
		{
			//
			OnBtnAddNewKeyClick();
		}
		
		GUILayout.EndHorizontal();
		
		ManageErrorMessageDisplay();
	}
	
	void KeysMenuUI()
	{
		//
		GUILayout.BeginHorizontal();
		
		mNewKey = EditorGUILayout.TextField("Key name : ",mNewKey);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(position.height*0.1f);
		
		//
		for(int i = 0; i<mLanguages.Length;i++)
		{
			GUILayout.BeginHorizontal();
			mTempKeysValue[i] =  EditorGUILayout.TextField(mLanguages[i] +" :", mTempKeysValue[i]);
			GUILayout.EndHorizontal();
		}
		
		//
		ManageErrorMessageDisplay();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		
		GUI.enabled = !string.IsNullOrEmpty(mNewKey);
		
		//
		if(GUILayout.Button("Apply"))
		{
			//
			OnBtnApplyKeysChangeClick();
		}
		
		GUI.enabled = true;
		
		//
		if(GUILayout.Button("Cancel"))
		{
			//
			OnBtnCancelClick();
		}
		
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Delete"))
		{
			//
			OnBtnDeleteKeyClick();
		}
		
		GUILayout.EndHorizontal();
	}
	
	void LanguagesUI()
	{
		GUILayout.BeginHorizontal();
		//
		GUILayout.Label("Languages : ");
		
		//
		mCurrentLanguagePosition = EditorGUILayout.Popup(mCurrentLanguagePosition,mLanguages);
		
		//
		if(GUILayout.Button("Edit"))
		{
			//
			OnBtnEditLanguageClick();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		//
		mNewLanguage = EditorGUILayout.TextField("New Language :", mNewLanguage);
		
		//
		if(GUILayout.Button("Add"))
		{
			//
			OnBtnAddNewLanguageClick();
		}
		
		GUILayout.EndHorizontal();
		
		//
		ManageErrorMessageDisplay();
		
		GUILayout.BeginHorizontal();
		
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Cancel"))
		{
			//
			OnBtnCancelClick();
		}
		
		GUILayout.FlexibleSpace();
		
		GUILayout.EndHorizontal();
	}
	
	void EditLanguagesUI()
	{
		GUILayout.BeginHorizontal();
		mNewLanguage = EditorGUILayout.TextField("Language : ",mNewLanguage);
		GUILayout.EndHorizontal();
		
		//
		ManageErrorMessageDisplay();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		
		GUI.enabled = !string.IsNullOrEmpty(mNewLanguage);
		//
		if(GUILayout.Button("Apply"))
		{
			//
			OnBtnApplyLanguageClick();
		}
		
		GUI.enabled = true;
		
		//
		if(GUILayout.Button("Cancel"))
		{
			mNewLanguage = "";
			
			//
			OnBtnCancelEditLanguageClick();
		}
		
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Delete"))
		{
			//
			OnBtnDeleteLanguageClick();
		}
		
		GUILayout.EndHorizontal();
	}
	
	void Toolbar()
	{
		GUILayout.BeginHorizontal(EditorStyles.toolbar);
		//
		if(GUILayout.Button("File", EditorStyles.toolbarDropDown)) 
		{
            // Now create the menu, add items and show it
            GenericMenu menuFile = new GenericMenu();
            
            menuFile.AddItem (new GUIContent("Edit Languages"), false, FileCallback, "edit_languages");
			
			menuFile.DropDown(new Rect(5,0,0,16));
		}
		
		//
		GUILayout.FlexibleSpace();
  		GUILayout.EndHorizontal();
	}

	#endregion
	
	#region SCREENS_CHANGE
	
	void ShowMainMenu()
	{
		//Clear old textfield text.
		GUIUtility.keyboardControl = 0;
		mIsShowingKeyEdit = false;
		mIsShowingLanguages = false;
		mIsShowingMainMenu = true;
		mIsShowingErrorMessage = false;
		mIsShowingEditLanguages = false;
		
		//
		SetErrorMessage(false);
		
		mNewKey = "";
	}
	
	void ShowEditKeyMenu()
	{
		//Clear old textfield text.
		GUIUtility.keyboardControl = 0;
		mIsShowingKeyEdit = true;
		mIsShowingLanguages = false;
		mIsShowingMainMenu = false;
		mIsShowingErrorMessage = false;
		mIsShowingEditLanguages = false;
		
		//
		SetErrorMessage(false);
	}
	
	void ShowLanguagesMenu()
	{
		mNewLanguage = "";
		
		//Clear old textfield text.
		GUIUtility.keyboardControl = 0;
		mIsShowingKeyEdit = false;
		mIsShowingLanguages = true;
		mIsShowingMainMenu = false;
		mIsShowingErrorMessage = false;
		
		//
		SetErrorMessage(false);
	}
	
	void ShowEditLanguagesMenu()
	{
		//Clear old textfield text.
		GUIUtility.keyboardControl = 0;
		mIsShowingEditLanguages = true;
		mIsShowingKeyEdit = false;
		mIsShowingLanguages = false;
		mIsShowingMainMenu = false;
		
		//
		SetErrorMessage(false);
	}
	
	#endregion
	
	#region BUTTONS_METHODS
	
	void OnBtnAddNewKeyClick()
	{
		if (!string.IsNullOrEmpty(mNewKey))
		{
			if (AddKey(mNewKey))
			{
				//
				SaveData();
				
				mCurrentEditKey = mNewKey;
				mTempKeysValue = new string[mLanguages.Length];
				
				//
				ShowEditKeyMenu();
				
				SetErrorMessage(false);
			}
			else
			{
				SetErrorMessage(true,"The key you are trying to add already exists.");
			}
		}
	}

	void OnBtnAddNewLanguageClick()
	{
		if (!string.IsNullOrEmpty(mNewLanguage))
		{
			if (AddLanguage(mNewLanguage))
			{
				//
				ShowLanguagesMenu();
				
				SetErrorMessage(false);
			}
			else
			{
				SetErrorMessage(true,"The language you are trying to add already exists");
			}
		}
	}
	
	void OnBtnEditClick()
	{
		if (mCurrentKeyPosition > 0)
		{
			mCurrentEditKey = mKeys[mCurrentKeyPosition];
			mNewKey = mCurrentEditKey;
			
			//
			SetTempKeys();
			
			//
			ShowEditKeyMenu();
		}
	}
	
	void OnBtnEditLanguageClick()
	{
		if (mCurrentLanguagePosition >= 0)
		{
			mCurrentEditLanguage = mLanguages[mCurrentLanguagePosition];
			mNewLanguage = mCurrentEditLanguage;
			
			//
			ShowEditLanguagesMenu();
		}
	}
	
	void OnBtnApplyKeysChangeClick()
	{
		//
		for(int i =0;i<mLanguages.Length;i++)
		{
			mLanguagesKeyValues[mLanguages[i]][mCurrentEditKey] = mTempKeysValue[i];
		}
		
		if (mCurrentEditKey != mNewKey)
		{
			bool containsKey = false;
			
			//
			foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
			{
				containsKey = (languages.Value.ContainsKey(mNewKey));
				break;
			}
			
			//
			if (!containsKey)
			{
				//
				foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
				{
					//
					string str = languages.Value[mCurrentEditKey];
					
					//
					languages.Value.Remove(mCurrentEditKey);
					
					//
					languages.Value.Add(mNewKey,str);
				}
				
				//
				SaveData();
				
				//
				ShowMainMenu();
				
				SetErrorMessage(false);
			}
			else
			{
				SetErrorMessage(true,"The key you are trying to add already exists.");
			}
		}
		else
		{
			//
			SaveData();
			
			//
			ShowMainMenu();
		}
	}
	
	void OnBtnLanguageClick()
	{
		//
		ShowMainMenu();
	}
	
	void OnBtnDeleteKeyClick()
	{
		foreach(KeyValuePair<string,Dictionary<string,string>> pair in mLanguagesKeyValues)
		{
			pair.Value.Remove(mCurrentEditKey);
		}
		
		//
		mCurrentKeyPosition = 0;
		
		//
		SaveData();
		
		//
		ShowMainMenu();
	}
	
	void OnBtnDeleteLanguageClick()
	{
		//
		mLanguagesKeyValues.Remove(mCurrentEditLanguage);
		
		//
		mCurrentLanguagePosition = 0;
		
		//
		SaveData();
		
		//
		ShowLanguagesMenu();
	}
	
	void OnBtnApplyLanguageClick()
	{
		if (mNewLanguage != mCurrentEditLanguage)
		{
			if (!mLanguagesKeyValues.ContainsKey(mNewLanguage) && (!string.IsNullOrEmpty(mNewLanguage)))
			{
				Dictionary<string,string> keys = mLanguagesKeyValues[mCurrentEditLanguage];
				
				//
				mLanguagesKeyValues.Remove(mCurrentEditLanguage);
				
				//
				mLanguagesKeyValues.Add(mNewLanguage,keys);
				
				//
				SaveData();
				
				//
				ShowLanguagesMenu();
				
				//
				SetErrorMessage(false);
			}
			else
			{
				SetErrorMessage(true,"The language already exists !");
			}
		}
		else
		{
			//
			ShowLanguagesMenu();
		}
	}
	
	void OnBtnCancelClick()
	{
		ShowMainMenu();
	}
	
	void OnBtnCancelEditLanguageClick()
	{
		ShowLanguagesMenu();
	}
	
	#endregion
	
	#region TOOLBAR_CALLBACK
	
	void FileCallback( object result )
	{		
		switch(result.ToString())
		{
			case "edit_languages":
			{
				ShowLanguagesMenu();
				break;
			}
		}
	}
	
	#endregion
	
	#region WRITE_FILES_METHODS

	void WriteStringsEnum()
	{
		//
		List<string> strings = new List<string>();
		
		//
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			foreach(KeyValuePair<string,string> keys in languages.Value)
			{
				strings.Add(keys.Key);
			}
			break;
		}
		
		//
		LocalizationXMLParser.Instance.WriteEnum(STRING_ENUM_NAME,strings.ToArray());
	}
	
	void WriteLanguageEnum()
	{
		//
		List<string> strings = new List<string>();
		
		//
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			//
			strings.Add(languages.Key);
		}
		
		//
		LocalizationXMLParser.Instance.WriteEnum(LANGUAGE_ENUM_NAME,strings.ToArray());
	}

	#endregion
	
	#region DATA_MANAGEMENT
	
	bool AddKey(string aKey)
	{	
		bool returnValue = true;
		
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			if (!languages.Value.ContainsKey(aKey))
			{
				//
				languages.Value.Add(aKey,"");
			}
			else
			{
				mErrorMessage = "The key already exists !";
				mIsShowingErrorMessage = true;
				returnValue = false;
				return returnValue;
			}
			break;
		}
		
		return returnValue;
	}
	
	bool AddLanguage(string aLanguage)
	{
		bool returnValue = true;
		
		if (!mLanguagesKeyValues.ContainsKey(aLanguage))
		{
			Dictionary<string,string> keys = new Dictionary<string, string>();
			
			foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
			{
				//Get all existing keys
				foreach(KeyValuePair<string,string> pair in languages.Value)
				{
					keys.Add(pair.Key,"");
				}
				
				break;
			}
			
			//
			mLanguagesKeyValues.Add(aLanguage,keys);
			
			//
			SaveData();
		}
		else
		{
			returnValue = false;
		}
		
		return returnValue;
	}
	
	void SetLanguagesStringArray()
	{
		List<string> array = new List<string>();
		
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			//
			array.Add(languages.Key);
		}
		
		mLanguages = array.ToArray();
	}
	
	void SetKeysStringArray()
	{
		List<string> array = new List<string>();
		
		//
		array.Add("-- Select Key --");
		
		//
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			//
			foreach(KeyValuePair<string,string> pair in languages.Value)
			{
				//
				array.Add(pair.Key);
			}
			break;
		}
		
		mKeys = array.ToArray();
	}
	
	void SetTempKeys()
	{
		//
		mTempKeysValue = new string[mLanguages.Length];
		
		//
		int i = 0;
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			
			mTempKeysValue[i] = languages.Value[mCurrentEditKey];
			
			i++;
		}
	}
	
	#endregion
	
	#region ERROR_MESSAGES
	
	void SetErrorMessage(bool aValue, string aMessage = "")
	{
		mIsShowingErrorMessage = aValue;
		mErrorMessage = aMessage;
	}
	
	void ManageErrorMessageDisplay()
	{
		if (mIsShowingErrorMessage)
		{
			GUILayout.Label(mErrorMessage,mErrorStyle);
		}
	}
	
	#endregion
	
	#region SAVE_LOAD_METHODS
	
	void SaveData()
	{
		//
		LocalizationXMLParser.Instance.SaveData(mLanguagesKeyValues);
		
		//
		WriteLanguageEnum();
		
		//
		WriteStringsEnum();
		
		//
		SetLanguagesStringArray();
		
		//
		SetKeysStringArray();
		
		//
		AssetDatabase.Refresh();
	}
	
	void LoadData()
	{
		//
		mLanguagesKeyValues = LocalizationXMLParser.Instance.LoadData(false);
		
		//If we failed to load data (first time)
		if (mLanguagesKeyValues.Keys.Count != BASE_LANGUAGES.Length)
		{
			Dictionary<string,string> keys = new Dictionary<string, string>();
			
			//
			foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
			{
				foreach(KeyValuePair<string,string> pair in languages.Value)
				{
					keys.Add(pair.Key,"");
				}
				break;
			}
			
			//
			foreach(string language in BASE_LANGUAGES)
			{
				if (!mLanguagesKeyValues.ContainsKey(language))
				{
					//
					mLanguagesKeyValues.Add(language,new Dictionary<string,string>(keys));
				}
			}	
			
			//
			SaveData();
		}
	}
	
	#endregion
	
	#region TEMP_METHODS
	
	void DisplayLanguagesDictionary(bool aDisplayKey = false)
	{
		//
		foreach(KeyValuePair<string,Dictionary<string,string>> languages in mLanguagesKeyValues)
		{
			Debug.Log("LANGAUGE : "+languages.Key);
			if (aDisplayKey)
			{
				foreach(KeyValuePair<string,string> pair in languages.Value)
				{
					Debug.Log("KEY : "+pair.Key + " , value : "+pair.Value);
				}
			}
		}
	}
	
	#endregion
}