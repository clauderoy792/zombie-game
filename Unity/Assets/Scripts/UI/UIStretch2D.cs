using UnityEngine;
using System.Collections;

public class UIStretch2D : MonoBehaviour 
{
	public enum StretchType
	{
		NONE,
		HORIZONTAL,
		VERTICAL,
		BOTH
	}

	//
	[SerializeField]private StretchType mStretchType = StretchType.NONE;
	private bool mRebuildNeeded = false;

	//
	void OnEnable () 
	{
		mRebuildNeeded = true;
	}

	//
	void Update()
	{
		if(mRebuildNeeded)
		{
			StretchGameObject();
		}
	}


	/// <summary>
	/// Gets or sets the stretch type.
	/// </summary>
	/// <value>The type.</value>
	public StretchType Type
	{
		get{return mStretchType;}
		set
		{
			if(mStretchType != value)
			{
				mStretchType = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					StretchGameObject();
				}
			}
		}
	}

	//
	void StretchGameObject()
	{
		//
		transform.localPosition = new Vector3(0,0,transform.localPosition.z);
		
		//
		switch(mStretchType)
		{
		case StretchType.HORIZONTAL:
			transform.localScale = new Vector3(UIScreen2D.Width, 1, 1) / UIScreen2D.Factor;
			break;
		case StretchType.VERTICAL:
			transform.localScale = new Vector3(1, UIScreen2D.Height, 1) / UIScreen2D.Factor;
			break;
		case StretchType.BOTH:
			transform.localScale = new Vector3(UIScreen2D.Width, UIScreen2D.Height, 1) / UIScreen2D.Factor;
			break;
		default :
			transform.localScale = new Vector3(1, 1, 1) / UIScreen2D.Factor;
			break;
		}
	}
}
