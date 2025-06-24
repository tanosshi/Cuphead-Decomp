using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Data
{
	public class UserDataStore_Cuphead : UserDataStore
	{
		private const string thisScriptName = "UserDataStore_PlayerPrefs";

		private const string editorLoadedMessage = "\nIf unexpected input issues occur, the loaded XML data may be outdated or invalid. Clear PlayerPrefs using the inspector option on the UserDataStore_PlayerPrefs component.";

		[SerializeField]
		private bool isEnabled = true;

		[SerializeField]
		private bool loadDataOnStart = true;

		[SerializeField]
		private string playerPrefsKeyPrefix = "RewiredSaveData";

		public override void Save()
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
			}
			else
			{
				SaveAll();
			}
		}

		public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
			}
			else
			{
				SaveControllerDataNow(playerId, controllerType, controllerId);
			}
		}

		public override void SaveControllerData(ControllerType controllerType, int controllerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
			}
			else
			{
				SaveControllerDataNow(controllerType, controllerId);
			}
		}

		public override void SavePlayerData(int playerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
			}
			else
			{
				SavePlayerDataNow(playerId);
			}
		}

		public override void SaveInputBehavior(int playerId, int behaviorId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
			}
			else
			{
				SaveInputBehaviorNow(playerId, behaviorId);
			}
		}

		public override void Load()
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
			}
			else
			{
				int num = LoadAll();
			}
		}

		public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
			}
			else
			{
				int num = LoadControllerDataNow(playerId, controllerType, controllerId);
			}
		}

		public override void LoadControllerData(ControllerType controllerType, int controllerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
			}
			else
			{
				int num = LoadControllerDataNow(controllerType, controllerId);
			}
		}

		public override void LoadPlayerData(int playerId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
			}
			else
			{
				int num = LoadPlayerDataNow(playerId);
			}
		}

		public override void LoadInputBehavior(int playerId, int behaviorId)
		{
			if (!isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
			}
			else
			{
				int num = LoadInputBehaviorNow(playerId, behaviorId);
			}
		}

		protected override void OnInitialize()
		{
			if (loadDataOnStart)
			{
				Load();
			}
		}

		protected override void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			if (isEnabled && args.controllerType == ControllerType.Joystick)
			{
				int num = LoadJoystickData(args.controllerId);
			}
		}

		protected override void OnControllerPreDiscconnect(ControllerStatusChangedEventArgs args)
		{
			if (isEnabled && args.controllerType == ControllerType.Joystick)
			{
				SaveJoystickData(args.controllerId);
			}
		}

		protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (isEnabled)
			{
			}
		}

		private int LoadAll()
		{
			int num = 0;
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				num += LoadPlayerDataNow(allPlayers[i]);
			}
			return num + LoadAllJoystickCalibrationData();
		}

		private int LoadPlayerDataNow(int playerId)
		{
			return LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
		}

		private int LoadPlayerDataNow(Player player)
		{
			if (player == null)
			{
				return 0;
			}
			int num = 0;
			num += LoadInputBehaviors(player.id);
			num += LoadControllerMaps(player.id, ControllerType.Keyboard, 0);
			num += LoadControllerMaps(player.id, ControllerType.Mouse, 0);
			foreach (Joystick joystick in player.controllers.Joysticks)
			{
				num += LoadControllerMaps(player.id, ControllerType.Joystick, joystick.id);
			}
			return num;
		}

		private int LoadAllJoystickCalibrationData()
		{
			int num = 0;
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				num += LoadJoystickCalibrationData(joysticks[i]);
			}
			return num;
		}

		private int LoadJoystickCalibrationData(Joystick joystick)
		{
			if (joystick == null)
			{
				return 0;
			}
			return joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)) ? 1 : 0;
		}

		private int LoadJoystickCalibrationData(int joystickId)
		{
			return LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		private int LoadJoystickData(int joystickId)
		{
			int num = 0;
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					num += LoadControllerMaps(player.id, ControllerType.Joystick, joystickId);
				}
			}
			return num + LoadJoystickCalibrationData(joystickId);
		}

		private int LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			int num = 0;
			num += LoadControllerMaps(playerId, controllerType, controllerId);
			return num + LoadControllerDataNow(controllerType, controllerId);
		}

		private int LoadControllerDataNow(ControllerType controllerType, int controllerId)
		{
			int num = 0;
			if (controllerType == ControllerType.Joystick)
			{
				num += LoadJoystickCalibrationData(controllerId);
			}
			return num;
		}

		private int LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			int num = 0;
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return num;
			}
			Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
			if (controller == null)
			{
				return num;
			}
			List<string> allControllerMapsXml = GetAllControllerMapsXml(player, true, controllerType, controller);
			if (allControllerMapsXml.Count == 0)
			{
				return num;
			}
			return num + player.controllers.maps.AddMapsFromXml(controllerType, controllerId, allControllerMapsXml);
		}

		private int LoadInputBehaviors(int playerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return 0;
			}
			int num = 0;
			IList<InputBehavior> inputBehaviors = ReInput.mapping.GetInputBehaviors(player.id);
			for (int i = 0; i < inputBehaviors.Count; i++)
			{
				num += LoadInputBehaviorNow(player, inputBehaviors[i]);
			}
			return num;
		}

		private int LoadInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return 0;
			}
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return 0;
			}
			return LoadInputBehaviorNow(player, inputBehavior);
		}

		private int LoadInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player == null || inputBehavior == null)
			{
				return 0;
			}
			string inputBehaviorXml = GetInputBehaviorXml(player, inputBehavior.id);
			if (inputBehaviorXml == null || inputBehaviorXml == string.Empty)
			{
				return 0;
			}
			return inputBehavior.ImportXmlString(inputBehaviorXml) ? 1 : 0;
		}

		private void SaveAll()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				SavePlayerDataNow(allPlayers[i]);
			}
			SaveAllJoystickCalibrationData();
			PlayerPrefs.Save();
		}

		private void SavePlayerDataNow(int playerId)
		{
			SavePlayerDataNow(ReInput.players.GetPlayer(playerId));
			PlayerPrefs.Save();
		}

		private void SavePlayerDataNow(Player player)
		{
			if (player != null)
			{
				PlayerSaveData saveData = player.GetSaveData(true);
				SaveInputBehaviors(player, saveData);
				SaveControllerMaps(player, saveData);
			}
		}

		private void SaveAllJoystickCalibrationData()
		{
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				SaveJoystickCalibrationData(joysticks[i]);
			}
		}

		private void SaveJoystickCalibrationData(int joystickId)
		{
			SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		private void SaveJoystickCalibrationData(Joystick joystick)
		{
			if (joystick != null)
			{
				JoystickCalibrationMapSaveData calibrationMapSaveData = joystick.GetCalibrationMapSaveData();
				string joystickCalibrationMapPlayerPrefsKey = GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
				PlayerPrefs.SetString(joystickCalibrationMapPlayerPrefsKey, calibrationMapSaveData.map.ToXmlString());
			}
		}

		private void SaveJoystickData(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					SaveControllerMaps(player.id, ControllerType.Joystick, joystickId);
				}
			}
			SaveJoystickCalibrationData(joystickId);
		}

		private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			SaveControllerMaps(playerId, controllerType, controllerId);
			SaveControllerDataNow(controllerType, controllerId);
			PlayerPrefs.Save();
		}

		private void SaveControllerDataNow(ControllerType controllerType, int controllerId)
		{
			if (controllerType == ControllerType.Joystick)
			{
				SaveJoystickCalibrationData(controllerId);
			}
			PlayerPrefs.Save();
		}

		private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData)
		{
			foreach (ControllerMapSaveData allControllerMapSaveDatum in playerSaveData.AllControllerMapSaveData)
			{
				string controllerMapPlayerPrefsKey = GetControllerMapPlayerPrefsKey(player, allControllerMapSaveDatum);
				PlayerPrefs.SetString(controllerMapPlayerPrefsKey, allControllerMapSaveDatum.map.ToXmlString());
			}
		}

		private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null || !player.controllers.ContainsController(controllerType, controllerId))
			{
				return;
			}
			ControllerMapSaveData[] mapSaveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, true);
			if (mapSaveData != null)
			{
				for (int i = 0; i < mapSaveData.Length; i++)
				{
					string controllerMapPlayerPrefsKey = GetControllerMapPlayerPrefsKey(player, mapSaveData[i]);
					PlayerPrefs.SetString(controllerMapPlayerPrefsKey, mapSaveData[i].map.ToXmlString());
				}
			}
		}

		private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData)
		{
			if (player != null)
			{
				InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
				for (int i = 0; i < inputBehaviors.Length; i++)
				{
					SaveInputBehaviorNow(player, inputBehaviors[i]);
				}
			}
		}

		private void SaveInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player != null)
			{
				InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
				if (inputBehavior != null)
				{
					SaveInputBehaviorNow(player, inputBehavior);
					PlayerPrefs.Save();
				}
			}
		}

		private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player != null && inputBehavior != null)
			{
				string inputBehaviorPlayerPrefsKey = GetInputBehaviorPlayerPrefsKey(player, inputBehavior);
				PlayerPrefs.SetString(inputBehaviorPlayerPrefsKey, inputBehavior.ToXmlString());
			}
		}

		private string GetBasePlayerPrefsKey(Player player)
		{
			string text = playerPrefsKeyPrefix;
			return text + "|playerName=" + player.name;
		}

		private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=ControllerMap";
			basePlayerPrefsKey = basePlayerPrefsKey + "|controllerMapType=" + saveData.mapTypeString;
			string text = basePlayerPrefsKey;
			basePlayerPrefsKey = text + "|categoryId=" + saveData.map.categoryId + "|layoutId=" + saveData.map.layoutId;
			basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareIdentifier=" + saveData.controllerHardwareIdentifier;
			if (saveData.mapType == typeof(JoystickMap))
			{
				basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareGuid=" + ((JoystickMapSaveData)saveData).joystickHardwareTypeGuid.ToString();
			}
			return basePlayerPrefsKey;
		}

		private string GetControllerMapXml(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=ControllerMap";
			basePlayerPrefsKey = basePlayerPrefsKey + "|controllerMapType=" + controller.mapTypeString;
			string text = basePlayerPrefsKey;
			basePlayerPrefsKey = text + "|categoryId=" + categoryId + "|layoutId=" + layoutId;
			basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareIdentifier=" + controller.hardwareIdentifier;
			if (controllerType == ControllerType.Joystick)
			{
				Joystick joystick = (Joystick)controller;
				basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			}
			if (!PlayerPrefs.HasKey(basePlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(basePlayerPrefsKey);
		}

		private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller)
		{
			List<string> list = new List<string>();
			IList<InputMapCategory> mapCategories = ReInput.mapping.MapCategories;
			for (int i = 0; i < mapCategories.Count; i++)
			{
				InputMapCategory inputMapCategory = mapCategories[i];
				if (userAssignableMapsOnly && !inputMapCategory.userAssignable)
				{
					continue;
				}
				IList<InputLayout> list2 = ReInput.mapping.MapLayouts(controllerType);
				for (int j = 0; j < list2.Count; j++)
				{
					InputLayout inputLayout = list2[j];
					string controllerMapXml = GetControllerMapXml(player, controllerType, inputMapCategory.id, inputLayout.id, controller);
					if (!(controllerMapXml == string.Empty))
					{
						list.Add(controllerMapXml);
					}
				}
			}
			return list;
		}

		private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData)
		{
			string text = playerPrefsKeyPrefix;
			text += "|dataType=CalibrationMap";
			text = text + "|controllerType=" + saveData.controllerType;
			text = text + "|hardwareIdentifier=" + saveData.hardwareIdentifier;
			return text + "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
		}

		private string GetJoystickCalibrationMapXml(Joystick joystick)
		{
			string text = playerPrefsKeyPrefix;
			text += "|dataType=CalibrationMap";
			text = text + "|controllerType=" + joystick.type;
			text = text + "|hardwareIdentifier=" + joystick.hardwareIdentifier;
			text = text + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=InputBehavior";
			return basePlayerPrefsKey + "|id=" + saveData.id;
		}

		private string GetInputBehaviorXml(Player player, int id)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=InputBehavior";
			basePlayerPrefsKey = basePlayerPrefsKey + "|id=" + id;
			if (!PlayerPrefs.HasKey(basePlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(basePlayerPrefsKey);
		}
	}
}
