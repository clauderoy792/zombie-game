using UnityEngine;
using System.Collections;

public static class VectorExtension 
{
	/// <summary>
	/// Grids to world.
	/// </summary>
	/// <returns>The to world.</returns>
	/// <param name="aPos">A position.</param>
	public static Vector2 GridToWorld(this Vector2 aPos)
	{
		aPos.Set(aPos.x*Room.UNIT_CELL_WIDTH,aPos.y*Room.UNIT_CELL_HEIGHT);

		return aPos;
	}

	/// <summary>
	/// Gets the cell position which contains this position.
	/// </summary>
	/// <returns>The cell position.</returns>
	/// <param name="aPosition">A position.</param>
	public static Vector2 WorldToGrid(this Vector2 aPosition)
	{	
		float x1 = (int)(aPosition.x/Room.UNIT_CELL_WIDTH);
		float y1 = (int)(aPosition.y/Room.UNIT_CELL_HEIGHT);

		return new Vector2(x1,y1);
	}

	/// <summary>
	/// Gets the cell position which contains this position.
	/// </summary>
	/// <returns>The cell position.</returns>
	/// <param name="aPosition">A position.</param>
	public static Vector2 WorldToGrid(this Vector3 aPosition)
	{	
		float x1 = (int)(aPosition.x/Room.UNIT_CELL_WIDTH);
		float y1 = (int)(aPosition.y/Room.UNIT_CELL_HEIGHT);
		
		return new Vector2(x1,y1);
	}

	/// <summary>
	/// Average Vector2 from the specified vectors.
	/// </summary>
	/// <param name="vectors">Vectors.</param>
	public static Vector2 Average(params Vector2[] vectors)
	{
		if(vectors == null || vectors.Length ==0)
		{
			return Vector2.zero;
		}

		float x = 0, y = 0;

		for(int i = 0; i < vectors.Length; i++)
		{
			x += vectors[i].x;
			y += vectors[i].y;
		}

		return new Vector2(x/vectors.Length, y/vectors.Length);
	}

	/// <summary>
	/// Average Vector3 from the specified vectors.
	/// </summary>
	/// <param name="vectors">Vectors.</param>
	public static Vector3 Average(params Vector3[] vectors)
	{
		if(vectors == null || vectors.Length ==0)
		{
			return Vector3.zero;
		}
		
		float x = 0, y = 0, z = 0;
		
		for(int i = 0; i < vectors.Length; i++)
		{
			x += vectors[i].x;
			y += vectors[i].y;
			z += vectors[i].z;
		}
		
		return new Vector3(x/vectors.Length, y/vectors.Length, z/vectors.Length);
	}
}
