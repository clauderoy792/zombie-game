using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterVisualSerializationInfo {

	public ECharacterType characterType;
	public ECivilianClothingType mCivilType;
	public SerializableColor mCivilShirtColor;
	public SerializableColor mCivilPantColor;
	public SerializableColor mSkinColor;
	
	public CharacterVisualSerializationInfo(CharacterVisual visual)
	{
		characterType 		= visual.CharacterType;
		mCivilType		 	= visual.CivilType;
		mCivilShirtColor 	= new SerializableColor(visual.CivilShirtColor);
		mCivilPantColor 	= new SerializableColor(visual.CivilPantColor);
		mSkinColor			= new SerializableColor(visual.skin.color);
	}
}
