using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Xml;

public class PixelArtEditor : EditorWindow 
{
	const int mPreviewZoom = 6;
	int mDocumentWidth = 16;
	int mDocumentHeight = 16;
	int mTmpWidth = 16;
	int mTmpHeight = 16;
	Color mSelectedColor = Color.white;
	Texture2D[] mPalette = null;
	Color[] mColors = null;
	Color[] mPixels = null;
	Rect[] mRects = null;
	Rect mCanvasSize = new Rect();
	bool mPreviewOpen = false;
	Texture2D mPreview;
	
	[MenuItem("Tools/Pixel Art Editor")]
	static void Init()
	{
		GetWindow(typeof(PixelArtEditor));
	}
	
	//
	void OnEnable()
	{
		if(mPalette == null || mColors == null)
		{
			InitializePalette();
		}
	}
	
	//
	void OnGUI()
	{
		CanvasGUI();
		ToolsGUI();
		FileToolbarGUI();
		PreviewGUI();
		InputHandler();
	}
	
	//
	void FileToolbarGUI()
	{
		GUILayout.BeginHorizontal(EditorStyles.toolbar);
		
		//
		if(GUILayout.Button("Save", EditorStyles.toolbarButton)) 
		{
           string path = EditorUtility.SaveFilePanelInProject("Save BehaviorTree", "MyBehaviorTree","xml", "");
			
			if(path != "")
			{
				Save(path);
			}
		}
		
		//
		if(GUILayout.Button("Open", EditorStyles.toolbarButton)) 
		{
            string path = EditorUtility.OpenFilePanel("Load BehaviorTree",Application.dataPath, "xml");
			
			if(path != "")
			{
				Load(path);
			}
		}
		
		//
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Export", EditorStyles.toolbarButton)) 
		{
           	string path = EditorUtility.SaveFilePanelInProject("Generate Texture", "MyPixelArt","png", "");
			
			if(path != "")
			{
				Export(path);
			}
		}
		
		//
  		GUILayout.EndHorizontal();
	}
	
	//
	void ToolsGUI()	
	{
		GUILayout.BeginArea(new Rect(0,17,200,position.height-17), "", "BOX");
		
		GUILayout.Label("Documents");
		
		mTmpWidth = EditorGUILayout.IntField("Width", mTmpWidth);
		mTmpHeight = EditorGUILayout.IntField("Height", mTmpHeight);
		
		if(GUILayout.Button("New Document"))
		{
			CreateDocument();
		}
		
		EditorGUILayout.Separator();
		
		GUILayout.Label("Colors");
		
		mSelectedColor = EditorGUILayout.ColorField( mSelectedColor);
		
		GUI.Box(new Rect(11,129, 177, 397), "");
		
		for(int y = 0; y < Mathf.RoundToInt( mPalette.Length/3 ); y++)
		{
			for(int x = 0; x < 3; x++)
			{
				GUIStyle mPaletteStyle = new GUIStyle();
				mPaletteStyle.normal.background = mPalette[x + (3*y)];
				if(GUI.Button(new Rect(x * 60 + 12,y * 40 + 130, 55, 35), "", mPaletteStyle))
				{
					mSelectedColor = mColors[x + (3*y)];
				}
			}
		}

		GUILayout.EndArea();
	}
	
	void CreateDocument(Color[] aPixelsColor = null)
	{
		mDocumentWidth = mTmpWidth;
		mDocumentHeight = mTmpHeight;
		
		mPixels = aPixelsColor != null ? aPixelsColor : new Color[mDocumentWidth*mDocumentHeight];
		mRects = new Rect[mPixels.Length];
		
		for(int y = 0; y < mDocumentHeight; y++)
		{
			for(int x = 0; x < mDocumentWidth; x++)
			{
				mRects[x + (mDocumentWidth * ((mDocumentHeight-1) - y))] = new Rect(x*20 + 215, y*20 + 32, 20,20);
			}
		}
		
		//
		mCanvasSize = new Rect( 215, 32, mDocumentWidth * 20, mDocumentHeight * 20 );
		mPreview = new Texture2D(mDocumentWidth, mDocumentHeight, TextureFormat.RGBA32, false);
		mPreview.filterMode = FilterMode.Point;
	}
	
