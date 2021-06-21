using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditorInternal;

[CustomEditor(typeof(UILabel))]
[CanEditMultipleObjects]
public class UILabelInspector : Editor
{
	static string ABSOLUTE_PATH = "";
	static string RELATIVE_PATH = "";
	
	FontsData mFontData;
	string[] mStringFontFamilies;
	string[] mSortingLayerList;
	int mSelectedSortingLayer = 0;
	int mFontSelected = 0;
	int mFonStyle = 0;
	System.Drawing.FontFamily mSelectedFontFamily;
	System.Drawing.FontFamily[] mFontFamilies;

	void OnEnable()	
	{
		ABSOLUTE_PATH = Application.dataPath + "/Fonts/FontData/";
		RELATIVE_PATH = "Assets/Fonts/FontData/";

		//
		mFontData = AssetDatabase.LoadAssetAtPath(FontsData.Path, typeof(FontsData)) as FontsData;

		//
		System.Drawing.Text.InstalledFontCollection installedFontCollection = new System.Drawing.Text.InstalledFontCollection();
		mFontFamilies = installedFontCollection.Families;

		//
		mStringFontFamilies = new string[mFontFamilies.Length];

		//
		UILabel label = target as UILabel;

		// Buil font list.
		for(int i = 0; i < mFontFamilies.Length; i++)
		{
			if(label.FontName == mFontFamilies[i].Name)
			{
				mFontSelected = i;
				mSelectedFontFamily = mFontFamilies[i];
			}

			mStringFontFamilies[i] = mFontFamilies[i].Name;
		}

		// Get sorting layer
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		mSortingLayerList = (string[])sortingLayersProperty.GetValue(null, new object[0]);

		//
		for(int i = 0; i < mSortingLayerList.Length; i++)
		{
			if(label.GetComponent<Renderer>().sortingLayerName == mSortingLayerList[i])
			{
				mSelectedSortingLayer = i;
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		// Add sorting layer
		// Get the renderer from the target object
		var renderer = (target as UILabel).gameObject.GetComponent<Renderer>();
		
		// If there is no renderer, we can't do anything
		if (renderer)
		{
			// Expose the sorting layer
			mSelectedSortingLayer = EditorGUILayout.Popup("Sorting Layer", mSelectedSortingLayer, mSortingLayerList);
			if (mSortingLayerList[mSelectedSortingLayer] != renderer.sortingLayerName) {
				Undo.RecordObject(renderer, "Edit Sorting Layer Name");
				renderer.sortingLayerName = mSortingLayerList[mSelectedSortingLayer];
				EditorUtility.SetDirty(renderer);
			}
			
			// Expose the manual sorting order
			int newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.sortingOrder);
			if (newSortingLayerOrder != renderer.sortingOrder) {
				Undo.RecordObject(renderer, "Edit Sorting Order");
				renderer.sortingOrder = newSortingLayerOrder;
				EditorUtility.SetDirty(renderer);
			}
		}



		//
		UILabel label = target as UILabel;

		//
		mFontSelected = EditorGUILayout.Popup("Font", mFontSelected, mStringFontFamilies);
		mSelectedFontFamily = mFontFamilies[mFontSelected];
		label.FontName = mSelectedFontFamily.Name;

		//
		GUI.enabled = !string.IsNullOrEmpty(label.FontName);
		GUILayout.BeginHorizontal();
		label.FontSize = EditorGUILayout.IntField("Font Size", label.FontSize);

		//
		if(mFontData != null && mFontData.ContainsData(label.FontName, label.FontSize))
		{
			if(GUILayout.Button("Select"))
			{
				//
				label.SetFontData(mFontData);
				
				//
				RefreshFontsDataLinks();
				
				//
				label.SetMaterial(mFontData.GetFontInfo(label.FontName).Material);
				
				//
				label.RebuildNeeded();
			}
		}
		else
		{
			if(GUILayout.Button("Add"))
			{
				//
				CheckPath();
				
				//
				Texture2D generatedAtlas = new Texture2D(2048,2048, TextureFormat.Alpha8, false);
				Glyph[] glyphs = FontExtractor.GenerateAtlas(label.FontName, label.FontSize, FontStyle.Normal, generatedAtlas);
				
				//
				label.SetFontData(mFontData);
				
				//
				if(!mFontData.AddFont(label.FontName, label.FontSize, glyphs, generatedAtlas))
				{
					Debug.Log("An error as occurs while adding font's data.");
				}
				else
				{
					//
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
					//
					RefreshFontsDataLinks();
					
					//
					EditorUtility.SetDirty(mFontData);
					
					//
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
					//
					label.SetMaterial(mFontData.GetFontInfo(label.FontName).Material);
					
					//
					label.RebuildNeeded();
				}
			}
		}

		GUI.enabled = true;

		GUILayout.EndHorizontal();

		label.TextColor = EditorGUILayout.ColorField("Tint Color", label.TextColor);

		label.Text = GUILayout.TextArea(label.Text, GUILayout.Height(100));

		label.CharSpacing = EditorGUILayout.IntField("Char Spacing", label.CharSpacing);

		label.LineSpacing = EditorGUILayout.IntField("Line Spacing", label.LineSpacing);

		label.MaxWidth = EditorGUILayout.IntField("Max Width", label.MaxWidth);

		label.Anchor = (TextAnchor)EditorGUILayout.EnumPopup("Anchor", label.Anchor);

		//
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	void RefreshFontsDataLinks ()
	{
		for(int i = 0; i < mFontData.mFontInfo.Count; i++)
		{
			string path = "Assets/Fonts/FontData/" + mFontData.mFontInfo[i].Name;

			if(AssetDatabase.LoadAssetAtPath(path + ".png", typeof(Texture2D)) != null)
			{
				mFontData.mFontInfo[i].SetAtlas(AssetDatabase.LoadAssetAtPath(path + ".png", typeof(Texture2D)) as Texture2D);

				TextureImporter texImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mFontData.mFontInfo[i].Atlas)) as TextureImporter;
				texImporter.textureType = TextureImporterType.Advanced;
				texImporter.isReadable = true;
				texImporter.linearTexture = true;
				texImporter.mipmapEnabled = false;
				texImporter.textureFormat = TextureImporterFormat.RGBA32;
				texImporter.wrapMode = TextureWrapMode.Clamp;
				texImporter.spriteImportMode = SpriteImportMode.None;
				texImporter.filterMode = FilterMode.Point;

				AssetDatabase.ImportAsset(path + ".png");
			}
			else
			{
				Debug.LogError("There is no Texture " + path + ".png");
			}

			if(AssetDatabase.LoadAssetAtPath(path + ".mat", typeof(Material)) != null)
			{
				Material mat = AssetDatabase.LoadAssetAtPath(path + ".mat", typeof(Material)) as Material;
				mFontData.mFontInfo[i].SetMaterial(mat);
			}
			else
			{
				Material newMat = new Material(Shader.Find("GUI/Text Shader"));
				AssetDatabase.CreateAsset(newMat, path + ".mat");
				newMat.mainTexture = AssetDatabase.LoadAssetAtPath(path + ".png", typeof(Texture2D)) as Texture2D;
				mFontData.mFontInfo[i].SetMaterial(newMat);
			}
		}
	}

	//
	void CheckPath()
	{
		bool isExists = System.IO.Directory.Exists(ABSOLUTE_PATH);
		
		if(!isExists)
		{
			System.IO.Directory.CreateDirectory(ABSOLUTE_PATH);

			mFontData = ScriptableObject.CreateInstance<FontsData>();
			AssetDatabase.CreateAsset(mFontData, FontsData.Path);
		}
		else if(AssetDatabase.LoadAssetAtPath(FontsData.Path, typeof(FontsData)) == null)
		{
			mFontData = ScriptableObject.CreateInstance<FontsData>();
			AssetDatabase.CreateAsset(mFontData, FontsData.Path);
		}

		AssetDatabase.Refresh();
	}
}
