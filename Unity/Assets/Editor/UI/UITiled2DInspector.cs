using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(UITiled2D))]
[CanEditMultipleObjects]
public class UITiled2DInspector : Editor
{
	string[] mSortingLayerList;
	int mSelectedSortingLayer = 0;
	
	void OnEnable()	
	{
		//
		UITiled2D tiled = target as UITiled2D;
		
		// Get sorting layer
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		mSortingLayerList = (string[])sortingLayersProperty.GetValue(null, new object[0]);
		
		//
		for(int i = 0; i < mSortingLayerList.Length; i++)
		{
			if(tiled.GetComponent<Renderer>().sortingLayerName == mSortingLayerList[i])
			{
				mSelectedSortingLayer = i;
			}
		}
	}
	
	public override void OnInspectorGUI ()
	{
		// Add sorting layer
		// Get the renderer from the target object
		var renderer = (target as UITiled2D).gameObject.GetComponent<Renderer>();
		
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
		UITiled2D tiled = target as UITiled2D;
		
		tiled.TiledSprite = EditorGUILayout.ObjectField("Sprite", tiled.TiledSprite, typeof(Sprite), false) as Sprite;
		tiled.SizeX = EditorGUILayout.IntField("Size X", tiled.SizeX);
		tiled.SizeY = EditorGUILayout.IntField("Size Y", tiled.SizeY);
		
		//
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
