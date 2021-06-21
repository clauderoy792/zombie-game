using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterVisual : MonoBehaviour
{
	public ECharacterType characterType = ECharacterType.VirusScientist;
	public SpriteRenderer skin;
	public SpriteRenderer clothing;
	public SpriteRenderer secondaryClothing;

	private ECivilianClothingType mCivilType;
	private Color mCivilShirtColor;
	private Color mCivilPantColor;


	void Awake()
	{
		mCivilType = (ECivilianClothingType)Random.Range(0, (int)ECivilianClothingType.COUNT);
		mCivilShirtColor = new Color(Random.value, Random.value, Random.value);
		mCivilPantColor = new Color(Random.value, Random.value, Random.value);
	}

	#region ACCESSORS

	public Color CivilShirtColor
	{
		get{return mCivilShirtColor;}
	}

	public Color CivilPantColor
	{
		get{return mCivilPantColor;}
	}

	public ECivilianClothingType CivilType
	{
		get{return mCivilType;}
	}

	public ECharacterType CharacterType
	{
		get{return characterType;}
	}

	#endregion

	#region PUBLIC_METHODS

	// -- DO NOT REMOVE, CALLED BY ANIMATION EVENTS --
	public void SetAnimationFrame(int aFrame)
	{
		if(characterType == ECharacterType.Civilian)
		{
			// Set pants
			clothing.sprite = GraphicsManager.Instance.GetCivilianSprites(mCivilType, EClothingPart.Pants, aFrame);
			secondaryClothing.sprite = GraphicsManager.Instance.GetCivilianSprites(mCivilType, EClothingPart.Shirt, aFrame);

			clothing.color = mCivilPantColor;
			secondaryClothing.color = mCivilShirtColor;
		}
		else
		{
			clothing.sprite = GraphicsManager.Instance.GetCostumeSpriteForAnimation(characterType, aFrame);
			clothing.color = Color.white;
		}
	}

	#endregion

	#region SERIALIZATION

	public void Deserialize(CharacterVisualSerializationInfo aInfo)
	{
		characterType 		= aInfo.characterType;
		mCivilType 			= aInfo.mCivilType;
		mCivilShirtColor 	= aInfo.mCivilShirtColor.ToColor();
		mCivilPantColor 	= aInfo.mCivilPantColor.ToColor();
		skin.color			= aInfo.mSkinColor.ToColor();
	}

	public CharacterVisualSerializationInfo Serialize()
	{
		return new CharacterVisualSerializationInfo(this);
	}

	#endregion
}
