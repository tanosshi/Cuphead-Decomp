using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateLevelSquidInkOverlay : LevelProperties.Pirate.Entity
{
	public class SplatGroup
	{
		public class Splat
		{
			public enum Type
			{
				Small = 0,
				Large = 1
			}

			public readonly Type type;

			public readonly Vector2 position;

			public int delay;

			public Splat(Transform transform)
			{
				position = transform.position;
				if (transform.name.ToLower().Contains("small"))
				{
					type = Type.Small;
				}
				else
				{
					type = Type.Large;
				}
			}
		}

		public List<Splat> splats;

		public SplatGroup(Transform parent)
		{
			splats = new List<Splat>();
			foreach (Transform item in parent)
			{
				splats.Add(new Splat(item));
			}
		}

		public void RandomizeDelay(int max)
		{
			foreach (Splat splat in splats)
			{
				splat.delay = Random.Range(0, max);
			}
		}
	}

	private const int DELAY_MAX = 10;

	private const float DELAY_WAIT = 0.025f;

	[SerializeField]
	private Effect largeSplat;

	[SerializeField]
	private Effect smallSplat;

	private SpriteRenderer renderer;

	private List<SplatGroup> splatGroups;

	private bool SFXSplatScreenActive;

	private Color color;

	private float targetAlpha;

	public static PirateLevelSquidInkOverlay Current { get; private set; }

	private float alpha
	{
		get
		{
			return renderer.color.a;
		}
		set
		{
			color.a = Mathf.Clamp(value, 0f, 1f);
			renderer.color = color;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Current = this;
		renderer = GetComponent<SpriteRenderer>();
		renderer.enabled = false;
		alpha = 0f;
		color = renderer.color;
		splatGroups = new List<SplatGroup>();
		foreach (Transform item in base.transform)
		{
			if (item.name.ToLower().Contains("group"))
			{
				splatGroups.Add(new SplatGroup(item));
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Current = null;
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		foreach (Transform item in base.baseTransform)
		{
			if (!item.gameObject.activeInHierarchy)
			{
				continue;
			}
			foreach (Transform item2 in item)
			{
				if (item2.name.ToLower().Contains("small"))
				{
					Gizmos.DrawWireSphere(item2.position, 20f);
				}
				else if (item2.name.ToLower().Contains("large"))
				{
					Gizmos.DrawWireSphere(item2.position, 40f);
				}
			}
		}
	}

	public override void LevelInit(LevelProperties.Pirate properties)
	{
		base.LevelInit(properties);
	}

	public void Hit()
	{
		if (!SFXSplatScreenActive)
		{
			AudioManager.Play("level_pirate_squid_blackout_screen");
			SFXSplatScreenActive = true;
		}
		LevelProperties.Pirate.Squid squid = base.properties.CurrentState.squid;
		renderer.enabled = true;
		StopAllCoroutines();
		StartCoroutine(splats_cr());
		StartCoroutine(hit_cr(squid));
	}

	private IEnumerator splats_cr()
	{
		SplatGroup group = splatGroups[Random.Range(0, splatGroups.Count)];
		group.RandomizeDelay(10);
		for (int i = 0; i < 10; i++)
		{
			foreach (SplatGroup.Splat splat in group.splats)
			{
				if (splat.delay == i)
				{
					Vector3 position = splat.position;
					if (splat.type == SplatGroup.Splat.Type.Large)
					{
						largeSplat.Create(position);
					}
					else
					{
						smallSplat.Create(position);
					}
				}
			}
			yield return CupheadTime.WaitForSeconds(this, 0.025f);
		}
	}

	private IEnumerator hit_cr(LevelProperties.Pirate.Squid p)
	{
		if (!SFXSplatScreenActive)
		{
			AudioManager.Play("level_pirate_squid_blackout_screen");
			SFXSplatScreenActive = true;
		}
		targetAlpha = Mathf.Clamp(targetAlpha + p.opacityAdd, 0f, 1f);
		float t = 0f;
		while (t < p.opacityAddTime)
		{
			alpha = Mathf.Lerp(t: t / p.opacityAddTime, a: alpha, b: targetAlpha);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		yield return CupheadTime.WaitForSeconds(this, p.darkHoldTime);
		yield return StartCoroutine(fade_cr(p));
	}

	private IEnumerator fade_cr(LevelProperties.Pirate.Squid p)
	{
		float t = 0f;
		while (t < p.darkFadeTime)
		{
			alpha = Mathf.Lerp(t: t / p.darkFadeTime, a: alpha, b: 0f);
			targetAlpha = alpha;
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		alpha = 0f;
		targetAlpha = alpha;
		renderer.enabled = false;
		SFXSplatScreenActive = false;
	}
}
