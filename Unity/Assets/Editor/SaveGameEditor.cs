using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class SaveGameEditor : EditorWindow {

	const string BACKUP_SUFFIX = ".bac";

	//
	[MenuItem("Tools/SaveGame Editor")]
	static void Init()
	{
		GetWindow(typeof(SaveGameEditor));
	}

	#region MONO_METHODS

	void OnGUI()
	{
		if (GUILayout.Button("Delete SaveGame"))
		{
			DeleteSaveGame();
		}

		if (GUILayout.Button("Backup SaveGame"))
		{
			BackupSaveGame();
		}

		if (GUILayout.Button("Restore SaveGame backup"))
		{
			RestoreSaveGameBackup();
		}
	}

	#endregion

	#region PRIVATE_METHODS

	void BackupSaveGame()
	{
		//
		if (File.Exists(SaveGameManager.SAVEGAME_FILE))
		{
			try
			{
				//
				if (File.Exists(SaveGameManager.SAVEGAME_FILE+BACKUP_SUFFIX))
				{
					File.Delete(SaveGameManager.SAVEGAME_FILE+BACKUP_SUFFIX);
				}

				//
				File.Copy(SaveGameManager.SAVEGAME_FILE,SaveGameManager.SAVEGAME_FILE+BACKUP_SUFFIX);

				EditorUtility.DisplayDialog("Success","A backup of the SaveGame has been successfully saved.","Ok");
			}
			catch(System.Exception e)
			{
				EditorUtility.DisplayDialog("Failure","Could not backup SaveGame : "+e.Message,"Ok");
			}
		}
		else
		{
			EditorUtility.DisplayDialog("Error","There is no SaveGame to backup.","Ok");
		}
	}

	void RestoreSaveGameBackup()
	{
		//
		if (File.Exists(SaveGameManager.SAVEGAME_FILE+BACKUP_SUFFIX))
		{
			try
			{
				//
				if (File.Exists(SaveGameManager.SAVEGAME_FILE))
				{
					File.Delete(SaveGameManager.SAVEGAME_FILE);
				}
				
				//
				File.Copy(SaveGameManager.SAVEGAME_FILE+BACKUP_SUFFIX,SaveGameManager.SAVEGAME_FILE);
				
				EditorUtility.DisplayDialog("Success","SaveGame was restored from backup successfully.","Ok");
			}
			catch(System.Exception e)
			{
				EditorUtility.DisplayDialog("Failure","Could restore SaveGame backup : "+e.Message,"Ok");
			}
		}
		else
		{
			EditorUtility.DisplayDialog("Error","There is no backup to restore.","Ok");
		}
	}

	void DeleteSaveGame()
	{
		if (EditorUtility.DisplayDialog("Delete SaveGame ?","Do you really want to delete your SaveGame, all progress will be lost ?","Yes","No"))
		{
			try
			{
				File.Delete(SaveGameManager.SAVEGAME_FILE);
				
				//Try to delete backup
				if (File.Exists(SaveGameManager.SAVEGAME_FILE+SaveGameManager.TEMP_SUFFIX))
				{
					File.Delete(SaveGameManager.SAVEGAME_FILE+SaveGameManager.TEMP_SUFFIX);
				}
				
				EditorUtility.DisplayDialog("Delete succeded","Deleted SaveGame successfully !","Ok");
			}
			catch(System.Exception e)
			{
				EditorUtility.DisplayDialog("Delete failed","Could not delete SaveGame : "+e.Message,"Ok");
			}
		}
	}

	#endregion
}
