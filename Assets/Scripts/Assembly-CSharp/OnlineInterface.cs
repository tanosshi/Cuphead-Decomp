using System.Collections.Generic;
using UnityEngine;

public interface OnlineInterface
{
	OnlineUser MainUser { get; }

	OnlineUser SecondaryUser { get; }

	bool CloudStorageInitialized { get; }

	bool SupportsMultipleUsers { get; }

	bool SupportsUserSignIn { get; }

	event SignInEventHandler OnUserSignedIn;

	event SignOutEventHandler OnUserSignedOut;

	void Init();

	void Reset();

	void SignInUser(bool silent, PlayerId player, ulong controllerId);

	void SwitchUser(PlayerId player, ulong controllerId);

	OnlineUser GetUserForController(ulong id);

	List<ulong> GetControllersForUser(PlayerId player);

	bool IsUserSignedIn(PlayerId player);

	OnlineUser GetUser(PlayerId player);

	void SetUser(PlayerId player, OnlineUser user);

	Texture2D GetProfilePic(PlayerId player);

	void GetAchievement(PlayerId player, string id, AchievementEventHandler achievementRetrievedHandler);

	void UnlockAchievement(PlayerId player, string id);

	void SyncAchievementsAndStats();

	void SetStat(PlayerId player, string id, int value);

	void SetStat(PlayerId player, string id, float value);

	void SetStat(PlayerId player, string id, string value);

	void IncrementStat(PlayerId player, string id, int value);

	void SetRichPresence(PlayerId player, string id, bool active);

	void SetRichPresenceActive(PlayerId player, bool active);

	void InitializeCloudStorage(PlayerId player, InitializeCloudStoreHandler handler);

	void UninitializeCloudStorage();

	void SaveCloudData(IDictionary<string, string> data, SaveCloudDataHandler handler);

	void LoadCloudData(string[] keys, LoadCloudDataHandler handler);

	void UpdateControllerMapping();

	bool ControllerMappingChanged();
}