	//
	void CanvasGUI()
	{
		if(mPixels != null && mRects != null)
		{
			for(int i = 0; i < mRects.Length; i++)
			{
				GUI.color = mPixels[i] == Color.clear ? new Color(1,1,1,0.1f) : mPixels[i];
				GUI.Box(mRects[i], "");
			}
			
			GUI.color = Color.white;
		}
	}
	
	//
	void PreviewGUI()
	{
		if(!mPreviewOpen)
		{
			if(GUI.Button(new Rect(position.width - 60, position.height -20, 60, 20), "Preview"))
			{
				mPreviewOpen = true;
			}
		}
		else
		{
			if(GUI.Button(new Rect(position.width - 20 - (mPreview.width*mPreviewZoom), position.height -20 - (mPreview.height*mPreviewZoom), 20, 20), "X"))
			{
				mPreviewOpen = false;
			}
			
			mPreview.SetPixels(mPixels);
			mPreview.Apply();
			
			Rect previewWindow = new Rect(position.width - (mPreview.width*mPreviewZoom), position.height - (mPreview.height*mPreviewZoom), mPreview.width*mPreviewZoom, mPreview.height*mPreviewZoom);
			
			GUI.color = new Color(1,1,1,0.2f);
			GUI.Box(previewWindow, "");
			GUI.color = Color.white;
			GUI.DrawTexture(previewWindow, mPreview);
		}
	}
	
	//
	void InputHandler()
	{
		Event e = Event.current;
		
		if(e.isMouse && e.type == EventType.mouseDown && e.button == 0 && mCanvasSize.Contains(e.mousePosition))
		{
			for(int i = 0; i < mRects.Length; i++)
			{
				if( mRects[i].Contains(e.mousePosition))
				{
					mPixels[i] = mSelectedColor;
					Repaint();
					break;
				}
			}
		}
		else if(e.isMouse && e.type == EventType.mouseDrag && e.button == 0 && mCanvasSize.Contains(e.mousePosition))
		{
			for(int i = 0; i < mRects.Length; i++)
			{
				if( mRects[i].Contains(e.mousePosition))
				{
					mPixels[i] = mSelectedColor;
					Repaint();
					break;
				}
			}
		}
		
		if(e.isMouse && e.type == EventType.mouseDown && e.button == 1 && mCanvasSize.Contains(e.mousePosition))
		{
			for(int i = 0; i < mRects.Length; i++)
			{
				if( mRects[i].Contains(e.mousePosition))
				{
					mPixels[i] = Color.clear;
					Repaint();
					break;
				}
			}
		}
		else if(e.isMouse && e.type == EventType.mouseDrag && e.button == 1 && mCanvasSize.Contains(e.mousePosition))
		{
			for(int i = 0; i < mRects.Length; i++)
			{
				if( mRects[i].Contains(e.mousePosition))
				{
					mPixels[i] = Color.clear;
					Repaint();
					break;
				}
			}
		}
	}
	
