using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class UITiled2D : MonoBehaviour 
{
	[SerializeField]private Sprite mSprite = null;
	[SerializeField]private int mSizeX = 64;
	[SerializeField]private int mSizeY = 64;
	private bool mRebuildNeeded = false;

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

	//
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

	//
	public Sprite TiledSprite
	{
		get{return mSprite;}
		set
		{
			if(mSprite != value)
			{
				//
				mSprite = value;

				if(mSprite != null)
				{
					mSizeX = (int)(mSprite.textureRect.width);// * mSprite.texture.width);
					mSizeY = (int)(mSprite.textureRect.height);// * mSprite.texture.height);
				}

				mRebuildNeeded = true;
				
				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	//
	public int SizeX
	{
		get{return mSizeX;}
		set
		{
			if(mSizeX != value)
			{
				//
				mSizeX = value;
				mRebuildNeeded = true;
				
				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateMesh();
				}
			}
		}
	}

	//
	public int SizeY
	{
		get{return mSizeY;}
		set
		{
			if(mSizeY != value)
			{
				//
				mSizeY = value;
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
		if(mSprite != null)
		{
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			Mesh mesh = new Mesh();

			//
			List<Vector3> verticies = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uvs = new List<Vector2>();

			//
			Vector2 originalSize = new Vector2(mSprite.textureRect.width, mSprite.textureRect.height);
			Rect originalRect = new Rect( 	mSprite.textureRect.x / mSprite.texture.width, 
			                             	mSprite.textureRect.y / mSprite.texture.height,
			                             	mSprite.textureRect.width / mSprite.texture.width,
			                             	mSprite.textureRect.height / mSprite.texture.height);

			//
			int numberOfFullWidthQuad = (int)( mSizeX / originalSize.x); 
			int numberOfFullHeightQuad = (int)( mSizeY / originalSize.y);
			float widthDecimals = ( mSizeX / originalSize.x) - numberOfFullWidthQuad;
			float heightDecimals = ( mSizeY / originalSize.y) - numberOfFullHeightQuad;

			//
			numberOfFullWidthQuad += Mathf.CeilToInt(widthDecimals);
			numberOfFullHeightQuad += Mathf.CeilToInt(heightDecimals);

			for(int y = 0; y < numberOfFullHeightQuad; y++)
			{
				for(int x = 0; x < numberOfFullWidthQuad; x++)
				{
					Vector2 spriteSize = originalSize;
					Vector2 offset = new Vector2(x * originalSize.x, y * originalSize.y);

					if(x == numberOfFullWidthQuad-1 && widthDecimals > 0.0f)
					{
						spriteSize.x = originalSize.x * widthDecimals;
					}

					if(y == numberOfFullHeightQuad-1 && heightDecimals > 0.0f)
					{
						spriteSize.y = originalSize.y * heightDecimals;
					}

					// Create Verticies
					int index = verticies.Count;
					verticies.Add(new Vector3(offset.x, offset.y, 0));
					verticies.Add(new Vector3(spriteSize.x + offset.x, offset.y,0));
					verticies.Add(new Vector3(spriteSize.x + offset.x, spriteSize.y + offset.y,0));
					verticies.Add(new Vector3(offset.x, spriteSize.y + offset.y,0));

					// Triangles
					triangles.AddRange(new int[]{index+1,index,index+3});
					triangles.AddRange(new int[]{index+3,index+2,index+1});

					// Uvs
					uvs.Add(new Vector2(originalRect.x, originalRect.y));
					uvs.Add(new Vector2(originalRect.x + spriteSize.x / mSprite.texture.width, originalRect.y));
					uvs.Add(new Vector2(originalRect.x + spriteSize.x / mSprite.texture.width, originalRect.y + spriteSize.y / mSprite.texture.height));
					uvs.Add(new Vector2(originalRect.x, originalRect.y + spriteSize.y / mSprite.texture.height));
				}
			}

			//
			mesh.name = "TiledSprite";
			mesh.vertices = verticies.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();

			//
			if(meshFilter.sharedMesh != null)
			{
				DestroyImmediate(meshFilter.sharedMesh);
			}

			meshFilter.sharedMesh = mesh;
		}
	}
}
