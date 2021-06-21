using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	#region MEMBERS
	
	private static InputManager mInstance;
	
	private IInputs mInputs;

	private Collider2D mPressedObject;

	#endregion
	
	#region ACCESSORS
	
	public static InputManager Instance
	{
		get {return mInstance;}
	}
	
	public IInputs Inputs
	{
		get {return mInputs;}
	}
	
	#endregion
	
	#region MONO_METHODS

	void Awake()
	{
		//
		mInstance = this;

		//Assign inputs.
		if (Application.isEditor || 
		    Application.platform == RuntimePlatform.WindowsPlayer ||
		    Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			mInputs = new KeyboardInputs();
		}
		else
		{
			mInputs = new MobileInputs();
		}
	}

	void Update()
	{
		if(Inputs.IsTouchDown())
		{
			RaycastHit2D hit = GetRaycast();

			if(hit.collider != null)
			{
				hit.collider.gameObject.SendMessage("OnTouchDown2D", SendMessageOptions.DontRequireReceiver);
				mPressedObject = hit.collider;

				if(hit.collider.tag == "UI")
				{
					Pan.enabled = false;
				}
				else
				{
					Pan.enabled = true;
				}
			}
		}

		if(Inputs.IsTouch())
		{
			RaycastHit2D hit = GetRaycast();
			
			if(hit.collider != null)
			{
				hit.collider.gameObject.SendMessage("OnTouch2D", SendMessageOptions.DontRequireReceiver);
			}
		}

		if(Inputs.IsTouchUp())
		{
			//
			Pan.enabled = true;

			//
			RaycastHit2D hit = GetRaycast();

			if(hit.collider != null)
			{
				hit.collider.gameObject.SendMessage("OnTouchUp2D", SendMessageOptions.DontRequireReceiver);
				
				if(mPressedObject == hit.collider)
				{
					hit.collider.gameObject.SendMessage("OnClick2D", SendMessageOptions.DontRequireReceiver);
				}
				
				mPressedObject = null;
			}
		}

		/*
		if(!Blueprint.isVisible)
		{
			if(Inputs.IsTouchPressed())
			{
				mStartPos = Inputs.GetScreenPosition();
			}

			if(Inputs.IsTouchReleased())
			{
				if(!UIManager.Instance.IsRadialMenuOpen)
				{
					mEndPos = Inputs.GetScreenPosition();

					if(Vector2.Distance(mStartPos, mEndPos) <= mClickThreshold)
					  {
						if(Camera.main != null)
						{
							Ray worldRay = Camera.main.ScreenPointToRay(Inputs.GetScreenPosition());
							Ray guiRay = mNCam.ScreenPointToRay(Inputs.GetScreenPosition());

							RaycastHit2D worldHit = Physics2D.Raycast(worldRay.origin, worldRay.direction);
							RaycastHit guiHit;
							Physics.Raycast(guiRay, out guiHit);
							
							if(worldHit.collider != null && guiHit.collider != null && 
							   worldHit.collider.tag == "TouchHandler" && guiHit.collider.tag == "TouchHandler")
							{
								Vector2 inputPos = Inputs.GetScreenPosition();
								float x = inputPos.x - (Screen.width/2.0f);
								float y = inputPos.y - (Screen.height/2.0f);
								
								Vector2 finalPos = new Vector2(mNGUIScreenSize.x * x / Screen.width, mNGUIScreenSize.y * y / Screen.height);
								
								// UIManager.Instance.ShowRadialMenu(finalPos);
							}
						}
					}
				}
				else
				{
					UIManager.Instance.HideRadialMenu();
				}
			}
		}
		*/
	}

	RaycastHit2D GetRaycast()
	{
		Ray worldRay = Camera.main.ScreenPointToRay(Inputs.GetScreenPosition());
		return Physics2D.Raycast(worldRay.origin, worldRay.direction);
	}
	
	#endregion
}
