using UnityEngine;
using System.Collections;

public class UIAnchor : MonoBehaviour 
{
	public enum Anchor
	{
		TOP_LEFT,
		TOP_CENTER,
		TOP_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT
	}

	public Anchor anchor = Anchor.MIDDLE_CENTER;
	public Vector2 offset;

	// Use this for initialization
	void Start () 
	{
		Vector3 pos = Vector3.zero;
		float zDepth = transform.localPosition.z;

		switch(anchor)
		{
		case Anchor.TOP_LEFT : pos = new Vector3(offset.x, (UIScreen2D.Height / UIScreen2D.Factor) + offset.y, zDepth); break;
		case Anchor.TOP_CENTER : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor)/2 + offset.x, (UIScreen2D.Height / UIScreen2D.Factor) + offset.y, zDepth); break;
		case Anchor.TOP_RIGHT : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor) + offset.x, (UIScreen2D.Height / UIScreen2D.Factor) + offset.y, zDepth); break;

		case Anchor.MIDDLE_LEFT : pos = new Vector3(offset.x, (UIScreen2D.Height / UIScreen2D.Factor)/2 + offset.y, zDepth); break;
		case Anchor.MIDDLE_CENTER : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor)/2 + offset.x, (UIScreen2D.Height / UIScreen2D.Factor)/2 + offset.y, zDepth); break;
		case Anchor.MIDDLE_RIGHT : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor) + offset.x, (UIScreen2D.Height / UIScreen2D.Factor)/2 + offset.y, zDepth); break;

		case Anchor.BOTTOM_LEFT : pos = new Vector3(offset.x, offset.y, zDepth); break;
		case Anchor.BOTTOM_CENTER : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor)/2 + offset.x, offset.y, zDepth); break;
		case Anchor.BOTTOM_RIGHT : pos = new Vector3((UIScreen2D.Width/UIScreen2D.Factor) + offset.x, offset.y, zDepth); break;
		}

		transform.localPosition = pos;
	}
}
