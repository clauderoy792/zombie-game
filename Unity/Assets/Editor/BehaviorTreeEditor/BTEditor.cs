using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BTEditor : EditorWindow 
{
	//
	string[] iconsName = new string[]{"Sequence","Selector","Custom","Action"};
	
	Texture2D[] icons = new Texture2D[4]; 
	int iconWidth = 60;
	List<BTNode> mNodes = new List<BTNode>(){new BTRootNode()};
	bool mMovingElement = false;
	BTNode mSelectedNode = null;
	BTParentNode mSelectedParent = null;
	int mSelectedParentConnector = 0;
	Vector2 mCanvasInitialOffset = Vector2.zero;
	Vector2 mCanvasInitialMousePosition = Vector2.zero;
	Vector2 mCanvasOffset = Vector2.zero;
	
	//****
	
	[MenuItem("Tools/Behavior Tree Editor")]
	public static void Init()
	{
		GetWindow(typeof(BTEditor));
	}
	
	//
	public void OnEnable()
	{
		icons[0] = Resources.Load("Icons/IcoSequence") as Texture2D;
		icons[1] = Resources.Load("Icons/IcoSelector") as Texture2D;
		icons[2] = Resources.Load("Icons/IcoCustom") as Texture2D;
		icons[3] = Resources.Load("Icons/IcoAction") as Texture2D;
		
		mNodes[0].Position = new Vector2(400, 50);
	}
	
	//
	public void OnGUI()
	{
		// Draw connection from connector to mouse while creating new connection
		GUI.BeginGroup(new Rect(0,17,position.width, position.height-17));
		if(mSelectedParent != null && !mMovingElement)
		{
			GLDraw.DrawLine(mSelectedParent.Connector[mSelectedParentConnector].center
							+new Vector2(mSelectedParent.Rectangle.x, mSelectedParent.Rectangle.y)
							+mCanvasOffset, Event.current.mousePosition, Color.black, 2);
			Repaint();
		}
		
		//
		InputHandler();
		ShowNodeHierarchy();
		CanvasGUI();
		ToolbarGUI();
		SelectedNodePropertiesGUI();
		
		// Draw selection Rectangle
		if(mSelectedNode != null)
		{
			Rect border = mSelectedNode.Rectangle;
			GLDraw.DrawBox(new Rect(border.x + mCanvasOffset.x +1, border.y + mCanvasOffset.y+1, border.width-1, border.height-1), Color.red, 1);
		}
		GUI.EndGroup();
		
		SaveLoadToolbarGUI();
	}
	
	/// <summary>
	/// Shows the node hierarchy.
	/// </summary>
	private void ShowNodeHierarchy()
	{
		// 
		foreach(BTNode parentNode in mNodes)
		{
			if(parentNode is BTParentNode)
			{
				for(int i = 0; i < (parentNode as BTParentNode).ChildNodes.Count; i++)
				{
					if((parentNode as BTParentNode).ChildNodes[i] != null)
					{
						// Draw Line from parent to all childs of this parent
						Vector2 parentConnector = (parentNode as BTParentNode).Connector[i].center;
						parentConnector.x += (parentNode as BTParentNode).Rectangle.x;
						parentConnector.y += (parentNode as BTParentNode).Rectangle.y;
						
						//
						GLDraw.DrawLine(parentConnector + mCanvasOffset, (parentNode as BTParentNode).ChildNodes[i].Position + mCanvasOffset, Color.gray, 2);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// GUI of canvas.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance canvas GU; otherwise, <c>false</c>.
	/// </returns>
	private void CanvasGUI()
	{
		foreach(BTNode node in mNodes)
		{
			node.DrawGUI(mCanvasOffset);
		}
	}
	
	/// <summary>
	/// Selected node properties GUI.
	/// </summary>
	private void SelectedNodePropertiesGUI()
	{
		if(mSelectedNode == null)
		{
			return;
		}
		
		Rect rect = new Rect(0,0,150, position.height - 106);
		
		// Draw element
		GUI.skin.label.alignment = TextAnchor.MiddleLeft; 
		GUI.Box(rect, "Properties");
		
		//
		GUI.enabled = mSelectedNode.NameEditEnable;
		GUI.Label(new Rect(5,20,50,20), "Name");
		mSelectedNode.Name = EditorGUI.TextField( new Rect(55,20,85,20), mSelectedNode.Name);
		GUI.enabled = true;
		
		//
		if(mSelectedNode is BTParentNode)
		{
			GUI.Label(new Rect(5,100,70,20), "Connector");
			(mSelectedNode as BTParentNode).ConnectorCount = EditorGUI.IntField(new Rect(75,100,65,20), (mSelectedNode as BTParentNode).ConnectorCount);
			
			GUI.enabled = !(mSelectedNode is BTRootNode);
			GUI.Label(new Rect(5,50,140,20), "UID (Letters Only)");
			mSelectedNode.UniqueIdentifier = EditorGUI.TextField( new Rect(5,70,140,20), mSelectedNode.UniqueIdentifier);
			GUI.enabled = true;
		}
	}
	
	/// <summary>
	/// The GUI of the Nodes Toolbars.
	/// </summary>
	private void ToolbarGUI()
	{
		//
		GUI.Box(new Rect(0,position.height-107, position.width, 90), "", "BOX");
		
		//
		for(int i = 0; i < icons.Length; i++)
		{
			if(GUI.Button(new Rect((i*100) + (position.width - icons.Length*100.0f + iconWidth/2)/2, position.height - 97, iconWidth, iconWidth), icons[i]))
			{
				mSelectedNode = null;
				
				switch(i)
				{
				case 0: mNodes.Add(new BTSequenceNode(){Position = new Vector2(180,40)}); break;
				case 1 : mNodes.Add(new BTSelectorNode(){Position = new Vector2(180,40)}); break;
				case 2 : mNodes.Add(new BTCustomNode(){Position = new Vector2(180,40)}); break;
				case 3 : mNodes.Add(new BTActionNode(){Position = new Vector2(180,40)}); break;
				}
			}
			
			GUI.skin.label.alignment = TextAnchor.MiddleCenter; 
			GUI.Label(new Rect((i*100) + (position.width - icons.Length*100.0f + iconWidth/2)/2, position.height - 37, iconWidth, 15), iconsName[i]);
		}
	}
	
	//
	private void SaveLoadToolbarGUI()
	{
		GUILayout.BeginHorizontal(EditorStyles.toolbar);
		
		//
		if(GUILayout.Button("Save", EditorStyles.toolbarButton)) 
		{
           string path = EditorUtility.SaveFilePanelInProject("Save BehaviorTree", "MyBehaviorTree","xml", "");
			
			if(path != "")
			{
				BTFileManager.Save(mNodes, path);
			}
		}
		
		//
		if(GUILayout.Button("Open", EditorStyles.toolbarButton)) 
		{
            string path = EditorUtility.OpenFilePanel("Load BehaviorTree",Application.dataPath, "xml");
			
			if(path != "")
			{
				mNodes = BTFileManager.Load(path);
			}
		}
		
		//
		GUILayout.FlexibleSpace();
		
		//
		if(GUILayout.Button("Export", EditorStyles.toolbarButton)) 
		{
           	string path = EditorUtility.SaveFilePanelInProject("Generate BehaviorTree class", "MyBehaviorTree","cs", "");
			
			if(path != "")
			{
				BTFileManager.Export(mNodes[0] as BTRootNode, path);
			}
		}
		
		//
  		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Inputs the handler.
	/// </summary>
	public void InputHandler()
	{
		Event e = Event.current;
		
		// Move or Connect nodes inputs
		if(e.isMouse && e.type == EventType.mouseDown && e.button == 0)
		{
			for(int i = mNodes.Count-1; i >= 0; i--)
			{
				if(mNodes[i].Rectangle.Contains(e.mousePosition-mCanvasOffset))
				{
					mSelectedNode = mNodes[i];
					if(mNodes[i].Movable)
					{
						mMovingElement = true;
					}
					else
					{
						Repaint();
						return;
					}
					Repaint();
					break;
				}
			}
			
			// Connect
			if(!mMovingElement)
			{
				for(int i = mNodes.Count-1; i >= 0; i--)
				{
					if(mNodes[i] is BTParentNode)
					{
						for(int c = 0; c < (mNodes[i] as BTParentNode).Connector.Count; c++)
						{
							Rect rect = (mNodes[i] as BTParentNode).Connector[c];
							rect.x += mNodes[i].Rectangle.x;
							rect.y += mNodes[i].Rectangle.y; 
							
							if(rect.Contains(e.mousePosition - mCanvasOffset))
							{
								mSelectedParent = mNodes[i] as BTParentNode;
								mSelectedParentConnector = c;
								Repaint();
								break;
							}
						}
					}
				}
			}
			
			if(!mMovingElement && mSelectedParent == null && mSelectedNode != null)
			{
				if(new Rect(150,0,position.width-150, position.height-90).Contains( e.mousePosition))
				{
					mSelectedNode = null;
				}
			}
		}
		else if(e.isMouse && e.type == EventType.mouseDrag && e.button == 0)
		{
			if(mMovingElement)
			{
				mSelectedNode.Position = e.mousePosition - mCanvasOffset;
				Repaint();
			}
		}
		else if(e.isMouse && e.type == EventType.mouseUp && e.button == 0)
		{
			mMovingElement = false;
			
			//
			if(mSelectedParent != null)
			{
				for(int i = mNodes.Count-1; i >= 0; i--)
				{
					if(mNodes[i] != mSelectedParent)
					{
						if(mNodes[i].Rectangle.Contains(e.mousePosition-mCanvasOffset))
						{
							if(mSelectedParent.ChildNodes.Contains(mNodes[i]))
							{
								mSelectedParent.ChildNodes[mSelectedParent.ChildNodes.IndexOf(mNodes[i])] = null;
							}
							
							if(mSelectedParent.ChildNodes[mSelectedParentConnector] != null)
							{
								mSelectedParent.ChildNodes[mSelectedParentConnector].Parent = null;
							}
							
							mSelectedParent.ChildNodes[mSelectedParentConnector] = mNodes[i];
							mNodes[i].Parent = mSelectedParent;
							Repaint();
							
							break;
						}
					}
				}
				
				//
				mSelectedParent = null;
			}
			//
			Repaint();
		}
	
		// Pan Inputs
		if(e.isMouse && e.type == EventType.mouseDown && e.button == 2)
		{
			mCanvasInitialOffset = mCanvasOffset;
			mCanvasInitialMousePosition = e.mousePosition;
			Repaint();
		}
		else if(e.isMouse && e.type == EventType.mouseDrag && e.button == 2)
		{
			mCanvasOffset = mCanvasInitialOffset + (e.mousePosition-mCanvasInitialMousePosition);
			Repaint();
		}
		
		// Delete node Inputs
		if(e.isKey && e.type == EventType.keyDown && e.keyCode == KeyCode.Delete && mSelectedNode != null && mSelectedNode.Deletable) 
		{
			if(mSelectedNode != null && mSelectedNode.Parent != null)
			{
				mSelectedNode.Parent.ChildNodes[mSelectedNode.Parent.ChildNodes.IndexOf(mSelectedNode)] = null;
			}
			
			mNodes.Remove(mSelectedNode);
			mSelectedNode = null;
			Repaint();
		}
	}
}
