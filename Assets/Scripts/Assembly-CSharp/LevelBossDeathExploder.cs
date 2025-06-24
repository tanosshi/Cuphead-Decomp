using System.Collections;
using UnityEngine;

public class LevelBossDeathExploder : AbstractMonoBehaviour
{
	private enum State
	{
		Steady = 0,
		Random = 1
	}

	public Effect ExplosionPrefabOverride;

	private const float STEADY_DELAY = 0.3f;

	private const float MIN_DELAY = 0.4f;

	private const float MAX_DELAY = 1f;

	[SerializeField]
	private Vector2 offset = Vector2.zero;

	[SerializeField]
	private float radius = 100f;

	private State state;

	protected Effect effectPrefab;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		if ((bool)ExplosionPrefabOverride)
		{
			effectPrefab = ExplosionPrefabOverride;
		}
		else
		{
			effectPrefab = Level.Current.LevelResources.levelBossDeathExplosion;
		}
		base.Start();
		Level.Current.OnBossDeathExplosionsEvent += StartExplosion;
		Level.Current.OnBossDeathExplosionsFalloffEvent += OnExplosionsRand;
		Level.Current.OnBossDeathExplosionsEndEvent += StopExplosions;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		try
		{
			Level.Current.OnBossDeathExplosionsEvent -= StartExplosion;
		}
		catch
		{
		}
		try
		{
			Level.Current.OnBossDeathExplosionsFalloffEvent -= OnExplosionsRand;
		}
		catch
		{
		}
		try
		{
			Level.Current.OnBossDeathExplosionsEndEvent -= StopExplosions;
		}
		catch
		{
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere((Vector2)base.baseTransform.position + offset, radius);
	}

	public void StartExplosion()
	{
		if (!(this == null) && base.enabled && base.isActiveAndEnabled)
		{
			StartCoroutine(go_cr());
		}
	}

	public void OnExplosionsRand()
	{
		state = State.Random;
	}

	public void StopExplosions()
	{
		StopAllCoroutines();
	}

	private Vector2 GetRandomPoint()
	{
		Vector2 vector = (Vector2)base.transform.position + offset;
		Vector2 vector2 = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * (radius * Random.value) * 2f;
		return vector + vector2;
	}

	private IEnumerator go_cr()
	{
		HitFlash flash = GetComponent<HitFlash>();
		AudioManager.Play("level_explosion_boss_death");
		while (true)
		{
			effectPrefab.Create(GetRandomPoint());
			if (flash != null)
			{
				flash.Flash();
			}
			CupheadLevelCamera.Current.Shake(10f, 0.4f);
			State state = this.state;
			if (state != State.Random)
			{
				yield return CupheadTime.WaitForSeconds(this, 0.3f);
			}
			else
			{
				yield return CupheadTime.WaitForSeconds(this, Random.Range(0.4f, 1f));
			}
		}
	}
}
