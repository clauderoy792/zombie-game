using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColorConverter : EditorWindow
{
	float r = 255;
	float g = 255;
	float b = 255;
	float a = 255;
	bool valueAreInFullRGBA = true;

	//
	[MenuItem("Tools/Color Converter")]
	static void init () {
		GetWindow(typeof(ColorConverter));
	}
	
	//
	void OnGUI ()
	{
		r = EditorGUILayout.FloatField("Red Channel", r);
		g = EditorGUILayout.FloatField("Green Channel", g);
		b = EditorGUILayout.FloatField("Blue Channel", b);
		a = EditorGUILayout.FloatField("Alpha Channel", a);

		GUILayout.Space(10);

		valueAreInFullRGBA = GUILayout.Toggle(valueAreInFullRGBA, "Value given are in 255 RGBA");

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Result:");

		GUILayout.BeginVertical();
		string result = GUILayout.TextField( string.Format("new Color({0}f,{1}f,{2}f,{3}f)", 
		                               valueAreInFullRGBA ? Mathf.Round((r/255.0f) * 1000f) / 1000f : (int)(r*255),
		                               valueAreInFullRGBA ? Mathf.Round((g/255.0f) * 1000f) / 1000f : (int)(g*255), 
		                               valueAreInFullRGBA ? Mathf.Round((b/255.0f) * 1000f) / 1000f : (int)(b*255),
		                               valueAreInFullRGBA ? Mathf.Round((a/255.0f) * 1000f) / 1000f : (int)(a*255)) );



		if(GUILayout.Button("Copy to clipboard"))
		{
			EditorGUIUtility.systemCopyBuffer = result;
		}

		GUILayout.EndVertical();

		if(valueAreInFullRGBA)
		{
			GUI.color = new Color( Mathf.Round((r/255.0f) * 1000f) / 1000f, Mathf.Round((g/255.0f) * 1000f) / 1000f, Mathf.Round((b/255.0f) * 1000f) / 1000f, Mathf.Round((a/255.0f) * 1000f) / 1000f);
		}	
		else
		{
			GUI.color = new Color(r,g,b,a);
		}


		GUILayout.Box("", GUILayout.Width(50), GUILayout.Height(50));
		GUI.color = Color.white;

		GUILayout.EndHorizontal();
	}
}
