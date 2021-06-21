using UnityEngine;
using System.Collections;

public class UIScreen2D : MonoBehaviour 
{
	//
	public Transform mGui;
	public Transform mGame;
	public Transform mBackground;

	Camera mCamera;
	int mScaleFactor = 1;
	static int mMinHeight = 320;
	static int mFactor = 1;
	static int mNearestWidth = 480;
	static int mNearestHeight = 320;
	static int mCurrentWidth = 480;
	static int mCurrentHeight = 320;
	static Vector3 mScale;
	public static Transform Gui;
	public static Transform Game;
	public static Transform Background;
	const int MULTIPLIER = 100;

	//
	void Awake()
	{
		//
		InitializeScreenSize();

		//
		Gui = mGui;
		Game = mGame;
		Background = mBackground;
	}

	//
	void OnEnable()
	{
		InitializeScreenSize();
	}

	//
	void InitializeScreenSize()
	{
		//
		mCamera = GetComponent<Camera>();

		//
		mCurrentWidth = Screen.width;
		mCurrentHeight = Screen.height;

		//
		mFactor = Mathf.RoundToInt((float)mCurrentHeight / (float)mMinHeight);

		//
		mScaleFactor = mFactor < 1 ? 1 : (int)mFactor;

		//
		mNearestHeight = Mathf.RoundToInt((float)mMinHeight * mScaleFactor);

		//
		mNearestWidth = Mathf.RoundToInt((float)mCurrentHeight * mCurrentWidth / mCurrentHeight);

		//
		float size = 1.0f / mCurrentHeight * mScaleFactor * MULTIPLIER;

		//
		mCamera.orthographicSize = 0.5f * MULTIPLIER;

		//
		transform.localScale = new Vector3(size, size, size);

		//
		for(int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			float depth = child.localPosition.z;
			child.localPosition = new Vector3(-mCurrentWidth/2 + 0.01f,-mCurrentHeight/2 + 0.01f, depth) / mFactor;
			child.localScale = Vector3.one;
		}

		//
		mScale = transform.localScale;
	}

	#region ACCESSORS

	/// <summary>
	/// Gets the factor.
	/// </summary>
	/// <value>The factor.</value>
	public static int Factor
	{
		get{return mFactor;}
	}

	/// <summary>
	/// Gets the canvas width in pixel.
	/// </summary>
	/// <value>The width.</value>
	public static int Width
	{
		get{return mCurrentWidth;}
	}

	/// <summary>
	/// Gets the canvas height in pixel.
	/// </summary>
	/// <value>The height.</value>
	public static int Height
	{
		get{return mCurrentHeight;}
	}

	/// <summary>
	/// Gets the width of the nearest base width mutiplier.
	/// </summary>
	/// <value>The width of the nearest.</value>
	public static int NearestWidth
	{
		get{return mNearestHeight;}
	}

	/// <summary>
	/// Gets the height of the nearest base height multiplier.
	/// </summary>
	/// <value>The height of the nearest.</value>
	public static int NearestHeight
	{
		get{return mNearestHeight;}
	}

	/// <summary>
	/// Gets the base height.
	/// </summary>
	/// <value>The height of the base.</value>
	public static int BaseHeight
	{
		get{return mMinHeight;}
	}

	/// <summary>
	/// Gets the scale.
	/// </summary>
	/// <value>The scale.</value>
	public static Vector3 Scale
	{
		get{return mScale;}
	}

	#endregion
}
