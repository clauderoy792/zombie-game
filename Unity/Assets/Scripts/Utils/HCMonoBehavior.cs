using UnityEngine;
using System.Collections;

public class HCMonoBehavior : MonoBehaviour {
	
	#region PROTECTED_MEMBERS
	
	protected Transform mTransform;
	protected GameObject mGameObject;
	protected Renderer mRenderer;
	
	#endregion
	
	#region ACCESSORS
	
	public Transform Transform
	{
		get{return mTransform;}
	}
	
	public GameObject GameObject
	{
		get{return mGameObject;}
	}
	
	public Renderer Renderer
	{
		get{return mRenderer;}
	}
	
	public Vector3 TransformPosition
	{
		get{return mTransform.position;}
		set
		{
			mTransform.position = value;
		}
	}

	public Vector3 LocalTransformPosition
	{
		get{return mTransform.localPosition;}
		set
		{
			mTransform.localPosition = value;
		}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected virtual void Awake()
	{
		mTransform = transform;
		mGameObject = gameObject;
		mRenderer = GetComponent<Renderer>();
	}
	
	#endregion
}
