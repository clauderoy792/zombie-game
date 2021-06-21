using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager
{
	#region CONSTANTS

	const string PATH = "Sounds/";

	#endregion

	#region STATIC_MEMBERS

	private static SoundManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS

	Dictionary<ESoundType, AudioClip> mSounds;

	#endregion

	#region ACCESSORS

	public static SoundManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new SoundManager();
			}

			return sInstance;
		}
	}
	#endregion

	#region CONSTRUCTORS

	private SoundManager ()
	{
		mSounds = new Dictionary<ESoundType, AudioClip>()
		{
			{ESoundType.ButtonPress, null}
		};
	}

	#endregion

	#region PUBLIC_METHODS

	public void PlayClipAtPoint(ESoundType aSoundType, Vector2 aPos)
	{
		AudioSource.PlayClipAtPoint(mSounds[aSoundType], aPos);
	}

	#endregion

	#region PRIVATE_METHODS

	AudioClip GetResource(string aName)
	{
		return Resources.Load(PATH + aName) as AudioClip;
	}

	#endregion
}
