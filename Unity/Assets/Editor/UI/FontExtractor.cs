using UnityEngine;
using UnityEditor;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Collections.Generic;


//
public static class FontExtractor
{
	const string mCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZàÀâÂèÈéÉêÊëËîÎïÏôÔûÛçÇ«»0123456789.,:;!?@#$%()_-/*+-=<>\"'{}[]&|";

	public static Glyph[] GenerateAtlas(string aFontFamily, int aFontSize, UnityEngine.FontStyle aFontStyle, Texture2D aAtlasTexture)
	{
		System.Drawing.Font font = new System.Drawing.Font(aFontFamily, aFontSize, ConvertUnityFontStyle(aFontStyle), GraphicsUnit.Pixel);

		Glyph[] glyphs = new Glyph[mCharacters.Length];
		Bitmap[] charBitmaps = new Bitmap[mCharacters.Length];

		for(int i = 0; i < mCharacters.Length; i ++)
		{
			Bitmap bmp = new Bitmap(1,1);
			int charWidth = 0;
			int charHeight = 0;

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
			{
				SizeF size = g.MeasureString(mCharacters[i].ToString(), font);
				charWidth = Mathf.CeilToInt(size.Width);
				charHeight = Mathf.CeilToInt(size.Height);
			}

			bmp.Dispose();
			bmp = new Bitmap(charWidth, charHeight);

			using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
			{
				// Draw string to screen.
				g.DrawString(mCharacters[i].ToString(), font, Brushes.Black, new PointF(0, 0));
			}

			charBitmaps[i] = bmp;
			glyphs[i] = new Glyph(mCharacters[i], 0, new Rect());
		}

		// Convert all bitmaps to texture2D
		Texture2D[] convertedBitmap = ConvertBitmapsToTexture2D(charBitmaps, glyphs);

		// Create and pack all chars texture inside the new atlas
		Rect[] charUvs = aAtlasTexture.PackTextures(convertedBitmap, 2, 2048);

		for(int i = 0; i < glyphs.Length; i++)
		{
			glyphs[i].SetUv(charUvs[i]);
		}

		//
		return glyphs;
	}

	//
	public static Texture2D SaveAtlas(string fileName, Texture2D aAtlas)
	{
		//
		byte[] bytes = aAtlas.EncodeToPNG();
		File.WriteAllBytes(FontsData.AtlasPath + fileName + ".png", bytes);
		
		// Refresh assets to be able to see newly created atlas.
		AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

		//
		return AssetDatabase.LoadAssetAtPath(FontsData.AtlasPath + fileName, typeof(Texture2D)) as Texture2D;
	}

	//
	static Texture2D[] ConvertBitmapsToTexture2D (Bitmap[] charBitmaps, Glyph[] glyphs)
	{
		if(charBitmaps == null || glyphs == null)
			return null;
		
		Texture2D[] result = new Texture2D[charBitmaps.Length];
		for(int i = 0; i < charBitmaps.Length; i++)
		{
			TBitmap trimedBitmap = TrimBitmap(charBitmaps[i]);

			glyphs[i].SetYOffset(trimedBitmap.yOffset);
		
			int bW = trimedBitmap.bitmap.Width;
			int bH = trimedBitmap.bitmap.Height;

			glyphs[i].SetWidth(bW);
			glyphs[i].SetHeight(bH);

			Texture2D tex = new Texture2D(bW, bH, TextureFormat.Alpha8, false);

			for(int x = 0; x < bW; x++)
			{
				for(int y = bH - 1; y >= 0; y--)
				{
					System.Drawing.Color pix = trimedBitmap.bitmap.GetPixel(x, (bH - 1) - y);
					tex.SetPixel(x, y, new UnityEngine.Color( pix.R / 255.0f, pix.G / 255.0f, pix.B / 255.0f, pix.A / 255.0f ));
				}
			}

			result[i] = tex;
		}

		return result;
	}

	static TBitmap TrimBitmap(Bitmap source)
	{
		try
		{
			int xMin = -1;
			int xMax = -1;
			int yMin = -1;
			int yMax = -1;
			int sourceWidth = source.Width;
			int sourceHeight = source.Height;

			// Get xMin
			for(int x = 0; x < sourceWidth; x++)
			{
				if(xMin != -1){
					break;
				}

				//
				for(int y = 0; y < sourceHeight; y++)
				{
					if(source.GetPixel(x,y).A != 0)
					{
						xMin = x;
						break;
					}
				}
			}

			//
			if(xMin == -1)
			{
				//Image should never be all transparent. if true, return the source as the output.
				return new TBitmap(source, 0);
			}

			// Get xMax
			for(int x = sourceWidth-1; x >= 0; x--)
			{
				if(xMax != -1){
					break;
				}
				
				//
				for(int y = sourceHeight - 1; y >= 0; y--)
				{
					if(source.GetPixel(x,y).A != 0)
					{
						xMax = x;
						break;
					}
				}
			}

			// Get yMin
			for(int y = 0; y < sourceHeight; y++)
			{
				if(yMin != -1){
					break;
				}
				
				//
				for(int x = 0; x < sourceWidth; x++)
				{
					if(source.GetPixel(x,y).A != 0)
					{
						yMin = y;
						break;
					}
				}
			}

			// Get yMax
			for(int y = sourceHeight-1; y >= 0; y--)
			{
				if(yMax != -1){
					break;
				}
				
				//
				for(int x = sourceWidth - 1; x >= 0; x--)
				{
					if(source.GetPixel(x,y).A != 0)
					{
						yMax = y;
						break;
					}
				}
			}

			// add one pixel to xMax and yMax otherwise these pixels will be clipped.
			xMax += 1;
			yMax += 1;

			// Get only trimmed pixel for the new bitmap.
			int newWidth = xMax - xMin;
			int newHeight = yMax - yMin;

			Bitmap output = new Bitmap(newWidth, newHeight);

			// Assign pixels
			for(int x = 0; x < newWidth; x++)
			{
				//
				for(int y = 0; y < newHeight; y++)
				{
					output.SetPixel(x,y, source.GetPixel(x + xMin, y + yMin));
				}
			}

			return new TBitmap(output, yMin);
		}
		catch(System.Exception e)
		{
			throw e;
			//return new TBitmap();
		}
	}

	
	static System.Drawing.FontStyle ConvertUnityFontStyle(UnityEngine.FontStyle fontStyle)
	{
		switch(fontStyle)
		{
		case UnityEngine.FontStyle.Normal : return System.Drawing.FontStyle.Regular;
		case UnityEngine.FontStyle.Bold : return System.Drawing.FontStyle.Bold;
		case UnityEngine.FontStyle.Italic : return System.Drawing.FontStyle.Italic;
		default : return System.Drawing.FontStyle.Regular;
		}
	}
}

/// <summary>
/// Trimmed bitmap struct.
/// </summary>
public struct TBitmap
{
	public Bitmap bitmap;
	public int yOffset;
	
	public TBitmap(Bitmap aBitmap, int aOffsetY)
	{
		bitmap = aBitmap;
		yOffset = aOffsetY;
	}
}
