using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class UISliced2D : MonoBehaviour
{
	[SerializeField]private Sprite 		mSprite 			= null;
	[SerializeField]private int 		mLeftBorder 		= 0;
	[SerializeField]private int 		mRightBorder		= 0;
	[SerializeField]private int 		mTopBorder 			= 0;
	[SerializeField]private int 		mBottomBorder 		= 0;
	private bool 						mRebuildNeeded		= false;

	//
	void Awake()
	{
		if(mSprite != null && mSprite.texture != null)
		{
			GetComponent<Renderer>().sharedMaterial.mainTexture = mSprite.texture;
		}
	}

	//
	void Start()
	{
		if(mSprite != null)
		{
			mRebuildNeeded = true;
			GenerateMesh();
		}
	}

	void Update() 
	{
		if(!mSprite)
			return;

		//
		if(mRebuildNeeded)
		{
			GenerateMesh();		
		}
	}

	/// <summary>
	/// Gets or sets the sliced sprite.
	/// </summary>
	/// <value>The sliced sprite.</value>
	public Sprite SlicedSprite
	{
		get{return mSprite;}
		set
		{
			if(mSprite != value)
			{
				//
				mSprite = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the top border.
	/// </summary>
	/// <value>The top border.</value>
	public int TopBorder
	{
		get{return mTopBorder;}
		set
		{
			if(mTopBorder != value)
			{
				//
				mTopBorder = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the bottom border.
	/// </summary>
	/// <value>The bottom border.</value>
	public int BottomBorder
	{
		get{return mBottomBorder;}
		set
		{
			if(mBottomBorder != value)
			{
				mBottomBorder = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the left border.
	/// </summary>
	/// <value>The left border.</value>
	public int LeftBorder
	{
		get{return mLeftBorder;}
		set
		{
			if(mLeftBorder != value)
			{
				mLeftBorder = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the right border.
	/// </summary>
	/// <value>The right border.</value>
	public int RightBorder
	{
		get{return mRightBorder;}
		set
		{
			if(mRightBorder != value)
			{
				mRightBorder = value;
				mRebuildNeeded = true;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	//
	void GenerateMesh() 
	{
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = mesh = new Mesh();

		Vector3[] verticies = new Vector3[16];
		Vector3 size = transform.localScale;
		Vector3 screenScale = UIScreen2D.Scale;

		// Top left corner
		verticies[0] = new Vector3(-(size.x/2), size.y/2);
		verticies[1] = new Vector3(verticies[0].x + mLeftBorder, verticies[0].y);
		verticies[2] = new Vector3(verticies[0].x + mLeftBorder, verticies[0].y - mTopBorder);
		verticies[3] = new Vector3(verticies[0].x, verticies[0].y - mTopBorder);

		// Top right corner
		verticies[4] = new Vector3(size.x/2, size.y/2);
		verticies[5] = new Vector3(verticies[4].x, verticies[4].y - mTopBorder);
		verticies[6] = new Vector3(verticies[4].x - mRightBorder, verticies[4].y - mTopBorder);
		verticies[7] = new Vector3(verticies[4].x - mRightBorder, verticies[4].y);

		// Bottom right corner
		verticies[8] = new Vector3(size.x/2, -(size.y/2));
		verticies[9] = new Vector3(verticies[8].x - mRightBorder, verticies[8].y);
		verticies[10] = new Vector3(verticies[8].x - mRightBorder, verticies[8].y + mBottomBorder);
		verticies[11] = new Vector3(verticies[8].x, verticies[8].y + mBottomBorder);

		// Bottom left corner
		verticies[12] = new Vector3(-(size.x/2), -(size.y/2));
		verticies[13] = new Vector3(verticies[12].x, verticies[12].y + mBottomBorder);
		verticies[14] = new Vector3(verticies[12].x + mLeftBorder, verticies[12].y + mBottomBorder );
		verticies[15] = new Vector3(verticies[12].x + mLeftBorder, verticies[12].y);

		// Scale down mesh
		for(int i = 0; i < verticies.Length; i++)
		{
			verticies[i].x /= size.x;
			verticies[i].y /= size.y;
		}

		// Top left triangles
		int[] triangles = new int[]
		{
			0,1,2,2,3,0,
			1,7,6,6,2,1,
			4,5,6,6,7,4,
			6,5,11,11,10,6,
			8,9,10,10,11,8,
			14,10,9,9,15,14,
			12,13,14,14,15,12,
			3,2,14,14,13,3,
			2,6,10,10,14,2
		};

		//
		mesh.name = "slicedSprite";
		mesh.vertices = verticies;
		mesh.triangles = triangles;

		// Set UVs
		if(mSprite != null && mSprite.texture != null)
		{
			Rect rect = mSprite.textureRect;
			Vector2 texSize = new Vector2(mSprite.texture.width, mSprite.texture.height);

			Vector2[] uvs = new Vector2[16];

			// Top left
			uvs[0] = new Vector2(rect.x / texSize.x, (rect.y / texSize.y));
			uvs[1] = new Vector2((rect.x + mLeftBorder) / texSize.x, uvs[0].y);
			uvs[2] = new Vector2(uvs[1].x, (rect.y + mTopBorder) / texSize.y);
			uvs[3] = new Vector2(uvs[0].x, uvs[2].y);

			// Top right
			uvs[4] = new Vector2((rect.x + rect.width) / texSize.x, uvs[0].y);
			uvs[5] = new Vector2(uvs[4].x, uvs[2].y);
			uvs[6] = new Vector2(((rect.x + rect.width) - mRightBorder) / texSize.x, uvs[2].y);
			uvs[7] = new Vector2(uvs[6].x, uvs[0].y);

			//Bottom right
			uvs[8] = new Vector2(uvs[4].x, (rect.y + rect.height) / texSize.y);
			uvs[9] = new Vector2(uvs[6].x, uvs[8].y);
			uvs[10] = new Vector2(uvs[6].x, (rect.y + rect.height - mBottomBorder) / texSize.y);
			uvs[11] = new Vector2(uvs[4].x, uvs[10].y);

			//Bottom Left
			uvs[12] = new Vector2(uvs[0].x, uvs[8].y);
			uvs[13] = new Vector2(uvs[0].x, uvs[10].y);
			uvs[14] = new Vector2(uvs[1].x, uvs[10].y);
			uvs[15] = new Vector2(uvs[1].x, uvs[8].y);

			//
			mesh.uv = uvs;
		}

		if(meshFilter.sharedMesh != null)
		{
			DestroyImmediate(meshFilter.sharedMesh);
		}

		meshFilter.sharedMesh = mesh;
	}
}