	//
	void InitializePalette()
	{
		mColors = new Color[]
		{
			// Grayscale
			RGBColor.FromRGB(255,255,255),
			RGBColor.FromRGB(237,237,235),
			RGBColor.FromRGB(210,214,206),
			RGBColor.FromRGB(185,188,181),
			RGBColor.FromRGB(135,137,132),
			RGBColor.FromRGB(100,100,100),
			RGBColor.FromRGB(85,87,83),
			RGBColor.FromRGB(46,52,54),
			RGBColor.FromRGB(0,0,0),
			
			// Yellow
			RGBColor.FromRGB(251,232,79),
			RGBColor.FromRGB(236,211,0),
			RGBColor.FromRGB(195,159,0),
			
			// Orange
			RGBColor.FromRGB(251,174,62),
			RGBColor.FromRGB(244,120,0),
			RGBColor.FromRGB(205,92,0),
			
			// Brown
			RGBColor.FromRGB(232,184,109),
			RGBColor.FromRGB(192,124,17),
			RGBColor.FromRGB(142,89,2),
			
			// Green
			RGBColor.FromRGB(137,225,52),
			RGBColor.FromRGB(114,209,22),
			RGBColor.FromRGB(78,153,6),
			
			// Blue
			RGBColor.FromRGB(113,158,206),
			RGBColor.FromRGB(52,101,163),
			RGBColor.FromRGB(32,74,134),
			
			// Purple
			RGBColor.FromRGB(172,126,167),
			RGBColor.FromRGB(116,80,122),
			RGBColor.FromRGB(92,53,102),
			
			// Red
			RGBColor.FromRGB(238,41,41),
			RGBColor.FromRGB(203,0,0),
			RGBColor.FromRGB(163,0,0),
		};
		
		mPalette = new Texture2D[mColors.Length];
		
		//
		for(int i = 0; i < mColors.Length; i++)
		{
			Texture2D t = new Texture2D(1,1);
			t.SetPixel(0,0, mColors[i]);
			t.Apply();
			
			mPalette[i] = t;
		}
	}
	
	//
	void Export(string aPath)
	{
		Texture2D tex = new Texture2D(mDocumentWidth, mDocumentHeight, TextureFormat.RGBA32, false);
		tex.filterMode = FilterMode.Point;
		tex.SetPixels(mPixels);
		tex.Apply();
		
		//
		byte[] bytes = tex.EncodeToPNG();
		var file = File.Open(aPath,FileMode.Create);
		
   		using (BinaryWriter binary = new BinaryWriter(file))
		{
   			binary.Write(bytes);
		}
		
		AssetDatabase.Refresh();
	}
	
	//
	void Save(string aPath)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.ConformanceLevel = ConformanceLevel.Fragment;
		settings.OmitXmlDeclaration = true;
		
		XmlWriter writer = XmlWriter.Create(aPath, settings);
	    writer.WriteStartElement("Nodes");
		writer.WriteAttributeString("Width", mDocumentWidth.ToString());
		writer.WriteAttributeString("Height", mDocumentHeight.ToString());
		
		foreach(Color color in mPixels)
		{
			//
			writer.WriteStartElement("Pixel");
			writer.WriteAttributeString("Red", color.r.ToString());
			writer.WriteAttributeString("Green", color.g.ToString());
			writer.WriteAttributeString("Blue", color.b.ToString());
			writer.WriteAttributeString("Alpha", color.a.ToString());
			writer.WriteEndElement();
		}
		
		writer.Flush();
		writer.Close();
		
		//
		AssetDatabase.Refresh();
	}
	
	//
	void Load(string aPath)
	{
		//
		XmlDocument xmlDoc = new XmlDocument();
		
		
		//
		using(FileStream fs = new FileStream(aPath, FileMode.Open, FileAccess.Read))
		{
			//
			xmlDoc.Load(fs);
			
			mTmpWidth = int.Parse(xmlDoc.ChildNodes[0].Attributes["Width"].Value);
			mTmpHeight = int.Parse(xmlDoc.ChildNodes[0].Attributes["Height"].Value);
			
			XmlNodeList elemList = xmlDoc.ChildNodes[0].ChildNodes;//("/Nodes");
		
			Color pixel = Color.white;
			Color[] result = new Color[mTmpWidth*mTmpHeight];
			
			//
			for(int i = 0; i < elemList.Count; i++)
			{
				XmlNode currentElem = elemList[i];
				
				if(currentElem.Name == "Pixel")
				{
					// Create Texture from xml data
					pixel.r = float.Parse(currentElem.Attributes["Red"].Value);
					pixel.g = float.Parse(currentElem.Attributes["Green"].Value);
					pixel.b = float.Parse(currentElem.Attributes["Blue"].Value);
					pixel.a = float.Parse(currentElem.Attributes["Alpha"].Value);
					result[i] = pixel;
				}
			}
	
			CreateDocument(result);
		}
	}
}
