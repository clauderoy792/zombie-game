using UnityEngine;
using UnityEditor;
using System.Collections;

public class WidgetCreator : Editor 
{
	[MenuItem("   UI   /Create Label")]
	static void CreateLabel()
	{
		// get selection
		GameObject go = Selection.activeGameObject;

		//
		GameObject label = new GameObject("Label");
		UILabel lbl = label.AddComponent<UILabel>();
		lbl.Text = "Hello World";

		//
		if(go)
		{
			label.transform.parent = go.transform;
		}

		//
		label.transform.localPosition = Vector3.zero;
		label.transform.localScale = Vector3.one;

		//
		Selection.activeGameObject = label;
	}

	[MenuItem("   UI   /Create Tiled Sprite")]
	static void CreateTiled()
	{
		// get selection
		GameObject go = Selection.activeGameObject;
		
		//
		GameObject tiled = new GameObject("Tiled");
		UITiled2D t = tiled.AddComponent<UITiled2D>();
		
		//
		if(go)
		{
			tiled.transform.parent = go.transform;
		}
		
		//
		tiled.transform.localPosition = Vector3.zero;
		tiled.transform.localScale = Vector3.one;

		//
		Selection.activeGameObject = tiled;
	}

	[MenuItem("   UI   /Create Sliced Sprite")]
	static void CreateSliced()
	{
		// get selection
		GameObject go = Selection.activeGameObject;
		
		//
		GameObject sliced = new GameObject("Sliced");
		UISliced2D s = sliced.AddComponent<UISliced2D>();
		
		//
		if(go)
		{
			sliced.transform.parent = go.transform;
		}
		
		//
		sliced.transform.localPosition = Vector3.zero;
		sliced.transform.localScale = Vector3.one;

		//
		Selection.activeGameObject = sliced;
	}

	[MenuItem("   UI   /Create Button")]
	static void CreateButton()
	{
		// get selection
		GameObject go = Selection.activeGameObject;
		
		// Create button
		GameObject button = new GameObject("Button");
		UIButton2D btn = button.AddComponent<UIButton2D>();

		// Create and add label
		GameObject label = new GameObject("Label");
		UILabel lbl = label.AddComponent<UILabel>();
		lbl.Text = "Hello World";
		label.transform.parent = button.transform;
		label.transform.localPosition = Vector3.zero;
		label.transform.localScale = Vector3.one;

		// Create and add background
		GameObject background = new GameObject("Background");
		SpriteRenderer sprite = background.AddComponent<SpriteRenderer>();
		background.transform.parent = button.transform;
		background.transform.localPosition = Vector3.zero;
		background.transform.localScale = Vector3.one;

		//
		if(go)
		{
			button.transform.parent = go.transform;
		}
		
		//
		button.transform.localPosition = Vector3.zero;
		button.transform.localScale = Vector3.one;

		//
		Selection.activeGameObject = button;
	}
}
