using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(UISliced2D))]
[CanEditMultipleObjects]
public class UISliced2DInspector : Editor 
{
	string[] mSortingLayerList;
	int mSelectedSortingLayer = 0;

	void OnEnable()	
	{
		//
		UISliced2D sliced = target as UISliced2D;
		
		// Get sorting layer
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		mSortingLayerList = (string[])sortingLayersProperty.GetValue(null, new object[0]);
		
		//
		for(int i = 0; i < mSortingLayerList.Length; i++)
		{
			if(sliced.GetComponent<Renderer>().sortingLayerName == mSortingLayerList[i])
			{
				mSelectedSortingLayer = i;
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		// Add sorting layer
		// Get the renderer from the target object
		var renderer = (target as UISliced2D).gameObject.GetComponent<Renderer>();
		
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
		UISliced2D sliced = target as UISliced2D;

		sliced.SlicedSprite = EditorGUILayout.ObjectField("Sprite", sliced.SlicedSprite, typeof(Sprite), false) as Sprite;

		GUILayout.BeginHorizontal();
		sliced.TopBorder = EditorGUILayout.IntField("Top", sliced.TopBorder);
		sliced.BottomBorder = EditorGUILayout.IntField("Bottom", sliced.BottomBorder);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		sliced.LeftBorder = EditorGUILayout.IntField("Left", sliced.LeftBorder);
		sliced.RightBorder = EditorGUILayout.IntField("Right", sliced.RightBorder);
		GUILayout.EndHorizontal();

		//
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
