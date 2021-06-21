using UnityEngine;
using System.Collections;

public class XMLAssetManager : MonoBehaviour
{
	#region STATIC_MEMBERS

	private static XMLAssetManager sInstance;

	#endregion

	#region PUBLIC_MEMBERS
	
	public TextAsset mNameXML;
	
	public TextAsset mLocalizationXML;
	
	#endregion

	#region ACCESSORS

	public static XMLAssetManager Instance
	{
		get {return sInstance;}
	}

	#endregion

	#region MONO_METHODS

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
		}
		else
		{
			Debug.Log("There is already an instance of the XMLAssetManager in the scene.");
		}
	}

	#endregion
}
