using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DummyOnlineInterface : OnlineInterface
{
	public OnlineUser MainUser
	{
		get
		{
			return null;
		}
	}

	public OnlineUser SecondaryUser
	{
		get
		{
			return null;
		}
	}

	public bool CloudStorageInitialized
	{
		get
		{
			return true;
		}
	}

	public bool SupportsMultipleUsers
	{
		get
		{
			return false;
		}
	}

	public bool SupportsUserSignIn
	{
		get
		{
			return false;
		}
	}

	public event SignInEventHandler OnUserSignedIn;

	public event SignOutEventHandler OnUserSignedOut;

	public void Init()
	{
	}

	public void Reset()
	{
	}

	public void SignInUser(bool silent, PlayerId player, ulong controllerId)
	{
		this.OnUserSignedIn(null);
	}

	public void SwitchUser(PlayerId player, ulong controllerId)
	{
	}

	public OnlineUser GetUserForController(ulong id)
	{
		return null;
	}

	public List<ulong> GetControllersForUser(PlayerId player)
	{
		return null;
	}

	public bool IsUserSignedIn(PlayerId player)
	{
		return false;
	}

	public OnlineUser GetUser(PlayerId player)
	{
		return null;
	}

	public void SetUser(PlayerId player, OnlineUser user)
	{
	}

	public Texture2D GetProfilePic(PlayerId player)
	{
		return null;
	}

	public void GetAchievement(PlayerId player, string id, AchievementEventHandler achievementRetrievedHandler)
	{
	}

	public void UnlockAchievement(PlayerId player, string id)
	{
		Debug.Log("UNLOCKED:" + id);
	}

	public void SyncAchievementsAndStats()
	{
	}

	public void SetStat(PlayerId player, string id, int value)
	{
	}

	public void SetStat(PlayerId player, string id, float value)
	{
	}

	public void SetStat(PlayerId player, string id, string value)
	{
	}

	public void IncrementStat(PlayerId player, string id, int value)
	{
	}

	public void SetRichPresence(PlayerId player, string id, bool active)
	{
	}

	public void SetRichPresenceActive(PlayerId player, bool active)
	{
	}

	public void InitializeCloudStorage(PlayerId player, InitializeCloudStoreHandler handler)
	{
		handler(true);
	}

	public void UninitializeCloudStorage()
	{
	}

	public void SaveCloudData(IDictionary<string, string> data, SaveCloudDataHandler handler)
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cuphead\\");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		foreach (string key in data.Keys)
		{
			try
			{
				TextWriter textWriter = new StreamWriter(Path.Combine(text, key + ".sav"));
				textWriter.Write(data[key]);
				textWriter.Close();
			}
			catch
			{
				handler(false);
				return;
			}
		}
		handler(true);
	}

	public void LoadCloudData(string[] keys, LoadCloudDataHandler handler)
	{
		string[] array = new string[keys.Length];
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cuphead\\");
		for (int i = 0; i < array.Length; i++)
		{
			string path2 = Path.Combine(path, keys[i] + ".sav");
			if (File.Exists(path2))
			{
				try
				{
					TextReader textReader = new StreamReader(Path.Combine(path, keys[i] + ".sav"));
					array[i] = textReader.ReadToEnd();
					textReader.Close();
				}
				catch
				{
					Debug.Log("Error loading save file");
					handler(array, CloudLoadResult.Failed);
					return;
				}
				continue;
			}
			Debug.Log("No save file found");
			handler(array, CloudLoadResult.NoData);
			return;
		}
		handler(array, CloudLoadResult.Success);
	}

	public void UpdateControllerMapping()
	{
	}

	public bool ControllerMappingChanged()
	{
		return false;
	}
}
