using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
	private const string KEY = "cuphead_settings_data_v1";

	private static SettingsData _data;

	public bool hasBootedUpGame;

	public float overscan;

	public float chromaticAberration;

	public Resolution resolution;

	public int vSyncCount;

	public bool fullScreen;

	public float masterVolume;

	public float sFXVolume;

	public float musicVolume;

	public bool chromaticAberrationEffect;

	public bool noiseEffect;

	public bool subtleBlurEffect;

	[SerializeField]
	private float brightness;

	public static SettingsData Data
	{
		get
		{
			if (_data == null)
			{
				if (hasKey())
				{
					try
					{
						_data = JsonUtility.FromJson<SettingsData>(PlayerPrefs.GetString("cuphead_settings_data_v1"));
					}
					catch (ArgumentException)
					{
						Debug.Log("Unable to parse Settings data");
						_data = new SettingsData();
						Save();
					}
				}
				else
				{
					_data = new SettingsData();
					Save();
				}
				if (_data == null)
				{
					Debug.LogWarning("[SettingsData] Data could not be loaded!");
					return null;
				}
				Debug.Log("[SettingsData] Data loaded: \n" + JsonUtility.ToJson(_data));
				ApplySettings();
			}
			return _data;
		}
	}

	public bool vintageAudioEnabled
	{
		get
		{
			if (!PlayerData.inGame)
			{
				return false;
			}
			return PlayerData.Data.vintageAudioEnabled;
		}
	}

	public BlurGamma.Filter filter
	{
		get
		{
			if (!PlayerData.inGame)
			{
				return BlurGamma.Filter.None;
			}
			return PlayerData.Data.filter;
		}
	}

	public float Brightness
	{
		get
		{
			ClampBrightness();
			return brightness;
		}
		set
		{
			brightness = value;
			ClampBrightness();
		}
	}

	public static event Action OnSettingsAppliedEvent;

	public SettingsData()
	{
		overscan = 0f;
		chromaticAberration = 1f;
		resolution = Screen.currentResolution;
		fullScreen = Screen.fullScreen;
		vSyncCount = QualitySettings.vSyncCount;
		masterVolume = AudioManager.masterVolume;
		sFXVolume = AudioManager.sfxOptionsVolume;
		musicVolume = AudioManager.bgmOptionsVolume;
		hasBootedUpGame = false;
		SetCameraEffectDefaults();
	}

	public static void Save()
	{
		string text = JsonUtility.ToJson(_data);
		PlayerPrefs.SetString("cuphead_settings_data_v1", text);
		PlayerPrefs.Save();
		Debug.Log("[SettingsData] Settings Data Saved: \n" + text);
	}

	public static void Reset()
	{
		_data = new SettingsData();
		Save();
	}

	public static void ApplySettings()
	{
		if (SettingsData.OnSettingsAppliedEvent != null)
		{
			SettingsData.OnSettingsAppliedEvent();
		}
		Save();
	}

	public static void ApplySettingsOnStartup()
	{
		AudioManager.masterVolume = Data.masterVolume;
		AudioManager.sfxOptionsVolume = Data.sFXVolume;
		AudioManager.bgmOptionsVolume = Data.musicVolume;
	}

	private static bool hasKey()
	{
		return PlayerPrefs.HasKey("cuphead_settings_data_v1");
	}

	private void SetCameraEffectDefaults()
	{
		chromaticAberrationEffect = true;
		noiseEffect = true;
		subtleBlurEffect = true;
		brightness = 0f;
	}

	private void ClampBrightness()
	{
		if (brightness < -1f)
		{
			brightness = -1f;
		}
		if (brightness > 1f)
		{
			brightness = 1f;
		}
	}
}
