using System;
using System.Collections;
using UnityEngine;

public class SlimeLevelTinySlime : AbstractCollidableObject
{
	[SerializeField]
	private SpriteRenderer sprite;

	private LevelProperties.Slime.Tombstone properties;

	private SlimeLevelTombstone parent;

	private DamageDealer damageDealer;

	private DamageReceiver damageReceiver;

	private bool goingRight;

	private bool melted;

	private float Health;

	public void Init(Vector3 pos, LevelProperties.Slime.Tombstone properties, bool goingRight, SlimeLevelTombstone parent)
	{
		base.transform.position = pos;
		this.properties = properties;
		this.goingRight = goingRight;
		this.parent = parent;
		SlimeLevelTombstone slimeLevelTombstone = this.parent;
		slimeLevelTombstone.onDeath = (Action)Delegate.Combine(slimeLevelTombstone.onDeath, new Action(Death));
		if (goingRight)
		{
			base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
		}
		else
		{
			base.transform.SetScale(base.transform.localScale.x, 1f, 1f);
		}
	}

	protected override void Start()
	{
		base.Start();
		damageDealer = DamageDealer.NewEnemy();
		damageReceiver = GetComponent<DamageReceiver>();
		damageReceiver.OnDamageTaken += OnDamageTaken;
		Health = properties.tinyHealth;
		StartCoroutine(move_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (damageDealer != null)
		{
			damageDealer.Update();
		}
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.OnCollisionPlayer(hit, phase);
		damageDealer.DealDamage(hit);
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		Health -= info.damage;
		if (Health < 0f && !melted)
		{
			Melt();
		}
	}

	private void Melt()
	{
		melted = true;
		StartCoroutine(melt_cr());
	}

	private IEnumerator melt_cr()
	{
		GetComponent<Collider2D>().enabled = false;
		AudioManager.Stop("level_blobrunner");
		AudioManager.Play("level_frogs_tall_firefly_death");
		base.animator.Play("Melt");
		yield return base.animator.WaitForAnimationToEnd(this, "Melt");
		yield return CupheadTime.WaitForSeconds(this, properties.tinyMeltDelay);
		base.animator.SetTrigger("Continue");
		AudioManager.Play("level_blobrunner_reform");
		emitAudioFromObject.Add("level_blobrunner_reform");
		yield return CupheadTime.WaitForSeconds(this, properties.tinyTimeUntilUnmelt);
		base.animator.SetTrigger("Continue");
		yield return base.animator.WaitForAnimationToEnd(this, "Unmelt");
		melted = false;
		Health = properties.tinyHealth;
		GetComponent<Collider2D>().enabled = true;
	}

	private IEnumerator move_cr()
	{
		float sizeX = sprite.bounds.size.x;
		float sizeY = sprite.bounds.size.y;
		float left = -640f + sizeX / 2f;
		float right = 640f - sizeX / 2f;
		float down = (float)Level.Current.Ground + sizeY / 3f;
		float t = 0f;
		float time = properties.tinyRunTime;
		EaseUtils.EaseType ease = EaseUtils.EaseType.linear;
		float speed = 600f;
		float acceleration = 10f;
		Vector3 endPos = Vector3.zero;
		endPos = ((!goingRight) ? new Vector3(left, down) : new Vector3(right, down));
		while (base.transform.position != endPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, endPos, speed * (float)CupheadTime.Delta);
			speed += acceleration;
			yield return null;
		}
		float start = 0f;
		float end = 0f;
		sprite.sortingLayerName = SpriteLayer.Projectiles.ToString();
		if (goingRight)
		{
			start = base.transform.position.x;
			end = right;
		}
		else
		{
			start = base.transform.position.x;
			end = left;
		}
		time = 0f;
		while (true)
		{
			t = 0f;
			while (t < time)
			{
				if (!melted)
				{
					float value = t / time;
					base.transform.SetPosition(EaseUtils.Ease(ease, start, end, value));
					t += (float)CupheadTime.Delta;
				}
				yield return null;
			}
			base.animator.Play("Turn");
			yield return base.animator.WaitForAnimationToEnd(this, "Turn");
			base.transform.SetScale(0f - base.transform.localScale.x, 1f, 1f);
			goingRight = !goingRight;
			if (goingRight)
			{
				start = left;
				end = right;
			}
			else
			{
				start = right;
				end = left;
			}
			time = properties.tinyRunTime;
			yield return null;
		}
	}

	private void Death()
	{
		StopAllCoroutines();
		GetComponent<Collider2D>().enabled = false;
		base.animator.Play("Melt");
	}

	protected override void OnDestroy()
	{
		SlimeLevelTombstone slimeLevelTombstone = parent;
		slimeLevelTombstone.onDeath = (Action)Delegate.Remove(slimeLevelTombstone.onDeath, new Action(Death));
		base.OnDestroy();
	}
}
