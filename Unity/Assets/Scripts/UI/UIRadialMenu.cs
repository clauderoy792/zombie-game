using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIRadialMenu : AdvancedMonoBehaviour 
{
	public enum MenuType
	{
		BUILD,
	}

	public GameObject[] baseButtons;
	public Sprite[] buildSprites;
	private Dictionary<MenuType, Sprite[]> mSprites;
	private List<UIRadialButton2D> mButtons;
	private Animation mAnim;
	const string MENU_OPEN = "Menu_Open";
	const string MENU_CLOSE = "Menu_Close";

	#region MONO_METHODS

	//
	public override void Awake ()
	{
		base.Awake ();

		//
		mSprites = new Dictionary<MenuType, Sprite[]>()
		{
			{MenuType.BUILD, buildSprites},
		};

		//
		mAnim = GetComponent<Animation>();
	}

	//
	void Start()
	{
		IsOpen = false;
		CachedGameObject.SetActive(false);
	}

	#endregion

	#region ACCESSORS

	public bool IsOpen {get;set;}

	#endregion

	#region SHOW/HIDE MANAGEMENT

	//
	public void Show(MenuType aMenuType, Action[] aActions)
	{
		//
		if(IsInvoking("AnimationDonePlaying"))
		{
			CancelInvoke("AnimationDonePlaying");
		}
	
		//
		CachedGameObject.SetActive(true);

		// Clean old buttons.
		if(mButtons != null && mButtons.Count > 0)
		{
			for(int i = mButtons.Count-1; i >= 0; i--)
			{
				Destroy(mButtons[i].gameObject);
				mButtons.RemoveAt(i);
			}
		}

		// Create new buttons
		InitializeMenu(aMenuType, aActions);

		//
		mAnim.Play(MENU_OPEN);

		//
		IsOpen = true;

		//
		Invoke("AnimationDonePlaying", mAnim[MENU_OPEN].length);
	}

	//
	public void Show(MenuType aMenuType, Action<int>[] aActions)
	{
		//
		if(IsInvoking("AnimationDonePlaying"))
		{
			CancelInvoke("AnimationDonePlaying");
		}
		
		//
		CachedGameObject.SetActive(true);
		
		// Clean old buttons.
		if(mButtons != null && mButtons.Count > 0)
		{
			for(int i = mButtons.Count-1; i >= 0; i--)
			{
				Destroy(mButtons[i].gameObject);
				mButtons.RemoveAt(i);
			}
		}
		
		// Create new buttons
		InitializeMenu(aMenuType, aActions);
		
		//
		mAnim.Play(MENU_OPEN);
		
		//
		IsOpen = true;
		
		//
		Invoke("AnimationDonePlaying", mAnim[MENU_OPEN].length);
	}

	//
	public void Hide()
	{
		//
		for(int i = 0; i < mButtons.Count; i++)
		{
			mButtons[i].GetComponent<Collider2D>().enabled = false;
		}

		//
		mAnim.Play(MENU_CLOSE);

		//
		IsOpen = false;

		//
		Invoke("AnimationDonePlaying", mAnim[MENU_CLOSE].length);
	}

	//
	void AnimationDonePlaying()
	{
		if(IsOpen)
		{
			for(int i = 0; i < mButtons.Count; i++)
			{
				mButtons[i].GetComponent<Collider2D>().enabled = true;
			}
		}
		else
		{
			CachedGameObject.SetActive(false);
		}
	}

	#endregion

	#region MENU INITIALIZATION

	//
	void InitializeMenu (MenuType aMenuType, Action[] aActions)
	{
		//
		mButtons = new List<UIRadialButton2D>();

		//
		int count = mSprites[aMenuType].Length;

		if(count <= 1 || count > 10)
		{
			Debug.LogError("Radial menu with " + count + " buttons is not supported!");
			return;
		}

		float degrees = 360.0f / count;
	
		//
		for(int i = 0; i < count; i++)
		{
			GameObject rotation = new GameObject("Rotation");
			rotation.transform.parent = CachedTransform;
			rotation.transform.localPosition = Vector3.zero;
			rotation.transform.localScale = Vector3.one;

			GameObject go = Instantiate(baseButtons[count-2]) as GameObject;
			go.transform.parent = rotation.transform;
			go.transform.localPosition = new Vector3(0,65,0);
			go.transform.localScale = Vector3.one;

			rotation.transform.localRotation = Quaternion.Euler(0,0, degrees * i);

			UIRadialButton2D radialBtn = go.GetComponent<UIRadialButton2D>();
			radialBtn.icon.transform.localRotation = Quaternion.Euler(0,0, -(degrees * i));
			radialBtn.icon.sprite = mSprites[aMenuType][i];
			radialBtn.ID = i;

			//
			if(aActions != null && aActions.Length > i)
			{
				radialBtn.Action = aActions[i];
			}
			else
			{
				radialBtn.Action = null;
			}

			//
			mButtons.Add(radialBtn);
		}
	}

	//
	void InitializeMenu (MenuType aMenuType, Action<int>[] aActions)
	{
		//
		mButtons = new List<UIRadialButton2D>();
		
		//
		int count = mSprites[aMenuType].Length;
		
		if(count <= 1 || count > 10)
		{
			Debug.LogError("Radial menu with " + count + " buttons is not supported!");
			return;
		}
		
		float degrees = 360.0f / count;
		
		//
		for(int i = 0; i < count; i++)
		{
			GameObject rotation = new GameObject("Rotation");
			rotation.transform.parent = CachedTransform;
			rotation.transform.localPosition = Vector3.zero;
			rotation.transform.localScale = Vector3.one;
			
			GameObject go = Instantiate(baseButtons[count-2]) as GameObject;
			go.transform.parent = rotation.transform;
			go.transform.localPosition = new Vector3(0,65,0);
			go.transform.localScale = Vector3.one;
			
			rotation.transform.localRotation = Quaternion.Euler(0,0, degrees * i);
			
			UIRadialButton2D radialBtn = go.GetComponent<UIRadialButton2D>();
			radialBtn.icon.transform.localRotation = Quaternion.Euler(0,0, -(degrees * i));
			radialBtn.icon.sprite = mSprites[aMenuType][i];
			radialBtn.ID = i;
			
			//
			if(aActions != null && aActions.Length > i)
			{
				radialBtn.ActionWithParam = aActions[i];
			}
			else
			{
				radialBtn.Action = null;
			}
			
			//
			mButtons.Add(radialBtn);
		}
	}

	#endregion
}
