using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour 
{
	#region CONSTANT

	static UIManager mInstance;

	#endregion

	#region PUBLIC_MEMBERS

	public UIRadialMenu radialMenu;
	public UIPopup popup;
	public UIButton2D zoomBtn;
	public Sprite zoomIcon;
	public Sprite unzoomIcon;

	#endregion

	#region PRIVATE_MEMBERS

	private bool mIsZoomed = false;

	#endregion

	#region ACCESSORS

	public static UIManager Instance
	{
		get{return mInstance;}
	}

	public bool IsZoomed
	{
		get{return mIsZoomed;}
	}

	#endregion

	#region MONO_METHODS

	//
	public void Awake()
	{
		mInstance = this;
	}

	#endregion

	#region UI_MANAGEMENT
	
	//
	public void ShowRadialMenu(Vector2 aPosition, UIRadialMenu.MenuType aMenuType, Action[] aActions)
	{	
		if(!Blueprint.isVisible)
		{
			//
			radialMenu.CachedTransform.localPosition = new Vector3(aPosition.x, aPosition.y, -10);
			radialMenu.Show(aMenuType, aActions);
		}
	}

	//
	public void ShowRadialMenu(Vector2 aPosition, UIRadialMenu.MenuType aMenuType, Action<int>[] aActions)
	{
		if(!Blueprint.isVisible)
		{
			//
			radialMenu.CachedTransform.localPosition = new Vector3(aPosition.x, aPosition.y, -10);
			radialMenu.Show(aMenuType, aActions);
		}
	}

	//
	public void HideRadialMenu()
	{
		//
		radialMenu.Hide();
	}

	//
	public void ShowPopup(string aTitle, string aMessage, Action[] aActions)
	{
		//
		popup.Show(aTitle, aMessage, aActions);
	}

	//
	public void HidePopup()
	{
		//
		popup.Hide();
	}

	//
	public void SetZoom()
	{
		mIsZoomed = !mIsZoomed;

		if(mIsZoomed)
		{
			UIScreen2D.Game.localScale = new Vector3(2,2,1);

			Vector3 pos = UIScreen2D.Game.localPosition;
			pos.x += pos.x;
			pos.y += pos.y;
			pos.x = (int)pos.x + 0.01f;
			pos.y = (int)pos.y + 0.01f;

			UIScreen2D.Game.localPosition = pos;
			zoomBtn.icon.sprite = unzoomIcon;
		}
		else
		{
			UIScreen2D.Game.localScale = new Vector3(1,1,1);
			
			Vector3 pos = UIScreen2D.Game.localPosition;
			pos.x -= pos.x/2;
			pos.y -= pos.y/2;
			pos.x = (int)pos.x + 0.01f;
			pos.y = (int)pos.y + 0.01f;
			
			UIScreen2D.Game.localPosition = pos;
			zoomBtn.icon.sprite = zoomIcon;
		}
	}

	#endregion
}
