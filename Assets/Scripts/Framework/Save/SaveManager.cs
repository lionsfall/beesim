using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Dogabeey.SimpleJSON;
// using Firebase.Database;
using System.Threading;

namespace Dogabeey
{
	public class SaveManager : SingletonComponent<SaveManager>
	{
		#region Member Variables

		private List<ISaveable>	saveables;
		private JSONNode		loadedSave;
		private const string LogTag = "SaveManager";
		static string DATABASE_URL = "https://wordstones-users.firebaseio.com/";
		public static string userId;
		[SerializeField] bool saveOnQuit = true;

		#endregion

		#region Properties

		/// <summary>
		/// Path to the save file on the device
		/// </summary>
		public static string SaveFilePath { get { return Application.persistentDataPath + "/save.json"; } }

		/// <summary>
		/// List of registered saveables
		/// </summary>
		private List<ISaveable> Saveables
		{
			get
			{
				if (saveables == null)
				{
					saveables = new List<ISaveable>();
				}

				return saveables;
			}
		}

		#endregion

		#region Unity Methods

		private void Start()
		{
			Debug.Log("Save file path: " + SaveFilePath);
		}

		private void OnDestroy()
		{
			if(saveOnQuit)
			{
				Save();
			}
			
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause && saveOnQuit)
			{
				Save();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registers a saveable to be saved
		/// </summary>
		public void Register(ISaveable saveable)
		{
			Saveables.Add(saveable);
		}

		/// <summary>
		/// Loads the save data for the given saveable
		/// </summary>
		public JSONNode LoadSave(ISaveable saveable)
		{
			return LoadSave(saveable.SaveId);
		}

		/// <summary>
		/// Loads the save data for the given save id
		/// </summary>
		public JSONNode LoadSave(string saveId)
		{
			// Check if the save file has been loaded and if not try and load it
			if (loadedSave == null && !LoadSave(out loadedSave))
			{
				return null;
			}

			// Check if the loaded save file has the given save id
			if (!loadedSave.AsObject.HasKey(saveId))
			{
				return null;
			}

			// Return the JSONNode for the save id
			return loadedSave[saveId];
		}

		public SimpleJSON.JSONObject LoadSaveObject(string saveId)
		{
			JSONNode node = LoadSave(saveId);
			if(node == null)
			{
				return null;
			}
			return node.AsObject;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Saves all registered saveables to the save file
		/// </summary>
		public void Save(Action completionHandler = null)
		{
			Dictionary<string, object> saveJson = new Dictionary<string, object>();
			if(saveables != null)
			{
				for (int i = 0; i < saveables.Count; i++)
				{
					//saveJson.Add(saveables[i].SaveId, saveables[i].Save());
					if(saveJson.ContainsKey(saveables[i].SaveId))
					{
						saveJson[saveables[i].SaveId] = saveables[i].Save();
					}else
					{
						saveJson.Add(saveables[i].SaveId, saveables[i].Save());
					}
				}
				
				if(UserManager.currentUser != null){
					saveToCloud(userId,"gameData",saveJson);
				}
				
				
				System.IO.File.WriteAllText(SaveFilePath, JsonConvert.SerializeObject(saveJson));
			}
			
			if(completionHandler != null){completionHandler();};
		}

		/// <summary>
		/// Tries to load the save file
		/// </summary>
		private bool LoadSave(out JSONNode json)
		{
			json = null;

			if (!System.IO.File.Exists(SaveFilePath))
			{
				return false;
			}


			// if(UserManager.currentUser != null && UserManager.currentUser.gameData != null)
			// {
			// 	//Firebase bos stringleri donuste okunamadigi icin yerine dolar koyuyoruz. Simdi donus degerinde replace ediyoruz.
			// 	json = JSON.Parse(JsonConvert.SerializeObject(UserManager.currentUser.gameData).Replace("$$","\\u0000"));
			// 	//json = JSON.Parse(JsonUtility.ToJson(UserManager.currentUser.gameData));
			// 	Debug.Log("--------> read data from user data");
			// }else
			// {
			// 	json = JSON.Parse(System.IO.File.ReadAllText(SaveFilePath));
			// 	Debug.Log("--------> read data from local");
			// }

			json = JSON.Parse(System.IO.File.ReadAllText(SaveFilePath));

			

			return json != null;
		}


		public void saveToCloud(string userId,string tag,object model)
		{
			Thread thread = new Thread(()=>{
				//Firebase bos stringleri donuste okunamadigi icin yerine dolar koyuyoruz.
				string js = JsonConvert.SerializeObject(model).Replace("\\u0000","$$");


				// FirebaseDatabase.GetInstance(DATABASE_URL).GetReference("users").Child(userId).Child(tag).SetRawJsonValueAsync(js).ContinueWith((task => {
				// 	if (task.IsFaulted) {
				// 		Debug.Log("------->data save failed");
				// 	}
				// 	else if (task.IsCompleted) {
				// 		Debug.Log("------->data saved successfully");
				// 	}
				// }));
			});
			thread.Start();
			
		}

		#endregion
	}
}
