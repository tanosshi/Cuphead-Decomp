using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelHeart : AbstractProjectile
{
	private int direction;

	private float time;

	private Vector3 pos;

	private new DamageDealer damageDealer;

	private LevelProperties.SallyStagePlay properties;

	protected override void Awake()
	{
		damageDealer = DamageDealer.NewEnemy();
		base.Awake();
	}

	protected override void Update()
	{
		damageDealer.Update();
		base.Update();
	}

	public void InitHeart(LevelProperties.SallyStagePlay properties, int direction, bool isParryable)
	{
		this.properties = properties;
		time = 0f;
		this.direction = direction;
		pos = base.transform.position;
		if (!isParryable)
		{
			GetComponent<SpriteRenderer>().color = Color.blue;
		}
		else
		{
			SetParryable(true);
		}
		StartCoroutine(wave_cr());
	}

	private IEnumerator wave_cr()
	{
		yield return animator.WaitForAnimationToEnd(this, "Intro");
		while (true)
		{
			Vector3 newPos = pos;
			newPos.y = Mathf.Sin(time * properties.CurrentState.kiss.sineWaveSpeed) * properties.CurrentState.kiss.sineWaveStrength;
			base.transform.position = newPos;
			pos += Vector3.left * direction * properties.CurrentState.kiss.heartSpeed * CupheadTime.Delta;
			time += CupheadTime.Delta;
			yield return null;
			if (base.transform.position.x < (float)(Level.Current.Left - 150) || base.transform.position.x > (float)(Level.Current.Right + 150))
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		damageDealer.DealDamage(hit);
		base.OnCollisionPlayer(hit, phase);
		Object.Destroy(base.gameObject);
	}

	protected override void OnDestroy()
	{
		StopAllCoroutines();
		base.OnDestroy();
	}
}
