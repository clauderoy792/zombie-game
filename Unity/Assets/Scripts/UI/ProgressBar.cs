using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
	public SpriteRenderer background;
	public SpriteRenderer foreground;
	private float mProgress = 0;

	/// <summary>
	/// Gets or sets the progress.
	/// </summary>
	/// <value>The progress.</value>
	public float Progress
	{
		get{return mProgress;}
		set
		{
			mProgress = Mathf.Clamp(value, 0, 1);

			Vector3 scale = background.transform.localScale;
			foreground.transform.localScale = Vector3.Lerp(new Vector3(0,scale.y,scale.z), scale, mProgress);
		}
	}

	//
	public void Start()
	{
		Progress = 0.0f;
	}

	//
	public void Show()
	{
		background.enabled = true;
		foreground.enabled = true;
	}

	//
	public void Hide()
	{
		background.enabled = false;
		foreground.enabled = false;
	}
}
