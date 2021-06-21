using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FxManager
{
	#region CONSTANTS

	const string PATH = "VFX/";

	#endregion

	#region STATIC_MEMBERS

	private static FxManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	Dictionary<EFxType, GameObject> mFx;

	#endregion

	#region ACCESSORS

	public static FxManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new FxManager();
			}

			return sInstance;
		}
	}
	#endregion

	#region CONSTRUCTORS

	private FxManager ()
	{
		mFx = new Dictionary<EFxType, GameObject>()
		{
			{EFxType.FightCloud, GetResourceByName("FightingCloud")},
		};
	}

	#endregion

	#region PUBLIC_METHODS

	public GameObject PlayFxAtPoint(EFxType aFxType, Vector2 aPos)
	{
		return AutomaticPoolSystem.Instance.InstantiateObject(mFx[EFxType.FightCloud], aPos, Quaternion.identity) as GameObject;
	}

	#endregion

	#region PRIVATE_METHODS

	GameObject GetResourceByName(string aFileName)
	{
		return Resources.Load(PATH + aFileName) as GameObject;
	}

	#endregion
}
