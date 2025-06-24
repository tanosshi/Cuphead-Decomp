using System.Collections;
using UnityEngine;

public class FrogsLevelBisonBullet : AbstractFrogsLevelSlotBullet
{
	public enum Direction
	{
		Up = 0,
		Down = 1
	}

	public Transform flame;

	private Direction direction;

	private float bigX;

	private new Animator animator;

	protected override EaseUtils.EaseType Y_Ease
	{
		get
		{
			return EaseUtils.EaseType.easeOutElastic;
		}
	}

	protected override float Y
	{
		get
		{
			return -60f;
		}
	}

	protected override float Y_Time
	{
		get
		{
			return 2f;
		}
	}

	public FrogsLevelBisonBullet Create(Vector2 pos, float s, Direction direction, float bigX, float smallX)
	{
		FrogsLevelBisonBullet frogsLevelBisonBullet = Create(pos, s) as FrogsLevelBisonBullet;
		frogsLevelBisonBullet.Init(direction, bigX, smallX);
		return frogsLevelBisonBullet;
	}

	private void Init(Direction dir, float big, float small)
	{
		animator = GetComponent<Animator>();
		flame.GetComponent<Collider2D>().enabled = false;
		flame.GetComponent<CollisionChild>().OnPlayerCollision += base.DealDamage;
		direction = dir;
		bigX = big;
		StartCoroutine(bison_cr());
		StartCoroutine(small_cr());
	}

	private IEnumerator small_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.1f);
		flame.GetComponent<Collider2D>().enabled = true;
		animator.SetTrigger("Small");
	}

	private IEnumerator bison_cr()
	{
		if (direction == Direction.Down)
		{
			flame.SetEulerAngles(0f, 0f, 180f);
			flame.AddLocalPosition(0f, -115f);
			flame.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
		}
		yield return null;
		yield return null;
		yield return null;
		bool big = false;
		while (true)
		{
			float distance = float.MaxValue;
			AbstractPlayerController p1 = PlayerManager.GetPlayer(PlayerId.PlayerOne);
			AbstractPlayerController p2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
			if (p1 != null)
			{
				distance = Mathf.Min(distance, base.transform.position.x - p1.center.x);
			}
			if (p2 != null)
			{
				distance = Mathf.Min(distance, base.transform.position.x - p2.center.x);
			}
			if (distance <= bigX && !big)
			{
				big = true;
				AudioManager.Play("level_frogs_flame_platform_fire_burst");
				emitAudioFromObject.Add("level_frogs_flame_platform_fire_burst");
				AudioManager.PlayLoop("level_frogs_flame_platform_fire_loop");
				emitAudioFromObject.Add("level_frogs_flame_platform_fire_loop");
				Debug.Log("Big");
				flame.GetComponent<Collider2D>().enabled = true;
				animator.SetTrigger("Big");
			}
			yield return null;
		}
	}

	protected override void End()
	{
		AudioManager.Stop("level_frogs_flame_platform_fire_loop");
		base.End();
	}
}
