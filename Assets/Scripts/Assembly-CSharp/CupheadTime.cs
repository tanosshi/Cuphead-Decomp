using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CupheadTime
{
	public enum Layer
	{
		Default = 0,
		Player = 1,
		Enemy = 2,
		UI = 3
	}

	public class DeltaObject
	{
		public float this[Layer layer]
		{
			get
			{
				return Time.deltaTime * GetLayerSpeed(layer) * GlobalSpeed;
			}
		}

		public static implicit operator float(DeltaObject d)
		{
			return d[Layer.Default] * GlobalSpeed;
		}
	}

	private static readonly DeltaObject delta;

	private static float globalSpeed;

	private static Dictionary<Layer, float> layers;

	public static DeltaObject Delta
	{
		get
		{
			return delta;
		}
	}

	public static float GlobalDelta
	{
		get
		{
			return Time.deltaTime;
		}
	}

	public static float FixedDelta
	{
		get
		{
			return Time.fixedDeltaTime * GlobalSpeed;
		}
	}

	public static float GlobalSpeed
	{
		get
		{
			return globalSpeed;
		}
		set
		{
			globalSpeed = Mathf.Clamp(value, 0f, 1f);
			OnChanged();
		}
	}

	public static event Action OnChangedEvent;

	static CupheadTime()
	{
		globalSpeed = 1f;
		delta = new DeltaObject();
		layers = new Dictionary<Layer, float>();
		Layer[] values = EnumUtils.GetValues<Layer>();
		Layer[] array = values;
		foreach (Layer key in array)
		{
			layers.Add(key, 1f);
		}
	}

	public static float GetLayerSpeed(Layer layer)
	{
		return layers[layer];
	}

	public static void SetLayerSpeed(Layer layer, float value)
	{
		layers[layer] = value;
		OnChanged();
	}

	public static void Reset()
	{
		SetAll(1f);
	}

	public static void SetAll(float value)
	{
		GlobalSpeed = value;
		Layer[] values = EnumUtils.GetValues<Layer>();
		foreach (Layer key in values)
		{
			layers[key] = value;
		}
		OnChanged();
	}

	private static void OnChanged()
	{
		if (CupheadTime.OnChangedEvent != null)
		{
			CupheadTime.OnChangedEvent();
		}
	}

	public static bool IsPaused()
	{
		return GlobalSpeed <= 1E-05f || PauseManager.state == PauseManager.State.Paused;
	}

	public static Coroutine WaitForSeconds(MonoBehaviour m, float time)
	{
		return m.StartCoroutine(waitForSeconds_cr(time, Layer.Default));
	}

	public static Coroutine WaitForSeconds(MonoBehaviour m, float time, Layer layer)
	{
		return m.StartCoroutine(waitForSeconds_cr(time, layer));
	}

	private static IEnumerator waitForSeconds_cr(float time, Layer layer)
	{
		float t = 0f;
		while (t < time)
		{
			t += Delta[layer];
			yield return null;
		}
	}

	public static Coroutine WaitForUnpause(MonoBehaviour m)
	{
		return m.StartCoroutine(waitForUnpause_cr());
	}

	private static IEnumerator waitForUnpause_cr()
	{
		while (GlobalSpeed == 0f)
		{
			yield return null;
		}
	}
}
