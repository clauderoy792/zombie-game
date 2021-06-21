using UnityEngine;
using System.Collections;

//
public class AdvancedMonoBehaviour : MonoBehaviour 
{
	#region MEMBERS

	private Transform mTransform;
	private GameObject mGameObject;
	private Renderer mRenderer;

	#endregion


	#region ACCESSORS

	public Transform CachedTransform
	{
		get{return mTransform;}
		set{mTransform = value;}
	}

	public GameObject CachedGameObject
	{
		get{return mGameObject;}
		set{mGameObject = value;}
	}

	public Renderer CachedRenderer
	{
		get{return mRenderer;}
		set{mRenderer = value;}
	}

	#endregion


	#region MONO

	public virtual void Awake()
	{
		mGameObject = gameObject;
		mTransform = transform;
		mRenderer = GetComponent<Renderer>();
	}

	#endregion

	#region NEW METHODS

	//
	public new Object Instantiate(Object original)
	{
		if(original is GameObject)
		{
			if((original as GameObject).transform)
			{
				return InstantiateObject(original as GameObject, (original as GameObject).transform.position, (original as GameObject).transform.rotation);
			}
			else
			{
				return InstantiateObject(original as GameObject, Vector3.zero, Quaternion.identity);
			}
		}
		else
		{
			return GameObject.Instantiate(original);
		}
	}

	//
	public new Object Instantiate(Object original, Vector3 position, Quaternion rotation)
	{
		if(original is GameObject)
		{
			return InstantiateObject(original as GameObject, position, rotation);
		}
		else
		{
			return GameObject.Instantiate(original, position, rotation);
		}
	}

	//
	public new void Destroy(Object obj)
	{
		if(obj is GameObject)
		{
			DestroyObject(obj as GameObject, 0);
		}
		else
		{
			GameObject.Destroy(obj);
		}
	}

	//
	public new void Destroy(Object obj, float t)
	{
		if(obj is GameObject)
		{
			DestroyObject(obj as GameObject, t);
		}
		else
		{
			GameObject.Destroy(obj, t);
		}
	}

	#endregion

	#region OBJECT MANAGEMENT

	private Object InstantiateObject(GameObject obj, Vector3 pos, Quaternion rot)
	{
		return AutomaticPoolSystem.Instance.InstantiateObject(obj, pos, rot);
	}

	private void DestroyObject(GameObject obj, float t)
	{
		if(t > 0.0f)
		{
			StartCoroutine( AutomaticPoolSystem.Instance.WaitAndDestroyObject(obj, t) );
		}
		else
		{
			AutomaticPoolSystem.Instance.DestroyObject(obj);
		}
	}

	#endregion
}



