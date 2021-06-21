using UnityEngine;

public static class RGBColor 
{
	public static Color FromRGB(int R, int G, int B)
	{
		return new Color(R/255.0f, G/255.0f, B/255.0f);
	}
	
	public static Color FromRGBA(int R, int G, int B, int A)
	{
		return new Color(R/255.0f, G/255.0f, B/255.0f,A/255.0f);
	}
}
