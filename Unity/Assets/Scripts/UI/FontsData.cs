using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class FontsData : ScriptableObject
{
	//public static string PATH = "Assets/Fonts/FontData/_FontData.asset";
	//public static string ATLAS_PATH = Application.dataPath + "/Fonts/FontData/";
	[SerializeField]public List<FontInfo> mFontInfo;
	
	//
	public FontsData()
	{
		mFontInfo = new List<FontInfo>();
	}

	//
	public static string Path
	{
		get{return "Assets/Fonts/FontData/_FontData.asset";}
	}

	//
	public static string AtlasPath
	{
		get{return Application.dataPath + "/Fonts/FontData/";}
	}

	//
	public bool AddFont(string font, int size, Glyph[] glyphs, Texture2D aAtlas)
	{
		int index = IndexOfFont(font);

		//
		if(index == -1)
		{
			//New font.
			FontInfo fi = new FontInfo(font);
			fi.AddSize(size, glyphs, aAtlas);

			mFontInfo.Add(fi);
		}
		else
		{
			//Already contained font.
			mFontInfo[index].AddSize(size, glyphs, aAtlas);
		}

		return true;
	}

	public FontInfo GetFontInfo(string fontName)
	{
		int index = IndexOfFont(fontName);
		
		if(index == -1)
			return null;
		
		return mFontInfo[index];
	}

	public Glyph[] GetGlyph (string fontName, int fontSize)
	{
		int index = IndexOfFont(fontName);
		
		if(index == -1)
			return null;

		return mFontInfo[index].GetGlyphBySize(fontSize);
	}

	//
	public bool ContainsData(string fontName, int size)
	{
		int index = IndexOfFont(fontName);

		if(index == -1)
			return false;

		if(mFontInfo[index].Contains(size))
		{
			return true;
		}

		return false;
	}

	//
	int IndexOfFont(string aName)
	{
		for(int i = 0; i < mFontInfo.Count; i++)
		{
			if(mFontInfo[i].Name == aName)
			{
				return i;
			}
		}
		return -1;
	}

	//
	bool ContainsFont(string aName)
	{
		for(int i = 0; i < mFontInfo.Count; i++)
		{
			if(mFontInfo[i].Name == aName)
			{
				return true;
			}
		}
		return false;
	}
}

//
[System.Serializable]
public class FontInfo
{
	[SerializeField]string mFontName;
	[SerializeField]List<int> mFontSize;
	[SerializeField]List<GlyphInfo> mGlyphs;
	[SerializeField]Texture2D mAtlas;
	[SerializeField]Material mMaterial;

	public FontInfo(string aName)
	{
		mFontName = aName;
		mFontSize = new List<int>();
		mGlyphs = new List<GlyphInfo>();
	}

	public void SetAtlas(Texture2D aTexture)
	{
		mAtlas = aTexture;
	}

	public void SetMaterial(Material aMaterial)
	{
		mMaterial = aMaterial;
	}

	public void AddSize(int aSize, Glyph[] aGlyphs, Texture2D aTexture)
	{
		if(!mFontSize.Contains(aSize))
		{
			mFontSize.Add(aSize);

			if(mAtlas == null)
			{
				//
				mFontSize.Clear();
				mGlyphs.Clear();

				//
				GlyphInfo glyphInfo = new GlyphInfo(aGlyphs);
				mGlyphs.Add(glyphInfo);
				mFontSize.Add(aSize);
				mAtlas = aTexture;

				byte[] bytes = mAtlas.EncodeToPNG();
				File.WriteAllBytes(FontsData.AtlasPath + mFontName + ".png", bytes);
			}
			else
			{
				// Get old chars.
				List<Texture2D> charsTex = new List<Texture2D>();

				//
				int oldWidth = mAtlas.width;
				int oldHeight = mAtlas.height;

				//
				for(int i = 0; i < mGlyphs.Count; i++)
				{
					for(int y = 0; y < mGlyphs[i].Glyphs.Length; y++)
					{
						int px = Mathf.RoundToInt(mGlyphs[i].Glyphs[y].UV.x * oldWidth);
						int py = Mathf.RoundToInt(mGlyphs[i].Glyphs[y].UV.y * oldHeight);
						int pW = Mathf.RoundToInt(mGlyphs[i].Glyphs[y].UV.width * oldWidth);
						int pH = Mathf.RoundToInt(mGlyphs[i].Glyphs[y].UV.height * oldHeight);

						Color[] colors = mAtlas.GetPixels(px, py, pW, pH);
					
						Texture2D tex = new Texture2D(pW, pH, TextureFormat.Alpha8, false); 
						tex.SetPixels(0,0, pW, pH, colors);

						//
						charsTex.Add(tex);
					}
				}

				// Change width & height for new atlas
				oldWidth = aTexture.width;
				oldHeight = aTexture.height;

				// Add new chars
				for(int i = 0; i < aGlyphs.Length; i++)
				{
					int px = Mathf.RoundToInt(aGlyphs[i].UV.x * oldWidth);
					int py = Mathf.RoundToInt(aGlyphs[i].UV.y * oldHeight);
					int pW = Mathf.RoundToInt(aGlyphs[i].UV.width * oldWidth);
					int pH = Mathf.RoundToInt(aGlyphs[i].UV.height * oldHeight);
					
					Color[] colors = aTexture.GetPixels(px, py, pW, pH);
					
					Texture2D tex = new Texture2D(pW, pH, TextureFormat.Alpha8, false); 
					tex.SetPixels(0,0, pW, pH, colors);

					//
					charsTex.Add(tex);
				}

				//
				GlyphInfo glyphInfo = new GlyphInfo(aGlyphs);
				mGlyphs.Add(glyphInfo);


				// Create new texture.
				Rect[] newUvs = mAtlas.PackTextures(charsTex.ToArray(), 2, 2048);
				int index = 0;

				//
				for(int n = 0; n < mGlyphs.Count; n++)
				{
					for(int g = 0; g < mGlyphs[n].Glyphs.Length; g++)
					{
						mGlyphs[n].Glyphs[g].SetUv( newUvs[index] );
						index ++;
					}
				}

				//
				byte[] bytes = mAtlas.EncodeToPNG();
				File.WriteAllBytes(FontsData.AtlasPath + mFontName + ".png", bytes);
			}
		}
	}

	//
	public bool Contains(int aSize)
	{
		return mFontSize.Contains(aSize);
	}

	//
	public string Name
	{
		get{return mFontName;}
	}

	//
	public Texture2D Atlas
	{
		get{return mAtlas;}
	}

	public Material Material 
	{
		get{return mMaterial;}
	}

	//
	public Glyph[] GetGlyphBySize(int aSize)
	{
		for(int i = 0; i < mFontSize.Count; i++)
		{
			if(mFontSize[i] == aSize)		
			{
				return mGlyphs[i].Glyphs;
			}
		}

		return null;
	}
}

//
[System.Serializable]
public class GlyphInfo
{
	[SerializeField]Glyph[] mGlyphs;

	public GlyphInfo(Glyph[] aGlyph)
	{
		mGlyphs = aGlyph;
	}

	public Glyph[] Glyphs
	{
		get{return mGlyphs;}
	}
}

