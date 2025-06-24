using System.Collections;
using UnityEngine;

public class FrogsLevelShortClapBullet : AbstractProjectile
{
	public enum Direction
	{
		Up = 0,
		Down = 1
	}

	private const float MAX_Y = 360f;

	[SerializeField]
	private Effect bounceEffect;

	private Vector2 velocity;

	private FrogsLevelShort.Direction frogDirection;

	private Direction direction;

	public FrogsLevelShortClapBullet Create(FrogsLevelShort.Direction frogDir, Direction dir, Vector2 pos, Vector2 velocity)
	{
		FrogsLevelShortClapBullet frogsLevelShortClapBullet = base.Create(pos) as FrogsLevelShortClapBullet;
		frogsLevelShortClapBullet.CollisionDeath.OnlyPlayer();
		frogsLevelShortClapBullet.DamagesType.OnlyPlayer();
		frogsLevelShortClapBullet.Init(frogDir, dir, pos, velocity);
		return frogsLevelShortClapBullet;
	}

	private void Init(FrogsLevelShort.Direction frogDir, Direction dir, Vector2 pos, Vector2 velocity)
	{
		frogDirection = frogDir;
		this.velocity = velocity;
		direction = dir;
		StartCoroutine(go_cr());
	}

	protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
	{
		base.damageDealer.DealDamage(hit);
	}

	protected override void Die()
	{
		base.Die();
		StopAllCoroutines();
	}

	private IEnumerator go_cr()
	{
		YieldInstruction wait = new WaitForFixedUpdate();
		float upY = velocity.y;
		float downY = 0f - velocity.y;
		float x = ((frogDirection != FrogsLevelShort.Direction.Right) ? (0f - velocity.x) : velocity.x);
		float y = ((direction != Direction.Up) ? downY : upY);
		if (direction == Direction.Up)
		{
			base.transform.LookAt2D((Vector2)base.transform.position + new Vector2(x, upY));
		}
		else
		{
			base.transform.LookAt2D((Vector2)base.transform.position + new Vector2(x, downY));
		}
		while (true)
		{
			if (direction == Direction.Up)
			{
				if (base.transform.position.y >= 360f)
				{
					AudioManager.Play("level_frogs_short_clap_bounce");
					emitAudioFromObject.Add("level_frogs_short_clap_bounce");
					direction = Direction.Down;
					bounceEffect.Create(base.transform.position, new Vector3(1f, -1f, 1f));
					y = downY;
					base.transform.LookAt2D((Vector2)base.transform.position + new Vector2(x, y));
				}
			}
			else if (base.transform.position.y <= (float)Level.Current.Ground)
			{
				AudioManager.Play("level_frogs_short_clap_bounce");
				emitAudioFromObject.Add("level_frogs_short_clap_bounce");
				direction = Direction.Up;
				bounceEffect.Create(base.transform.position);
				y = upY;
				base.transform.LookAt2D((Vector2)base.transform.position + new Vector2(x, y));
			}
			if (base.transform.position.x > 640f + GetComponent<SpriteRenderer>().bounds.size.x / 2f)
			{
				break;
			}
			base.transform.AddPosition(x * CupheadTime.FixedDelta, y * CupheadTime.FixedDelta);
			yield return wait;
		}
		Die();
	}
}
