using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class IngredientEditor : EditorWindow 
{
	bool mShowMainMenu = true;
	bool mShowIngredientCreation = false;
	bool mShowIngredientModification = false;
	string[] mIngredientsName = null;
	int mSelectedIngredient = 0;
	int mNewIngredientRage = 0;
	int mNewIngredientIntellect = 0;
	int mNewIngredientInfectivity = 0;
	int mNewIngredientStench = 0;
	string mNewIngredientName = "";
	string mErrorMessage = "";
	List<Ingredient> mIngredients = null;
	
	
	[MenuItem("Tools/IngredientEditor")]
	static void Init()
	{
		GetWindow(typeof(IngredientEditor));	
	}
	
	void OnEnable()
	{
		if(mIngredients == null || mIngredientsName == null)
		{
			LoadData();
		}
	}
	
	void OnGUI()
	{
		if(mShowMainMenu)
		{
			DrawMainMenu();
		}
		else if(mShowIngredientCreation)
		{
			DrawIngredientCreation();
		}
		else if(mShowIngredientModification)
		{
			DrawIngredientModification();
		}
	}
	
	void DrawMainMenu()
	{
		GUILayout.Space(25);
		
		// Editing part
		GUILayout.BeginHorizontal();
		GUILayout.Label("Ingredients:", GUILayout.Width(80));
		mSelectedIngredient = EditorGUILayout.Popup( mSelectedIngredient, mIngredientsName);
		GUILayout.Space(10);
		
		GUI.enabled = mSelectedIngredient == 0 ? false : true;
		if(GUILayout.Button("Edit", GUILayout.Width(40)))
		{
			//
			mNewIngredientName = mIngredients[mSelectedIngredient-1].Name;
			mNewIngredientRage = (int)mIngredients[mSelectedIngredient-1].Rage;
			mNewIngredientIntellect = (int)mIngredients[mSelectedIngredient-1].Intellect;
			mNewIngredientStench = (int)mIngredients[mSelectedIngredient-1].Stench;	
			mNewIngredientInfectivity = (int)mIngredients[mSelectedIngredient-1].Infectivity;	
					
			//
			mShowMainMenu = false;
			mShowIngredientCreation = false;
			mShowIngredientModification = true;
			
			//
			ClearFocus();
		}
		GUI.enabled = true;
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		// Creation part
		GUILayout.BeginHorizontal();
		GUILayout.Label("New Ingredient:", GUILayout.Width(100));
		mNewIngredientName = EditorGUILayout.TextField(mNewIngredientName);
		GUILayout.Space(10);
		if(GUILayout.Button("Add", GUILayout.Width(40)))
		{
			mShowMainMenu = false;
			mShowIngredientCreation = true;
			mShowIngredientModification = false;
			
			//
			ClearFocus();
		}
		GUILayout.EndHorizontal();
	}
	
	//
	void DrawIngredientCreation()
	{
		GUILayout.Space(25);
		
		//
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Name");
		mNewIngredientName = EditorGUILayout.TextField(mNewIngredientName);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Rage");
		mNewIngredientRage = EditorGUILayout.IntField(mNewIngredientRage);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Intellect");
		mNewIngredientIntellect = EditorGUILayout.IntField(mNewIngredientIntellect);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Stench");
		mNewIngredientStench = EditorGUILayout.IntField(mNewIngredientStench);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Infectivity");
		mNewIngredientInfectivity = EditorGUILayout.IntField(mNewIngredientInfectivity);
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button("Cancel"))
		{
			//
			mShowMainMenu = true;
			mShowIngredientCreation = false;
			mShowIngredientModification = false;
			
			//
			ClearFocus();
			FlushData();
		}
		
		//
		if(GUILayout.Button("Create Ingredient"))
		{
			if(NameIsValid(mNewIngredientName))
			{
				//TODO Set seconds - CR
				Ingredient newIngredient = new Ingredient(mNewIngredientName,-1,mNewIngredientIntellect,mNewIngredientInfectivity,
															mNewIngredientInfectivity,mNewIngredientStench,10);
				
				mIngredients.Add(newIngredient);
				
				//
				SaveData();
				LoadData();
				
				//
				mShowMainMenu = true;
				mShowIngredientCreation = false;
				mShowIngredientModification = false;
				
				//
				ClearFocus();
				FlushData();
			}
			else
			{
				mErrorMessage = "The Name specified is already use by another ingredient.";
			}
		}
		GUILayout.EndHorizontal();
		
		//
		GUILayout.Label(mErrorMessage);
		
	}
	
	void DrawIngredientModification()
	{
		GUILayout.Space(25);
		
		//
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Name");
		mNewIngredientName = EditorGUILayout.TextField(mNewIngredientName);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Rage");
		mNewIngredientRage = EditorGUILayout.IntField(mNewIngredientRage);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Intellect");
		mNewIngredientIntellect = EditorGUILayout.IntField(mNewIngredientIntellect);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Stench");
		mNewIngredientStench = EditorGUILayout.IntField(mNewIngredientStench);
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Infectivity");
		mNewIngredientInfectivity = EditorGUILayout.IntField(mNewIngredientInfectivity);
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		//
		if(GUILayout.Button("Delete"))
		{
			//
			mIngredients.RemoveAt(mSelectedIngredient-1);
			mSelectedIngredient -= 1;
			
			//
			SaveData();
			LoadData();
			
			//
			mShowMainMenu = true;
			mShowIngredientCreation = false;
			mShowIngredientModification = false;
			
			//
			ClearFocus();
			FlushData();
		}
		
		//
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Cancel"))
		{
			//
			mShowMainMenu = true;
			mShowIngredientCreation = false;
			mShowIngredientModification = false;
			
			//
			ClearFocus();
			FlushData();
		}
		
		//
		if(GUILayout.Button("Apply Change"))
		{
			if(NameIsValid(mNewIngredientName) || mNewIngredientName.ToUpper() == mIngredients[mSelectedIngredient-1].Name.ToUpper())
			{
				Ingredient editedIngredient = mIngredients[mSelectedIngredient-1];
				
				editedIngredient.SetStats(mNewIngredientName,mNewIngredientRage,mNewIngredientIntellect,mNewIngredientStench,mNewIngredientInfectivity);
				
				//
				SaveData();
				LoadData();
				
				//
				mShowMainMenu = true;
				mShowIngredientCreation = false;
				mShowIngredientModification = false;
				
				//
				ClearFocus();
				FlushData();
			}
			else
			{
				mErrorMessage = "The Name specified is already use by another ingredient.";
			}
		}
		GUILayout.EndHorizontal();
		
		
		//
		GUILayout.Label(mErrorMessage);
	}
	
	//
	void SaveData()
	{
		IngredientXMLParser.Instance.CreateXMLFile(mIngredients);
	}
	
	//
	void LoadData()
	{
		mIngredients = IngredientXMLParser.Instance.GetIngredients();
		mIngredientsName = new string[mIngredients.Count+1];
	
		mIngredientsName[0] = "-- Select an Ingredient --";
		for(int i = 0; i < mIngredients.Count; i++)
		{
			mIngredientsName[i+1] = mIngredients[i].Name;
		}
	}
	
	//
	void ClearFocus()
	{
		GUIUtility.keyboardControl = 0;
	}
	
	//
	void FlushData()
	{
		mNewIngredientName = "";
		mNewIngredientRage = 0;
		mNewIngredientIntellect = 0;
		mNewIngredientStench = 0;	
		mNewIngredientInfectivity = 0;	
		mErrorMessage = "";
	}
	
	//
	bool NameIsValid(string aName)
	{
		foreach(Ingredient i in mIngredients)
		{
			if(i.Name.ToUpper() == aName.ToUpper())
			{
				return false;
			}
		}
		
		return true;
	}
}
