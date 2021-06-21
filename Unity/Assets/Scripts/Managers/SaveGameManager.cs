using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveGameManager {

	#region CONSTANTS
	
	public static readonly string SAVEGAME_FILE = Application.persistentDataPath+"/SaveGame/data.svg";
	public static readonly string TEMP_SUFFIX = ".temp";
	
	#endregion

	#region PUBLIC_METHODS

	#endregion

	public static void DeleteTempFiles()
	{
		//
		if (File.Exists(SAVEGAME_FILE+TEMP_SUFFIX))
		{
			File.Delete(SAVEGAME_FILE+TEMP_SUFFIX);
		}
	}
	
	#region SAVE_METHODS
	
	public static void SaveGame()
	{
		FileStream fs = null;

		try
		{
			//
			if (!Directory.Exists(Application.persistentDataPath+"/SaveGame"))
			{
				Directory.CreateDirectory(Application.persistentDataPath+"/SaveGame");
			}

			//Backup save game to be able to restore it if something goes wrong
			if (File.Exists(SAVEGAME_FILE))
			{
				//
				File.Copy(SAVEGAME_FILE,SAVEGAME_FILE+TEMP_SUFFIX);
			}
			
			fs = new FileStream(SAVEGAME_FILE, FileMode.Create);
			
			//
			BinaryFormatter formatter = new BinaryFormatter();

			//Save Characters List count and Rooms count
			formatter.Serialize(fs,CharacterManager.Instance.Characters.Count);
			formatter.Serialize(fs,RoomManager.Instance.Rooms.Count);
			
			//Rooms
			foreach(Room room in RoomManager.Instance.Rooms)
			{
				//
				formatter.Serialize(fs,room.Serialize());
			}

			//Characters
			foreach(Character character in CharacterManager.Instance.Characters)
			{
				formatter.Serialize(fs,character.Serialize());
			}

			//Elevators
			formatter.Serialize(fs,ElevatorManager.Instance.Elevators);

			//GraphicsManager
			formatter.Serialize(fs,GraphicsManager.Instance.mDepth);

			//Id Generator
			formatter.Serialize(fs,Character.IDGenerator);

			formatter.Serialize(fs,GameManager.Instance.UserStats);

			//Ingredients and viruses
			formatter.Serialize(fs,CraftingManager.Instance.Serialize());
			
			//Junks
			List<Vector3SerializationInfo> junks = new List<Vector3SerializationInfo>();
			
			foreach(var junk in TargetManager.Instance.JunkQueue)
			{
				junks.Add(new Vector3SerializationInfo(junk.transform.position.x,
							junk.transform.position.y,junk.transform.position.z));
			}
			
			formatter.Serialize(fs,junks);

			//Time Manager
			formatter.Serialize(fs,TimeManager.Instance.Serialize());

			//Room Manager
			formatter.Serialize(fs,RoomManager.Instance.Serialize());

			//NameGenerator
			formatter.Serialize(fs,new SerializableDictionary<ERace, List<string>[]>(CharacterNameGenerator.Instance.UsedNames));

			//ContractManager
			formatter.Serialize(fs,ContractManager.Instance.Contracts);

			//
			fs.Close();
		}
		catch(Exception e)
		{
			Debug.Log("Could not save game : "+e.GetType()+" : "+e.Message+"\n STACK TRACE : "+e.StackTrace);

			//Close file.
			if (fs != null)
			{
				fs.Close();
			}

			//Restore backup if we had an error
			if (File.Exists(SAVEGAME_FILE+TEMP_SUFFIX))
			{
				try
				{
					//
					File.Delete(SAVEGAME_FILE);
					File.Copy(SAVEGAME_FILE+TEMP_SUFFIX,SAVEGAME_FILE);
				}
				catch(Exception e2)
				{
					Debug.Log("Could not restore save game : "+e2.Message);
				}
			}
		}
		finally
		{
			//Delete backup
			if (File.Exists(SAVEGAME_FILE+TEMP_SUFFIX))
			{
				File.Delete(SAVEGAME_FILE+TEMP_SUFFIX);
			}
		}
	}
	
	
	
	#endregion
	
	#region LOAD_METHODS
	
	public static void LoadGame()
	{
		try
		{
			//
			RoomManager.Instance.ResetRooms();
			CharacterManager.Instance.ResetCharacters();
			
			FileStream fs = new FileStream(SAVEGAME_FILE, FileMode.Open);
			
			BinaryFormatter formatter = new BinaryFormatter();

			int characterCount = (int)formatter.Deserialize(fs);
			int roomCount = (int)formatter.Deserialize(fs);
			
			//Rooms
			for(int i = 0;i <roomCount;i++)
			{
				//
				RoomSerializationInfo roomInfo = (RoomSerializationInfo)formatter.Deserialize(fs);
				
				//
				RoomManager.Instance.AddRoom(roomInfo);
			}

			//Characters
			for(int i = 0;i <characterCount;i++)
			{
				//
				CharacterSerializationInfo charInfo = (CharacterSerializationInfo)formatter.Deserialize(fs);
				
				//
				CharacterManager.Instance.AddCharacter(charInfo);
			}

			//Elevators
			List<ElevatorLine> elevators = formatter.Deserialize(fs) as List<ElevatorLine>;
			ElevatorManager.Instance.Initialize(elevators);

			//Room's charaters list
			foreach(Character character in CharacterManager.Instance.Characters)
			{
				RoomManager.Instance.UpdateCharacterRoom(character);
			}

			//
			GraphicsManager.Instance.mDepth = (float)formatter.Deserialize(fs);

			//
			RoomManager.Instance.InitializeCharactersInRoom();

			//Id Generator
			Character.InitializeIDGenerator((int)formatter.Deserialize(fs));

			//
			GameManager.Instance.UserStats = (UserStats)formatter.Deserialize(fs);
				
			//Ingredients and viruses
			CraftingManager.Instance.Deserialize((CraftingManagerSerializationInfo)formatter.Deserialize(fs));
			
			//Junks
			List<Vector3SerializationInfo> junks = (List<Vector3SerializationInfo>)formatter.Deserialize(fs);

			//TimeManager
			TimeManager.Instance.Deserialize((TimeManagerSerializationInfo)formatter.Deserialize(fs));
			
			//
			foreach(var junk in junks)
			{
				TargetManager.Instance.AddJunk(junk.x,junk.y,junk.z);
			}
			
			//
			foreach(Room r in RoomManager.Instance.Rooms)
			{
				r.PostInitialization();
			}

			//
			RoomManager.Instance.Deserialize(formatter.Deserialize(fs) as RoomManagerSerializationInfo);

			//NameGenerator
			CharacterNameGenerator.Instance.UsedNames = ((SerializableDictionary<ERace, List<string>[]>)formatter.Deserialize(fs)).ToDictionary();

			//ContractManager
			ContractManager.Instance.Contracts = (List<Contract>)formatter.Deserialize(fs);

			//Post serialization
			foreach(Character c in CharacterManager.Instance.Characters)
			{
				c.PostSerialization();
			}

			//
			fs.Close();
		}
		catch (Exception e)
		{
			Debug.Log("Could not load the SaveGame "+e.GetType()+" : "+e.Message+"\n STACK TRACE : "+System.Environment.StackTrace);
		}
	}
	
	#endregion
}
