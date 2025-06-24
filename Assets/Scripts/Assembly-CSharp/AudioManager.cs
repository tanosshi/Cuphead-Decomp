using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
	public enum Channel
	{
		Default = 0,
		Level = 1
	}

	public enum Property
	{
		MasterVolume = 0,
		Options_BGMVolume = 1,
		Options_SFXVolume = 2
	}

	public enum Snapshots
	{
		Cutscene = 0,
		FrontEnd = 1,
		Unpaused = 2,
		Unpaused_Clean = 3,
		Unpaused_1920s = 4,
		Loadscreen = 5,
		Paused = 6,
		Super = 7,
		Death = 8
	}

	public delegate bool OnCheckIfPlaying(string key);

	public delegate void OnSfxHandler(string key);

	public delegate void OnTransformHandler(string key, Transform transform);

	public delegate void OnAttenuationHandler(string key, bool attenuation, float endVolume);

	public delegate void OnChangeBGMHandler(float end, float time);

	public delegate void OnChangeBGMVolumeHandler(float end, float time, bool fadeOut);

	public delegate void OnChangeSFXHandler(string key, float end, float time);

	public delegate void OnWarbleBGMPitchHandler(float[] minValue, float[] maxValue, float[] warbleTime, float[] playTime);

	public delegate void OnSnapshotHandler(string[] names, float[] weight, float time);

	public delegate void OnBGMPlayListManualHandler(bool loopPlayListAfter);

	private const float VOLUME_MAX = 0f;

	private const float VOLUME_MIN = -80f;

	private static AudioMixer _mixer;

	private static bool _muted;

	private static bool checkIfPlaying;

	private static AudioMixer mixer
	{
		get
		{
			if (_mixer == null)
			{
				_mixer = AudioManagerMixer.GetMixer();
			}
			return _mixer;
		}
	}

	public static float sfxOptionsVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.Options_SFXVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.Options_SFXVolume.ToString(), value);
		}
	}

	public static float bgmOptionsVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.Options_BGMVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.Options_BGMVolume.ToString(), value);
		}
	}

	public static float masterVolume
	{
		get
		{
			float value;
			mixer.GetFloat(Property.MasterVolume.ToString(), out value);
			return value;
		}
		set
		{
			mixer.SetFloat(Property.MasterVolume.ToString(), value);
		}
	}

	public static bool muted
	{
		get
		{
			return _muted;
		}
		set
		{
			_muted = value;
			if (AudioManager.OnMuteChangedEvent != null)
			{
				AudioManager.OnMuteChangedEvent();
			}
		}
	}

	public static event OnCheckIfPlaying OnCheckEvent;

	public static event OnAttenuationHandler OnAttenuation;

	public static event OnTransformHandler OnFollowObject;

	public static event OnSnapshotHandler OnSnapshotEvent;

	public static event OnChangeBGMHandler OnBGMSlowdown;

	public static event OnChangeBGMVolumeHandler OnBGMFadeVolume;

	public static event OnChangeSFXHandler OnSFXSlowDown;

	public static event OnChangeSFXHandler OnSFXFadeVolume;

	public static event OnWarbleBGMPitchHandler OnBGMPitchWarble;

	public static event OnBGMPlayListManualHandler OnPlayManualBGM;

	public static event OnSfxHandler OnPlayEvent;

	public static event OnSfxHandler OnPlayLoopEvent;

	public static event OnSfxHandler OnStopEvent;

	public static event OnSfxHandler OnPauseEvent;

	public static event OnSfxHandler OnUnpauseEvent;

	public static event Action OnStopAllEvent;

	public static event Action OnStopBGMEvent;

	public static event Action OnPlayBGMEvent;

	public static event Action OnPlayBGMPlaylistEvent;

	public static event Action OnPauseAllSFXEvent;

	public static event Action OnUnpauseAllSFXEvent;

	public static event Action OnVolumeChangedEvent;

	public static event Action OnMuteChangedEvent;

	public static event Action OnStopManualBGMTrackEvent;

	public static bool CheckIfPlaying(string key)
	{
		checkIfPlaying = false;
		if (AudioManager.OnCheckEvent != null)
		{
			key = key.ToLower();
			Delegate[] invocationList = AudioManager.OnCheckEvent.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				OnCheckIfPlaying onCheckIfPlaying = (OnCheckIfPlaying)invocationList[i];
				if (onCheckIfPlaying(key))
				{
					checkIfPlaying = true;
				}
			}
			return checkIfPlaying;
		}
		return false;
	}

	public static void PlayBGMPlaylistManually(bool loopPlayListAfter)
	{
		if (AudioManager.OnPlayManualBGM != null)
		{
			AudioManager.OnPlayManualBGM(loopPlayListAfter);
		}
	}

	public static void StopBGMPlaylistManually()
	{
		if (AudioManager.OnStopManualBGMTrackEvent != null)
		{
			AudioManager.OnStopManualBGMTrackEvent();
		}
	}

	public static void ChangeSFXPitch(string key, float endPitch, float time)
	{
		if (AudioManager.OnSFXSlowDown != null)
		{
			AudioManager.OnSFXSlowDown(key, endPitch, time);
		}
	}

	public static void ChangeBGMPitch(float endPitch, float time)
	{
		if (AudioManager.OnBGMSlowdown != null)
		{
			AudioManager.OnBGMSlowdown(endPitch, time);
		}
	}

	public static void FadeBGMVolume(float endVolume, float time, bool fadeOut)
	{
		if (AudioManager.OnBGMFadeVolume != null)
		{
			AudioManager.OnBGMFadeVolume(endVolume, time, fadeOut);
		}
	}

	public static void WarbleBGMPitch(float[] minValue, float[] maxValue, float[] incrementTime, float[] playTime)
	{
		if (AudioManager.OnBGMPitchWarble != null)
		{
			AudioManager.OnBGMPitchWarble(minValue, maxValue, incrementTime, playTime);
		}
	}

	public static void Attenuation(string key, bool attenuation, float endVolume)
	{
		if (AudioManager.OnAttenuation != null)
		{
			AudioManager.OnAttenuation(key, attenuation, endVolume);
		}
	}

	public static void Play(string key)
	{
		key = key.ToLower();
		if (AudioManager.OnPlayEvent != null)
		{
			AudioManager.OnPlayEvent(key);
		}
	}

	public static void Stop(string key)
	{
		key = key.ToLower();
		if (AudioManager.OnStopEvent != null)
		{
			AudioManager.OnStopEvent(key);
		}
	}

	public static void PlayLoop(string key)
	{
		key = key.ToLower();
		if (AudioManager.OnPlayLoopEvent != null)
		{
			AudioManager.OnPlayLoopEvent(key);
		}
	}

	public static void Pause(string key)
	{
		key = key.ToLower();
		if (AudioManager.OnPauseEvent != null)
		{
			AudioManager.OnPauseEvent(key);
		}
	}

	public static void Unpaused(string key)
	{
		key = key.ToLower();
		if (AudioManager.OnUnpauseEvent != null)
		{
			AudioManager.OnUnpauseEvent(key);
		}
	}

	public static void FadeSFXVolume(string key, float endVolume, float time)
	{
		if (AudioManager.OnSFXFadeVolume != null)
		{
			AudioManager.OnSFXFadeVolume(key, endVolume, time);
		}
	}

	public static void FollowObject(IEnumerable<string> keys, Transform transform)
	{
		foreach (string key in keys)
		{
			FollowObject(keys, transform);
		}
	}

	public static void FollowObject(string key, Transform transform)
	{
		key.ToLower();
		if (AudioManager.OnFollowObject != null)
		{
			AudioManager.OnFollowObject(key, transform);
		}
	}

	[Obsolete("Use Play(string key) instead")]
	public static void Play(Sfx sfx)
	{
		Play(sfx.ToString());
	}

	[Obsolete("Use Stop(string key) instead")]
	public static void Stop(Sfx sfx)
	{
		Stop(sfx.ToString());
	}

	public static void StopAll()
	{
		if (AudioManager.OnStopAllEvent != null)
		{
			AudioManager.OnStopAllEvent();
		}
	}

	public static void StopBGM()
	{
		if (AudioManager.OnStopBGMEvent != null)
		{
			AudioManager.OnStopBGMEvent();
		}
	}

	public static void PlayBGM()
	{
		if (AudioManager.OnPlayBGMEvent != null)
		{
			AudioManager.OnPlayBGMEvent();
		}
	}

	public static void PlaylistBGM()
	{
		if (AudioManager.OnPlayBGMPlaylistEvent != null)
		{
			AudioManager.OnPlayBGMPlaylistEvent();
		}
	}

	public static void PauseAllSFX()
	{
		if (AudioManager.OnPauseAllSFXEvent != null)
		{
			AudioManager.OnPauseAllSFXEvent();
		}
	}

	public static void UnpauseAllSFX()
	{
		if (AudioManager.OnUnpauseAllSFXEvent != null)
		{
			AudioManager.OnUnpauseAllSFXEvent();
		}
	}

	public static void SnapshotTransition(string[] snapshotNames, float[] weights, float time)
	{
		if (AudioManager.OnSnapshotEvent != null)
		{
			AudioManager.OnSnapshotEvent(snapshotNames, weights, time);
		}
	}

	public static void HandleSnapshot(string snapshot, float time)
	{
		string[] array = new string[9]
		{
			Snapshots.Cutscene.ToString(),
			Snapshots.FrontEnd.ToString(),
			Snapshots.Unpaused.ToString(),
			Snapshots.Unpaused_Clean.ToString(),
			Snapshots.Unpaused_1920s.ToString(),
			Snapshots.Loadscreen.ToString(),
			Snapshots.Paused.ToString(),
			Snapshots.Super.ToString(),
			Snapshots.Death.ToString()
		};
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = ((!(array[i] == snapshot)) ? 0f : 1f);
		}
		SnapshotTransition(array, array2, time);
	}

	public static void SnapshotReset(string sceneName, float time)
	{
		string[] array = new string[9]
		{
			Snapshots.Cutscene.ToString(),
			Snapshots.FrontEnd.ToString(),
			Snapshots.Unpaused.ToString(),
			Snapshots.Unpaused_Clean.ToString(),
			Snapshots.Unpaused_1920s.ToString(),
			Snapshots.Loadscreen.ToString(),
			Snapshots.Paused.ToString(),
			Snapshots.Super.ToString(),
			Snapshots.Death.ToString()
		};
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (SettingsData.Data.vintageAudioEnabled)
			{
				if (array[i] == Snapshots.Unpaused_1920s.ToString())
				{
					num = i;
				}
			}
			else if (sceneName == Scenes.scene_level_retro_arcade.ToString())
			{
				if (array[i] == Snapshots.Unpaused_Clean.ToString())
				{
					num = i;
				}
			}
			else if (array[i] == Snapshots.Unpaused.ToString())
			{
				num = i;
			}
		}
		float[] array2 = new float[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array2[j] = ((j != num) ? 0f : 1f);
		}
		SnapshotTransition(array, array2, time);
	}

	private static void SetProperty(Property property, float value)
	{
		mixer.SetFloat(property.ToString(), value);
	}

	private static float CalculateVolume(float percentage)
	{
		return EaseUtils.Ease(EaseUtils.EaseType.easeOutCubic, -80f, 0f, percentage);
	}
}
