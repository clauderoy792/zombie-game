using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIStretch2D))]
[CanEditMultipleObjects]
public class UIStretch2DInspector : Editor 
{
	public override void OnInspectorGUI ()
	{
		//
		UIStretch2D stretch = target as UIStretch2D;

		stretch.Type = (UIStretch2D.StretchType)EditorGUILayout.EnumPopup("Stretch Type", stretch.Type);
		
		//
		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
