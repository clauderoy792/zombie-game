using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class UILabel : MonoBehaviour 
{
	[SerializeField]private string mFontName = "Arial";
	[SerializeField]private int mFontSize = 12;
	[SerializeField]private int mCharSpacing = 0;
	[SerializeField]private int mLineSpacing = 0;
	[SerializeField]private int mMaxWidth = 0;
	[SerializeField]private string mText = "Hello World";
	[SerializeField]private FontsData mFontData;
	[SerializeField]private Color mColor = Color.black;
	[SerializeField]private int mLastNumberOfLines = 0;
	[SerializeField]private TextAnchor mAnchor = TextAnchor.UpperLeft; 
	private MeshFilter mMeshFilter;
	private Renderer mRenderer;
	private Glyph[] mGlyphs;
	private MeshFlag mFlag = MeshFlag.NONE;


	private enum MeshFlag
	{
		NONE,
		VERTEX_ONLY,
		ALL
	}

	void Awake()
	{
		mMeshFilter = gameObject.GetComponent<MeshFilter>();
		mRenderer = gameObject.GetComponent<Renderer>();
	}

	void Start()
	{
		//
		if(mFontData != null)
		{
			mGlyphs = mFontData.GetGlyph(mFontName, mFontSize);

			//
			mFlag = MeshFlag.ALL;
		}
	}

	void Update()
	{
		if(mFontData != null)
		{
			if(mFlag == MeshFlag.ALL)
			{
				//
				GenerateAllMesh();

				//
				mFlag = MeshFlag.NONE;
			}
			else if(mFlag == MeshFlag.VERTEX_ONLY)
			{
				//
				RebuildVerticiesOnly();
				
				//
				mFlag = MeshFlag.NONE;
			}
		}
	}

	// Debug only
	void OnDrawGizmosSelected() 
	{
		if(mMaxWidth > 0)
		{
			Gizmos.color = Color.blue;
			Vector3 currentPos = transform.position;
			float width = mMaxWidth * transform.lossyScale.x;
			float height =  (mLastNumberOfLines+2) * (mFontSize + mLineSpacing) * transform.lossyScale.y;
			Gizmos.DrawWireCube(new Vector3(currentPos.x + (width/2.0f) + (width * GetAnchorOffsetMultiplier(mAnchor).x), currentPos.y - (height/2.0f) + (height * GetAnchorOffsetMultiplier(mAnchor).y), currentPos.z), new Vector3(width, height,0)); 
		}
	}

	#region ACCESSORS

	//
	public Color TextColor
	{
		get{return mColor;}
		set
		{
			if(mColor != value)
			{
				mColor = value;
				if(mRenderer == null)
				{
					mRenderer = GetComponent<Renderer>();
				}

				if(mRenderer.sharedMaterial != null)
					mRenderer.sharedMaterial.color = mColor;
			}
		}
	}

	//
	public string FontName
	{
		get{return mFontName;}
		set
		{
			if(mFontName != value)
			{
				mFontName = value;
				mFlag = MeshFlag.ALL;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateAllMesh();
				}
			}
		}
	}

	//
	public int FontSize
	{
		get{return mFontSize;}
		set
		{
			if(mFontSize != Mathf.Clamp(value, 8, 120))
			{
				mFontSize = Mathf.Clamp(value, 8, 120);
			}
		}
	}

	//
	public int CharSpacing
	{
		get{return mCharSpacing;}
		set
		{
			if(mCharSpacing != value)
			{
				mCharSpacing = value;

				if(mFlag != MeshFlag.ALL)
				{
					mFlag = MeshFlag.VERTEX_ONLY;
				}

				if(Application.isEditor && !Application.isPlaying)
				{
					RebuildVerticiesOnly();
				}
			}
		}
	}

	//
	public int LineSpacing
	{
		get{return mLineSpacing;}
		set
		{
			if(mLineSpacing != value)
			{
				mLineSpacing = value;

				if(mFlag != MeshFlag.ALL)
				{
					mFlag = MeshFlag.VERTEX_ONLY;
				}

				if(Application.isEditor && !Application.isPlaying)
				{
					RebuildVerticiesOnly();
				}
			}
		}
	}

	//
	public int MaxWidth
	{
		get{return mMaxWidth;}
		set
		{
			if(mMaxWidth != value)
			{
				mMaxWidth = value;
				mFlag = MeshFlag.VERTEX_ONLY;

				if(Application.isEditor && !Application.isPlaying)
				{
					RebuildVerticiesOnly();
				}
			}
		}
	}

	//
	public string Text
	{
		get{return mText;}
		set
		{
			if(mText != value)
			{
				mText = value;
				mFlag = MeshFlag.ALL;

				if(Application.isEditor && !Application.isPlaying)
				{
					GenerateAllMesh();
				}
			}
		}
	}

	//
	public TextAnchor Anchor
	{
		get{return mAnchor;}
		set
		{
			if(mAnchor != value)
			{
				mAnchor = value;
				mFlag = MeshFlag.VERTEX_ONLY;

				if(Application.isEditor && !Application.isPlaying)
				{
					RebuildVerticiesOnly();
				}
			}
		}
	}

	//
	public void SetFontData(FontsData aFontsData)
	{
		mFontData = aFontsData;
	}

	//
	public void SetMaterial(Material aMaterial)
	{
		if(mRenderer == null)
		{
			mRenderer = GetComponent<Renderer>();
		}

		mRenderer.sharedMaterial = aMaterial;
	}

	//
	public void RebuildNeeded()
	{
		if(mFontData != null)
		{
			mGlyphs = mFontData.GetGlyph(mFontName, mFontSize);
			mFlag = MeshFlag.ALL;

			if(Application.isEditor && !Application.isPlaying)
			{
				GenerateAllMesh();
			}
		}
	}

	#endregion

	#region MESH_MANAGEMENT

	void GenerateAllMesh()
	{
		if(mMeshFilter == null)
		{
			mMeshFilter = GetComponent<MeshFilter>();
		}

		if(mRenderer == null)
		{
			mRenderer = GetComponent<Renderer>();
		}

		Mesh mesh = new Mesh();
		
		int currentX = 0;
		int currentY = 0;
		List<Vector3> verticies = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		int index = 0;
		List<string> lines = WrapText(mText);
		Vector2 anchorOffset = GetAnchorOffsetMultiplier(mAnchor);

		for(int line = 0; line < lines.Count; line++)
		{
			int offsetX = (int)(GetStringWidth(lines[line].Trim()) * anchorOffset.x);
			int offsetY = (int)((lines.Count * (mFontSize + mLineSpacing)) * anchorOffset.y);

			for(int i = 0; i < lines[line].Length; i++)
			{
				if( lines[line][i] == ' ')
				{
					// Add space
					currentX += mFontSize/2;
				}
				else
				{
					Glyph glyph = GetCharacterInfo( lines[line][i]);
					if(glyph != null)
					{
						verticies.AddRange(GetVerticies(glyph, currentX + offsetX, currentY - offsetY));
						triangles.AddRange(new int[]
		           		{
							(index*4), (index*4) + 1, (index*4) + 2, 
							(index*4) + 2, (index*4) + 3, (index*4)
						});
						uvs.AddRange(new Vector2[]
			           	{
							new Vector2(glyph.UV.x, glyph.UV.y + glyph.UV.height),
							new Vector2(glyph.UV.x + glyph.UV.width, glyph.UV.y + glyph.UV.height),
							new Vector2(glyph.UV.x + glyph.UV.width, glyph.UV.y),
							new Vector2(glyph.UV.x, glyph.UV.y),
						});

						//
						currentX += glyph.Width + mCharSpacing;

						index ++;
					}
				}
			}

			// Add space
			currentX = 0;
			currentY += mFontSize + mLineSpacing;
		}

		//
		mesh.vertices = verticies.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		//
		if(mMeshFilter.mesh != null)
		{
			DestroyImmediate(mMeshFilter.sharedMesh);
		}

		mMeshFilter.mesh = mesh;
	}

	void RebuildVerticiesOnly()
	{
		if(mMeshFilter == null)
		{
			mMeshFilter = GetComponent<MeshFilter>();
		}
		
		if(mRenderer == null)
		{
			mRenderer = GetComponent<Renderer>();
		}

		Mesh mesh = mMeshFilter.sharedMesh;
		
		if(mesh == null)
		{
			GenerateAllMesh();
		}
		else
		{
			int currentX = 0;
			int currentY = 0;
			List<Vector3> verticies = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uvs = new List<Vector2>();
			int index = 0;
			List<string> lines = WrapText(mText);
			Vector2 anchorOffset = GetAnchorOffsetMultiplier(mAnchor);
			
			for(int line = 0; line < lines.Count; line++)
			{
				int offsetX = (int)(GetStringWidth(lines[line].Trim()) * anchorOffset.x);
				int offsetY = (int)((lines.Count * (mFontSize + mLineSpacing)) * anchorOffset.y);

				for(int i = 0; i < lines[line].Length; i++)
				{
					if( lines[line][i] == ' ')
					{
						// Add space
						currentX += mFontSize/2;
					}
					else
					{
						Glyph glyph = GetCharacterInfo( lines[line][i]);
						if(glyph != null)
						{
							//
							verticies.AddRange(GetVerticies(glyph, currentX + offsetX, currentY - offsetY));

							//
							currentX += glyph.Width + mCharSpacing;
							
							index ++;
						}
					}
				}
				
				// Add space
				currentX = 0;
				currentY += mFontSize + mLineSpacing;
			}
			
			//
			mesh.vertices = verticies.ToArray();
			
			//
			mMeshFilter.mesh = mesh;
		}
	}

	List<string> WrapText(string text)
	{
		string[] originalWords = text.Split(' ');
		List<string> wrappedLines = new List<string>();
		
		System.Text.StringBuilder actualLine = new System.Text.StringBuilder();
		int actualWidth = 0;

		//
		for(int i = 0; i < originalWords.Length; i++)
		{
			string[] splitOnReturns = originalWords[i].Split('\n');

			for(int y = 0; y < splitOnReturns.Length; y++)
			{
				int wordWidth = GetStringWidth(splitOnReturns[y]) + mFontSize/2;

				if(MaxWidth > 0 && actualWidth + wordWidth > MaxWidth)
				{
					wrappedLines.Add(actualLine.ToString());
					actualLine.Length = 0;
					actualWidth = 0;
				}

				//
				actualLine.Append(splitOnReturns[y] + " ");
				actualWidth += wordWidth;

				if(y < splitOnReturns.Length - 1)
				{
					wrappedLines.Add(actualLine.ToString());
					actualLine.Length = 0;
					actualWidth = 0;
				}
			}
		}
		
		if(actualLine.Length > 0)
			wrappedLines.Add(actualLine.ToString());

		//
		mLastNumberOfLines = wrappedLines.Count;

		return wrappedLines;
	}

	int GetStringWidth (string aString)
	{
		int size = 0;

		for(int c = 0; c < aString.Length; c++)
		{
			if(aString[c] == ' ')
			{
				size += mFontSize/2 + mCharSpacing;
			}
			else
			{
				Glyph glyph = GetCharacterInfo(aString[c]);
				if(glyph != null)
				{
					size += glyph.Width + mCharSpacing; 
				}
			}
		}

		return size;
	}

	Vector3[] GetVerticies(Glyph aGlyph, int offsetX, int offsetY)
	{
		return new Vector3[4]
		{
			new Vector3(offsetX, -(aGlyph.YOffset + offsetY), 0),
			new Vector3(offsetX + aGlyph.Width, -(aGlyph.YOffset + offsetY), 0),
			new Vector3(offsetX + aGlyph.Width, -(aGlyph.YOffset + aGlyph.Height + offsetY), 0),
			new Vector3(offsetX, -(aGlyph.YOffset + aGlyph.Height + offsetY), 0)
		};
	}

	Vector2 GetAnchorOffsetMultiplier(TextAnchor anchor)
	{
		Vector2 offset = Vector2.zero;

		switch(anchor)
		{
		case TextAnchor.UpperLeft : 
			offset.x = 0;
			offset.y = 0;
			break;
		case TextAnchor.UpperCenter : 
			offset.x = -0.5f;
			offset.y = 0;
			break;
		case TextAnchor.UpperRight : 
			offset.x = -1;
			offset.y = 0;
			break;
		case TextAnchor.MiddleLeft : 
			offset.x = 0;
			offset.y = 0.5f;
			break;
		case TextAnchor.MiddleCenter : 
			offset.x = -0.5f;
			offset.y = 0.5f;
			break;
		case TextAnchor.MiddleRight : 
			offset.x = -1;
			offset.y = 0.5f;
			break;
		case TextAnchor.LowerLeft : 
			offset.x = 0;
			offset.y = 1;
			break;
		case TextAnchor.LowerCenter : 
			offset.x = -0.5f;
			offset.y = 1;
			break;
		case TextAnchor.LowerRight : 
			offset.x = -1;
			offset.y = 1;
			break;
		}

		return offset;
	}

	Glyph GetCharacterInfo(char aChar)
	{
		int ID = aChar.GetHashCode();
		for(int i = 0; i < mGlyphs.Length; i++)
		{
			if(mGlyphs[i].CharID == ID)
			{
				return mGlyphs[i];
			}
		}

		return null;
	}

	#endregion
}
