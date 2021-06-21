using UnityEngine;
using System.Collections;

public class BorderManager : MonoBehaviour
{
	static BorderManager _instance = null;
	public UITiled2D[] borders = null;
	public Rect mBorder;
	const int DEPTH_LAYER = 0;
	public const int INIT_SIZE = 5;

	#region ACCESSORS
	
	public static BorderManager Instance
	{
		get{return _instance;}
	}
	
	#endregion
	
	
	#region MONO_METHODS
	
	public Rect Border
	{
		get {return mBorder;}
	}
	
	//
	public void Awake()
	{
		_instance = this;
		
		SetBorder(new Rect(PathFinder.GRID_WIDTH/2-INIT_SIZE/2,0,INIT_SIZE,INIT_SIZE));
	}
	
	#endregion
	
	//
	public void SetBorder(Rect aRect)
	{
		//
		mBorder = aRect;

		//
		transform.localPosition = new Vector3(aRect.x, aRect.y, DEPTH_LAYER);

		//Right
		borders[0].transform.localPosition = new Vector3(aRect.width, 0, 0);
		borders[0].SizeY = (int)aRect.height;

		//Left
		borders[1].transform.localPosition = new Vector3(0, 0, 0);
		borders[1].SizeY = (int)aRect.height;

		//Top
		borders[2].transform.localPosition = new Vector3(0, aRect.height + 4, 0);
		borders[2].SizeY = (int)aRect.width;
	}
}
