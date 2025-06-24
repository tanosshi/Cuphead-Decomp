using System;
using System.Collections;
using UnityEngine;

public class AirshipCrabLevelBubbles : AbstractProjectile
{
	private LevelProperties.AirshipCrab.Bubbles properties;

	private float speed;

	private float speedY;

	public void Init(Vector2 pos, LevelProperties.AirshipCrab.Bubbles properties, float speed)
	{
		this.properties = properties;
		base.transform.position = pos;
		this.speed = speed;
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (base.damageDealer != null)
		{
			base.damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		base.damageDealer.DealDamage(hit);
	}

	private IEnumerator move_cr()
	{
		speedY = properties.sinWaveStrength;
		float t = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		while (base.transform.position.x > -640f)
		{
			Vector3 pos = base.transform.position;
			pos.x = Mathf.MoveTowards(base.transform.position.x, -640f, speed * (float)CupheadTime.Delta);
			base.transform.position = new Vector3(pos.x, base.transform.position.y + Mathf.Sin(t) * speedY * (float)CupheadTime.Delta * 60f, 0f);
			t += (float)CupheadTime.Delta;
			yield return null;
		}
		Die();
		yield return null;
	}

	protected override void Die()
	{
		base.Die();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
