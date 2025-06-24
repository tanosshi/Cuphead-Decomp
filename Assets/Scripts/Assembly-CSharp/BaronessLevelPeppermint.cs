using System.Collections;
using UnityEngine;

public class BaronessLevelPeppermint : ParrySwitch
{
	private LevelProperties.Baroness.Peppermint properties;

	private DamageDealer damageDealer;

	private float speed;

	protected override void Awake()
	{
		base.Awake();
		damageDealer = DamageDealer.NewEnemy();
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	public void Init(LevelProperties.Baroness.Peppermint properties, Vector2 pos, float speed)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.speed = speed;
		AudioManager.Play("level_baroness_candy_roll");
		StartCoroutine(fade_color_cr());
		StartCoroutine(move_cr());
	}

	protected virtual IEnumerator fade_color_cr()
	{
		float fadeTime = 0.7f;
		float t = 0f;
		while (t < fadeTime)
		{
			GetComponent<SpriteRenderer>().color = new Color(t / fadeTime, t / fadeTime, t / fadeTime, 1f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		yield return null;
	}

	private IEnumerator move_cr()
	{
		float offsetX = 220f;
		Vector3 pos = base.transform.position;
		while (true)
		{
			if (base.transform.position.x > -640f - offsetX)
			{
				pos.x = Mathf.MoveTowards(base.transform.position.x, -640f - offsetX, speed * CupheadTime.FixedDelta);
			}
			else
			{
				Die();
			}
			base.transform.position = pos;
			yield return new WaitForFixedUpdate();
		}
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}

	public override void OnParryPrePause(AbstractPlayerController player)
	{
		base.OnParryPrePause(player);
		player.stats.ParryOneQuarter();
	}
}
