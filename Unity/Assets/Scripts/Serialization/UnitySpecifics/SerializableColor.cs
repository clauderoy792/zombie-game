using UnityEngine;
using System.Collections;

[System.Serializable]
public class SerializableColor {

	private float a;
	private float r;
	private float g;
	private float b;

	public SerializableColor(Color aColor)
	{
		a = aColor.a;
		r = aColor.r;
		g = aColor.g;
		b = aColor.b;
	}

	public Color ToColor()
	{
		return new Color(r,g,b,a);
	}
}
