using UnityEngine;
using System.Collections;
using System;

public class UIPopup : AdvancedMonoBehaviour 
{
	const string ANIM_NAME = "Menu_Open";

	public UILabel title;
	public UILabel message;
	public UIButton2D[] buttons;
	private Action[] mActions;
	Animation mAnim = null;

	//
	public override void Awake()
	{
		base.Awake();

		//
		mAnim = GetComponent<Animation>();
	}

	//
	public void Start()
	{
		IsOpen = false;
		CachedGameObject.SetActive(false);
	}

	#region ACCESSORS
	
	public bool IsOpen {get;set;}
	
	#endregion

	#region OPEN/CLOSE_MANAGEMENT
	
	//
	public void Show(string aTitle, string aMessage, Action[] aActions)
	{
		//
		title.Text = aTitle;
		message.Text = aMessage;
		
		//
		mActions = aActions;

		//
		CachedGameObject.SetActive(true);

		//
		mAnim[ANIM_NAME].speed = 1.0f;
		mAnim[ANIM_NAME].normalizedTime = 0.0f;
		
		//
		mAnim.Play(ANIM_NAME);

		//
		IsOpen = true;
	}
	
	public void Hide()
	{
		// Clear actions
		mActions = null;

		//
		mAnim[ANIM_NAME].speed = -1.0f;
		mAnim[ANIM_NAME].normalizedTime = 1.0f;
		
		//
		mAnim.Play(ANIM_NAME);

		//
		IsOpen = false;

		//
		Invoke("WaitAndHide", mAnim[ANIM_NAME].length);
	}

	//
	void WaitAndHide()
	{
		//
		CachedGameObject.SetActive(false);
	}
	
	#endregion

	//
	void OnButtonClick (string id) 
	{
		int castedId = 0;
		if(int.TryParse(id, out castedId))
		{
			if(mActions != null && mActions.Length > castedId)
			{
				mActions[castedId]();
			}
		}
		else
		{
			Debug.LogWarning("Invalid Parameter");
		}

		//
		Hide();
	}
}
