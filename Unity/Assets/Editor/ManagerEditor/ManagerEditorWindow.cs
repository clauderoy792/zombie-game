using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ManagerEditorWindow : EditorWindow {
	
	string mClassName;
	bool mIsMonoBehavior;
	
	[MenuItem("Tools/Manager Editor")]
	static void Init()
	{
		GetWindow(typeof(ManagerEditorWindow));
	}
	
	#region MONO_METHODS
	
	//
	void OnGUI()
	{
		MainMenuGUI();
	}
	
	#endregion
	
	#region PRIVATE_METHODS
	
	void MainMenuGUI()
	{
		//
		mIsMonoBehavior = EditorGUILayout.Toggle("Is MonoBehaviour ?",mIsMonoBehavior);
		
		//
		if(GUILayout.Button("Create Manager", EditorStyles.toolbarButton)) 
		{
			string path = EditorUtility.SaveFilePanelInProject("Save Manager", "MyManager","cs","");
		
			if(!string.IsNullOrEmpty(path))
			{
				//
				mClassName = path.Substring(path.LastIndexOf("/")+1,path.LastIndexOf(".")-(path.LastIndexOf("/")+1));
				
				//
				if (mIsMonoBehavior)
				{
					//
					CreateMonoBehaviourManager(path);
				}
				else
				{
					//
					CreateManager(path);
				}
				
				//
				AssetDatabase.Refresh();
			}
		}
	}
	
	void CreateManager(string aPath)
	{
		//
		FileStream stream = new FileStream(aPath,
							FileMode.Create,FileAccess.ReadWrite,FileShare.None);
		
		//
		TextWriter writer = new StreamWriter(stream);
		
		//Usings
		writer.WriteLine("using UnityEngine;");
		writer.WriteLine("using System.Collections;");
		writer.WriteLine("");
		
		//Class
		writer.WriteLine("public class "+mClassName);
		writer.WriteLine("{");
		
		//
		writer.WriteLine("\t#region CONSTANTS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region STATIC_MEMBERS");
		writer.WriteLine("");
		writer.WriteLine("\tprivate static "+mClassName+" sInstance;");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region PRIVATE_MEMBERS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region ACCESSORS");
		writer.WriteLine("");
		writer.WriteLine("\tpublic static "+mClassName+" Instance");
		writer.WriteLine("\t{");
		writer.WriteLine("\t\tget");
		writer.WriteLine("\t\t{");
		writer.WriteLine("\t\t\tif (sInstance == null)");
		writer.WriteLine("\t\t\t{");
		writer.WriteLine("\t\t\t\tsInstance = new "+mClassName+"();");
		writer.WriteLine("\t\t\t}");
		writer.WriteLine("");
		writer.WriteLine("\t\t\treturn sInstance;");
		writer.WriteLine("\t\t}");
		writer.WriteLine("\t}");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region CONSTRUCTORS");
		writer.WriteLine("");
		writer.WriteLine("\tprivate "+mClassName+" ()");
		writer.WriteLine("\t{");
		writer.WriteLine("");
		writer.WriteLine("\t}");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		writer.WriteLine("\t#region PUBLIC_METHODS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		writer.WriteLine("\t#region PRIVATE_METHODS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		
		writer.WriteLine("}");
	
		//
		writer.Close();
		stream.Close();
	}
	
	void CreateMonoBehaviourManager(string aPath)
	{
		//
		FileStream stream = new FileStream(aPath,
							FileMode.Create,FileAccess.ReadWrite,FileShare.None);
		
		//
		TextWriter writer = new StreamWriter(stream);
		
		//Usings
		writer.WriteLine("using UnityEngine;");
		writer.WriteLine("using System.Collections;");
		writer.WriteLine("");
		
		//Class
		writer.WriteLine("public class "+mClassName+" : MonoBehaviour");
		writer.WriteLine("{");
		
		//
		writer.WriteLine("\t#region CONSTANTS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region STATIC_MEMBERS");
		writer.WriteLine("");
		writer.WriteLine("\tprivate static "+mClassName+" sInstance;");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region PRIVATE_MEMBERS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region ACCESSORS");
		writer.WriteLine("");
		writer.WriteLine("\tpublic static "+mClassName+" Instance");
		writer.WriteLine("\t{");
		writer.WriteLine("\t\tget {return sInstance;}");
		writer.WriteLine("\t}");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		//
		writer.WriteLine("\t#region MONO_METHODS");
		writer.WriteLine("");
		writer.WriteLine("\tvoid Awake()");
		writer.WriteLine("\t{");
		writer.WriteLine("\t\tif (sInstance == null)");
		writer.WriteLine("\t\t{");
		writer.WriteLine("\t\t\tsInstance = this;");
		writer.WriteLine("\t\t}");
		writer.WriteLine("\t\telse");
		writer.WriteLine("\t\t{");
		writer.WriteLine("\t\t\tDebug.Log(\"There is already an instance of the "+mClassName+" in the scene.\");");
		writer.WriteLine("\t\t}");
		writer.WriteLine("\t}");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		writer.WriteLine("\t#region PUBLIC_METHODS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		writer.WriteLine("");
		
		writer.WriteLine("\t#region PRIVATE_METHODS");
		writer.WriteLine("");
		writer.WriteLine("\t#endregion");
		
		writer.WriteLine("}");
		
		//
		writer.Close();
		stream.Close();
	}
	
	#endregion
}
