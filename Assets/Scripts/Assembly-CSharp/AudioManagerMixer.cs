using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerMixer : MonoBehaviour
{
	[Serializable]
	public class Groups
	{
		[SerializeField]
		private AudioMixerGroup _master;

		[SerializeField]
		private AudioMixerGroup _master_Options;

		[SerializeField]
		private AudioMixerGroup _bgm_Options;

		[SerializeField]
		private AudioMixerGroup _sfx_Options;

		[Space(10f)]
		[Header("BGM")]
		[SerializeField]
		private AudioMixerGroup _bgm;

		[SerializeField]
		private AudioMixerGroup _levelBgm;

		[SerializeField]
		private AudioMixerGroup _musicSting;

		[Space(10f)]
		[Header("SFX")]
		[SerializeField]
		private AudioMixerGroup _sfx;

		[SerializeField]
		private AudioMixerGroup _levelSfx;

		[SerializeField]
		private AudioMixerGroup _ambience;

		[SerializeField]
		private AudioMixerGroup _creatures;

		[SerializeField]
		private AudioMixerGroup _announcer;

		[SerializeField]
		private AudioMixerGroup _super;

		[Space(10f)]
		[Header("Noise")]
		[SerializeField]
		private AudioMixerGroup _noise;

		[SerializeField]
		private AudioMixerGroup _noiseConstant;

		[SerializeField]
		private AudioMixerGroup _noiseShortterm;

		[SerializeField]
		private AudioMixerGroup _noise1920s;

		public AudioMixerGroup master
		{
			get
			{
				return _master;
			}
		}

		public AudioMixerGroup master_Options
		{
			get
			{
				return _master_Options;
			}
		}

		public AudioMixerGroup bgm_Options
		{
			get
			{
				return _bgm_Options;
			}
		}

		public AudioMixerGroup sfx_Options
		{
			get
			{
				return _sfx_Options;
			}
		}

		public AudioMixerGroup bgm
		{
			get
			{
				return _bgm;
			}
		}

		public AudioMixerGroup levelBgm
		{
			get
			{
				return _levelBgm;
			}
		}

		public AudioMixerGroup musicSting
		{
			get
			{
				return _musicSting;
			}
		}

		public AudioMixerGroup sfx
		{
			get
			{
				return _sfx;
			}
		}

		public AudioMixerGroup levelSfx
		{
			get
			{
				return _levelSfx;
			}
		}

		public AudioMixerGroup ambience
		{
			get
			{
				return _ambience;
			}
		}

		public AudioMixerGroup creatures
		{
			get
			{
				return _creatures;
			}
		}

		public AudioMixerGroup announcer
		{
			get
			{
				return _announcer;
			}
		}

		public AudioMixerGroup super
		{
			get
			{
				return _super;
			}
		}

		public AudioMixerGroup noise
		{
			get
			{
				return _noise;
			}
		}

		public AudioMixerGroup noiseConstant
		{
			get
			{
				return _noiseConstant;
			}
		}

		public AudioMixerGroup noiseShortterm
		{
			get
			{
				return _noiseShortterm;
			}
		}

		public AudioMixerGroup noise1920s
		{
			get
			{
				return _noise1920s;
			}
		}
	}

	private const string PATH = "Audio/AudioMixer";

	private static AudioManagerMixer Manager;

	[SerializeField]
	private AudioMixer mixer;

	[SerializeField]
	private Groups audioGroups;

	private static void Init()
	{
		if (Manager == null)
		{
			Manager = Resources.Load<AudioManagerMixer>("Audio/AudioMixer");
		}
	}

	public static AudioMixer GetMixer()
	{
		Init();
		return Manager.mixer;
	}

	public static Groups GetGroups()
	{
		Init();
		return Manager.audioGroups;
	}
}
